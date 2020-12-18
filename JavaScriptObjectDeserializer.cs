using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Helper
{
    internal class JavaScriptObjectDeserializer
    {
        private const string DateTimePrefix = "\"\\/Date(";
        private const int DateTimePrefixLength = 8;
        private int _depthLimit;
        internal JavaScriptString _s;

        private JavaScriptObjectDeserializer(string input, int depthLimit)
        {
            this._s = new JavaScriptString(input);
            this._depthLimit = depthLimit;
        }

        private void AppendCharToBuilder(char? c, StringBuilder sb)
        {
            char? nullable1 = c;
            if (nullable1.GetValueOrDefault() != '"' || !nullable1.HasValue)
            {
                char? nullable2 = c;
                if (nullable2.GetValueOrDefault() != '\'' || !nullable2.HasValue)
                {
                    char? nullable3 = c;
                    if (nullable3.GetValueOrDefault() != '/' || !nullable3.HasValue)
                    {
                        char? nullable4 = c;
                        if (nullable4.GetValueOrDefault() == 'b' && nullable4.HasValue)
                        {
                            sb.Append('\b');
                            return;
                        }
                        char? nullable5 = c;
                        if (nullable5.GetValueOrDefault() == 'f' && nullable5.HasValue)
                        {
                            sb.Append('\f');
                        }
                        else
                        {
                            char? nullable6 = c;
                            if (nullable6.GetValueOrDefault() == 'n' && nullable6.HasValue)
                            {
                                sb.Append('\n');
                            }
                            else
                            {
                                char? nullable7 = c;
                                if (nullable7.GetValueOrDefault() == 'r' && nullable7.HasValue)
                                {
                                    sb.Append('\r');
                                }
                                else
                                {
                                    char? nullable8 = c;
                                    if (nullable8.GetValueOrDefault() == 't' && nullable8.HasValue)
                                    {
                                        sb.Append('\t');
                                    }
                                    else
                                    {
                                        char? nullable9 = c;
                                        if (nullable9.GetValueOrDefault() != 'u' || !nullable9.HasValue)
                                            throw new ArgumentException(this._s.GetDebugString("Unrecognized escape sequence."));
                                        sb.Append((char)int.Parse(this._s.MoveNext(4), NumberStyles.HexNumber, (IFormatProvider)CultureInfo.InvariantCulture));
                                    }
                                }
                            }
                        }
                        return;
                    }
                }
            }
            sb.Append((object)c);
        }

        public static object DeserializeObject(string jsonStr) => JavaScriptObjectDeserializer.BasicDeserialize(jsonStr);

        public static Dictionary<string, object> DeserializeDic(string jsonStr)
        {
            object obj = JavaScriptObjectDeserializer.BasicDeserialize(jsonStr);
            return obj != null && obj is Dictionary<string, object> ? obj as Dictionary<string, object> : (Dictionary<string, object>)null;
        }

        public static ArrayList DeserializeArrayList(string jsonStr)
        {
            object obj = JavaScriptObjectDeserializer.BasicDeserialize(jsonStr);
            return obj != null && obj is ArrayList ? obj as ArrayList : (ArrayList)null;
        }

        public static List<Dictionary<string, object>> DeserializeList(string jsonStr)
        {
            object obj1 = JavaScriptObjectDeserializer.BasicDeserialize(jsonStr);
            if (obj1 == null || !(obj1 is ArrayList))
                return (List<Dictionary<string, object>>)null;
            ArrayList arrayList = obj1 as ArrayList;
            List<Dictionary<string, object>> dictionaryList = new List<Dictionary<string, object>>();
            foreach (object obj2 in arrayList)
                dictionaryList.Add(obj2 as Dictionary<string, object>);
            return dictionaryList;
        }

        internal static object BasicDeserialize(string input)
        {
            int depthLimit = 100;
            JavaScriptObjectDeserializer objectDeserializer = new JavaScriptObjectDeserializer(input, depthLimit);
            object obj = objectDeserializer.DeserializeInternal(0);
            char? nextNonEmptyChar = objectDeserializer._s.GetNextNonEmptyChar();
            if ((nextNonEmptyChar.HasValue ? new int?((int)nextNonEmptyChar.GetValueOrDefault()) : new int?()).HasValue)
                throw new ArgumentException(string.Format((IFormatProvider)CultureInfo.InvariantCulture, "Invalid JSON primitive: {0}.", (object)objectDeserializer._s.ToString()));
            return obj;
        }

        private char CheckQuoteChar(char? c)
        {
            char? nullable1 = c;
            if (nullable1.GetValueOrDefault() == '\'' && nullable1.HasValue)
                return c.Value;
            char? nullable2 = c;
            if (nullable2.GetValueOrDefault() != '"' || !nullable2.HasValue)
                throw new ArgumentException(this._s.GetDebugString("Invalid string passed in, '\"' expected."));
            return '"';
        }

        private IDictionary<string, object> DeserializeDictionary(int depth)
        {
            string message1 = "Invalid object passed in, member name expected.";
            string message2 = "Invalid object passed in, ':' or '}' expected.";
            IDictionary<string, object> dictionary = (IDictionary<string, object>)null;
            char? nullable1 = this._s.MoveNext();
            if (nullable1.GetValueOrDefault() != '{' || !nullable1.HasValue)
                throw new ArgumentException(this._s.GetDebugString("Invalid object passed in, '{' expected."));
            char? nextNonEmptyChar1;
            char? nullable2;
            do
            {
                char? nullable3 = nextNonEmptyChar1 = this._s.GetNextNonEmptyChar();
                if ((nullable3.HasValue ? new int?((int)nullable3.GetValueOrDefault()) : new int?()).HasValue)
                {
                    this._s.MovePrev();
                    char? nullable4 = nextNonEmptyChar1;
                    if (nullable4.GetValueOrDefault() == ':' && nullable4.HasValue)
                        throw new ArgumentException(this._s.GetDebugString(message1));
                    string key = (string)null;
                    char? nullable5 = nextNonEmptyChar1;
                    if (nullable5.GetValueOrDefault() != '}' || !nullable5.HasValue)
                    {
                        key = this.DeserializeMemberName();
                        if (string.IsNullOrEmpty(key))
                            throw new ArgumentException(this._s.GetDebugString(message1));
                        char? nextNonEmptyChar2 = this._s.GetNextNonEmptyChar();
                        if (nextNonEmptyChar2.GetValueOrDefault() != ':' || !nextNonEmptyChar2.HasValue)
                            throw new ArgumentException(this._s.GetDebugString(message2));
                    }
                    if (dictionary == null)
                    {
                        dictionary = (IDictionary<string, object>)new Dictionary<string, object>();
                        if (string.IsNullOrEmpty(key))
                        {
                            nextNonEmptyChar1 = this._s.GetNextNonEmptyChar();
                            goto label_16;
                        }
                    }
                    object obj = this.DeserializeInternal(depth);
                    dictionary[key] = obj;
                    nextNonEmptyChar1 = this._s.GetNextNonEmptyChar();
                    char? nullable6 = nextNonEmptyChar1;
                    if (nullable6.GetValueOrDefault() != '}' || !nullable6.HasValue)
                        nullable2 = nextNonEmptyChar1;
                    else
                        goto label_16;
                }
                else
                    goto label_16;
            }
            while (nullable2.GetValueOrDefault() == ',' && nullable2.HasValue);
            throw new ArgumentException(this._s.GetDebugString(message2));
        label_16:
            char? nullable7 = nextNonEmptyChar1;
            if (nullable7.GetValueOrDefault() != '}' || !nullable7.HasValue)
                throw new ArgumentException(this._s.GetDebugString(message2));
            return dictionary;
        }

        private object DeserializeInternal(int depth)
        {
            if (++depth > this._depthLimit)
                throw new ArgumentException(this._s.GetDebugString("RecursionLimit exceeded."));
            char? nextNonEmptyChar = this._s.GetNextNonEmptyChar();
            char? nullable = nextNonEmptyChar;
            if (!(nullable.HasValue ? new int?((int)nullable.GetValueOrDefault()) : new int?()).HasValue)
                return (object)null;
            this._s.MovePrev();
            if (this.IsNextElementDateTime())
                return this.DeserializeStringIntoDateTime();
            if (JavaScriptObjectDeserializer.IsNextElementObject(nextNonEmptyChar))
            {
                IDictionary<string, object> dictionary = this.DeserializeDictionary(depth);
                return !dictionary.ContainsKey("__type") ? (object)dictionary : throw new Exception("kevin can't implement this action");
            }
            if (JavaScriptObjectDeserializer.IsNextElementArray(nextNonEmptyChar))
                return (object)this.DeserializeList(depth);
            return JavaScriptObjectDeserializer.IsNextElementString(nextNonEmptyChar) ? (object)this.DeserializeString() : this.DeserializePrimitiveObject();
        }

        private IList DeserializeList(int depth)
        {
            IList list = (IList)new ArrayList();
            char? nullable1 = this._s.MoveNext();
            if (nullable1.GetValueOrDefault() != '[' || !nullable1.HasValue)
                throw new ArgumentException(this._s.GetDebugString("Invalid array passed in, '[' expected."));
            bool flag = false;
            char? nextNonEmptyChar;
            char? nullable2;
            do
            {
                char? nullable3 = nextNonEmptyChar = this._s.GetNextNonEmptyChar();
                if ((nullable3.HasValue ? new int?((int)nullable3.GetValueOrDefault()) : new int?()).HasValue)
                {
                    char? nullable4 = nextNonEmptyChar;
                    if (nullable4.GetValueOrDefault() != ']' || !nullable4.HasValue)
                    {
                        this._s.MovePrev();
                        object obj = this.DeserializeInternal(depth);
                        list.Add(obj);
                        flag = false;
                        nextNonEmptyChar = this._s.GetNextNonEmptyChar();
                        char? nullable5 = nextNonEmptyChar;
                        if (nullable5.GetValueOrDefault() != ']' || !nullable5.HasValue)
                        {
                            flag = true;
                            nullable2 = nextNonEmptyChar;
                        }
                        else
                            goto label_8;
                    }
                    else
                        goto label_8;
                }
                else
                    goto label_9;
            }
            while (nullable2.GetValueOrDefault() == ',' && nullable2.HasValue);
            throw new ArgumentException(this._s.GetDebugString("Invalid array passed in, ',' expected."));
        label_8:
        label_9:
            if (flag)
                throw new ArgumentException(this._s.GetDebugString("Invalid array passed in, extra trailing ','."));
            char? nullable6 = nextNonEmptyChar;
            if (nullable6.GetValueOrDefault() != ']' || !nullable6.HasValue)
                throw new ArgumentException(this._s.GetDebugString("Invalid array passed in, ']' expected."));
            return list;
        }

        private string DeserializeMemberName()
        {
            char? nextNonEmptyChar = this._s.GetNextNonEmptyChar();
            char? nullable = nextNonEmptyChar;
            if (!(nullable.HasValue ? new int?((int)nullable.GetValueOrDefault()) : new int?()).HasValue)
                return (string)null;
            this._s.MovePrev();
            return JavaScriptObjectDeserializer.IsNextElementString(nextNonEmptyChar) ? this.DeserializeString() : this.DeserializePrimitiveToken();
        }

        private object DeserializePrimitiveObject()
        {
            string s = this.DeserializePrimitiveToken();
            if (s.Equals("null"))
                return (object)null;
            if (s.Equals("true"))
                return (object)true;
            if (s.Equals("false"))
                return (object)false;
            bool flag = s.IndexOf('.') >= 0;
            if (s.LastIndexOf("e", StringComparison.OrdinalIgnoreCase) < 0)
            {
                if (!flag)
                {
                    int result1;
                    if (int.TryParse(s, NumberStyles.Integer, (IFormatProvider)CultureInfo.InvariantCulture, out result1))
                        return (object)result1;
                    long result2;
                    if (long.TryParse(s, NumberStyles.Integer, (IFormatProvider)CultureInfo.InvariantCulture, out result2))
                        return (object)result2;
                }
                Decimal result;
                if (Decimal.TryParse(s, NumberStyles.Number, (IFormatProvider)CultureInfo.InvariantCulture, out result))
                    return (object)result;
            }
            double result3;
            if (!double.TryParse(s, NumberStyles.Float, (IFormatProvider)CultureInfo.InvariantCulture, out result3))
                throw new ArgumentException(string.Format((IFormatProvider)CultureInfo.InvariantCulture, "Invalid JSON primitive: {0}.", (object)s));
            return (object)result3;
        }

        private string DeserializePrimitiveToken()
        {
            StringBuilder stringBuilder = new StringBuilder();
            char? nullable1 = new char?();
            while (true)
            {
                char? nullable2;
                char? nullable3 = nullable2 = this._s.MoveNext();
                if ((nullable3.HasValue ? new int?((int)nullable3.GetValueOrDefault()) : new int?()).HasValue)
                {
                    if (char.IsLetterOrDigit(nullable2.Value) || nullable2.Value == '.' || (nullable2.Value == '-' || nullable2.Value == '_' || nullable2.Value == '+'))
                        stringBuilder.Append((object)nullable2);
                    else
                        break;
                }
                else
                    goto label_5;
            }
            this._s.MovePrev();
        label_5:
            return stringBuilder.ToString();
        }

        private string DeserializeString()
        {
            StringBuilder sb = new StringBuilder();
            bool flag = false;
            char ch = this.CheckQuoteChar(this._s.MoveNext());
            while (true)
            {
                char? c;
                char? nullable1 = c = this._s.MoveNext();
                if ((nullable1.HasValue ? new int?((int)nullable1.GetValueOrDefault()) : new int?()).HasValue)
                {
                    char? nullable2 = c;
                    if (nullable2.GetValueOrDefault() == '\\' && nullable2.HasValue)
                    {
                        if (flag)
                        {
                            sb.Append('\\');
                            flag = false;
                        }
                        else
                            flag = true;
                    }
                    else if (flag)
                    {
                        this.AppendCharToBuilder(c, sb);
                        flag = false;
                    }
                    else
                    {
                        char? nullable3 = c;
                        int num = (int)ch;
                        if ((int)nullable3.GetValueOrDefault() != num || !nullable3.HasValue)
                            sb.Append((object)c);
                        else
                            goto label_9;
                    }
                }
                else
                    break;
            }
            throw new ArgumentException(this._s.GetDebugString("Unterminated string passed in."));
        label_9:
            return sb.ToString();
        }

        private object DeserializeStringIntoDateTime()
        {
            Match match = Regex.Match(this._s.ToString(), "^\"\\\\/Date\\((?<ticks>-?[0-9]+)(?:[a-zA-Z]|(?:\\+|-)[0-9]{4})?\\)\\\\/\"");
            long result;
            if (!long.TryParse(match.Groups["ticks"].Value, out result))
                return (object)this.DeserializeString();
            this._s.MoveNext(match.Length);
            long ticks = new DateTime(1970, 1, 1, 0, 0, 0, 1).Ticks;
            return (object)new DateTime(result * 10000L + ticks, DateTimeKind.Utc);
        }

        private static bool IsNextElementArray(char? c)
        {
            char? nullable = c;
            return nullable.GetValueOrDefault() == '[' && nullable.HasValue;
        }

        private bool IsNextElementDateTime()
        {
            string a = this._s.MoveNext(8);
            if (a == null)
                return false;
            this._s.MovePrev(8);
            return string.Equals(a, "\"\\/Date(", StringComparison.Ordinal);
        }

        private static bool IsNextElementObject(char? c)
        {
            char? nullable = c;
            return nullable.GetValueOrDefault() == '{' && nullable.HasValue;
        }

        private static bool IsNextElementString(char? c)
        {
            char? nullable1 = c;
            if (nullable1.GetValueOrDefault() == '"' && nullable1.HasValue)
                return true;
            char? nullable2 = c;
            return nullable2.GetValueOrDefault() == '\'' && nullable2.HasValue;
        }
    }
}

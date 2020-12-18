using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Helper
{
    internal class JavaScriptString
    {
        private int _index;
        private string _s;

        internal JavaScriptString(string s) => this._s = s;

        private static void AppendCharAsUnicode(StringBuilder builder, char c)
        {
            builder.Append("\\u");
            builder.AppendFormat((IFormatProvider)CultureInfo.InvariantCulture, "{0:x4}", (object)(int)c);
        }

        internal string GetDebugString(string message) => message + " (" + (object)this._index + "): " + this._s;

        internal char? GetNextNonEmptyChar()
        {
            while (this._s.Length > this._index)
            {
                char c = this._s[this._index++];
                if (!char.IsWhiteSpace(c))
                    return new char?(c);
            }
            return new char?();
        }

        internal char? MoveNext() => this._s.Length > this._index ? new char?(this._s[this._index++]) : new char?();

        internal string MoveNext(int count)
        {
            if (this._s.Length < this._index + count)
                return (string)null;
            string str = this._s.Substring(this._index, count);
            this._index += count;
            return str;
        }

        internal void MovePrev()
        {
            if (this._index <= 0)
                return;
            --this._index;
        }

        internal void MovePrev(int count)
        {
            for (; this._index > 0 && count > 0; --count)
                --this._index;
        }

        internal static string QuoteString(string value)
        {
            StringBuilder builder = (StringBuilder)null;
            if (string.IsNullOrEmpty(value))
                return string.Empty;
            int startIndex = 0;
            int count = 0;
            for (int index = 0; index < value.Length; ++index)
            {
                char c = value[index];
                int num;
                switch (c)
                {
                    case '\b':
                    case '\f':
                        num = 0;
                        break;
                    case '\t':
                    case '\r':
                    case '"':
                    case '\'':
                        num = 0;
                        break;
                    case '\n':
                    case '<':
                    case '>':
                    case '\\':
                        num = 0;
                        break;
                    default:
                        num = c >= ' ' ? 1 : 0;
                        break;
                }
                if (num == 0)
                {
                    if (builder == null)
                        builder = new StringBuilder(value.Length + 5);
                    if (count > 0)
                        builder.Append(value, startIndex, count);
                    startIndex = index + 1;
                    count = 0;
                }
                switch (c)
                {
                    case '\b':
                        builder.Append("\\b");
                        break;
                    case '\t':
                        builder.Append("\\t");
                        break;
                    case '\n':
                        builder.Append("\\n");
                        break;
                    case '\f':
                        builder.Append("\\f");
                        break;
                    case '\r':
                        builder.Append("\\r");
                        break;
                    case '"':
                        builder.Append("\\\"");
                        break;
                    case '\'':
                    case '<':
                    case '>':
                        JavaScriptString.AppendCharAsUnicode(builder, c);
                        break;
                    case '\\':
                        builder.Append("\\\\");
                        break;
                    default:
                        if (c < ' ')
                            JavaScriptString.AppendCharAsUnicode(builder, c);
                        else
                            ++count;
                        break;
                }
            }
            if (builder == null)
                return value;
            if (count > 0)
                builder.Append(value, startIndex, count);
            return builder.ToString();
        }

        internal static string QuoteString(string value, bool addQuotes)
        {
            string str = JavaScriptString.QuoteString(value);
            if (addQuotes)
                str = "\"" + str + "\"";
            return str;
        }

        public override string ToString() => this._s.Length > this._index ? this._s.Substring(this._index) : string.Empty;
    }
}

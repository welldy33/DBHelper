using System;
using System.Collections.Generic;

namespace Helper
{
    [Serializable]
    public class CustomsException:Exception
    {
        //    private Dictionary<string, object> _args;
        //    public CustomsException(string msg, params object[] args) : base(msg) {
        //        this._args = new Dictionary<string, object>();
        //        for (int index = 0; index < args.Length - 1; index += 2)
        //            this._args[args.Length > index + 1 ? args[index].ToString() : "ARG" + (object)(index / 2 + 1)] = args.Length > index + 1 ? args[index + 1] : args[index];
        //    }
        //    public Dictionary<string, object> Args => this._args;
        public CustomsException()
        {
        }

        public CustomsException(string message)
          : base(message)
        {
        }
        public CustomsException(Exception exception) :
                     this(exception.Message)
        {
        }
        public CustomsException(string message, Exception innerException)
        : base(message, innerException)
        {
        }
    }
}
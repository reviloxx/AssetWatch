using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssetWatch
{
    public class OnApiErrorEventArgs
    {
        public IApi Api { get; set; }

        public ErrorType ErrorType { get; set; }

        public string ErrorMessage { get; set; }
    }

    public enum ErrorType
    {
        General,
        Unauthorized,
        TooManyRequests
    }
}

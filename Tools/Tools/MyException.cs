using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tools
{
    public class MyException: Exception
    {
        public string outErrMsg { get; set; }
        public string InnerErrMsg { get; set; }
        public MyException(string outErrMsg, string InnerErrMsg)
        {
            this.outErrMsg = outErrMsg;this.InnerErrMsg = InnerErrMsg;
        }
    }
}

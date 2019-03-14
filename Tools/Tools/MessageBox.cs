using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tools
{
    public class MessageBox
    {
        public static string Show(string msg)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append("<script>");
            builder.Append("alert('"+ msg + "');");
            builder.Append("history.go(-1)");
            builder.Append("</script>");
            return builder.ToString();
        }
    }
}

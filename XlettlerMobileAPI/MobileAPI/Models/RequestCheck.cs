using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Models
{
    public class RequestCheck
    {
        public static string CheckStringValue(HttpRequestBase reqesut,string paraName,string title,bool isNull)
        {
           string val=reqesut[paraName];
            if ((!isNull)&&string.IsNullOrEmpty(val)) {
                throw new Exception($"【{paraName}:{title}】不可为空");
            }
            return val;
        }
        public static DateTime CheckDeteTimeValue(HttpRequestBase reqesut, string paraName, string title, bool isNull)
        {
            if (!DateTime.TryParse(reqesut[paraName], out DateTime val) && !isNull)
                throw new Exception($"【{paraName}:{title}】不可为空");
            return val;
        }
        public static int? CheckIntValue(HttpRequestBase reqesut, string paraName, string title, bool isNull)
        {
            if (!int.TryParse(reqesut[paraName], out int val) && !isNull)
                throw new Exception($"【{paraName}:{title}】不可为空");
            return val;
        }
        public static decimal CheckDecimalValue(HttpRequestBase reqesut, string paraName, string title, bool isNull)
        {
            if (!decimal.TryParse(reqesut[paraName], out decimal val) && !isNull)
                throw new Exception($"【{paraName}:{title}】不可为空");
            return val;
        }
    }
}

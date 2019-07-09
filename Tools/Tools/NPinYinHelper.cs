using NPinyin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tools
{
    public class NPinYinHelper
    {
        public static string TraformPinYin(string HanZi)
        {
            return Pinyin.GetPinyin(HanZi).Trim().Replace(" ", "");
        }
    }
}

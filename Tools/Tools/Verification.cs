using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Tools
{
    public class Verification
    {
        public static bool VerifyWithAccountID(string AccountID)
        {
            string regexstr = @"[\u4e00-\u9fa5]";
            if (Regex.IsMatch(AccountID, regexstr)) return true; 
            else return false;
        }
        public static bool IsRepeatHashSet(string[] array)
        {
            HashSet<string> hs = new HashSet<string>();
            for (int i = 0; i < array.Length; i++)
            {
                if (hs.Contains(array[i]))
                {
                    return true;
                }
                else
                {
                    hs.Add(array[i]);
                }
            }
            return false;
        }
        public static bool isImgFile(string ext)
        {
            ext=ext.ToUpper();
            switch (ext)
            {
                case "BMP":
                case "JPG":
                case "JPEG":
                case "PNG":
                case "GIF":
                    return true;
                default:
                    return false;
            }
        }
        public static string CaculateWeekDay(int y, int m, int d)
        {
            if (m == 1) m = 13;
            if (m == 2) m = 14;
            int week = (d + 2 * m + 3 * (m + 1) / 5 + y + y / 4 - y / 100 + y / 400) % 7 + 1;
            string weekstr = "";
            switch (week)
            {
                case 1: weekstr = "星期一"; break;
                case 2: weekstr = "星期二"; break;
                case 3: weekstr = "星期三"; break;
                case 4: weekstr = "星期四"; break;
                case 5: weekstr = "星期五"; break;
                case 6: weekstr = "星期六"; break;
                case 7: weekstr = "星期日"; break;
            }
            return weekstr;
        }
    }
}

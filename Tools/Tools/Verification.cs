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
    }
}

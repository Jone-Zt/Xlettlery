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
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Tools
{
    public class MD5Helper
    {
        public static string GetMd5Str(string encrapyStr)
        {
            byte[] result = Encoding.UTF8.GetBytes(encrapyStr);   
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] output = md5.ComputeHash(result);
            return BitConverter.ToString(output).Replace("-", "");  
        }
    }
}

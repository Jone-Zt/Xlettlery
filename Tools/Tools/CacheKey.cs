using PublicDefined;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tools
{
    public class CacheKey
    {
        private static char[] randomArray = new char[] { 'a', 'A', 'b', 'B', 'c', 'C', 'd', 'D', 'e', 'E', 'f', 'F',
            'g', 'G', 'h', 'H', 'i', 'I', 'j', 'J', 'k', 'K', 'l', 'L', 'm', 'M', 'n', 'N', 'o', 'O', 'p', 'P', 'q',
            'Q', 'r', 'R', 's', 'S', 't', 'T', 'u', 'U', 'v', 'V', 'w', 'W', 'x', 'X', 'y', 'Y', 'z', 'Z', '0', '1',
            '2', '3', '4', '5', '6', '7', '8', '9' };
        public static string PhoneCodeKey = "RK_PhoneCode";
        public static string GenerateCachePhoneCode(string phone, IPhoneCodeType type)
        {
            return $"{PhoneCodeKey}{phone}{type}";
        }
        public static string GenerateCacheGameBall<T>(string foolballID)
        {
            return $"{typeof(T)}:{foolballID}";
        }
        public static string GenerateRandomStr(int length)
        {
            StringBuilder RandomStr = new StringBuilder();
                Random random = new Random();
                for (int i = 0; i < length; i++)
                {
                    int index = random.Next(randomArray.Length);
                    RandomStr.Append(randomArray[index]);
                }
            return RandomStr.ToString();
        }
    }
}

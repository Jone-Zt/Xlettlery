using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DealManagement
{
   public class WorkOutUntiles
    {
        public string Descartes(List<string[]> list, int count, List<string> result, string data)
        {
            string temp = data;
            string[] astr = list[count];
            foreach (var item in astr)
            {
                StringBuilder builder = new StringBuilder();
                if (count + 1 < list.Count)
                {
                    temp += Descartes(list, count + 1, result, data+ item+",");
                }
                else
                {
                    result.Add(data+ item);
                }

            }
            return temp;
        }
    }
}

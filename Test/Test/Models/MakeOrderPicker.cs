using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test.Models
{
    [Serializable]
    public class MakeOrderPicker
    {
        public string payOrderId { get; set; }
        public string sign { get; set; }
        public payParams payParams { get; set; }
        public string retCode { get; set; }
    }
    [Serializable]
    public class payParams
    {
        public string payUrl { get; set; }
    }
}

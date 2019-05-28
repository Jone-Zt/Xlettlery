using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    //TODO  通用数据返回
    [Serializable]
    public class ResponsePicker<T> : IFormattable where T : class, new()
    {
        public ResponsePicker(string FlowID, ResType resType)
        {
            this.FlowID = FlowID;
            this.MsgCode = (int)resType;
        }
        public ResponsePicker(ResType resType)
        {
            this.FlowID = DateTime.Now.ToString("yyyyMMddmmss");
            this.MsgCode = (int)resType;
        }
        public ResponsePicker()
        {
            this.FlowID = DateTime.Now.ToString("yyyyMMddmmss");
            this.MsgCode = (int)ResType.Fail;
        }
        public string FlowID { get; set; }
        public string FailInfo { get; set; }
        public int? MsgCode { get; set; }
        public IList<T> List { get; set; }
        public T Data { get; set; }
        public override string ToString()
        {
            if (this.MsgCode == null)
            {
                if (this.Data != null || this.List != null) this.MsgCode = (int)ResType.Success;
                if (!string.IsNullOrEmpty(this.FailInfo)) this.MsgCode = (int)ResType.Fail;
            }
            JsonSerializerSettings jsetting = new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore };
            return JsonConvert.SerializeObject(this, Formatting.None, jsetting);
        }
        private bool TransformData(string transformData)
        {
            try
            {
                this.Data = JsonConvert.DeserializeObject<T>(transformData);
                MsgCode = (int)ResType.Success;
                return true;
            }
            catch (Exception)
            {
                FailInfo = "数据转换失败!";
                MsgCode = (int)ResType.Fail;
                return false;
            }
        }
        public static ResponsePicker<T> operator +(ResponsePicker<T> a, string data)
        {
            a.TransformData(data);
            return a;
        }
        public static implicit operator ResponsePicker<T>(string data)
        {
            ResponsePicker<T> picker = new ResponsePicker<T>(ResType.Fail);
            picker.TransformData(data);
            return picker;
        }

        public string ToString(string format, IFormatProvider formatProvider)
        {
            switch (format.ToLower())
            {
                case "data":
                    return GetData();
                default:
                    return base.ToString();
            }
        }
        private string GetData()
        {
            if (this.Data == null)
                throw new ArgumentNullException("Data");
            return JsonConvert.SerializeObject(this.Data);
        }
    }
    public enum ResType
    {
        Success = 200,
        Fail = 500,
        Unauthorized=401,
        NoFind=404,
    }
}

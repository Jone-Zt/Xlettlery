using Model;
using ServicesInterface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace XlettlerRealization
{
    [CallbackBehavior(UseSynchronizationContext = false)]
    public class MessageServices : IMessageInterface
    {
        public bool QueryInformation(int ID,out SESENT_InfoMation result, out string errMsg)
        {
            result = null;
            errMsg = string.Empty;
            try
            {
                using (ModelContainer container = new ModelContainer())
                {
                    result=container.SESENT_InfoMation.Where(a=>a.Id==ID).FirstOrDefault();
                    return true;
                }
            }
            catch (Exception err)
            {
                errMsg = "未知错误!";
                LogTool.LogWriter.WriteError("查询资讯信息失败!", err);
                return false;
            }
        }

        public bool QueryInformationWithList(out IList<Dictionary<string, object>> valuePairs, out string errMsg)
        {
            valuePairs = new List<Dictionary<string, object>>();
            errMsg = string.Empty;
            try
            {
                using (ModelContainer container = new ModelContainer())
                {
                    var result=container.SESENT_InfoMation.OrderBy(a => a.EnterTime).Take(5).ToList();
                    foreach (var item in result)
                    {
                        Dictionary<string, object> keyValuePairs = new Dictionary<string, object>();
                        keyValuePairs.Add("ID", item.Id);
                        keyValuePairs.Add("Keyword", item.Keyword);
                        keyValuePairs.Add("ReaderCount", item.ReaderCount);
                        keyValuePairs.Add("Title", item.Title);
                        keyValuePairs.Add("Cover", item.Conver);
                        keyValuePairs.Add("EnterTime", item.EnterTime);
                        valuePairs.Add(keyValuePairs);
                    }
                    return true;
                }
            }
            catch (Exception err)
            {
                errMsg = "未知错误!";
                LogTool.LogWriter.WriteError("查询资讯信息失败!", err);
                return false;
            }
        }
    }
}

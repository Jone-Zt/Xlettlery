using Model;
using PublicDefined;
using ServicesInterface;
using System;
using System.Collections.Generic;
using System.Linq;

namespace XlettlerRealization
{
    public class ShortMessageService : IShortMessageInterface
    {
        public bool QueryInbox(string AccountID, out IList<Dictionary<string,string>> result, out string errMsg)
        {
            result = null;
            errMsg = string.Empty;
            try
            {
                using (ModelContainer container=new ModelContainer())
                {
                   result = new List<Dictionary<string, string>>();
                   IQueryable<SESENT_MessageText> texts=container.SESENT_MessageText.Where(a => a.Type == (short)MessageObjType.All||(a.Type==(short)MessageObjType.Singler&&a.Recld==AccountID));
                    var item = texts.GetEnumerator();
                    while (item.MoveNext())
                    {
                        Dictionary<string, string> dic = new Dictionary<string, string>();
                        SESENT_MessageObj obj=container.SESENT_MessageObj.Where(a=>a.MessageID== item.Current.Id&&a.RecId==AccountID).FirstOrDefault();
                        dic.Add("ID", item.Current.Id.ToString());
                        if (obj != null)
                            dic.Add("Status", obj.Status.ToString());
                        else
                            dic.Add("Status", ((int)MessageStatus.Unreaded).ToString());
                        dic.Add("Title",item.Current.Title);
                        dic.Add("Content",item.Current.Content);
                        dic.Add("HasGift",(item.Current.HasGift?1:0).ToString());
                        dic.Add("CreateTime",item.Current.CreateDateTime.ToString("yyyy-MM-dd"));
                        dic.Add("SendID",item.Current.SendID);
                        result.Add(dic);
                    }
                }
                return true;
            }
            catch (Exception err)
            {
                errMsg = "未知错误!";
                LogTool.LogWriter.WriteError($"查询收件箱 【账号：{AccountID}】",err);
                return false;
            }
        }
    }
}

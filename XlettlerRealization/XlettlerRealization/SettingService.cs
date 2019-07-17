using Model;
using PublicDefined;
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
    public class SettingService : ISettingInterface
    {

        public bool Settingpage(SettingType type, out List<SESENT_Settings> result, out string errMsg)
        {
            errMsg = string.Empty;
            result = null;
            try
            {
                using (ModelContainer container = new ModelContainer())
                {
                    short Statu = (short)Status.Open;
                    short settingType = (short)type;
                    List<SESENT_Settings> queryDatas = container.SESENT_Settings.Where(a => a.Status == Statu && a.Type == settingType).ToList();
                    if (queryDatas == null || queryDatas.Count == 0) { errMsg = "未配置数据!"; return false;}else{ result = queryDatas;return true; }
                }
            }
            catch (Exception err)
            {
                errMsg = "未知错误!";
                LogTool.LogWriter.WriteError("轮播页错误", err);
                return false;
            }
        }
    }
}

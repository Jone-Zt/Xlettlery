using Model;
using PublicDefined;
using ServicesInterface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XlettlerRealization
{
    public class SettingService : ISettingInterface
    {

        public bool Settingpage(SettingType type, out IList<object> result, out string errMsg)
        {
            errMsg = string.Empty;
            result = null;
            try
            {
                using (ModelContainer container = new ModelContainer())
                {
                    short Statu = (short)Status.Open;
                    short settingType = (short)type;
                    result = container.SESENT_Settings.Where(a => a.Status == Statu && a.Type == settingType).ToList() as IList<object>;
                    if (result == null || result.Count == 0) {
                        errMsg = "未配置数据!";
                        return false;
                    }
                    return true;
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

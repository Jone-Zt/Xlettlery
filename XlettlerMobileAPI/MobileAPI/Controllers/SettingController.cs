using Models;
using PublicDefined;
using ServicesInterface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Tools;

namespace MobileAPI.Controllers
{
    public class SettingController : Controller
    {
        private ISettingInterface GetManger()
        {
            ISettingInterface proxy = null;
            if (proxy == null)
            {
                proxy = RemotingAngency.GetRemoting().GetProxy<ISettingInterface>();
            }
            return proxy;
        }

        /// <summary>
        /// 获取引导页配置信息
        /// </summary>
        /// <returns></returns>
        public ActionResult GettingConfig()
        {
            ResponsePicker<object> picker = new ResponsePicker<object>();
            try
            {
                string flowid = RequestCheck.CheckStringValue(Request, "flowID", "流水号", false);
                picker.FlowID = flowid;
                int? type = RequestCheck.CheckIntValue(Request,"type","配置类别",false);
                SettingType Settingtype = (SettingType)type;
                ISettingInterface proxy = GetManger();
                if (proxy == null) 
                    throw new Exception("未挂载函数!");
                if (proxy.Settingpage(Settingtype, out IList<object> result, out string errMsg))
                    picker.List = result;
                else
                    picker.FailInfo = errMsg;
            }
            catch (Exception err)
            {
                picker.FailInfo = err.Message;
            }
            return Content(picker.ToString());
        }
    }
}
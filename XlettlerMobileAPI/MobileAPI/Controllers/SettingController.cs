using Model;
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
    //[Authorize]
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
            ResponsePicker<SESENT_Settings> picker = new ResponsePicker<SESENT_Settings>();
            try
            {
                string flowid = RequestCheck.CheckStringValue(Request, "flowID", "流水号", false);
                picker.FlowID = flowid;
                int? type = RequestCheck.CheckIntValue(Request,"type","配置类别",false);
                SettingType Settingtype = (SettingType)type;
                ISettingInterface proxy = GetManger();
                if (proxy == null) 
                    throw new Exception("未挂载函数!");
                if (proxy.Settingpage(Settingtype, out List<SESENT_Settings> result, out string errMsg))
                    picker.List = result;
                else
                    picker.FailInfo = errMsg;
            }
            catch (Exception err)
            {
                picker.FailInfo = (err as MyException)?.outErrMsg ?? err.Message;
            }
            return Content(picker.ToString());
        }
    }
}
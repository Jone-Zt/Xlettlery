using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AdmainManger.Filter
{
    public class CheckLoginAttribute: ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (filterContext.ActionDescriptor.ControllerDescriptor.IsDefined(typeof(SkipCheckLoginAttribute), false)){ return;}
            if (filterContext.ActionDescriptor.IsDefined(typeof(SkipCheckLoginAttribute), false)){return;}
            if (filterContext.HttpContext.Session["User"] == null){ filterContext.HttpContext.Response.Redirect("/Login/Index");}
        }
    }
}
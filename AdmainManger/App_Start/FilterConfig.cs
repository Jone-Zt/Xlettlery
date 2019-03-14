using AdmainManger.Filter;
using System.Web;
using System.Web.Mvc;

namespace AdmainManger
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
            filters.Add(new CheckLoginAttribute());
        }
    }
}

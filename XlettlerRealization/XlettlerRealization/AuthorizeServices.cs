using AOPHandlerManager.MethordHandler;
using Model;
using ServicesInterface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XlettlerRealization
{
    public class AuthorizeServices : IAuthorizeInterface
    {
        [MethordTimingHandler]
        public bool CheckAuthorzeServices(string UserName, string UserPwd)
        {
            using (ModelContainer db = new ModelContainer()) 
            {
                SESENT_USERS uSERS= db.SESENT_USERS.Where(a => a.AccountID == UserName || a.Phone == UserName).FirstOrDefault();
                if (uSERS == null)
                    return false;
                if (!string.IsNullOrEmpty(UserPwd))
                   return uSERS.userPwd == UserPwd;
                return true;
            }
        }
    }
}

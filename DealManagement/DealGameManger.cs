using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DealManagement
{
    public class DealGameManger
    {
        private DealGameManger() { }
        private static DealGameManger _dealGameManger;
        private static object _sync = new object();
        public static DealGameManger GetManger()
        {
            if (_dealGameManger == null)
            {
                lock (_sync)
                {
                    Interlocked.CompareExchange(ref _dealGameManger, new DealGameManger(), null);
                }
            }
            return _dealGameManger;
        }



        //public bool WorkFootBallAmount(Dictionary<string,string> Fids,int type,out decimal result,out string errMeg, int Multiple = 1)
        //{
        //    errMeg = string.Empty;
        //    result = 0;
        //    try
        //    {
        //        using (Model.ModelContainer container = new Model.ModelContainer())
        //        {
        //            if (Fids != null && Fids.Count > 0)
        //            {
                        
        //            }
        //        }
        //    }
        //    catch (Exception err)
        //    {
        //        errMeg = "计算失败!";
        //        LogTool.LogWriter.WriteError($"计算足球价格失败:{err.Message}");
        //    }
        //}

    }
}

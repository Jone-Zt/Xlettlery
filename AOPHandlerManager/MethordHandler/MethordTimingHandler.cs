using PostSharp.Aspects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AOPHandlerManager.MethordHandler
{
    public class MethordTimingHandler: OnMethodBoundaryAspect
    {
        public override void OnEntry(MethodExecutionArgs args)
        {
            args.MethodExecutionTag = System.Diagnostics.Stopwatch.StartNew();
            base.OnEntry(args);
        }
        public override void OnExit(MethodExecutionArgs args)
        {
            var sw = args.MethodExecutionTag as System.Diagnostics.Stopwatch;
            if (sw != null)
            {
                sw.Stop();
                LogTool.LogWriter.WriteInfo(String.Format("方法{0}执行时间为:{1}s", args.Method.Name, sw.ElapsedMilliseconds / 1000));
                sw = null;
            }
            base.OnExit(args);
        }
    }
}

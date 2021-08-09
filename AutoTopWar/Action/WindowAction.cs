using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoTopWar.Action
{
    public class WindowAction
    {
        public static void CloseAllNoxProcess()
        {
            KAutoHelper.ADBHelper.ExecuteCMD(Constants.Constant.QUIT_ALL_NOX_PROCESS_CMD);
            KAutoHelper.ADBHelper.ExecuteCMD(Constants.Constant.QUIT_ALL_NOX_HANDLE_PROCESS_CMD);
        }

        public static KAutoHelper.NoxMultiIni GetNoxPid(int index)
        {
            var mutilNox = KAutoHelper.NoxMultiIni.GetNoxMultiIni();
            return mutilNox[index];
        }

        public static void CloseNoxWithPid(int index)
        {
            KAutoHelper.NoxMultiIni nox2Kill = GetNoxPid(index);
            KAutoHelper.ADBHelper.ExecuteCMD(string.Format(Constants.Constant.KILL_TASK_PROCESS_BY_PID_CMD, nox2Kill.pid));
            KAutoHelper.ADBHelper.ExecuteCMD(string.Format(Constants.Constant.KILL_TASK_PROCESS_BY_PID_CMD, nox2Kill.vmpid));
        }
    }
}

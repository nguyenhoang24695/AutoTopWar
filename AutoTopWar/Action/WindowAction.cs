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
    }
}

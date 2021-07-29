using AutoTopWar.Action;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoTopWar.Job
{
    public class GeneralJob
    {
        public static void OpenEmulator(int[] emulatorIdList)
        {
            Parallel.ForEach(emulatorIdList, id => {
                AndroidAction.StartNox(id);
            });
        }
    }
}

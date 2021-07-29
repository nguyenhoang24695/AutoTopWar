using AutoTopWar.Entity;
using Common.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoTopWar.Common.Utilities
{
    public class DataUtil
    {
        public static void SyncEmulatorData()
        {
            GetIdIpData();
            GlobalVariants.ID_IP_LIST.ForEach(i => {
                var emulator = GlobalVariants.EMULATOR_LIST.Where(em => em.Id == i.Id).FirstOrDefault();
                if (emulator != null)
                {
                    emulator.Ip = i.Ip;
                }                
            });
        }
        
        public static void GetIdIpData()
        {
            var path = Path.Combine(Environment.CurrentDirectory, Constants.Constant.IP_CONFIG_FILE_PATH);
            if (FileUtil.CheckExistFile(path))
            {
                GlobalVariants.ID_IP_LIST = FileUtil.LoadJsonFile<List<IpIdEntity>>(path);
            }
        }
    }
}

using AutoTopWar.Action;
using AutoTopWar.Common;
using AutoTopWar.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AutoTopWar.Job
{
    public class WorldJob
    {
        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(typeof(WorldJob));
        public void Rally(int emulatorId, int level, int rallyType)
        {
            bool isComplete = false;
            int tempCount = emulatorId;
            string deviceID = null;
            Log.Info(string.Format("Start Rally level {1} for Device ID: {0}", tempCount, level));
        a: try
            {
                new Thread(() => AndroidAction.StartNox(tempCount)).Start();
                var device = GlobalVariants.EMULATOR_LIST.Where(em => em.Id == tempCount).First();
                var listDevice = KAutoHelper.ADBHelper.GetDevices();
                while (true)
                {
                    if (listDevice.Count > 0 && listDevice[0].Contains(Constant.NOX_IP_DEFAULT))
                    {
                        if (listDevice.Contains(device.Ip))
                        {
                            deviceID = device.Ip;
                            break;
                        }

                    }

                    listDevice = KAutoHelper.ADBHelper.GetDevices();

                }

                IngameAction.CloseAdv(deviceID);
                IngameAction.OpenWorldMap(deviceID);

                //Kiếm tra xem Rally đã về thành chưa
                while (true)
                {
                    if (!AndroidAction.ExistImage(deviceID, "pic/world_queue"))
                    {
                        Thread.Sleep(10000);
                        continue;
                    }
                    IngameAction.OpenSearch(deviceID);
                    IngameAction.OpenRallyTab(deviceID);

                    //BumBum
                    if (rallyType == 2)
                    {
                        AndroidAction.ClickImageInstant(deviceID, "pic/search/rally/bumbum");

                        KAutoHelper.ADBHelper.TapByPercent(deviceID, 48.2, 60.9);
                    }
                    if (level == 1)
                    {
                        while (!AndroidAction.ExistImageInstant(deviceID, "pic/search/rally/lv10"))
                        {
                            KAutoHelper.ADBHelper.TapByPercent(deviceID, 23.2, 66.2);
                        }
                    }
                    if (level == 2)
                    {
                        int count = 0;
                        while (!AndroidAction.ExistImageInstant(deviceID, "pic/search/rally/lv20"))
                        {
                            if (count < 10)
                            {
                                KAutoHelper.ADBHelper.TapByPercent(deviceID, 76.5, 66.3);
                            }
                            else
                            {
                                KAutoHelper.ADBHelper.TapByPercent(deviceID, 23.2, 66.2);
                            }
                        }
                    }
                    IngameAction.OpenSearch(deviceID);

                    //Click vao boss bua chien

                    Thread.Sleep(4000);

                    // An vao bua chien
                    while (!AndroidAction.ClickImageInstant(deviceID, "pic/search/rally/boss_info"))
                    {
                        KAutoHelper.ADBHelper.TapByPercent(deviceID, 49.7, 45.7);
                        Thread.Sleep(1000);
                    }

                    if (!AndroidAction.ExistImageInstant(deviceID, "pic/search/battle"))
                    {
                        // Coi như hết thể lực
                        isComplete = true;
                        throw new Exception(string.Format("emulator IP: {0} het the luc", deviceID));
                    }

                    KAutoHelper.ADBHelper.TapByPercent(deviceID, 8.5, 91.6);
                    Thread.Sleep(2000);
                    KAutoHelper.ADBHelper.TapByPercent(deviceID, 49.1, 41.4);

                    if (!AndroidAction.ExistImage(deviceID, "pic/search"))
                    {
                        // Vấn đề khi start battle
                        throw new Exception(string.Format("emulator IP: {0} start battle loi", deviceID));
                    };
                    isComplete = true;
                };

            }
            catch (Exception ex)
            {
                try
                {
                    Log.Error(String.Format("Error in Rally:{0}", ex.Message));
                    Common.Utilities.ImageUtil.SaveImageToFile(KAutoHelper.ADBHelper.ScreenShoot(deviceID), string.Format("logs\\screenshot\\{0}.png", DateTime.Now.ToString("yyyyMMddHHmmss")));

                }
                catch (Exception ex2)
                {

                    Log.Error(String.Format("Error in Rally:{0}", ex2.Message));
                }

            }
            finally
            {
                new Thread(() => AndroidAction.QuitNox(tempCount)).Start();
            }

        }
    }
}

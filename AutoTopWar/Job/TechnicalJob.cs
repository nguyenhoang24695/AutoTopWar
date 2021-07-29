using KAutoHelper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using AutoTopWar.Action;
using AutoTopWar.Constants;
using AutoTopWar.Common;

namespace AutoTopWar.Job
{
    public class TechnicalJob
    {
        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(typeof(TechnicalJob));
        public void UpTech(int emulatorId)
        {
            int tempCount = emulatorId;
            Log.Info(string.Format("Start Uptech for Device ID: {0}", tempCount));
            try
            {

                new Thread(() => AndroidAction.StartNox(tempCount)).Start();
                string deviceID = null;
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

                //var openapp = AndroidAction.TapByImage(deviceID, "pic/openapp");
                //ADBHelper.TapByPercent(deviceID, 71.9, 33.0);
                //FileInfo[] files = new DirectoryInfo("pic").GetFiles();


                AndroidAction.WaitForTapByImage(deviceID, "pic/closeadv", 5);
                AndroidAction.TapByImage(deviceID, "pic/clanBtn");
                AndroidAction.TapByImage(deviceID, "pic/alliance_tech");
                while (!AndroidAction.TapByImage(deviceID, "pic/item_tech"))
                {
                    // Click Battle Tab
                    KAutoHelper.ADBHelper.TapByPercent(deviceID, 50.3, 9.3);
                }

                if (AndroidAction.ExistImage(deviceID, "pic/item_tech"))
                {

                    // Tang Diem Cong nghe
                    for (int i = 0; i < 10; i++)
                    {

                        // Click donate
                        KAutoHelper.ADBHelper.TapByPercent(deviceID, 72.6, 71.4);
                        Thread.Sleep(3000);

                        // Check het tien hoac len cap
                        if (AndroidAction.ExistImageInstant(deviceID, "pic/item_tech"))
                        {
                            continue;
                        }
                        if (AndroidAction.ExistImageInstant(deviceID, "pic/out_gold"))
                        {
                            KAutoHelper.ADBHelper.TapByPercent(deviceID, 66.6, 66.5);
                            Thread.Sleep(2000);
                            KAutoHelper.ADBHelper.TapByPercent(deviceID, 72.0, 54.1);
                            Thread.Sleep(2000);
                            KAutoHelper.ADBHelper.TapByPercent(deviceID, 72.0, 54.1);
                            Thread.Sleep(2000);
                            KAutoHelper.ADBHelper.TapByPercent(deviceID, 49.7, 68.2);
                            Thread.Sleep(2000);
                            KAutoHelper.ADBHelper.TapByPercent(deviceID, 50.0, 74.1);
                            Thread.Sleep(2000);
                            KAutoHelper.ADBHelper.TapByPercent(deviceID, 90.3, 14.5);
                            Thread.Sleep(2000);
                            Console.WriteLine("Het vang");
                            continue;
                        }
                        if (AndroidAction.ExistImageInstant(deviceID, "pic/level_up"))
                        {

                            Console.WriteLine("Len cap");
                            KAutoHelper.ADBHelper.TapByPercent(deviceID, 49.1, 80.9);
                            Thread.Sleep(2000);
                            continue;
                        }
                        break;
                    }

                }
                // Close up tech
                KAutoHelper.ADBHelper.TapByPercent(deviceID, 90.3, 15.4);
                Thread.Sleep(2000);
                //Close allance tech
                KAutoHelper.ADBHelper.TapByPercent(deviceID, 7.3, 3.2);
                Thread.Sleep(2000);
                //Close Alliance
                KAutoHelper.ADBHelper.TapByPercent(deviceID, 7.3, 3.2);
                Thread.Sleep(2000);

                //var isTap = ADBHelper.FindImageAndClick(deviceID, "pic");

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                new Thread(() => AndroidAction.QuitNox(tempCount)).Start();

            }


        }
    }
}

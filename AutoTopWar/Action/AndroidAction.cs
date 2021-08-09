using KAutoHelper;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using AutoTopWar.Constants;
using System.Linq;
using System.Threading;
using AutoTopWar.Common;

namespace AutoTopWar.Action
{
    public class AndroidAction
    {
        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(typeof(AndroidAction));
        public static bool TapByImage(string deviceID, string path)
        {
            Log.Info(String.Format("Tap By Image: IP:{0}, path: {1}", deviceID, path));
            for (int i = 0; i < 5; i++)
            {
                Log.Info(String.Format("Tap By Image: IP:{0}, path: {1}, Try count: {2} ", deviceID, path, i));
                var tap = ADBHelper.FindImageAndClick(deviceID, path);
                if (tap)
                {
                    return true;
                }
            }
            Log.Error(string.Format("Tap By Image fail in device ID:{0} with image path: {1}", deviceID, path));
            throw new Exception(deviceID + ":" + path);
        }

        public static bool WaitForTapByImage(string deviceID, string path, int waitMinuteTime)
        {
            Log.Info(String.Format("Wait for Tap By Image: IP:{0}, path: {1}", deviceID, path));
            var timeToWait = DateTime.Now.AddMinutes(double.Parse(waitMinuteTime.ToString()));
            while (true)
            {
                var currentTime = DateTime.Now;
                if (timeToWait < currentTime)
                {
                    break;
                }
                Log.Info(String.Format("Wait for Tap By Image: IP:{0}, path: {1}, Try at time: {2} ", deviceID, path, currentTime));
                var tap = ADBHelper.FindImageAndClick(deviceID, path);
                if (tap)
                {
                    Log.Info(String.Format("Wait for Tap By Image: IP:{0}, path: {1}, Success At time: {2}. ", deviceID, path, currentTime));
                    return true;
                }
            }
            Log.Error(string.Format("Wait for Tap By Image fail in device ID:{0} with image path: {1} cause out of time:{2}", deviceID, path, timeToWait));
            throw new Exception(deviceID + ":" + path);
        }

        public static bool ExistImage(string deviceID, string path)
        {
            Log.Info(String.Format("Check Exist Image: IP:{0}, path: {1}", deviceID, path));
            for (int i = 0; i < 3; i++)
            {
                Log.Info(String.Format("Check Exist Image: IP:{0}, path: {1}, Try count: {2} ", deviceID, path, i));
                var tap = ADBHelper.FindImage(deviceID, path);
                if (tap != null)
                {
                    return true;
                }
            }
            return false;
        }
        public static bool ExistImageInstant(string deviceID, string path)
        {
            Log.Info(String.Format("Check Exist Image Instant: IP:{0}, path: {1}", deviceID, path));
            var tap = ADBHelper.FindImage(deviceID, path, 2000, 2);
            if (tap != null)
            {
                return true;
            }
            return false;
        }

        public static bool ClickImageInstant(string deviceID, string path)
        {
            Log.Info(String.Format("Check Exist Image Instant: IP:{0}, path: {1}", deviceID, path));
            var tap = ADBHelper.FindImageAndClick(deviceID, path, 2000, 2);
            if (tap)
            {
                return true;
            }
            return false;
        }

        public static void StartNox(int index)
        {
            Log.Info(String.Format("Start nox with Nox ID: {0}", index));
            ExecuteCommand(string.Format(Constant.START_NOX, index, GlobalVariants.GENERAL_CONFIG.EmulatorPath));
        }

        public static void GetEmulatorIp(int id)
        {
            int temp = id;
            Log.Info(String.Format("Get Emolator Id with Nox ID: {0}", temp));

            new Thread(() => AndroidAction.StartNox(temp)).Start();

            List<string> listDevice = ADBHelper.GetDevices();

            string result = string.Empty;
            while (true)
            {
                if (listDevice.Count != 0 && listDevice[0].Contains(Constant.NOX_IP_DEFAULT))
                {

                    listDevice.ForEach(l =>
                    {
                        if (!GlobalVariants.ID_IP_LIST.Select(il => il.Ip).Contains(l))
                        {
                            result = l;
                            GlobalVariants.EMULATOR_LIST.Where(el => el.Id == id).First().Ip = l;
                            var ii = GlobalVariants.ID_IP_LIST.Where(el => el.Id == id).FirstOrDefault();
                            if (ii == null)
                            {
                                GlobalVariants.ID_IP_LIST.Add(new Entity.IpIdEntity()
                                {
                                    Id = id,
                                    Ip = l
                                });
                            }
                        }

                    });
                    if (result != string.Empty)
                    {

                        break;
                    }

                }
                listDevice = KAutoHelper.ADBHelper.GetDevices();
            }

            new Thread(() => AndroidAction.QuitNox(temp)).Start();

        }

        public static void QuitNox(int index)
        {
            ExecuteCommand(string.Format(Constant.QUIT_NOX, index, GlobalVariants.GENERAL_CONFIG.EmulatorPath));
        }
        public static string ExecuteCommand(string cmdCommand)
        {
            try
            {
                Process process = new Process();
                process.StartInfo = new ProcessStartInfo()
                {
                    WorkingDirectory = "",
                    FileName = "cmd.exe",
                    CreateNoWindow = true,
                    UseShellExecute = false,
                    WindowStyle = ProcessWindowStyle.Hidden,
                    RedirectStandardInput = true,
                    RedirectStandardOutput = true,
                    Verb = "runas"
                };
                process.Start();
                process.StandardInput.WriteLine(cmdCommand);
                process.StandardInput.Flush();
                process.StandardInput.Close();
                process.WaitForExit();
                return process.StandardOutput.ReadToEnd();
            }
            catch
            {
                return (string)null;
            }
        }
        public static List<string> getDeviceList()
        {
            var listDevice = KAutoHelper.ADBHelper.GetDevices();
            if (listDevice.Count > 0 && listDevice[0].Contains(Constant.NOX_IP_DEFAULT))
            {
                return listDevice;

            }
            return new List<string>();
        }
    }
}

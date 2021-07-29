using System;
using System.Collections.Generic;
using System.Text;

namespace AutoTopWar.Constants
{
    public class Constant
    {
        public static readonly string NOX_PATH = "D:\\Program Files\\Nox\\bin\\Nox.exe";
        public static readonly string START_NOX = "\"" + NOX_PATH + "\" -clone:Nox_{0} -resolution:540x960 -package:com.topwar.gp";
        public static readonly string QUIT_NOX = "\"" + NOX_PATH + "\" -clone:Nox_{0} -quit";
        public static readonly string VMS_FOLDER_PATH = "D:\\Program Files\\Nox\\bin\\BignoxVMS";
        public static readonly string NOX_IP_DEFAULT = "127.0.0.1";

        public static readonly string QUIT_ALL_NOX_PROCESS_CMD = "TASKKILL /IM nox.exe /F";
        public static readonly string QUIT_ALL_NOX_HANDLE_PROCESS_CMD = "TASKKILL /IM NoxVMHandle.exe /F";



        public static readonly string NOX_VMS_FOLDER = "BignoxVMS";
        public static readonly string CONFIG_FILE_PATH = "config.json";
        public static readonly string IP_CONFIG_FILE_PATH = "ip_config.json";



    }
}

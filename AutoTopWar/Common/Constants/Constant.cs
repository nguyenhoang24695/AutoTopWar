using System;
using System.Collections.Generic;
using System.Text;

namespace AutoTopWar.Constants
{
    public class Constant
    {
        //public static readonly string NOX_PATH = "F:\\Nox\\bin\\Nox.exe";
        public static readonly string START_NOX = "{1} -clone:Nox_{0} -resolution:540x960 -package:com.topwar.gp";
        public static readonly string QUIT_NOX = "{1} -clone:Nox_{0} -quit";
        public static readonly string VMS_FOLDER_PATH = "F:\\Nox\\bin\\BignoxVMS";
        public static readonly string NOX_IP_DEFAULT = "127.0.0.1";

        public static readonly string QUIT_ALL_NOX_PROCESS_CMD = "TASKKILL /IM nox.exe /F";
        public static readonly string QUIT_ALL_NOX_HANDLE_PROCESS_CMD = "TASKKILL /IM NoxVMHandle.exe /F";

        public static readonly string KILL_TASK_PROCESS_BY_PID_CMD = "taskkill /F /PID {0}";

        public static readonly string OPEN_LOG_FILE_CMD = "start logs\\{0}.log";



        public static readonly string NOX_VMS_FOLDER = "BignoxVMS";
        public static readonly string CONFIG_FILE_PATH = "config.json";
        public static readonly string IP_CONFIG_FILE_PATH = "ip_config.json";
        
        
        
        //Job
        public static readonly string RALLY_JOB = "Rally";
        public static readonly string UP_TECH_JOB = "Up Tech";





    }
}

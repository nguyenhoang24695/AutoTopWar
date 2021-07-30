using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoTopWar.Action
{
    public class IngameAction
    {
        public static void CloseAdv(string deviceID)
        {
            AndroidAction.WaitForTapByImage(deviceID, "pic/closeadv", 5);
        }

        public static void OpenClanPanel(string deviceID)
        {
            AndroidAction.TapByImage(deviceID, "pic/clanBtn");
        }

        public static void OpenAllianceTechTab(string deviceID)
        {
                AndroidAction.TapByImage(deviceID, "pic/alliance_tech");
        }

        public static void OpenWorldMap(string deviceID)
        {
            AndroidAction.TapByImage(deviceID, "pic/world_map");
        }

        public static void OpenSearch(string deviceID)
        {
            AndroidAction.TapByImage(deviceID, "pic/search");
        }

        public static void OpenRallyTab(string deviceID)
        {
            AndroidAction.TapByImage(deviceID, "pic/search/rally");
        }
    }
}

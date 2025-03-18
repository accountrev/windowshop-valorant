using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Windowshop
{
    public static class WindowshopGlobals
    {
        public static string version = "0.9.0";


        // Main window reference
        public static MainWindow mainWindow;
        public static LoadingScreen loadingScreen;


        // General variables
        public static bool menuLoaded { get; set; } = false;
        public static bool loggedIn { get; set; }
        public static bool alreadyShownMinimizedNoti { get; set; }


        // Riot account variables
        public static Dictionary<string, string> riotAccountTokens = new Dictionary<string, string> {
            {"tdid", "" },
            {"asid", "" },
            {"clid", "" },
            {"ssid", "" },
            {"csid", "" },
            {"sub", "" },
        };
        public static string riotAccessToken { get; set; }
        public static string riotIdToken { get; set; }
        public static string entitlementsToken { get; set; }
        public static string puuid { get; set; }
        public static string shard {  get; set; }


        // Shop related variables
        public static bool shopRetreived { get; set; } = false;
        public static long lastShopRefresh { get; set; }
        public static JObject shopData { get; set; }
        public static List<JObject> rawShopItems { get; set; }
        public static List<Dictionary<string, dynamic>>? mainShopItems { get; set; }
        public static int nextShopReset { get; set; }


        // Item view related variables
        public static int itemSelected = 0;
        public static int chromaSelected = 0;
        public static int levelSelected = 0;
        public static bool lookingAtChromas = false;
        public static string currentVideoUrl = "";

    }
}

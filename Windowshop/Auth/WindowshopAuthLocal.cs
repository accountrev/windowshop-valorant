using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windowshop.Helpers;
using Windowshop.Utility;

namespace Windowshop.Auth
{
    internal class WindowshopAuthLocal
    {
        public async Task Start()
        {
            WindowshopGlobals.loadingScreen.GetViewModel().ChangeLoadingScreenStatus("Logging in using saved credentials...");

            if (AppDataHandler.Exists("riot_tokens_DO_NOT_SHARE"))
            {
                string json = AppDataHandler.ReadFile("riot_tokens_DO_NOT_SHARE");
                WindowshopGlobals.riotAccountTokens = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);

                await WindowshopUtil.RefreshToken();
            }
            else
            {
                ErrorHandler.ThrowAndExit("Unable to find riot_tokens_DO_NOT_SHARE file. Restart Windowshop to get new credentials.");
            }
        }
    }
}

using System.IO;
using Windowshop.Utility;

namespace Windowshop.Helpers
{
    internal partial class Setup
    {
        bool updateNeeded = false;


        public async Task Initialize()
        {
            WindowshopGlobals.loadingScreen.GetViewModel().ChangeLoadingScreenStatus("Checking manifest (1/4)...");

            // check if the local files exist or outdated
            if (AppDataHandler.Exists("valorant_manifest"))
            {
                using (StreamReader sr = new StreamReader(AppDataHandler.PathToFile("valorant_manifest")))
                {
                    var oldManifest = sr.ReadToEnd();
                    var newManifest = await WindowshopUtil.AcquireValorantManifest();

                    sr.Close();

                    if (oldManifest != newManifest)
                    {
                        WindowshopGlobals.loadingScreen.GetViewModel().ChangeLoadingScreenStatus("Updating manifest (1/4)...");

                        // updates needed!
                        updateNeeded = true;
                        using (StreamWriter sw = new StreamWriter(AppDataHandler.PathToFile("valorant_manifest")))
                        {
                            string manifest = await WindowshopUtil.AcquireValorantManifest();
                            sw.Write(manifest);
                            sw.Close();
                        }
                    }
                }
            }
            else
            {
                WindowshopGlobals.loadingScreen.GetViewModel().ChangeLoadingScreenStatus("Creating manifest (1/4)...");

                // user might be running program for the first time, so create the file
                updateNeeded = true;

                using (StreamWriter sw = new StreamWriter(AppDataHandler.PathToFile("valorant_manifest")))
                {
                    string manifest = await WindowshopUtil.AcquireValorantManifest();
                    sw.Write(manifest);
                    sw.Close();
                }
            }


            if (updateNeeded)
            {
                WindowshopGlobals.loadingScreen.GetViewModel().ChangeLoadingScreenStatus("Downloading Valorant skin data (1/5)...");

                using (StreamWriter sw = new StreamWriter(AppDataHandler.PathToFile("valorant_skins_data.json")))
                {
                    // download the entire valorant skin api
                    var skinData = await WindowshopUtil.AcquireValorantSkinsData();
                    sw.Write(skinData);
                    sw.Close();
                }

                WindowshopGlobals.loadingScreen.GetViewModel().ChangeLoadingScreenStatus("Downloading Valorant content tiers data (2/5)...");

                using (StreamWriter sw = new StreamWriter(AppDataHandler.PathToFile("valorant_content_tiers_data.json")))
                {
                    // download the entire valorant skin api
                    var contentTierData = await WindowshopUtil.AcquireValorantContentTiersData();
                    sw.Write(contentTierData);
                    sw.Close();
                }
            }


            // after log in user, get entitlements
            WindowshopGlobals.loadingScreen.GetViewModel().ChangeLoadingScreenStatus("Getting entitlements (3/5)...");
            WindowshopGlobals.entitlementsToken = await WindowshopUtil.AcquireEntitlementsToken();

            // get puuid after entitlements
            WindowshopGlobals.loadingScreen.GetViewModel().ChangeLoadingScreenStatus("Getting player UUID (4/5)...");
            WindowshopGlobals.puuid = await WindowshopUtil.AcquirePUUID();

            // get shard
            WindowshopGlobals.loadingScreen.GetViewModel().ChangeLoadingScreenStatus("Getting player shard (5/5)...");
            WindowshopGlobals.shard = await WindowshopUtil.AcquireShard();

            WindowshopGlobals.loadingScreen.GetViewModel().ChangeLoadingScreenStatus("Starting app and other dependencies...");
        }








    }
}

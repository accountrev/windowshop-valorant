using Microsoft.Toolkit.Uwp.Notifications;
using Newtonsoft.Json.Linq;
using System.Diagnostics;
using System.Windows.Media.Imaging;
using Windowshop.Utility;

namespace Windowshop.Helpers
{
    class ShopUpdater
    {
        private int shopTimer;

        public async Task Start()
        {
            while (true)
            {
                await RefreshShop();

                Timer();

                await Task.Delay(WindowshopGlobals.nextShopReset * 1000 + 10000);
            }
        }

        public async Task RefreshShop()
        {
            WindowshopGlobals.menuLoaded = false;

            Trace.WriteLine("Refreshing shop...");

            if (WindowshopGlobals.rawShopItems == null)
            {
                WindowshopGlobals.rawShopItems = new List<JObject>();
            }

            if (WindowshopGlobals.mainShopItems == null)
            {
                WindowshopGlobals.mainShopItems = new List<Dictionary<string, dynamic>>();
            }

            WindowshopGlobals.mainShopItems.Clear();
            WindowshopGlobals.rawShopItems.Clear();

            // Get shop
            WindowshopGlobals.shopData = await WindowshopUtil.AcquireShopV3();

            try
            {
                JArray singleItemStoreOffers = null;

                singleItemStoreOffers = WindowshopGlobals.shopData["SkinsPanelLayout"]["SingleItemStoreOffers"] as JArray;


                foreach (var offer in singleItemStoreOffers)
                {
                    string offerID = offer["OfferID"].ToString();
                    JObject skin = WindowshopUtil.AcquireSkinFromOfferID(offerID);

                    WindowshopGlobals.rawShopItems.Add(skin);

                    string name = WindowshopUtil.AcquireNameOfSkin(skin);
                    string id = WindowshopUtil.AcquireIDFromSkin(skin);
                    int cost = (int)offer["Cost"]["85ad13f7-3d1b-5128-9eb2-7cd8ee0b5741"];





                    // set up images
                    List<BitmapImage> chromaImages = new List<BitmapImage>();
                    for (int i = 0; i < WindowshopUtil.AcquireChromaCountOfSkin(skin); i++)
                    {
                        chromaImages.Add(await WindowshopUtil.AcquireImageOfSkin(skin, i));
                    }

                    List<BitmapImage> levelImages = new List<BitmapImage>();
                    for (int i = 0; i < WindowshopUtil.AcquireLevelsCountOfSkin(skin); i++)
                    {
                        levelImages.Add(await WindowshopUtil.AcquireImageOfSkin(skin, 0, i));
                    }

                    Dictionary<string, dynamic> images = new Dictionary<string, dynamic>()
                    {
                        {"default", await WindowshopUtil.AcquireImageOfSkin(skin)},
                        {"chromas", chromaImages},
                        {"levels", levelImages}
                    };




                    // set up names
                    List<string> chromaNames = new List<string>();
                    for (int i = 0; i < WindowshopUtil.AcquireChromaCountOfSkin(skin); i++)
                    {
                        chromaNames.Add(WindowshopUtil.AcquireNameOfSkin(skin, i));
                    }

                    List<string> levelNames = new List<string>();
                    for (int i = 0; i < WindowshopUtil.AcquireLevelsCountOfSkin(skin); i++)
                    {
                        levelNames.Add(WindowshopUtil.AcquireNameOfSkin(skin, 0, i));
                    }

                    Dictionary<string, dynamic> names = new Dictionary<string, dynamic>()
                    {
                        {"default", WindowshopUtil.AcquireNameOfSkin(skin)},
                        {"chromas", chromaNames},
                        {"levels", levelNames}
                    };



                    // set up levels
                    List<string> levels = new List<string>();
                    Trace.WriteLine(WindowshopUtil.AcquireLevelsCountOfSkin(skin));
                    for (int i = 0; i < WindowshopUtil.AcquireLevelsCountOfSkin(skin); i++)
                    {
                        Trace.WriteLine(i);
                        levels.Add(WindowshopUtil.AcquireLevelNameOfSkin(skin, i));
                    }


                    // set up chroma swatches
                    List<BitmapImage> chromaSwatches = new List<BitmapImage>();

                    for (int i = 0; i < WindowshopUtil.AcquireChromaCountOfSkin(skin); i++)
                    {
                        chromaSwatches.Add(await WindowshopUtil.AcquireChromaSwatchOfSkin(skin, i));
                    }


                    // set up videos

                    List<string> chromaVideos = new List<string>();

                    for (int i = 0; i < WindowshopUtil.AcquireChromaCountOfSkin(skin); i++)
                    {
                        chromaVideos.Add(WindowshopUtil.AcquireVideoURLOfSkin(skin, true, i));
                    }

                    List<string> levelsVideos = new List<string>();

                    for (int i = 0; i < WindowshopUtil.AcquireLevelsCountOfSkin(skin); i++)
                    {
                        levelsVideos.Add(WindowshopUtil.AcquireVideoURLOfSkin(skin, false, i));
                    }

                    Dictionary<string, dynamic> videos = new Dictionary<string, dynamic>()
                    {
                        {"chromas", chromaVideos},
                        {"levels", levelsVideos}
                    };


                    string rarity = WindowshopUtil.AcquireRarityNameOfSkin(skin);

                    BitmapImage rarityImg = await WindowshopUtil.AcquireRarityImgFromRarityName(rarity);
                    string rarityColor = WindowshopUtil.AcquireRarityColorFromRarityName(rarity);

                    WindowshopGlobals.mainShopItems.Add(
                        new Dictionary<string, dynamic>()
                        {
                            {"id", id},
                            {"name", names},
                            {"cost", cost},
                            {"images", images},
                            {"rarityImg", rarityImg},
                            {"rarityColor", rarityColor},
                            {"levels", levels},
                            {"chromas", chromaSwatches},
                            {"videos", videos}
                        }
                    );
                }



                WindowshopGlobals.nextShopReset = (int)WindowshopGlobals.shopData["SkinsPanelLayout"]["SingleItemOffersRemainingDurationInSeconds"];
                shopTimer = WindowshopGlobals.nextShopReset;

                WindowshopGlobals.shopRetreived = true;
                WindowshopGlobals.lastShopRefresh = DateTimeOffset.Now.ToUnixTimeMilliseconds();
                WindowshopGlobals.mainWindow.GetViewModel().RefreshMainShopMenu();
                WindowshopGlobals.mainWindow.UpdateGradients();

                WindowshopGlobals.menuLoaded = true;

                ShowShopNoti();
            }
            catch (Exception e)
            {
                ErrorHandler.ThrowAndExit("Something went wrong refreshing the shop. Please try again, and if problem persists report it.", e.ToString());
            }


        }


        private async Task Timer()
        {
            while (shopTimer > 0)
            {
                shopTimer--;
                WindowshopGlobals.mainWindow.GetViewModel().RefreshTimer(shopTimer.ToString());
                await Task.Delay(1000);
            }
        }

        private void ShowShopNoti()
        {
            new ToastContentBuilder()
                .AddText("Your VALORANT shop as of " + DateTime.Now.ToString("MMMM dd, yyyy"))
                .AddText(
                    Truncate(WindowshopGlobals.mainShopItems[0]["name"]["default"]) + " - " + WindowshopGlobals.mainShopItems[0]["cost"].ToString() + " VP\n" +
                    Truncate(WindowshopGlobals.mainShopItems[1]["name"]["default"]) + " - " + WindowshopGlobals.mainShopItems[1]["cost"].ToString() + " VP\n" +
                    Truncate(WindowshopGlobals.mainShopItems[2]["name"]["default"]) + " - " + WindowshopGlobals.mainShopItems[2]["cost"].ToString() + " VP\n" +
                    Truncate(WindowshopGlobals.mainShopItems[3]["name"]["default"]) + " - " + WindowshopGlobals.mainShopItems[3]["cost"].ToString() + " VP\n"
                    )
                .AddAudio(new Uri("ms-winsoundevent:Notification.Mail"))
                .AddArgument("action", "open")
                .Show();
        }

        private string Truncate(string value)
        {
            return value.Length <= 30 ? value : value.Substring(0, 30) + "...";
        }

    }
}

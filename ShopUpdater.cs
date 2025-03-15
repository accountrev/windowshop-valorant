using Microsoft.Toolkit.Uwp.Notifications;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Windowshop
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

                await Task.Delay((WindowshopGlobals.nextShopReset * 1000) + 10000);
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
            WindowshopGlobals.shopData = await Util.AcquireShopV3();

            try
            {
                JArray singleItemStoreOffers = null;

                singleItemStoreOffers = WindowshopGlobals.shopData["SkinsPanelLayout"]["SingleItemStoreOffers"] as JArray;


                foreach (var offer in singleItemStoreOffers)
                {
                    string offerID = offer["OfferID"].ToString();
                    JObject skin = Util.AcquireSkinFromOfferID(offerID);

                    WindowshopGlobals.rawShopItems.Add(skin);

                    string name = Util.AcquireNameOfSkin(skin);
                    string id = Util.AcquireIDFromSkin(skin);
                    int cost = (int)offer["Cost"]["85ad13f7-3d1b-5128-9eb2-7cd8ee0b5741"];





                    // set up images
                    List<BitmapImage> chromaImages = new List<BitmapImage>();
                    for (int i = 0; i < Util.AcquireChromaCountOfSkin(skin); i++)
                    {
                        chromaImages.Add(await Util.AcquireImageOfSkin(skin, i));
                    }

                    List<BitmapImage> levelImages = new List<BitmapImage>();
                    for (int i = 0; i < Util.AcquireLevelsCountOfSkin(skin); i++)
                    {
                        levelImages.Add(await Util.AcquireImageOfSkin(skin, 0, i));
                    }

                    Dictionary <string, dynamic> images = new Dictionary<string, dynamic>()
                    {
                        {"default", await Util.AcquireImageOfSkin(skin)},
                        {"chromas", chromaImages},
                        {"levels", levelImages}
                    };




                    // set up names
                    List<string> chromaNames = new List<string>();
                    for (int i = 0; i < Util.AcquireChromaCountOfSkin(skin); i++)
                    {
                        chromaNames.Add(Util.AcquireNameOfSkin(skin, i));
                    }

                    List<string> levelNames = new List<string>();
                    for (int i = 0; i < Util.AcquireLevelsCountOfSkin(skin); i++)
                    {
                        levelNames.Add(Util.AcquireNameOfSkin(skin, 0, i));
                    }

                    Dictionary<string, dynamic> names = new Dictionary<string, dynamic>()
                    {
                        {"default", Util.AcquireNameOfSkin(skin)},
                        {"chromas", chromaNames},
                        {"levels", levelNames}
                    };



                    // set up levels
                    List<string> levels = new List<string>();
                    Trace.WriteLine(Util.AcquireLevelsCountOfSkin(skin));
                    for (int i = 0; i < Util.AcquireLevelsCountOfSkin(skin); i++)
                    {
                        Trace.WriteLine(i);
                        levels.Add(Util.AcquireLevelNameOfSkin(skin, i));
                    }


                    // set up chroma swatches
                    List<BitmapImage> chromaSwatches = new List<BitmapImage>();

                    for (int i = 0; i < Util.AcquireChromaCountOfSkin(skin); i++)
                    {
                        chromaSwatches.Add(await Util.AcquireChromaSwatchOfSkin(skin, i));
                    }


                    // set up videos

                    List<string> chromaVideos = new List<string>();

                    for (int i = 0; i < Util.AcquireChromaCountOfSkin(skin); i++)
                    {
                        chromaVideos.Add(Util.AcquireVideoURLOfSkin(skin, true, i));
                    }

                    List<string> levelsVideos = new List<string>();

                    for (int i = 0; i < Util.AcquireLevelsCountOfSkin(skin); i++)
                    {
                        levelsVideos.Add(Util.AcquireVideoURLOfSkin(skin, false, i));
                    }
                    
                    Dictionary<string, dynamic> videos = new Dictionary<string, dynamic>()
                    {
                        {"chromas", chromaVideos},
                        {"levels", levelsVideos}
                    };


                    string rarity = Util.AcquireRarityNameOfSkin(skin);

                    BitmapImage rarityImg = await Util.AcquireRarityImgFromRarityName(rarity);
                    string rarityColor = Util.AcquireRarityColorFromRarityName(rarity);

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

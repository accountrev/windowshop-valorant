using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Xml.XPath;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TaskbarClock;

namespace Windowshop
{

    public class WPFGlobalsBridge : ViewModelBase
    {
        private string item1Name = "Loading...";
        private string item2Name = "Loading...";
        private string item3Name = "Loading...";
        private string item4Name = "Loading...";

        private string item1Price = "????";
        private string item2Price = "????";
        private string item3Price = "????";
        private string item4Price = "????";

        private string shopTimerSec = "Loading...";

        private string loadingScreenStatus = "Starting...";

        private BitmapImage item1Img = Util.LoadLocalImage("/Resources/no_item_placeholder.png");
        private BitmapImage item2Img = Util.LoadLocalImage("/Resources/no_item_placeholder.png");
        private BitmapImage item3Img = Util.LoadLocalImage("/Resources/no_item_placeholder.png");
        private BitmapImage item4Img = Util.LoadLocalImage("/Resources/no_item_placeholder.png");

        private BitmapImage item1RarityImg = Util.LoadLocalImage("/Resources/no_tier_placeholder.png");
        private BitmapImage item2RarityImg = Util.LoadLocalImage("/Resources/no_tier_placeholder.png");
        private BitmapImage item3RarityImg = Util.LoadLocalImage("/Resources/no_tier_placeholder.png");
        private BitmapImage item4RarityImg = Util.LoadLocalImage("/Resources/no_tier_placeholder.png");

        private string item1RarityColor;
        private string item2RarityColor;
        private string item3RarityColor;
        private string item4RarityColor;

        private string itemViewName = "SAMPLE TEXT";
        private string itemViewDescription = "SAMPLE TEXT";
        private BitmapImage itemViewImg;
        private BitmapImage itemViewRarityImg;




        // text strings
        public string LoadingScreenStatus
        {
            get { return loadingScreenStatus; }
            set { loadingScreenStatus = value; OnPropertyChanged(nameof(LoadingScreenStatus)); }
        }

        public string Item1NameText
        {
            get { return item1Name; }
            set { item1Name = value; OnPropertyChanged(nameof(Item1NameText)); }
        }
        public string Item2NameText
        {
            get { return item2Name; }
            set { item2Name = value; OnPropertyChanged(nameof(Item2NameText)); }
        }
        public string Item3NameText
        {
            get { return item3Name; }
            set { item3Name = value; OnPropertyChanged(nameof(Item3NameText)); }
        }
        public string Item4NameText
        {
            get { return item4Name; }
            set { item4Name = value; OnPropertyChanged(nameof(Item4NameText)); }
        }
        public string Item1PriceText
        {
            get { return item1Price; }
            set { item1Price = value; OnPropertyChanged("Item1PriceText"); }
        }
        public string Item2PriceText
        {
            get { return item2Price; }
            set { item2Price = value; OnPropertyChanged("Item2PriceText"); }
        }
        public string Item3PriceText
        {
            get { return item3Price; }
            set { item3Price = value; OnPropertyChanged("Item3PriceText"); }
        }
        public string Item4PriceText
        {
            get { return item4Price; }
            set { item4Price = value; OnPropertyChanged("Item4PriceText"); }
        }

        public string ShopTimerSec
        {
            get { return shopTimerSec; }
            set { shopTimerSec = value; OnPropertyChanged("ShopTimerSec"); }
        }

        public BitmapImage Item1Img
        {
            get { return item1Img; }
            set { item1Img = value; OnPropertyChanged("Item1Img"); }
        }

        public BitmapImage Item2Img
        {
            get { return item2Img; }
            set { item2Img = value; OnPropertyChanged("Item2Img"); }
        }

        public BitmapImage Item3Img
        {
            get { return item3Img; }
            set { item3Img = value; OnPropertyChanged("Item3Img"); }
        }

        public BitmapImage Item4Img
        {
            get { return item4Img; }
            set { item4Img = value; OnPropertyChanged("Item4Img"); }
        }

        public BitmapImage Item1RarityImg
        {
            get { return item1RarityImg; }
            set { item1RarityImg = value; OnPropertyChanged("Item1RarityImg"); }
        }

        public BitmapImage Item2RarityImg
        {
            get { return item2RarityImg; }
            set { item2RarityImg = value; OnPropertyChanged("Item2RarityImg"); }
        }
        public BitmapImage Item3RarityImg
        {
            get { return item3RarityImg; }
            set { item3RarityImg = value; OnPropertyChanged("Item3RarityImg"); }
        }
        public BitmapImage Item4RarityImg
        {
            get { return item4RarityImg; }
            set { item4RarityImg = value; OnPropertyChanged("Item4RarityImg"); }
        }

        public string Item1RarityColor
        {
            get { return item1RarityColor; }
            set { item1RarityColor = value; OnPropertyChanged("Item1RarityColor"); }
        }

        public string Item2RarityColor
        {
            get { return item2RarityColor; }
            set { item2RarityColor = value; OnPropertyChanged("Item2RarityColor"); }
        }

        public string Item3RarityColor
        {
            get { return item3RarityColor; }
            set { item3RarityColor = value; OnPropertyChanged("Item3RarityColor"); }
        }

        public string Item4RarityColor
        {
            get { return item4RarityColor; }
            set { item4RarityColor = value; OnPropertyChanged("Item4RarityColor"); }
        }

        public string ItemViewName
        {
            get { return itemViewName; }
            set { itemViewName = value; OnPropertyChanged("ItemViewName"); }
        }

        public string ItemViewDescription
        {
            get { return itemViewDescription; }
            set { itemViewDescription = value; OnPropertyChanged("ItemViewDescription"); }
        }

        public BitmapImage ItemViewImg
        {
            get { return itemViewImg; }
            set { itemViewImg = value; OnPropertyChanged("ItemViewImg"); }
        }

        public BitmapImage ItemViewRarityImg
        {
            get { return itemViewRarityImg; }
            set { itemViewRarityImg = value; OnPropertyChanged("ItemViewRarityImg"); }
        }

        public void RefreshMainShopMenu()
        {
            Item1NameText = ((Dictionary<string, dynamic>)WindowshopGlobals.mainShopItems[0]["name"])["default"].ToString().ToUpper();
            Item2NameText = ((Dictionary<string, dynamic>)WindowshopGlobals.mainShopItems[1]["name"])["default"].ToString().ToUpper(); ;
            Item3NameText = ((Dictionary<string, dynamic>)WindowshopGlobals.mainShopItems[2]["name"])["default"].ToString().ToUpper(); ;
            Item4NameText = ((Dictionary<string, dynamic>)WindowshopGlobals.mainShopItems[3]["name"])["default"].ToString().ToUpper(); ;
            Item1PriceText = WindowshopGlobals.mainShopItems[0]["cost"].ToString().ToUpper();
            Item2PriceText = WindowshopGlobals.mainShopItems[1]["cost"].ToString().ToUpper();
            Item3PriceText = WindowshopGlobals.mainShopItems[2]["cost"].ToString().ToUpper();
            Item4PriceText = WindowshopGlobals.mainShopItems[3]["cost"].ToString().ToUpper();
            Item1Img = (BitmapImage)((Dictionary<string, dynamic>)WindowshopGlobals.mainShopItems[0]["images"])["default"];
            Item2Img = (BitmapImage)((Dictionary<string, dynamic>)WindowshopGlobals.mainShopItems[1]["images"])["default"];
            Item3Img = (BitmapImage)((Dictionary<string, dynamic>)WindowshopGlobals.mainShopItems[2]["images"])["default"];
            Item4Img = (BitmapImage)((Dictionary<string, dynamic>)WindowshopGlobals.mainShopItems[3]["images"])["default"];
            Item1RarityImg = (BitmapImage)WindowshopGlobals.mainShopItems[0]["rarityImg"];
            Item2RarityImg = (BitmapImage)WindowshopGlobals.mainShopItems[1]["rarityImg"];
            Item3RarityImg = (BitmapImage)WindowshopGlobals.mainShopItems[2]["rarityImg"];
            Item4RarityImg = (BitmapImage)WindowshopGlobals.mainShopItems[3]["rarityImg"];
        }

        public void RefreshItemViewMenu(string show = "levels")
        {
            int showIndex = 0;
            if (show == "levels")
                showIndex = WindowshopGlobals.levelSelected;
            else if (show == "chromas")
                showIndex = WindowshopGlobals.chromaSelected;

            foreach (var item in WindowshopGlobals.mainShopItems[WindowshopGlobals.itemSelected]["images"][show])
            {
                Trace.WriteLine((BitmapImage)item);
            }

            ItemViewImg = WindowshopGlobals.mainShopItems[WindowshopGlobals.itemSelected]["images"][show][showIndex];

            var skinString = WindowshopGlobals.mainShopItems[WindowshopGlobals.itemSelected]["name"][show][showIndex].ToString().ToUpper();

            string[] splitSkinString = skinString.Split(new[] { "\n" }, StringSplitOptions.None);

            ItemViewName = splitSkinString[0].Replace("\r", string.Empty);
            ItemViewDescription = splitSkinString.Length > 1 ? splitSkinString[1] : string.Empty;

            ItemViewRarityImg = (BitmapImage)WindowshopGlobals.mainShopItems[WindowshopGlobals.itemSelected]["rarityImg"];

            WindowshopGlobals.mainWindow.CheckForVideo();
        }

        public void RefreshTimer(string sec)
        {
            TimeSpan secToTime = TimeSpan.FromSeconds(Convert.ToDouble(sec));
            ShopTimerSec = secToTime.ToString(@"hh\:mm\:ss");
        }

        public void ChangeLoadingScreenStatus(string status)
        {
            LoadingScreenStatus = status;
        }
    }
}

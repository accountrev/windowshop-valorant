using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Windowshop
{
    internal class WindowshopApp
    {
        MainWindow mainWindow;
        ShopUpdater shopUpdater;
        AuthUpdater authUpdater;
        NotificationHandler notificationHandler;

        public async void Start(MainWindow mainWindowInstance)
        {
            WindowshopGlobals.mainWindow = mainWindowInstance;

            notificationHandler = new NotificationHandler();

            shopUpdater = new ShopUpdater();
            shopUpdater.Start();

            authUpdater = new AuthUpdater();
            authUpdater.Start();


        }
    }
}

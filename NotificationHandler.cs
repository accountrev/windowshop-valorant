using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Toolkit.Uwp.Notifications;
using Windows.Storage;

namespace Windowshop
{
    class NotificationHandler
    {
        public NotificationHandler()
        {
            ToastNotificationManagerCompat.OnActivated += ToastNotificationManagerCompat_OnActivated;
        }

        private void ToastNotificationManagerCompat_OnActivated(ToastNotificationActivatedEventArgsCompat args)
        {
            if (args.Argument.Contains("open"))
            {
                WindowshopGlobals.mainWindow.ShowWindow();
            }
        }
    }
}

using Microsoft.Toolkit.Uwp.Notifications;

namespace Windowshop.Helpers
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

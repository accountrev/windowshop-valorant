using System.Windows;
using MessageBox = System.Windows.MessageBox;

namespace Windowshop.Helpers
{
    internal class ErrorHandler
    {
        public static MessageBoxResult Throw(string message = "", string ex = "", MessageBoxButton actions = MessageBoxButton.OK)
        {
            var messageBoxResult = MessageBox.Show($"{message}\n\n{ex}", "Windowshop - Error", actions, MessageBoxImage.Error);

            return messageBoxResult;
        }

        public static void ThrowAndExit(string message, string ex = "")
        {
            Throw(message, ex);
            Environment.Exit(0);
        }
    }
}

using IWshRuntimeLibrary;
using System.Diagnostics;
using System.IO;
using System.Windows;
using File = System.IO.File;
using MessageBox = System.Windows.MessageBox;

namespace Windowshop.Helpers
{
    internal class AutostartHandler
    {
        bool isStartupLaunch;

        public AutostartHandler(bool _isStartupLaunch = false)
        {
            isStartupLaunch = _isStartupLaunch;
        }


        public void Start()
        {
            string startupFolderPath = "";

            try
            {
                startupFolderPath = Environment.GetFolderPath(Environment.SpecialFolder.Startup);
            }
            catch
            {
                Trace.WriteLine("Couldn't find Startup.");
            }

            try
            {
                if (!AppDataHandler.Exists("no_startup") && startupFolderPath != "")
                {
                    if (!File.Exists(startupFolderPath + @"\Windowshop.lnk") && !isStartupLaunch)
                    {
                        var result = MessageBox.Show(
                            "Would you like Windowshop to launch when you start up your computer? This feature can be turned off if you change your mind.",
                            "Windowshop",
                            MessageBoxButton.YesNo
                        );

                        if (result == MessageBoxResult.Yes)
                        {
                            // Get the path to your application's executable
                            string appPath = Process.GetCurrentProcess().MainModule.FileName;

                            // Create a shortcut name
                            string shortcutPath = Path.Combine(startupFolderPath, "Windowshop.lnk");

                            // Create the shortcut
                            WshShell shell = new WshShell();
                            IWshShortcut shortcut = (IWshShortcut)shell.CreateShortcut(shortcutPath);
                            shortcut.TargetPath = appPath;
                            shortcut.Arguments = "--startup";
                            shortcut.WorkingDirectory = Path.GetDirectoryName(appPath);
                            shortcut.Description = "Windowshop";
                            shortcut.Save();
                        }
                        else
                        {
                            AppDataHandler.WriteFile("no_startup", "");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.Throw("Something went wrong when creating a shortcut to StartupFolder. Try running Windowshop as an administrator next time.", ex.StackTrace.ToString());
            }
        }
    }
}

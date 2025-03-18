using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Windowshop
{
    internal class AppDataHandler
    {
        private static string GetAppDataFolder()
        {
            string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string appFolder = Path.Combine(appDataPath, "Windowshop");

            return appFolder;
        }

        private static bool AppFolderExists()
        {
            string appFolder = GetAppDataFolder();
            if (!Directory.Exists(appFolder))
            {
                return false;
            }

            return true;
        }

        public static void CreateAppDataFolder()
        {
            string appFolder = GetAppDataFolder();
            if (!AppFolderExists())
            {
                try
                {
                    Directory.CreateDirectory(appFolder);
                }
                catch (Exception e)
                {
                    ErrorHandler.ThrowAndExit("Failed to create AppData folder. Try running Windowshop with administrative privileges.", e.ToString());
                }
                
            }
        }

        public static bool WriteFile(string fileName, string content)
        {
            if (!AppFolderExists())
            {
                CreateAppDataFolder();
            }

            try
            {
                File.WriteAllText(GetAppDataFolder() + "/" + fileName, content);
            }
            catch
            {
                return false;
            }

            return true;
        }

        public static string ReadFile(string fileName)
        {
            if (!AppFolderExists())
            {
                CreateAppDataFolder();
            }

            string content = "";

            try
            {
                content = File.ReadAllText(GetAppDataFolder() + "/" + fileName);
            }
            catch
            {
                return "";
            }

            return content;
        }

        public static string PathToFile(string fileName)
        {
            return GetAppDataFolder() + "/" + fileName;
        }

        public static bool DeleteFile(string fileName)
        {
            if (!AppFolderExists())
            {
                CreateAppDataFolder();
            }

            try
            {
                File.Delete(GetAppDataFolder() + "/" + fileName);
            }
            catch
            {
                return false;
            }

            return true;
        }

        public static bool Exists(string fileName)
        {
            bool exists = File.Exists(GetAppDataFolder() + "/" + fileName);
            return exists;
        }


    }
}

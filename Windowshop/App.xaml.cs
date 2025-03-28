using System.Windows;
using Windowshop.Auth;
using Windowshop.Helpers;

namespace Windowshop
{
    public partial class App : System.Windows.Application
    {
        private static Mutex mutex;

        protected override async void OnStartup(StartupEventArgs e)
        {
            const string appName = "Windowshop";
            bool createdNew;

            mutex = new Mutex(true, appName, out createdNew);

            if (!createdNew)
            {
                ErrorHandler.ThrowAndExit("Windowshop is already running! Please check your system tray to open Windowshop.");
            }

            var githubChecker = new GithubChecker();
            await githubChecker.CheckForUpdates();


            AppDataHandler.CreateAppDataFolder();

            bool isStartupLaunch = e.Args.Contains("--startup");
            AutostartHandler autoStartHandler = new AutostartHandler(isStartupLaunch);
            autoStartHandler.Start();



            base.OnStartup(e);



            LoadingScreen loadingScreen = new LoadingScreen();
            WindowshopGlobals.loadingScreen = loadingScreen;

            if (!isStartupLaunch)
                loadingScreen.Show();

            if (AppDataHandler.Exists("riot_tokens_DO_NOT_SHARE"))
            {
                WindowshopAuthLocal auth = new WindowshopAuthLocal();
                await auth.Start();
            }
            else
            {
                WindowshopAuth auth = new WindowshopAuth();
                auth.Start();
            }

            Setup setup = new Setup();
            await setup.Initialize();

            MainWindow mainWindow = new MainWindow();
            Current.MainWindow = mainWindow;
            loadingScreen.Close();

            if (!isStartupLaunch)
                mainWindow.Show();

            WindowshopApp windowshopInstance = new WindowshopApp();
            windowshopInstance.Start(mainWindow);
        }

        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);
        }
    }

}

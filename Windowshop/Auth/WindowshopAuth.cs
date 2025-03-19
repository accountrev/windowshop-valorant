using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.WinForms;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Windowshop.Helpers;

namespace Windowshop.Auth
{
    internal partial class WindowshopAuth
    {
        static Form form;
        static WebView2 webView;

        [STAThread]
        public void Start()
        {
            StartWebViewWindow().GetAwaiter().GetResult();
        }

        private async Task StartWebViewWindow()
        {
            WindowshopGlobals.loadingScreen.GetViewModel().ChangeLoadingScreenStatus("Please log in to Riot Games to continue. Your credentials will be saved for next time.");
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            form = new Form
            {
                Text = "Windowshop - Log in to Riot Games to continue (don't close this window!)",
                Width = 1200,
                Height = 1000,
                StartPosition = FormStartPosition.CenterScreen
            };

            TableLayoutPanel tableLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                RowCount = 2,
                ColumnCount = 1
            };
            tableLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 50)); // Panel row
            tableLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100)); // WebView2 row

            Panel panel = new Panel
            {
                Dock = DockStyle.Fill
            };

            webView = new WebView2
            {
                Dock = DockStyle.Fill
            };

            Button backButton = new Button
            {
                Text = "Previous\n<-",
                Dock = DockStyle.Left
            };

            Button forwardButton = new Button
            {
                Text = "Next\n->",
                Dock = DockStyle.Left
            };

            backButton.Click += (sender, e) =>
            {
                if (webView.CanGoBack)
                {
                    webView.GoBack();
                }
            };

            forwardButton.Click += (sender, e) =>
            {
                if (webView.CanGoForward)
                {
                    webView.GoForward();
                }
            };



            // Add buttons to the panel
            panel.Controls.Add(forwardButton);
            panel.Controls.Add(backButton);

            // Add the panel and WebView2 to the TableLayoutPanel
            tableLayout.Controls.Add(panel, 0, 0);
            tableLayout.Controls.Add(webView, 0, 1);

            // Add the TableLayoutPanel to the form
            form.Controls.Add(tableLayout);


            form.Load += async (sender, e) =>
            {
                await webView.EnsureCoreWebView2Async(null);
                webView.CoreWebView2.CookieManager.DeleteAllCookies();
                webView.CoreWebView2.NavigationCompleted += CoreWebView2_NavigationCompleted;
                webView.CoreWebView2.WebResourceResponseReceived += CoreWebView2_WebResourceResponseReceived;
                webView.CoreWebView2.Navigate("https://auth.riotgames.com/authorize?redirect_uri=https%3A%2F%2Fplayvalorant.com%2Fopt_in&client_id=play-valorant-web-prod&response_type=token%20id_token&nonce=1&scope=account%20openid");

            };

            Application.Run(form);
        }

        private void CoreWebView2_NavigationCompleted(object sender, CoreWebView2NavigationCompletedEventArgs e)
        {
            string url = webView.CoreWebView2.Source;

            if (url.Contains("access_token"))
            {
                WindowshopGlobals.loadingScreen.GetViewModel().ChangeLoadingScreenStatus("Getting access and ID token...");

                var uri = new Uri(url);
                var fragment = uri.Fragment.TrimStart('#');
                var queryParams = System.Web.HttpUtility.ParseQueryString(fragment);

                WindowshopGlobals.riotAccessToken = queryParams["access_token"];
                WindowshopGlobals.riotIdToken = queryParams["id_token"];

                WindowshopGlobals.loadingScreen.GetViewModel().ChangeLoadingScreenStatus("Saving Riot tokens...");

                try
                {
                    string json = JsonConvert.SerializeObject(WindowshopGlobals.riotAccountTokens, Formatting.Indented);
                    AppDataHandler.WriteFile("riot_tokens_DO_NOT_SHARE", json);
                }
                catch (Exception ex)
                {
                    ErrorHandler.ThrowAndExit("Failed to save Riot tokens. Try running Windowshop as an administrator.", ex.StackTrace.ToString());
                }

                form.Close();
            }
        }

        private static void CoreWebView2_WebResourceResponseReceived(object sender, CoreWebView2WebResourceResponseReceivedEventArgs e)
        {
            foreach (var header in e.Response.Headers)
            {
                if (header.Key == "set-cookie")
                {
                    var counterToEqls = 0;
                    foreach (char character in header.Value)
                    {
                        if (character == '=')
                        {
                            break;
                        }
                        counterToEqls++;
                    }

                    string tokenType = header.Value.Substring(0, counterToEqls);
                    string tokenData = header.Value.Substring(counterToEqls + 1);

                    if (WindowshopGlobals.riotAccountTokens.ContainsKey(tokenType))
                        WindowshopGlobals.riotAccountTokens[tokenType] = tokenData;
                }
            }
        }
    }
}

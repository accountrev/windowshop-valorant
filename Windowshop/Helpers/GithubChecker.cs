using System.Diagnostics;
using System.Net.Http;
using System.Windows;
using Newtonsoft.Json.Linq;

namespace Windowshop.Helpers
{
    class GithubChecker
    {
        public async Task CheckForUpdates()
        {
            var client = new HttpClient();

            HttpRequestMessage msg_github = new HttpRequestMessage(HttpMethod.Get, "https://api.github.com/repos/accountrev/windowshop-valorant/releases");
            msg_github.Headers.Add("User-Agent", "Windowshop");

            var response_github = await client.SendAsync(msg_github);
            var content_github = await response_github.Content.ReadAsStringAsync();

            Trace.WriteLine(content_github);

            var json_github = JArray.Parse(content_github);

            var latest_release_version = new Version(json_github[0]["tag_name"].ToString());
            var current_version = new Version(WindowshopGlobals.version);

            if (latest_release_version > current_version)
            {
                var result = System.Windows.MessageBox.Show(
                    $"A new update was found for Windowshop on Github. The new version is version {json_github[0]["tag_name"].ToString()}, while the current version is version {WindowshopGlobals.version}. Would you like to close Windowshop and download the new update?",
                    "Windowshop - New Update",
                    MessageBoxButton.YesNo
                );

                if (result == MessageBoxResult.Yes)
                {
                    Process.Start(new ProcessStartInfo(json_github[0]["html_url"].ToString()) { UseShellExecute = true });
                    Environment.Exit(0);
                }
            }
        }
    }
}

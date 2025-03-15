using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.IO;
using System.Drawing;
using System.Net;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace Windowshop
{
    internal class Util
    {
        private static HttpClient client = new HttpClient();

        #region Riot API Calls
        public async static Task<string> AcquireEntitlementsToken()
        {
            if (WindowshopGlobals.riotAccessToken == null)
            {
                ErrorHandler.ThrowAndExit("Failed to retrive data for Windowshop.", "Could not get entitlements token because Riot access token was not found.");
                return "";
            }

            try
            {
                HttpRequestMessage msg_entitlement = new HttpRequestMessage(HttpMethod.Post, "https://entitlements.auth.riotgames.com/api/token/v1");
                msg_entitlement.Content = JsonContent.Create(new { });

                msg_entitlement.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", WindowshopGlobals.riotAccessToken);

                var response_entitlement = await client.SendAsync(msg_entitlement);

                if (response_entitlement.StatusCode == HttpStatusCode.OK)
                {
                    var content_entitlement = await response_entitlement.Content.ReadAsStringAsync();

                    var content_to_json = JObject.Parse(content_entitlement);

                    return content_to_json["entitlements_token"].ToString();
                }
                else
                {
                    throw new Exception("Entitlements token response returned " + response_entitlement.StatusCode);
                }
            }
            catch (Exception e)
            {
                ErrorHandler.ThrowAndExit("Something went wrong when fetching entitlements data from Riot. Make sure you're connected to the internet and try again.", e.ToString());
                return "";
            }
        }

        public async static Task<string> AcquirePUUID()
        {
            if (WindowshopGlobals.riotAccessToken == null)
            {
                ErrorHandler.ThrowAndExit("Failed to retrive data for Windowshop.", "Could not get player puuid because Riot access token was not found.");
                return "";
            }

            try
            {
                HttpRequestMessage msg_puuid = new HttpRequestMessage(HttpMethod.Get, "https://auth.riotgames.com/userinfo");
                msg_puuid.Content = JsonContent.Create(new { });

                msg_puuid.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", WindowshopGlobals.riotAccessToken);

                var response_puuid = await client.SendAsync(msg_puuid);

                if (response_puuid.StatusCode == HttpStatusCode.OK)
                {
                    var content_puuid = await response_puuid.Content.ReadAsStringAsync();

                    var content_to_json = JObject.Parse(content_puuid);

                    return content_to_json["sub"].ToString();
                }
                else
                {
                    throw new Exception("Entitlements token response returned " + response_puuid.StatusCode);
                }
            }
            catch (Exception e)
            {
                ErrorHandler.ThrowAndExit("Something went wrong when fetching puuid data from Riot. Make sure you're connected to the internet and try again.", e.ToString());
                return "";
            }
            
        }

        public async static Task<JObject> AcquireShopV3(bool debug = false)
        {
            if (WindowshopGlobals.riotAccessToken == null)
            {
                ErrorHandler.ThrowAndExit("Failed to retrive data for Windowshop.", "Could not get player puuid because Riot access token was not found.");
                return null;
            }
            else if (WindowshopGlobals.entitlementsToken == null)
            {
                ErrorHandler.ThrowAndExit("Failed to retrive data for Windowshop.", "Could not get player puuid because entitlements token was not found.");
                return null;
            }
            else if (WindowshopGlobals.shard == null)
            {
                ErrorHandler.ThrowAndExit("Failed to retrive data for Windowshop.", "Could not get player puuid because shard was not found.");
                return null;
            }

            
            try
            {
                HttpRequestMessage msg_shop = new HttpRequestMessage(HttpMethod.Post, $"https://pd.{WindowshopGlobals.shard}.a.pvp.net/store/v3/storefront/{WindowshopGlobals.puuid}");
                msg_shop.Content = JsonContent.Create(new { });

                msg_shop.Headers.Add("X-Riot-ClientPlatform", "ew0KCSJwbGF0Zm9ybVR5cGUiOiAiUEMiLA0KCSJwbGF0Zm9ybU9TIjogIldpbmRvd3MiLA0KCSJwbGF0Zm9ybU9TVmVyc2lvbiI6ICIxMC4wLjE5MDQyLjEuMjU2LjY0Yml0IiwNCgkicGxhdGZvcm1DaGlwc2V0IjogIlVua25vd24iDQp9");
                msg_shop.Headers.Add("X-Riot-ClientVersion", "release-10.01-shipping-9-3197283");
                msg_shop.Headers.Add("X-Riot-Entitlements-JWT", WindowshopGlobals.entitlementsToken);
                msg_shop.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", WindowshopGlobals.riotAccessToken);

                var response_shop = await client.SendAsync(msg_shop);

                Console.WriteLine(response_shop.StatusCode);
                Console.WriteLine(response_shop.Headers);

                var content_shop = await response_shop.Content.ReadAsStringAsync();

                var content_to_json = JObject.Parse(content_shop);

                if (debug) Trace.WriteLine(content_shop);

                return content_to_json;
            }
            catch (Exception e)
            {
                ErrorHandler.ThrowAndExit("Something went wrong when fetching shop data from Riot. Make sure you're connected to the internet and try again.", e.ToString());
                return null;
            }
            
        }

        public static async Task<string> AcquireShard()
        {
            if (WindowshopGlobals.riotAccessToken == null)
            {
                ErrorHandler.ThrowAndExit("Failed to retrive data for Windowshop.", "Could not get player shard because Riot access token was not found.");
                return null;
            }
            else if (WindowshopGlobals.riotIdToken == null)
            {
                ErrorHandler.ThrowAndExit("Failed to retrive data for Windowshop.", "Could not get player shard because Riot id token was not found.");
                return null;
            }

            try
            {
                HttpRequestMessage msg_shard = new HttpRequestMessage(HttpMethod.Put, "https://riot-geo.pas.si.riotgames.com/pas/v1/product/valorant");
                msg_shard.Content = JsonContent.Create(new { id_token = WindowshopGlobals.riotIdToken });

                msg_shard.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", WindowshopGlobals.riotAccessToken);

                var response_shard = await client.SendAsync(msg_shard);

                var content_shard = await response_shard.Content.ReadAsStringAsync();

                var content_to_json = JObject.Parse(content_shard);

                return content_to_json["affinities"]["live"].ToString();
            }
            catch (Exception e)
            {
                ErrorHandler.ThrowAndExit("Something went wrong when fetching shard data from Riot. Make sure you're connected to the internet and try again.", e.ToString());
                return null;
            }
        }

        public async static Task RefreshToken()
        {
            var tokens = CovertTokensToString();

            var handler = new HttpClientHandler
            {
                AllowAutoRedirect = false
            };

            HttpClient client = new HttpClient(handler);
            try
            {
                HttpRequestMessage msg_reauth = new HttpRequestMessage(HttpMethod.Get, "https://auth.riotgames.com/authorize?redirect_uri=https%3A%2F%2Fplayvalorant.com%2Fopt_in&client_id=play-valorant-web-prod&response_type=token%20id_token&nonce=1&scope=account%20openid");
                msg_reauth.Content = JsonContent.Create(new { });

                msg_reauth.Headers.Add("Cookie", tokens);

                var response_reauth = await client.SendAsync(msg_reauth);

                if (response_reauth.StatusCode == HttpStatusCode.SeeOther)
                {
                    string link = response_reauth.Headers.Location.ToString();

                    var uri = new Uri(link);
                    var fragment = uri.Fragment.TrimStart('#');
                    var queryParams = System.Web.HttpUtility.ParseQueryString(fragment);

                    if (queryParams["access_token"] == null || queryParams["id_token"] == null)
                    {
                        throw new Exception("Reauth token response did not return access or id token.");
                    }

                    WindowshopGlobals.riotAccessToken = queryParams["access_token"];
                    WindowshopGlobals.riotIdToken = queryParams["id_token"];
                }
                else
                {
                    throw new Exception("Reauth token response returned " + response_reauth.StatusCode);
                }
            }
            catch (Exception e)
            {
                AppDataHandler.DeleteFile("riot_tokens_DO_NOT_SHARE");
                ErrorHandler.ThrowAndExit("Windowshop failed to reauthenticate using the saved credentials. Your login may be expired. Make sure you're connected to the internet, and log in again.", e.Message);
            }
        }

        #endregion

        #region Content related calls
        public static async Task<string> AcquireValorantSkinsData()
        {
            try
            {
                HttpRequestMessage msg_valapi_skins = new HttpRequestMessage(HttpMethod.Get, "https://valorant-api.com/v1/weapons/skins/");

                var response_valapi_skins = await client.SendAsync(msg_valapi_skins);
                var content_valapi_skins = await response_valapi_skins.Content.ReadAsStringAsync();

                return content_valapi_skins;
            }
            catch (Exception e)
            {
                ErrorHandler.ThrowAndExit("Something went wrong when fetching skin list data from valorant-api. Make sure you're connected to the internet and try again.", e.ToString());
                return null;
            }
        }

        public static async Task<string> AcquireValorantContentTiersData()
        {
            try
            {
                HttpRequestMessage msg_valapi_contenttiers = new HttpRequestMessage(HttpMethod.Get, "https://valorant-api.com/v1/contenttiers/");

                var response_valapi_contenttiers = await client.SendAsync(msg_valapi_contenttiers);
                var content_valapi_contenttiers = await response_valapi_contenttiers.Content.ReadAsStringAsync();

                return content_valapi_contenttiers;
            }
            catch (Exception e)
            {
                ErrorHandler.ThrowAndExit("Something went wrong when fetching content tier list data from valorant-api. Make sure you're connected to the internet and try again.", e.ToString());
                return null;
            }
        }

        // TECHNICALLY the offer id IS the skin level 0 id, but it is used to search the full skin JObject in this case
        public static JObject AcquireSkinFromOfferID(string offerID)
        {
            try
            {
                JObject temp_file;
                using (StreamReader sr = new StreamReader(AppDataHandler.PathToFile("valorant_skins_data.json")))
                {
                    temp_file = JObject.Parse(sr.ReadToEnd());
                }

                var valorant_skins_data = temp_file["data"];

                foreach (JObject skin in valorant_skins_data)
                {
                    if (skin["levels"][0]["uuid"].ToString() == offerID)
                    {
                        return skin;
                    }
                }

                return null;
            }
            catch (Exception e)
            {
                ErrorHandler.ThrowAndExit("Something went wrong when fetching rarity id data locally. Maybe run with administrator?", e.ToString());
                return null;
            }
        }

        public static string AcquireNameOfSkin(JObject skin, int chroma = 0, int level = 0)
        {
            chroma = Math.Max(0, chroma);
            level = Math.Max(0, level);

            string skinName = "Error";

            // make sure we can fallback just in case
            if (!skin.ContainsKey("displayName"))
            {
                ErrorHandler.ThrowAndExit("Something went wrong with skin JObject for skin name.", "JObject does not contain displayName key.");
                return null;
            }


            // if chroma is not 0, lets just focus on chroma
            if (chroma != 0)
            {
                // check if chroma exists
                if (skin.ContainsKey("chromas"))
                {
                    if (skin["chromas"].Count() >= chroma + 1)
                    {
                        skinName = skin["chromas"][chroma]["displayName"].ToString();
                    }
                }
                else
                {
                    // default to lvl 0
                    skinName = skin["displayName"].ToString();
                }
            }
            else
            {
                if (level != 0)
                {
                    // check if levels exist
                    if (skin.ContainsKey("levels"))
                    {
                        if (skin["levels"].Count() >= level + 1)
                        {
                            skinName = skin["levels"][level]["displayName"].ToString();
                        }
                    }
                    else
                    {
                        // default to lvl 0
                        skinName = skin["displayName"].ToString();
                    }
                }
                else
                {
                    skinName = skin["displayName"].ToString();
                }
            }

            Trace.WriteLine(skinName);

            return skinName;
        }

        public static string AcquireIDFromSkin(JObject skin)
        {
            string skinId = "";

            if (skin.ContainsKey("uuid"))
                skinId = skin["uuid"].ToString();
            else
            {
                ErrorHandler.ThrowAndExit("Something went wrong with skin JObject for rarity name.", "JObject does not contain contentTierUuid key.");
            }

            return skinId;
        }

        public static string AcquireRarityNameOfSkin(JObject skin)
        {
            string contentTierUuid = "";

            if (skin.ContainsKey("contentTierUuid"))
                contentTierUuid = skin["contentTierUuid"].ToString();
            else
            {
                ErrorHandler.ThrowAndExit("Something went wrong with skin JObject for rarity name.", "JObject does not contain contentTierUuid key.");
            }

            try
            {
                JObject temp_file;
                using (StreamReader sr = new StreamReader(AppDataHandler.PathToFile("valorant_content_tiers_data.json")))
                {
                    temp_file = JObject.Parse(sr.ReadToEnd());
                }

                var valorant_content_tiers_data = temp_file["data"];

                foreach (JObject content_tier in valorant_content_tiers_data)
                {
                    if (content_tier["uuid"].ToString() == contentTierUuid)
                    {
                        return content_tier["displayName"].ToString();
                    }
                }

                return "";
            }
            catch (Exception e)
            {
                ErrorHandler.ThrowAndExit("Something went wrong when fetching rarity id data locally. Maybe run with administrator?", e.ToString());
                return "";
            }
        }

        public static async Task<BitmapImage> AcquireImageOfSkin(JObject skin, int chroma = 0, int level = 0)
        {
            string UseDefault()
            {
                if (skin["displayIcon"].Type != JTokenType.Null)
                    return skin["displayIcon"].ToString();
                else if (skin["levels"][0]["displayIcon"].Type != JTokenType.Null)
                    return skin["levels"][0]["displayIcon"].ToString();
                else
                    return null;
            }

            chroma = Math.Max(0, chroma);
            level = Math.Max(0, level);

            string skinImgUrl = "";

            Trace.Write(skin);

            // make sure we can fallback just in case
            if (!skin.ContainsKey("displayIcon"))
            {
                ErrorHandler.ThrowAndExit("Something went wrong with skin JObject for skin img.", "JObject does not contain displayIcon key.");
                return null;
            }


            // if chroma is not 0, lets just focus on chroma
            if (chroma != 0)
            {
                // check if chroma exists
                if (skin.ContainsKey("chromas"))
                {
                    if (skin["chromas"].Count() >= chroma + 1)
                    {
                        if (skin["chromas"][chroma]["displayIcon"] != null && skin["chromas"][chroma]["displayIcon"].Type != JTokenType.Null)
                        {
                            skinImgUrl = skin["chromas"][chroma]["displayIcon"].ToString();
                        }
                        else if (skin["chromas"][chroma]["fullRender"] != null && skin["chromas"][chroma]["fullRender"].Type != JTokenType.Null)
                        {
                            skinImgUrl = skin["chromas"][chroma]["fullRender"].ToString();
                        }
                        else
                            skinImgUrl = UseDefault();
                    }
                    else
                        skinImgUrl = UseDefault();
                }
                else
                {
                    // default to lvl 0
                    skinImgUrl = UseDefault();
                }
            }
            else
            {
                if (level != 0)
                {
                    // check if levels exist
                    if (skin.ContainsKey("levels"))
                    {
                        if (skin["levels"].Count() >= level + 1)
                        {
                            if (skin["levels"][level]["displayIcon"] != null && skin["levels"][level]["displayIcon"].Type != JTokenType.Null)
                            {
                                skinImgUrl = skin["levels"][level]["displayIcon"].ToString();
                            }
                            else if (skin["levels"][level]["fullRender"] != null && skin["levels"][level]["fullRender"].Type != JTokenType.Null)
                            {
                                skinImgUrl = skin["levels"][level]["fullRender"].ToString();
                            }
                            else
                                skinImgUrl = UseDefault();
                        }
                        else
                            skinImgUrl = UseDefault();
                    }
                    else
                    {
                        // default to lvl 0
                        skinImgUrl = UseDefault();
                    }
                }
                else
                {
                    skinImgUrl = UseDefault();
                }
            }

            try
            {
                HttpRequestMessage msg_skin_img = new HttpRequestMessage(HttpMethod.Get, skinImgUrl);

                Trace.WriteLine(skinImgUrl);
                Trace.WriteLine(skinImgUrl);

                var response_skin_img = await client.SendAsync(msg_skin_img);

                byte[] imageData = await response_skin_img.Content.ReadAsByteArrayAsync();
                BitmapImage bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.StreamSource = new System.IO.MemoryStream(imageData);
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.EndInit();

                return bitmap;
            }
            catch (Exception e)
            {
                ErrorHandler.ThrowAndExit("Something went wrong when fetching skin image data online. Make sure you're connected to the internet and try again.", e.ToString());
                return null;
            } 
        }

        public static string AcquireVideoURLOfSkin(JObject skin, bool isChroma = false, int index = 0)
        {
            index = Math.Max(0, index);

            // make sure we can fallback just in case
            if (!skin.ContainsKey("levels") || !skin.ContainsKey("chromas"))
            {
                return null;
            }


            if (isChroma)
            {
                if (skin["chromas"].Count() >= index + 1)
                {
                    if (skin["chromas"][index]["streamedVideo"].Type != JTokenType.Null)
                    {
                        return skin["chromas"][index]["streamedVideo"].ToString();
                    }
                    else
                        return null;
                }
                else
                    return null;
            }
            else
            {
                if (skin["levels"].Count() >= index + 1)
                {
                    if (skin["levels"][index]["streamedVideo"].Type != JTokenType.Null)
                    {
                        return skin["levels"][index]["streamedVideo"].ToString();
                    }
                    else
                        return null;
                }
                else
                    return null;
            }
        }

        public static int AcquireChromaCountOfSkin(JObject skin)
        {
            int chromaCount = 1;

            if (skin.ContainsKey("chromas"))
                chromaCount = skin["chromas"].Count();
            else
            {
                ErrorHandler.ThrowAndExit("Something went wrong with skin JObject for chroma count.", "JObject does not contain chromas key.");
            }

            return chromaCount;
        }

        public static int AcquireLevelsCountOfSkin(JObject skin)
        {
            int levelsCount = 1;

            if (skin.ContainsKey("levels"))
                levelsCount = skin["levels"].Count();
            else
            {
                ErrorHandler.ThrowAndExit("Something went wrong with skin JObject for levels count.", "JObject does not contain levels key.");
            }

            return levelsCount;
        }

        public static string AcquireLevelNameOfSkin(JObject skin, int level)
        {
            level = Math.Max(0, level);
            string skinLevel = "DEFAULT";

            // make sure we can fallback just in case
            if (!skin.ContainsKey("levels"))
            {
                ErrorHandler.ThrowAndExit("Something went wrong with skin JObject for skin level.", "JObject does not contain levelItem key.");
                return null;
            }

            if (skin["levels"].Count() >= level + 1)
            {
                Trace.WriteLine(skin["levels"]);
                string fullSkinLevel = "None";

                if (skin["levels"][level]["levelItem"].Type != JTokenType.Null)
                {
                    fullSkinLevel = skin["levels"][level]["levelItem"].ToString();
                    skinLevel = fullSkinLevel.Split("::")[1];
                }
                    
                
            }

            return skinLevel;
        }

        public static async Task<BitmapImage> AcquireChromaSwatchOfSkin(JObject skin, int chroma)
        {
            string chromaSwatchUrl = "";

            if (skin.ContainsKey("chromas"))
            {
                Trace.WriteLine(skin["chromas"][chroma]);
                if (skin["chromas"][chroma]["swatch"].Type != JTokenType.Null)
                {
                    chromaSwatchUrl = skin["chromas"][chroma]["swatch"].ToString();
                }
                else
                {
                    BitmapImage bitmap = new BitmapImage();
                    bitmap.BeginInit();
                    bitmap.UriSource = new Uri("/Resources/no_tier_placeholder.png", UriKind.RelativeOrAbsolute);
                    bitmap.EndInit();

                    return bitmap;
                }
            }
                
            else
            {
                ErrorHandler.ThrowAndExit("Something went wrong with skin JObject for chroma swatch.", "JObject does not contain chromas key.");
            }

            try
            {
                HttpRequestMessage msg_swatch_img = new HttpRequestMessage(HttpMethod.Get, chromaSwatchUrl);

                var response_swatch_img = await client.SendAsync(msg_swatch_img);

                byte[] imageData = await response_swatch_img.Content.ReadAsByteArrayAsync();
                BitmapImage bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.StreamSource = new System.IO.MemoryStream(imageData);
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.EndInit();

                return bitmap;
            }
            catch (Exception e)
            {
                ErrorHandler.ThrowAndExit("Something went wrong when fetching swatch image data online. Make sure you're connected to the internet and try again.", e.ToString());
                return null;
            }
        }

        public static async Task<string> AcquireValorantManifest()
        {
            try
            {
                HttpRequestMessage msg_version = new HttpRequestMessage(HttpMethod.Get, "https://valorant-api.com/v1/version");
                var response_version = await client.SendAsync(msg_version);
                var content_version = await response_version.Content.ReadAsStringAsync();
                var content_to_json = JObject.Parse(content_version);

                return content_to_json["data"]["manifestId"].ToString();
            }
            catch (Exception e)
            {
                ErrorHandler.ThrowAndExit("Something went wrong when fetching version data from valorant-api. Make sure you're connected to the internet and try again.", e.ToString());
                return null;
            }
        }

        public static async Task<BitmapImage> AcquireRarityImgFromRarityName(string rarityName)
        {
            try
            {
                BitmapImage bitmap = new BitmapImage();

                // Initialize the BitmapImage
                bitmap.BeginInit();

                switch (rarityName)
                {
                    case "Deluxe Edition":
                        bitmap.UriSource = new Uri("/Resources/Deluxe_Edition.png", UriKind.RelativeOrAbsolute);
                        break;
                    case "Exclusive Edition":
                        bitmap.UriSource = new Uri("/Resources/Exclusive_Edition.png", UriKind.RelativeOrAbsolute);
                        break;
                    case "Premium Edition":
                        bitmap.UriSource = new Uri("/Resources/Premium_Edition.png", UriKind.RelativeOrAbsolute);
                        break;
                    case "Select Edition":
                        bitmap.UriSource = new Uri("/Resources/Select_Edition.png", UriKind.RelativeOrAbsolute);
                        break;
                    case "Ultra Edition":
                        bitmap.UriSource = new Uri("/Resources/Ultra_Edition.png", UriKind.RelativeOrAbsolute);
                        break;
                }

                bitmap.EndInit();

                return bitmap;
            }
            catch (Exception e)
            {
                ErrorHandler.Throw("Something went wrong when getting rarity icon image.", e.ToString());
                return null;
            }
        }

        public static string AcquireRarityColorFromRarityName(string rarityName)
        {
            switch (rarityName)
            {
                case "Deluxe Edition":
                    return "009587";
                case "Exclusive Edition":
                    return "f5955b";
                case "Premium Edition":
                    return "d1548d";
                case "Select Edition":
                    return "5a9fe2";
                case "Ultra Edition":
                    return "fad663";
                default:
                    return "ffffff";
            }
        }



        #endregion









        private static string CovertTokensToString()
        {
            if (WindowshopGlobals.riotAccountTokens == null)
            {
                ErrorHandler.ThrowAndExit("Failed to retrive data for Windowshop.", "Could not convert token to string because Riot account tokens were not found.");
                return "";
            }

            var tokensString = "";
            foreach (var token in WindowshopGlobals.riotAccountTokens)
            {
                tokensString += token.Key + "=" + token.Value + "; ";
            }

            return tokensString;
        }

        public static BitmapImage LoadLocalImage(string path)
        {
            BitmapImage bitmap = new BitmapImage();

            bitmap.BeginInit();
            bitmap.UriSource = new Uri(path, UriKind.RelativeOrAbsolute);
            bitmap.EndInit();

            return bitmap;
        }






































        //public static async Task<string> AcquireSkinNameFromID(string offerID)
        //{
        //    try
        //    {
        //        HttpRequestMessage msg_valapi_skins = new HttpRequestMessage(HttpMethod.Get, "https://valorant-api.com/v1/weapons/skinlevels/" + offerID);

        //        var response_valapi_skins = await client.SendAsync(msg_valapi_skins);
        //        var content_valapi_skins = await response_valapi_skins.Content.ReadAsStringAsync();

        //        var json_valapi_skins = JObject.Parse(content_valapi_skins);

        //        return json_valapi_skins["data"]["displayName"].ToString();
        //    }
        //    catch (Exception e)
        //    {
        //        ErrorHandler.ThrowAndExit("Something went wrong when fetching skin name data from valorant-api. Make sure you're connected to the internet and try again.", e.ToString());
        //        return null;
        //    }
        //}



        //public static async Task<BitmapImage> AcquireSkinImgFromID(string offerID)
        //{
        //    try
        //    {
        //        HttpRequestMessage msg_valapi_skins = new HttpRequestMessage(HttpMethod.Get, "https://valorant-api.com/v1/weapons/skinlevels/" + offerID);

        //        var response_valapi_skins = await client.SendAsync(msg_valapi_skins);
        //        var content_valapi_skins = await response_valapi_skins.Content.ReadAsStringAsync();

        //        var json_valapi_skins = JObject.Parse(content_valapi_skins);

        //        HttpRequestMessage msg_skin_img = new HttpRequestMessage(HttpMethod.Get, json_valapi_skins["data"]["displayIcon"].ToString());

        //        var response_skin_img = await client.SendAsync(msg_skin_img);

        //        byte[] imageData = await response_skin_img.Content.ReadAsByteArrayAsync();
        //        BitmapImage bitmap = new BitmapImage();
        //        bitmap.BeginInit();
        //        bitmap.StreamSource = new System.IO.MemoryStream(imageData);
        //        bitmap.CacheOption = BitmapCacheOption.OnLoad;
        //        bitmap.EndInit();

        //        return bitmap;
        //    }
        //    catch (Exception e)
        //    {
        //        ErrorHandler.ThrowAndExit("Something went wrong when fetching skin image data from valorant-api. Make sure you're connected to the internet and try again.", e.ToString());
        //        return null;
        //    }
        //}

        //public static async Task<string> AcquireRarityFromID(string offerID)
        //{
        //    try
        //    {
        //        JObject json_skins;
        //        using (StreamReader sr = new StreamReader(AppDataHandler.PathToFile("valorant_skins_data.json")))
        //        {
        //            json_skins = JObject.Parse(sr.ReadToEnd());
        //        }

        //        var json_skins_data = json_skins["data"];
        //        string contentTierId = "null";


        //        foreach (JObject skin in json_skins_data)
        //        {
        //            if (skin["levels"][0]["uuid"].ToString() == offerID)
        //            {
        //                contentTierId = skin["contentTierUuid"].ToString();
        //                break;
        //            }
        //        }

        //        HttpRequestMessage msg_valapi_contentTier = new HttpRequestMessage(HttpMethod.Get, "https://valorant-api.com/v1/contenttiers/" + contentTierId);

        //        var response_valapi_contentTier = await client.SendAsync(msg_valapi_contentTier);
        //        var content_valapi_contentTier = await response_valapi_contentTier.Content.ReadAsStringAsync();

        //        var json_valapi_contentTier = JObject.Parse(content_valapi_contentTier);
        //        Trace.WriteLine(json_valapi_contentTier);

        //        return json_valapi_contentTier["data"]["displayName"].ToString();
        //    }
        //    catch (Exception e)
        //    {
        //        ErrorHandler.ThrowAndExit("Something went wrong when fetching rarirty id data from valorant-api. Make sure you're connected to the internet and try again.", e.ToString());
        //        return null;
        //    }
        //}
    }
}

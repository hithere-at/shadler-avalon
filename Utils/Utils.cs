using System;
using System.Text.RegularExpressions;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace Shadler.Utilities
{
    public static class ShadlerHttp
    {
        public static void SetDefaultHeader(HttpClient client)
        {
            client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:109.0) Gecko/20100101 Firefox/112.0");
            client.DefaultRequestHeaders.Add("Referer", "https://allmanga.to");
        }

    }

    public static class Anime
    {
        private static string ANIME_QUERY_VARS = "{%22search%22:{%22query%22:%22#QUERY#%22,%22allowAdult%22:false,%22allowUnknown%22:false},%22limit%22:26,%22page%22:1,%22translationType%22:%22sub%22,%22countryOrigin%22:%22ALL%22}";
        private static string ANIME_STREAM_VARS = "{%22showId%22:%22#ANIME_ID#%22,%22translationType%22:%22sub%22,%22episodeString%22:%22#EPISODE#%22}";
        private static string ANIME_QUERY_HASH = "06327bc10dd682e1ee7e07b6db9c16e9ad2fd56c1b769e47513128cd5c9fc77a";
        private static string ANIME_STREAM_HASH = "5f1a64b73793cc2234a389cf3a8f93ad82de7043017dd551f38f65b89daa65e0";
        private static string ANIME_DETAIL_HASH = "9d7439c90f203e534ca778c4901f9aa2d3ad42c06243ab2c5e6b79612af32028";

        private static string API_EXT = "{%22persistedQuery%22:{%22version%22:1,%22sha256Hash%22:%22#HASH#%22}}";
        private static string DETAIL_VARS = "{%22_id%22:%22#DEATH#%22}";

        public static string GetQueryUrl(string query)
        {
            string queryEnc = Uri.EscapeDataString(query);
            string queryVar = ANIME_QUERY_VARS.Replace("#QUERY#", queryEnc);
            string extVar = API_EXT.Replace("#HASH#", ANIME_QUERY_HASH);
            string fullUrl = "https://api.allanime.day/api?variables=" + queryVar + "&extensions=" + extVar;

            return fullUrl;
        }

        public static async Task<String> GetVideoStreamUrl(string id, string episode)
        {
            string streamVar = ANIME_STREAM_VARS.Replace("#ANIME_ID#", id).Replace("#EPISODE#", episode);
            string extVar = API_EXT.Replace("#HASH#", ANIME_STREAM_HASH);
            string fullUrl = "https://api.allanime.day/api?variables=" + streamVar + "&extensions=" + extVar;
            string videoUrl;

            using (HttpClient client = new HttpClient()) {
                ShadlerHttp.SetDefaultHeader(client);

                string response = await client.GetStringAsync(fullUrl);
                Console.WriteLine(response);
                Regex regex = new Regex("apivtwo/[^\"]*");
                Match match = regex.Match(response);
                string matchedString = match.Value.Replace("clock", "clock.json").Replace("/dr", "");

                response = await client.GetStringAsync("http://blog.allanime.day/" + matchedString);
                Console.WriteLine(response);
                JsonDocument doc = JsonDocument.Parse(response);
                JsonElement root = doc.RootElement;
                videoUrl = root.GetProperty("links")[0].GetProperty("link").ToString();

            }

            return videoUrl;

        }

        public static string GetDetailUrl(string id)
        {
            string detailVar = DETAIL_VARS.Replace("#DEATH#", id);
            string extVar = API_EXT.Replace("#HASH#", ANIME_DETAIL_HASH);
            string fullUrl = "https://api.allanime.day/api?variables=" + detailVar + "&extensions=" + extVar;
            return fullUrl;
        }

    }

    public static class Manga
    {

        private static string MANGA_QUERY_VARS = "{%22search%22:{%22query%22:%22#QUERY#%22,%22isManga%22:true},%22limit%22:26,%22page%22:1,%22translationType%22:%22sub%22,%22countryOrigin%22:%22ALL%22}";
        private static string MANGA_READ_VARS = "{%22mangaId%22:%22#MANGA_ID#%22,%22translationType%22:%22sub%22,%22chapterString%22:%22#CHAPTER#%22,%22limit%22:10,%22offset%22:0}";
        private static string MANGA_QUERY_HASH = "3a4b7e9ef62953484a05dd40f35b35b118ad2ff3d5e72d2add79bcaa663271e7";
        private static string MANGA_READ_HASH = "4a048654fbac31f11e201ac8bd34d748b514c28d2781b674d057d064282e620e";
        private static string MANGA_DETAIL_HASH = "90024aeae9c1a4d3ace0473871dd1902e47fbcb8781ccbcd8ad81f8bb1f313ee";

        private static string API_EXT = "{%22persistedQuery%22:{%22version%22:1,%22sha256Hash%22:%22#HASH#%22}}";
        private static string DETAIL_VARS = "{%22_id%22:%22#DEATH#%22}";

        public static string GetQueryUrl(string query)
        {
            string queryEnc = Uri.EscapeDataString(query);
            string queryVar = MANGA_QUERY_VARS.Replace("#QUERY#", queryEnc);
            string extVar = API_EXT.Replace("#HASH#", MANGA_QUERY_HASH);
            string fullUrl = "https://api.allanime.day/api?variables=" + queryVar + "&extensions=" + extVar;

            return fullUrl;
        }

        public static string GetStreamUrl(string id, string chapter)
        {
            string streamVar = MANGA_READ_VARS.Replace("#MANGA_ID#", id).Replace("#CHAPTER#", chapter);
            string extVar = API_EXT.Replace("#HASH#", MANGA_READ_HASH);
            string fullUrl = "https://api.allanime.day/api?variables=" + streamVar + "&extensions=" + extVar;

            return fullUrl;
        }

        public static string GetDetailUrl(string id)
        {
            string detailVar = DETAIL_VARS.Replace("#DEATH#", id);
            string extVar = API_EXT.Replace("#HASH#", MANGA_DETAIL_HASH);
            string fullUrl = "https://api.allanime.day/api?variables=" + detailVar + "&extensions=" + extVar;
            return fullUrl;
        }

    }
}


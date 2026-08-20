using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace TrayTemps
{
    internal static class UpdateCheckHelper
    {
        private const string GitHubTagsApiUrl = "https://api.github.com/repos/nmd-113/Tray-Temps/tags?per_page=100";
        private const string GitHubReleasePageUrl = "https://github.com/nmd-113/Tray-Temps/releases/tag/";
        private static readonly HttpClient Client = CreateClient();

        internal static Task<string> GetGitHubTagsAsync()
        {
            return Client.GetStringAsync(GitHubTagsApiUrl);
        }

        internal static string GetReleaseUrl(string tag)
        {
            return GitHubReleasePageUrl + Uri.EscapeDataString(tag);
        }

        internal static bool TryGetLatestGitHubTag(string tagsJson, out Version latestVersion, out string latestTag)
        {
            latestVersion = null;
            latestTag = null;

            using (var document = JsonDocument.Parse(tagsJson))
            {
                if (document.RootElement.ValueKind != JsonValueKind.Array)
                    return false;

                foreach (JsonElement tagElement in document.RootElement.EnumerateArray())
                {
                    if (!tagElement.TryGetProperty("name", out JsonElement nameElement) ||
                        !TryParseVersion(nameElement.GetString(), out Version tagVersion))
                    {
                        continue;
                    }

                    if (latestVersion == null || tagVersion > latestVersion)
                    {
                        latestVersion = tagVersion;
                        latestTag = nameElement.GetString();
                    }
                }
            }

            return latestVersion != null;
        }

        internal static bool TryParseVersion(string value, out Version version)
        {
            version = null;
            if (string.IsNullOrWhiteSpace(value))
                return false;

            string versionText = value.Trim();
            if (versionText.StartsWith("v", StringComparison.OrdinalIgnoreCase))
                versionText = versionText.Substring(1);

            return Version.TryParse(versionText, out version);
        }

        private static HttpClient CreateClient()
        {
            var client = new HttpClient { Timeout = TimeSpan.FromSeconds(10) };
            client.DefaultRequestHeaders.TryAddWithoutValidation("User-Agent", "TrayTemps update checker");
            return client;
        }
    }
}

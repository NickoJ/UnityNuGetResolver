using Newtonsoft.Json;

namespace NuGetResolver.Data
{
    internal sealed class LibraryInfo
    {
        
        public readonly string Id;
        public readonly string Version;
        public readonly string Framework;

        [JsonConstructor]
        public LibraryInfo(
            [JsonProperty] string id = "",
            [JsonProperty] string version = "",
            [JsonProperty] string framework = "")
        {
            Id = id;
            Version = version;
            Framework = framework;
        }

    }
}
using Newtonsoft.Json;

namespace NuGetResolver.Data
{
    
    internal sealed class ResolverManifest
    {

        public readonly string WorkingDirectory;
        public readonly string CacheDirectory;
        public readonly string Repository;

        [JsonConstructor]
        public ResolverManifest (
            [JsonProperty("work_dir")] string workingDirectory,
            [JsonProperty("cache_dir")] string cacheDirectory,
            [JsonProperty("repository")] string repository)
        {
            WorkingDirectory = workingDirectory;
            CacheDirectory = cacheDirectory;
            Repository = repository ?? Const.Urls.DefaultNuGet;
        }
        
    }
    
}
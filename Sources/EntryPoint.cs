using System.IO;
using NuGetResolver.Data;
using NuGetResolver.Logging;
using NuGetResolver.Utilities;

namespace NuGetResolver
{
    
    internal class EntryPoint
    {
        
        internal static void Main(string manifestPath,
            string logFilePath,
            LogType logType = LogType.Console)
        {
            using var logger = LoggerBuilder.Build(logType, 
                PathUtilities.GenerateFullPath(logFilePath, Const.FileNames.LogFileName));
            logger.LogInformation("Application started.");
            
            var manifest = LoadManifest(PathUtilities.GenerateFullPath(manifestPath, Const.FileNames.ResolverManifestName));

            var resolver = new Resolver(manifest, logger);
            var task = resolver.ResolveAsync();
            task.Wait();
            
            logger.LogInformation("Application stopped.");
        }

        private static ResolverManifest LoadManifest(string manifestPath)
        {
            var manifestJson = File.ReadAllText(manifestPath);
            return Newtonsoft.Json.JsonConvert.DeserializeObject<ResolverManifest>(manifestJson);            
        }
        
    }
    
}

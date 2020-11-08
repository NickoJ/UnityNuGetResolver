using System.Collections.Generic;
using Newtonsoft.Json;

namespace NuGetResolver.Data
{
    
    internal sealed class LibrariesFolderManifest
    {

        public readonly IReadOnlyList<LibraryInfo> LibrariesInfo;

        [JsonConstructor]
        public LibrariesFolderManifest(
            [JsonProperty("libs")] LibraryInfo[] librariesInfo)
        {
            LibrariesInfo = librariesInfo;
        }
        
    }
}
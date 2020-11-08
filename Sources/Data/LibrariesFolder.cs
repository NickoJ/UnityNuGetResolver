namespace NuGetResolver.Data
{
    
    internal sealed class LibrariesFolder
    {

        public readonly string Directory;
        public readonly LibrariesFolderManifest Manifest;

        public LibrariesFolder(string directory, LibrariesFolderManifest manifest)
        {
            Directory = directory;
            Manifest = manifest;
        }
        
    }
    
}
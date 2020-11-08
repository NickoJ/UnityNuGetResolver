using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using NuGet.Common;
using NuGet.Configuration;
using NuGet.PackageManagement;
using NuGet.Packaging;
using NuGet.Packaging.Core;
using NuGet.Packaging.Signing;
using NuGet.ProjectManagement;
using NuGet.Protocol.Core.Types;
using NuGet.Resolver;
using NuGet.Versioning;
using NuGetResolver.Data;
using NuGetResolver.Utilities;

namespace NuGetResolver
{
    
    using ProvidersContainer = List<Lazy<INuGetResourceProvider>>;
    
    internal sealed class Resolver
    {
        
        private const bool AllowPrereleaseVersions = true;
        private const bool AllowUnlisted = false;
        
        private readonly ResolverManifest _manifest;
        private readonly ILogger _logger;

        private readonly string _cacheDirectory;
        
        private readonly SourceRepository _sourceRepository;
        private readonly ResolutionContext _resolutionContext;
        private readonly INuGetProjectContext _projectContext;
        private readonly NuGetPackageManager _packageManager;

        public Resolver(in ResolverManifest manifest, in ILogger logger)
        {
            _manifest = manifest ?? throw new ArgumentNullException(nameof(manifest));
            _logger = logger;

            var providersContainer = new ProvidersContainer();
            providersContainer.AddRange(Repository.Provider.GetCoreV3());
            
            var packageSource = new PackageSource(manifest.Repository);
            _sourceRepository = new SourceRepository(packageSource, providersContainer);
            
            _resolutionContext = new ResolutionContext(
                DependencyBehavior.Ignore,
                AllowPrereleaseVersions,
                AllowUnlisted,
                VersionConstraints.None);

            _cacheDirectory = PathUtilities.GenerateFullPath(manifest.CacheDirectory, Const.Directories.DefaultCache);

            var settings = Settings.LoadDefaultSettings(_cacheDirectory);
            _projectContext = new ConsoleProjectContext(_logger)
            {
                PackageExtractionContext = new PackageExtractionContext(
                    PackageSaveMode.Defaultv3,
                    XmlDocFileSaveMode.Skip,
                    ClientPolicyContext.GetClientPolicy(settings, _logger),
                    _logger
                )
            };
            
            var packageSourceProvider = new PackageSourceProvider(settings);
            var sourceRepositoryProvider = new SourceRepositoryProvider(packageSourceProvider, providersContainer);
            var project = new FolderNuGetProject(_cacheDirectory);
            
            _packageManager = new NuGetPackageManager(sourceRepositoryProvider, settings, _cacheDirectory)
            {
                PackagesFolderNuGetProject = project
            };
        }

        public async Task ResolveAsync()
        {
            var libFolders = GetLibFolders(_manifest.WorkingDirectory, _logger);
            
            foreach (var libFolder in libFolders)
            {
                _logger.LogInformation($"Folder is found: {libFolder.Directory}");
                await ResolveLibrariesFolderAsync(libFolder);
            }
        }

        private async Task ResolveLibrariesFolderAsync(LibrariesFolder folder)
        {
            DeleteOldLibFiles(folder.Directory);
            await DownloadNewLibFilesAsync(folder);
        }

        private void DeleteOldLibFiles(string dir)
        {
            var dirInfo = new DirectoryInfo(dir);
            var libs = dirInfo.GetFiles("*.dll");
            
            foreach (var lib in libs)
            {
                if (lib is null || !lib.Exists) continue;
                
                lib.Delete();
                _logger.LogInformation($"\tLibrary is deleted: {lib.Name}");
            }
        }

        private async Task DownloadNewLibFilesAsync(LibrariesFolder folder)
        {
            foreach (var libraryInfo in folder.Manifest.LibrariesInfo)
            {
                var packageId = new PackageIdentity(libraryInfo.Id, new NuGetVersion(libraryInfo.Version));

                await _packageManager.InstallPackageAsync(_packageManager.PackagesFolderNuGetProject,
                    packageId, _resolutionContext, _projectContext, _sourceRepository,
                    new List<SourceRepository>(), CancellationToken.None);
                
                var loadedFolderDirInfo = new DirectoryInfo(Path.Combine(_cacheDirectory, packageId.ToString()));
                if (!loadedFolderDirInfo.Exists) continue;

                var libFolder = new DirectoryInfo(Path.Combine(loadedFolderDirInfo.FullName, "lib", libraryInfo.Framework));

                var dllFileInfo = libFolder.Exists
                    ? libFolder.GetFiles($"{libraryInfo.Id}.dll").FirstOrDefault()
                    : null;


                if (dllFileInfo is null)
                {
                    var sb = new StringBuilder($"Couldn't find \"{libraryInfo.Id}\" library with target framework \"{libraryInfo.Framework}\".");
                    if (libFolder.Parent != null)
                    {
                        sb.AppendLine("Available frameworks are: ");
                        foreach (var frameworkDir in libFolder.Parent.GetDirectories())
                        {
                            sb.Append("\t").AppendLine(frameworkDir.Name);
                        }
                    }

                    _logger.LogError(sb.ToString());
                }
                else
                {
                    dllFileInfo.CopyTo(Path.Combine(folder.Directory, dllFileInfo.Name));
                    _logger.LogInformation($"\tLibrary is downloaded successfully: {dllFileInfo.Name}");
                }
            }
        }
        
        private static IEnumerable<LibrariesFolder> GetLibFolders(string rootDirectory, ILogger logger)
        {
            var directories = new Queue<DirectoryInfo>();
            var root = new DirectoryInfo(rootDirectory);
            directories.Enqueue(root);
            
            logger.LogInformation($"Start searching from: {root.FullName}");
            
            while (directories.TryDequeue(out var dir))
            {
                if (!dir.Exists) continue;

                foreach (var childDir in dir.GetDirectories()) directories.Enqueue(childDir);
                
                if (!TryLoadLibrariesFolder(dir, out var librariesFolder) || librariesFolder is null) continue;
                yield return librariesFolder;
            }            
        }

        private static bool TryLoadLibrariesFolder(in DirectoryInfo dir, out LibrariesFolder? folder)
        {
            var foundFiles = dir.GetFiles(Const.FileNames.LibrariesFolderManifestName);
            foreach (var file in foundFiles)
            {
                if (!string.Equals(file.Name, Const.FileNames.LibrariesFolderManifestName)) continue;
                
                var manifestText = File.ReadAllText(file.FullName);
                var manifest = JsonConvert.DeserializeObject<LibrariesFolderManifest>(manifestText);
                
                folder = new LibrariesFolder(dir.FullName, manifest);
                return true;
            }

            folder = default;
            return false;
        }
        
    }
    
}
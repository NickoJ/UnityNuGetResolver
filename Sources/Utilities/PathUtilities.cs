﻿using System.Diagnostics;
using System.IO;

namespace NuGetResolver.Utilities
{
    internal static class PathUtilities
    {

        public static string GenerateFullPath(string path, in string defaultPath)
        {
            path ??= string.Empty;
            path = path.Trim();

            if (string.IsNullOrEmpty(path)) path = defaultPath;

            if (!Path.IsPathRooted(path))
            {
                var executingFolder = Process.GetCurrentProcess().MainModule?.FileName;
                executingFolder = Path.GetDirectoryName(executingFolder);

                path = Path.Combine(executingFolder!, path);
                path = path.Replace(@"file:\", string.Empty);
            }

            return path;
        }
        
    }
}
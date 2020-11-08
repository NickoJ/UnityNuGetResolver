using System;

namespace NuGetResolver.Logging
{
    
    [Flags]
    internal enum LogType
    {
        None = 0,
        Console = 0b1,
        File = 0b10,
        ConsoleAndFile = Console | File
    }
}
using System;
using System.IO;

namespace NuGetResolver.Logging
{
    
    internal static class LoggerBuilder
    {

        public static IResolverLogger Build(LogType logType, string logFilePath)
        {
            IResolverLogger logger = default;

            if ((logType & LogType.Console) != 0) logger = new ConsoleLogger();
            if ((logType & LogType.File) != 0)
            {
                var stream = new FileStream(logFilePath, FileMode.Append, FileAccess.Write);
                var streamLogger = new StreamLogger(stream);

                logger = CombineLoggers(logger, streamLogger);
            }
            
            if (logger is null) logger = NullResolveLogger.Instance;

            return logger;
        }

        private static IResolverLogger CombineLoggers(IResolverLogger lhv, IResolverLogger rhv)
        {
            if (lhv is null && rhv is null) return null;
            if (lhv is null) return rhv;
            if (rhv is null) return lhv;
            
            var multiLogger = lhv as MultiLogger ?? rhv as MultiLogger ?? new MultiLogger();

            if (!ReferenceEquals(multiLogger, lhv)) multiLogger.AddLogger(lhv);
            if (!ReferenceEquals(multiLogger, rhv)) multiLogger.AddLogger(rhv);

            return multiLogger;
        }
        
    }
    
}
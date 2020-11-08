using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NuGet.Common;

namespace NuGetResolver.Logging
{
    internal sealed class MultiLogger : LoggerBase, IResolverLogger
    {
        
        private readonly List<IResolverLogger> _loggers = new List<IResolverLogger>();

        public void AddLogger(IResolverLogger logger)
        {
            if (logger is null) throw new ArgumentNullException(nameof(logger));
            
            _loggers.Add(logger);
        }

        public override void Log(ILogMessage message)
        {
            foreach (var logger in _loggers) logger.Log(message);
        }

        public override Task LogAsync(ILogMessage message)
        {
            var tasks = new List<Task>();
            foreach (var logger in _loggers)
            {
                var task = logger.LogAsync(message);
                tasks.Add(task);
            }

            return Task.WhenAll(tasks);
        }

        public void Dispose()
        {
            foreach (var logger in _loggers)
            {
                logger.Dispose();
            }
            _loggers.Clear();
        }
    }
}
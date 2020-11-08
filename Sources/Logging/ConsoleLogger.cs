using System;
using System.Threading.Tasks;
using NuGet.Common;

namespace NuGetResolver.Logging
{
    
    internal sealed class ConsoleLogger : LoggerBase, IResolverLogger
    {
        
        public override void Log(ILogMessage message)
        {
            Console.WriteLine($"[{message.Time.ToString()}|{message.Level}] {message.Message}");
        }

        public override Task LogAsync(ILogMessage message)
        {
            Log(message);
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            
        }

    }
    
}
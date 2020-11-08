using System.Threading.Tasks;
using NuGet.Common;

namespace NuGetResolver.Logging
{
    
    internal class NullResolveLogger : LoggerBase, IResolverLogger
    {

        private static NullResolveLogger _instance;
        
        public static NullResolveLogger Instance => _instance ??= new NullResolveLogger();

        public override void Log(ILogMessage message) {}

        public override Task LogAsync(ILogMessage message) => Task.CompletedTask;

        public void Dispose() {}
    }
}
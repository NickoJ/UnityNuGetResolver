using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using NuGet.Common;

namespace NuGetResolver.Logging
{
    
    internal class StreamLogger : LoggerBase, IResolverLogger
    {

        private readonly StreamWriter _writer;
        
        public StreamLogger(Stream stream)
        {
            _writer = new StreamWriter(stream);
        }

        public override void Log(ILogMessage message)
        {
            _writer.WriteLine(GenerateRecord(message));
        }

        public override Task LogAsync(ILogMessage message)
        {
            return _writer.WriteLineAsync(GenerateRecord(message));
        }

        public void Dispose()
        {
            _writer.Dispose();
        }

        private static string GenerateRecord(ILogMessage message)
        {
            return $"[{message.Time.ToString(CultureInfo.InvariantCulture)}|{message.Level}] {message.Message}";
        }
        
    }
    
}
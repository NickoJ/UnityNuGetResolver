using System.Collections.Generic;

namespace NuGetResolver.Utilities
{
    
    internal static class QueueUtilities
    {

        public static bool TryDequeue<T>(this Queue<T> queue, out T item)
        {
            item = default;
            if (queue.Count == 0) return false;

            item = queue.Dequeue();
            return true;
        }
        
    }
    
}
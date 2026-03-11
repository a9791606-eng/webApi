using System.Collections.Concurrent;
using IceCreamProject.Models;

namespace IceCreamProject.Services
{
    public class LoggingQueue
    {
        private readonly ConcurrentQueue<LogEntry> queue = new ConcurrentQueue<LogEntry>();

        public void Enqueue(LogEntry entry) => queue.Enqueue(entry);

        public bool TryDequeue(out LogEntry entry) => queue.TryDequeue(out entry);
    }
}
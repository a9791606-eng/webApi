using System;

namespace IceCreamNamespace.Models
{
    public class LogEntry
    {
        public DateTime Start { get; set; }
        public string Controller { get; set; }
        public string Action { get; set; }
        public string Username { get; set; }
        public long DurationMs { get; set; }
        public string Path { get; set; }
        public int StatusCode { get; set; }
    }
}
using System;

namespace IceCreamNamespace.Models
{
    public class LogEntry
    {
        public DateTime Start { get; set; }
        public string Controller { get; set; } = string.Empty; // תיקון
        public string Action { get; set; } = string.Empty;     // תיקון
        public string Username { get; set; } = string.Empty;   // תיקון
        public long DurationMs { get; set; }
        public string Path { get; set; } = string.Empty;       // תיקון
        public int StatusCode { get; set; }
    }
}
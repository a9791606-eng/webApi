using System;

namespace IceCreamNamespace.Models
{
    public class IceCreamUpdatedMessage
    {
        public int UserId { get; set; }
        public string Username { get; set; } = string.Empty;
        public string IceCreamName { get; set; } = string.Empty; 
        public DateTime Timestamp { get; set; }
    }
}
using System;

namespace IceCreamNamespace.Models
{
    public class IceCreamUpdatedMessage 
    {
        public int UserId { get; set; }
        public string Username { get; set; }
        public string IceCreamName { get; set; }
        public DateTime Timestamp { get; set; }
    }
}

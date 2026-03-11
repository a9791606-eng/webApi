using System;

namespace IceCreamProject.Models
{
    public class IceCreamUpdatedMessage
    {
        public int UserId { get; set; }
        public string Username { get; set; }
        public string IceCreamName { get; set; }
        public DateTime Timestamp { get; set; }
    }
}

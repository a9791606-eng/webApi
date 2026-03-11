namespace IceCreamNamespace.Models
{
    public class IceCream
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool isGloutenFree { get; set; }
        public int UserId { get; internal set; }
    }
}

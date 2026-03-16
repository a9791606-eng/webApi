namespace IceCreamNamespace.Models
{
    public class IceCream
    {
        public int UserId { get; internal set; }
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty; 
        public bool IsGlutenFree { get; set; } 
    }
}
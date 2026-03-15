using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using IceCreamNamespace.Interfaces;
using IceCreamNamespace.Models;
using Microsoft.AspNetCore.Hosting;


namespace IceCreamNamespace.Services
{
    public class IceCreamRepository : IIceCreamRepository
    {
        private readonly List<IceCream> IceCreams;
        private readonly string filePath;

        public IceCreamRepository(IWebHostEnvironment webHost)
        {
            var dataDir = Path.Combine(webHost.ContentRootPath, "Data");
            Directory.CreateDirectory(dataDir);
            filePath = Path.Combine(dataDir, "IceCream.json");

            if (File.Exists(filePath))
            {
                using var jsonFile = File.OpenText(filePath);
                IceCreams = JsonSerializer.Deserialize<List<IceCream>>(jsonFile.ReadToEnd(),
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true })
                    ?? new List<IceCream>();
            }
            else
            {
                IceCreams = new List<IceCream>();
                Save();
            }
        }

        private void Save() => File.WriteAllText(filePath, JsonSerializer.Serialize(IceCreams));

        public List<IceCream> GetAll() => IceCreams;

        public IceCream Get(int id) => IceCreams.FirstOrDefault(i => i.Id == id);

        public void Add(IceCream IceCream)
        {
            IceCream.Id = IceCreams.Count == 0 ? 1 : IceCreams.Max(i => i.Id) + 1;;
            IceCreams.Add(IceCream);
            Save();
        }

        public void Delete(int id)
        {
            var IceCream = Get(id);
            if (IceCream is null)
                return;

            IceCreams.Remove(IceCream);
            Save();
        }

        public void Update(IceCream IceCream)
        {
            var index = IceCreams.FindIndex(i => i.Id == IceCream.Id);
            if (index == -1)
                return;

            IceCreams[index] = IceCream;
            Save();
        }

        public int Count => IceCreams.Count;
    }
}

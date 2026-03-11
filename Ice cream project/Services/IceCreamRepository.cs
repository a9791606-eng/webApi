using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using IceCreamProject.Models;
using IceCreamProject.Interfaces;
using Microsoft.AspNetCore.Hosting;
using IceCreamNamespace.Models;

namespace IceCreamProject.Services
{
    public class IceCreamRepository : IIceCreamRepository
    {
        private readonly List<IceCream> iceCreams;
        private readonly string filePath;

        public IceCreamRepository(IWebHostEnvironment webHost)
        {
            filePath = Path.Combine(webHost.ContentRootPath, "Data", "IceCream.json");

            // ensure data folder exists
            var dir = Path.GetDirectoryName(filePath);
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            // if file missing or empty, create default seed data
            if (!File.Exists(filePath) || string.IsNullOrWhiteSpace(File.ReadAllText(filePath)))
            {
                iceCreams = new List<IceCream>
                {
                    new IceCream { Id = 1, Name = "Vanilla", isGloutenFree = true, UserId = 1 },
                    new IceCream { Id = 2, Name = "Strawberry", isGloutenFree = true, UserId = 1 },
                    new IceCream { Id = 3, Name = "Chocolate", isGloutenFree = true, UserId = 1 },
                    new IceCream { Id = 4, Name = "Pistachio", isGloutenFree = false, UserId = 1 }
                };

                Save();
                return;
            }

            // otherwise load existing data
            using var jsonFile = File.OpenText(filePath);
            iceCreams = JsonSerializer.Deserialize<List<IceCream>>(jsonFile.ReadToEnd(),
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true })
                ?? new List<IceCream>();
        }

        private void Save() => File.WriteAllText(filePath, JsonSerializer.Serialize(iceCreams));

        public List<IceCream> GetAll() => iceCreams;

        public IceCream Get(int id) => iceCreams.FirstOrDefault(i => i.Id == id);

        public void Add(IceCream iceCream)
        {
            iceCream.Id = iceCreams.Count == 0 ? 1 : iceCreams.Max(i => i.Id) + 1;
            iceCreams.Add(iceCream);
            Save();
        }

        public void Delete(int id)
        {
            var iceCream = Get(id);
            if (iceCream is null)
                return;

            iceCreams.Remove(iceCream);
            Save();
        }

        public void Update(IceCream iceCream)
        {
            var index = iceCreams.FindIndex(i => i.Id == iceCream.Id);
            if (index == -1)
                return;

            iceCreams[index] = iceCream;
            Save();
        }

        public int Count => iceCreams.Count;
    }
}

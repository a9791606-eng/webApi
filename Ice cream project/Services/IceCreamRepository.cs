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

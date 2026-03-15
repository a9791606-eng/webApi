using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using IceCreamNamespace.Models;
using IceCreamNamespace.Interfaces;
using Microsoft.AspNetCore.Hosting;

namespace IceCreamNamespace.Services
{
    public class UserRepository : IUserRepository
    {
        private readonly List<User> users;
        private readonly string filePath;

        public UserRepository(IWebHostEnvironment webHost)
        {
            var dataDir = Path.Combine(webHost.ContentRootPath, "Data");
            Directory.CreateDirectory(dataDir);
            filePath = Path.Combine(dataDir, "Users.json");

            if (File.Exists(filePath))
            {
                using var jsonFile = File.OpenText(filePath);
                users = JsonSerializer.Deserialize<List<User>>(jsonFile.ReadToEnd(), new JsonSerializerOptions { PropertyNameCaseInsensitive = true })
                    ?? new List<User>();
            }
            else
            {
                users = new List<User>
                {
                    new User { Id = 1, Username = "admin", Password = "admin", IsAdmin = true }
                };
                Save();
            }
        }

        private void Save() => File.WriteAllText(filePath, JsonSerializer.Serialize(users));

        public List<User> GetAll() => users;

        public User Get(int id) => users.FirstOrDefault(u => u.Id == id);

        public void Add(User user)
        {
            user.Id = users.Count == 0 ? 1 : users.Max(u => u.Id) + 1;
            users.Add(user);
            Save();
        }

        public void Update(User user)
        {
            var idx = users.FindIndex(u => u.Id == user.Id);
            if (idx == -1) return;
            users[idx] = user;
            Save();
        }

        public void Delete(int id)
        {
            var u = Get(id);
            if (u == null) return;
            users.Remove(u);
            Save();
        }

        public int Count => users.Count;
    }

    public static partial class UserExtension
    {
        public static IServiceCollection AddAppRepositories(this IServiceCollection services)
        {
            services.AddScoped<IUserRepository, UserRepository>();
            return services;
        }
    }
}

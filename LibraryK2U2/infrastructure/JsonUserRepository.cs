using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using LibraryK2U2.interfaces;
using LibraryK2U2.models;

namespace LibraryK2U2.infrastructure
{
    public class JsonUserRepository : IUserRepository
    {
        private readonly string path;
        private List<User> users = new();

        public JsonUserRepository()
        {
            path = Path.Combine(
                Directory.GetParent(AppContext.BaseDirectory)!.Parent!.Parent!.FullName,
                "users.json"
            );

            Load();
            EnsureAdminExists();
        }

        private void Load()
        {
            if (!File.Exists(path))
            {
                users = new List<User>();
                Save();
                return;
            }

            var json = File.ReadAllText(path);
            users = JsonSerializer.Deserialize<List<User>>(json) ?? new List<User>();
        }

        private void EnsureAdminExists()
        {
            var admin = users.FirstOrDefault(u =>
                u.Username.Equals("admin", StringComparison.OrdinalIgnoreCase));

            if (admin == null)
            {
                users.Add(new User(
                    username: "admin",
                    displayName: "Administrator",
                    pin: "0000",
                    isAdmin: true
                ));

                Save();
                return;
            }

            if (!admin.IsAdmin())
            {
                admin.GrantAdmin();
                Save();
            }
        }

        public User? Get(string username)
        {
            return users.FirstOrDefault(u =>
                u.Username.Equals(username, StringComparison.OrdinalIgnoreCase));
        }

        public List<User> GetAll()
        {
            return users
                .OrderBy(u => u.Username)
                .ToList();
        }

        public void Add(User user)
        {
            users.Add(user);
        }

        public void Update(User user)
        {
            var index = users.FindIndex(u =>
                u.Username.Equals(user.Username, StringComparison.OrdinalIgnoreCase));

            if (index >= 0)
                users[index] = user;
        }

        public void Delete(string username)
        {
            var user = Get(username);
            if (user != null)
                users.Remove(user);
        }

        public void Save()
        {
            var json = JsonSerializer.Serialize(users, new JsonSerializerOptions
            {
                WriteIndented = true
            });

            File.WriteAllText(path, json);
        }
    }
}

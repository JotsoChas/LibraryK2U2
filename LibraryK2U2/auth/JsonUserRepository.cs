using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using LibraryK2U2.auth;


namespace LibraryK2U2.auth
{
    public class JsonUserRepository : IUserRepository
    {
        // File location inside project root during debugging
        private readonly string path = Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "users.json");

        private List<User> users = new();


        public JsonUserRepository()
        {
            Load();

            // Ensure that an admin account always exists
            var admin = users.FirstOrDefault(u =>
                u.Username.Equals("admin", StringComparison.OrdinalIgnoreCase));

            if (admin == null)
            {
                // Create admin if missing
                users.Add(new User("admin", "Administrator", "0000", true));
                Save();
            }
            else if (!admin.IsAdmin())
            {
                // Fix admin if JSON has admin but admin=false
                admin.GrantAdmin();
                Save();
            }
        }


        // Load file or create a new empty JSON on first startup
        private void Load()
        {
            if (File.Exists(path))
            {
                try
                {
                    var json = File.ReadAllText(path);
                    users = JsonSerializer.Deserialize<List<User>>(json) ?? new();
                }
                catch
                {
                    users = new(); // fallback if file corrupt
                }
            }
            else
            {
                users = new();
                Save();
            }
        }


        // Save changes to file
        public void Save()
        {
            var options = new JsonSerializerOptions { WriteIndented = true };
            File.WriteAllText(path, JsonSerializer.Serialize(users, options));
        }


        // --- CRUD operations ---

        public User? Get(string username) =>
            users.FirstOrDefault(x => x.Username.Equals(username, StringComparison.OrdinalIgnoreCase));

        public List<User> GetAll() =>
            users.ToList();

        public void Add(User u)
        {
            users.Add(u);
            Save();
        }

        public void Update(User u) =>
            Save();

        public void Delete(string username)
        {
            users.RemoveAll(x => x.Username.Equals(username, StringComparison.OrdinalIgnoreCase));
            Save();
        }
    }
}

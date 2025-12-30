using System.Collections.Generic;
using LibraryK2U2.models;

namespace LibraryK2U2.interfaces
{
    public interface IUserRepository
    {
        User? Get(string username);
        List<User> GetAll();
        void Add(User user);
        void Update(User user);
        void Delete(string username);
        void Save();
    }
}
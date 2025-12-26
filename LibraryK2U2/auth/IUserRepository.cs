using System.Collections.Generic;


namespace LibraryK2U2.auth
{
    public interface IUserRepository
    {
        List<User> GetAll();
        User? Get(string username);
        void Add(User user);
        void Update(User user);
        void Delete(string username);
        void Save();
    }
}

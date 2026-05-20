using HarcaBak.Entities;
namespace HarcaBak.Services
{
    public interface IUserService
    {
        void Add(User user);
        User? GetById(int id);
        void Delete(int id);
        void Update(User user);
        List<User> GetAll();
        User? GetByEmail(string email);
        bool ChangePassword(int userId, string oldPassword, string newPassword);
    }
}

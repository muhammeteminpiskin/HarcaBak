using HarcaBak.Data;
using HarcaBak.Entities;
using Microsoft.AspNetCore.Identity;

namespace HarcaBak.Services
{
    public class UserService: IUserService
    {
        private readonly AppDbContext _context;

        public UserService(AppDbContext appDbContext)
        {
            _context = appDbContext;
        }

        public void Add(User user)
        {
            _context.Users.Add(user);
            _context.SaveChanges();
        }
        public List<User> GetAll()
        {
            return _context.Users.ToList();
        }
        public User? GetById(int id)
        {
            return _context.Users.FirstOrDefault(x => x.Id == id);
        }
        public void Delete(int id)
        {
            var existingUser = GetById(id);
            if (existingUser != null)
            {
                _context.Remove(existingUser);
                _context.SaveChanges();
            }
        }
        public void Update(User user)
        {
            _context.Update(user);
            _context.SaveChanges();
        }
        public User? GetByEmail(string email)
        {
            return _context.Users
                .FirstOrDefault(user => user.Email == email);
        }
        public bool ChangePassword(int userId, string oldPassword, string newPassword)
        {
            var user = GetById(userId);

            if (user == null)
            {
                return false;
            }

            var passwordHasher = new PasswordHasher<User>();

            var verificationResult = passwordHasher.VerifyHashedPassword(
                user,
                user.PasswordHash,
                oldPassword);

            if (verificationResult == PasswordVerificationResult.Failed)
            {
                return false;
            }

            user.PasswordHash = passwordHasher.HashPassword(user, newPassword);

            Update(user);

            return true;
        }
    }
}

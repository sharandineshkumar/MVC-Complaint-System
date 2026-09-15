using MVC_Project.Data;
using MVC_Project.Models;

namespace MVC_Project.Services
{
    public class UserService : IUserService
    {
        private readonly AppDbContext _db;

        public UserService(AppDbContext db)
        {
            _db = db;
        }

        public bool EmailExists(string email)  // REGISTER PART 
        {
            return _db.Users.Any(u => u.Email == email);
        }

        public void RegisterUser(User user, string password)
        {
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(password);
            user.CreatedOn = DateTime.UtcNow;
            user.Role = "Citizen";

            _db.Users.Add(user);
            _db.SaveChanges();
        }

        public User? GetUserByEmailAndPassword(string email, string password)   // LOGIN PART
        {
            var user = _db.Users.FirstOrDefault(u => u.Email == email);
            if (user == null) return null;

            bool isValid = false;
            try
            {
                isValid = BCrypt.Net.BCrypt.Verify(password, user.PasswordHash);
            }
            catch
            {
                // Fallback for legacy plain text passwords created prior to BCrypt hashing
                if (user.PasswordHash == password)
                {
                    isValid = true;
                    // Automatically upgrade to BCrypt hash
                    user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(password);
                    _db.SaveChanges();
                }
            }

            return isValid ? user : null;
        }
    }
}
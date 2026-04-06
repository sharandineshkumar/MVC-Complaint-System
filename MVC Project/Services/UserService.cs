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

        public bool EmailExists(string email)  //REGISTER PART 
        {
            return _db.Users.Any(u => u.Email == email);
        }

        public void RegisterUser(User user, string password)
        {
            user.PasswordHash = password;
            user.CreatedOn = DateTime.Now;
            user.Role = "Citizen";

            _db.Users.Add(user);
            _db.SaveChanges();
        }

        public User? GetUserByEmailAndPassword(string email, string password)   
        {
            return _db.Users.FirstOrDefault(u => u.Email == email && u.PasswordHash == password);
        }
    }
}
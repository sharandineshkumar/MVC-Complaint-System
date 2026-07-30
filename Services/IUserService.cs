using MVC_Project.Models;

namespace MVC_Project.Services
{
    public interface IUserService
    {
        bool EmailExists(string email);
        void RegisterUser(User user, string password);
        User? GetUserByEmailAndPassword(string email, string password);
    }
}
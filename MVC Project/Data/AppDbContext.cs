using Microsoft.EntityFrameworkCore;
using MVC_Project.Models;

namespace MVC_Project.Data
{
    public class AppDbContext : DbContext //to execute  linq queries and save changes to db 
    {
        public  AppDbContext(DbContextOptions<AppDbContext> options) /* This parameter carries the connection string and database settings
                                                                          .ASP.NET Core reads it from appsettings.json and passes it here automatically.*/

            :base(options) //The parent needs the connection string to know which database to connect to.//
            {
            }

    
    public DbSet<Complaint> Complaints { get; set; }  

    public DbSet<User> Users { get; set; }  //represents the Users table in the database. Each User object corresponds to a row in that table.
    }  
}


using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SharedLibrary.Dtos;

namespace API.Data
{
    public class AppDbContext : IdentityDbContext<IdentityUser>
    {
        public AppDbContext(DbContextOptions options) : base(options)
        {

        }

        public DbSet<Resource> Resources { get; set; }
    }
}

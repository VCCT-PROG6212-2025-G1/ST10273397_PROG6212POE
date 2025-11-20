using Microsoft.EntityFrameworkCore;
using PROG6212POE.Models;

namespace PROG6212POE.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<ClaimModel> ClaimModel { get; set; }
        public DbSet<PROG6212POE.Models.UserModel> UserModel { get; set; } = default!;
    }
}


using CoreWithSwagger.Models.Refresh.Token.DBContext.Entity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CoreWithSwagger.Models.IdentityDbContext
{
    public class AppIdentityDbContext : IdentityDbContext<ApplicationUser>
    {
        public AppIdentityDbContext(DbContextOptions<AppIdentityDbContext> options):base(options)
        {

        }

        public DbSet<JwtRefreshToken> JwtRefreshTokens { get; set; }
    }
}

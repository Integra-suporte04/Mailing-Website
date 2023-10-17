using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace IntegraMailing.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
     : base(options)
        {
        }
        public DbSet<Campanhas> Campanhas { get; set; }
        public DbSet<tabela_mailing> tabela_mailing { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Campanhas>().ToTable("campanhas");
        }

        // Seus DbSets para outras entidades, se houver
        // public DbSet<SuaEntidade> SuaEntidades { get; set; }
    }
}

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
        public DbSet<MailingFinalizado> mailing_finalizado { get; set; }
        public DbSet<Empresas> Empresas { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            //Adicione aqui caso a classe tenha um nome diferente da tabela
            builder.Entity<Campanhas>().ToTable("campanhas");
            builder.Entity<MailingFinalizado>().ToTable("mailing_finalizado");
        }

        // Seus DbSets para outras entidades, se houver
        // public DbSet<SuaEntidade> SuaEntidades { get; set; }
    }
}

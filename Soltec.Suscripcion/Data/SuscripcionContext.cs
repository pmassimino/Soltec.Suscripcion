using Microsoft.EntityFrameworkCore;
using Soltec.Suscripcion.Code;
using Soltec.Suscripcion.Model;
using System.Security.Cryptography;
using System.Text;

namespace Soltec.Suscripcion.Data
{
    public class SuscripcionContext : DbContext
    {
        public DbSet<Usuario> Usuario { get; set; }        
        public DbSet<Rol> Rol { get; set; }
        public DbSet<Plan> Plan { get; set; }
        public DbSet<TicketValidacion> TicketValidacion { get; set; }        
        public DbSet<Model.Suscripcion> Suscripcion { get; set; }
        IConfiguration configuration;


        public string DbPath { get; }

        public SuscripcionContext()
        {
            IConfigurationBuilder configurationBuilder = new ConfigurationBuilder()
               .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

            this.configuration = configurationBuilder.Build();
            this.DbPath = configuration["DatabasePath"] + "\\Suscripcion.db";
        }

        // The following configures EF to create a Sqlite database file in the
        // special "local" folder for your platform.
        //protected override void OnConfiguring(DbContextOptionsBuilder options)
        //    => options.UseSqlite($"Data Source={DbPath}");
        // }
        protected override void OnConfiguring(DbContextOptionsBuilder options)
            //=> options.UseSqlite($"Data Source={DbPath}");
            => options.UseSqlServer(this.configuration.GetConnectionString("DefaultConnection"));
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<UsuarioRol>()
            .HasKey(ur => new { ur.IdUsuario, ur.IdRol });
            modelBuilder.Entity<UsuarioRol>()
                .HasOne(ur => ur.Usuario)
                .WithMany(u => u.Roles)
                .HasForeignKey(ur => ur.IdUsuario);
            modelBuilder.Entity<UsuarioRol>()
                .HasOne(ur => ur.Rol)
                .WithMany(r => r.Usuarios)
                .HasForeignKey(ur => ur.IdRol);

            //Clientes
            modelBuilder.Entity<UsuarioCuenta>()
            .HasKey(ur => new { ur.IdUsuario, ur.IdCuenta });
            modelBuilder.Entity<UsuarioCuenta>()
                .HasOne(ur => ur.Usuario)
                .WithMany(u => u.Cuentas)
                .HasForeignKey(ur => ur.IdUsuario);
            


        }



    }
}
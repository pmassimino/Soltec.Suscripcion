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
        
        public string DbPath { get; }

        public SuscripcionContext()
        {
            //var folder = Environment.SpecialFolder.LocalApplicationData;
            // var folder = Environment.SpecialFolder.LocalApplicationData;
            // var path = Environment.GetFolderPath(folder);
            // DbPath = System.IO.Path.Join(path, "Suscripcion.db");
        }

        // The following configures EF to create a Sqlite database file in the
        // special "local" folder for your platform.
        //protected override void OnConfiguring(DbContextOptionsBuilder options)
        //    => options.UseSqlite($"Data Source={DbPath}");
        // }
        protected override void OnConfiguring(DbContextOptionsBuilder options)
            => options.UseSqlite($"Data Source=Suscripcion.db");
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
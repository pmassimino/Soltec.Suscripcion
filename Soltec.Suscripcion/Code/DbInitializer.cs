using Microsoft.EntityFrameworkCore;
using Soltec.Suscripcion.Data;
using Soltec.Suscripcion.Model;

namespace Soltec.Suscripcion.Code
{
    public class DbInitializer
    {
        public static void Seed(SuscripcionContext context)
        {
            // context.Database.EnsureCreated();
            //Plan
            if (!context.Rol.Any())
            {                
                List<Plan> list = new List<Plan>();
                Plan item = new Plan() { Nombre = "Plan Abono General" };
                list.Add(item);               
                context.Plan.AddRange(list);
                context.SaveChanges();
            }
            //Roles
            if (!context.Rol.Any())
            {             
                List<Rol> list = new List<Rol>();
                Rol item = new Rol() { Nombre = "Admin"};
                list.Add(item);
                item = new Rol() { Nombre = "User" };
                list.Add(item);
                context.Rol.AddRange(list);
                context.SaveChanges();                
            }
            if (!context.Usuario.Any())
            {
                //Crear Password
                string password = "activasol";
                string salt = SecurityHelper.CreateSalt(10);
                string passwordHash = SecurityHelper.CreatePasswordHash(password, salt);
                var roles = context.Rol.ToList();                                
                Usuario tmpUsuario = new Usuario() {  Nombre = "admin", Password = passwordHash, Salt = salt, Email = "pmassimino@hotmail.com", Estado = "ACTIVO"};
                tmpUsuario.Roles.Add(new UsuarioRol { IdUsuario = 1, IdRol = 1 });
                tmpUsuario.Roles.Add(new UsuarioRol { IdUsuario = 1, IdRol = 2 });
                context.Usuario.AddRange(tmpUsuario);
                context.SaveChanges();
                //Crear Password
                password = "activasol";
                salt = SecurityHelper.CreateSalt(10);
                passwordHash = SecurityHelper.CreatePasswordHash(password, salt);
                
                tmpUsuario = new Usuario() { Nombre = "usuario", Password = passwordHash, Salt = salt, Email = "pablo.soltec@gmail.com", Estado = "ACTIVO" };
                tmpUsuario.Roles.Add(new UsuarioRol { IdUsuario = 2, IdRol = 2 });
                tmpUsuario.Cuentas.Add(new UsuarioCuenta { IdUsuario = 2, IdCuenta = "00001" });
                context.Usuario.AddRange(tmpUsuario);
                context.SaveChanges();
            }
        }
    }
}

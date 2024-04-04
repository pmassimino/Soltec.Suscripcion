using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Soltec.Suscripcion.Model
{
    public class Usuario
    {
        public Usuario()
        {
            this.Roles = new List<UsuarioRol>();
            this.Cuentas = new List<UsuarioCuenta>();
        }
        
        [Key, DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Nombre { get; set; }
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
        [DataType(DataType.Password)]
        public string Password { get; set; }
        public string Salt { get; set; }        
        public virtual IList<UsuarioRol> Roles { get; set; }
        public virtual IList<UsuarioCuenta> Cuentas { get; set; }
        public string Estado { get; set; } = "PENDIENTE";
    }
    public class Rol
    {
        [Key, DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Nombre { get; set; }
        public virtual IList<UsuarioRol> Usuarios { get; set; }

    }

    public class UsuarioRol
    {
        [Key, Column(Order = 0), Required, ForeignKey("Usuario")]
        public int IdUsuario { get; set; }
        [Key, Column(Order = 1), Required, ForeignKey("Rol")]
        public int IdRol { get; set; }
        public virtual Rol Rol { get; set; }
        public  virtual Usuario Usuario { get; set; }       
        
    }
    public class UsuarioCuenta
    {
        [Key, Column(Order = 0), Required, ForeignKey("Usuario")]
        public int IdUsuario { get; set; }
        [Key, Column(Order = 1), Required]
        public string IdCuenta { get; set; }        
        public virtual Usuario Usuario { get; set; }
    }


}

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Soltec.Suscripcion.Model
{
    public class Suscripcion
    {
        public Suscripcion()
        {           
        }
        [Key, DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string IdCuenta { get; set; }
        [Required , ForeignKey("Plan")]
        public int IdPlan { get; set; }
        public string Estado { get; set; }
        public decimal Importe { get; set; }
        public virtual Plan? Plan {get ;set;}

    }  



}

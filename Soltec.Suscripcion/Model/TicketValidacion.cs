using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Soltec.Suscripcion.Model
{
    public class TicketValidacion
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public int IdUsuario { get; set; } 
        public string Tipo { get; set; } = "";
        public DateTime Fecha { get; set; }=DateTime.Now;
        public DateTime FechaVencimiento { get; set; }=DateTime.Now.AddDays(5);
        public string Estado { get; set; } = "PENDIENTE";
    }
}

namespace Soltec.Suscripcion.Model
{
    public class Sujeto
    {
        public Sujeto()
        {
            this.Subdiarios = new List<Subdiario>();
        }
        public string Id { get; set; }
        public string Nombre { get; set; }
        public string NumeroDocumento { get; set; }
        public string NumeroIngBruto { get; set; } = "0";
        public string Domicilio { get; set; }
        public string Localidad { get; set; }
        public string IdProvincia { get; set; }
        public string Provincia { get; set; }
        public string IdCategoria { get; set; }
        public string Categoria { get; set; }
        public string CodigoPostal { get; set; }
        public string CondicionIva { get; set; } = "";
        public string CondicionIB { get; set; } = "";
        public List<Subdiario> Subdiarios { get; set; }
    }   
    public class Subdiario
    {
        public string Id { get; set; }
        public string Nombre { get; set; }
        public int IdDivisa { get; set; }
    }

}

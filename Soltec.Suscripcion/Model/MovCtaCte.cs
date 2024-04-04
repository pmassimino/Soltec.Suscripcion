namespace Soltec.Suscripcion.Model
{
    public class MovCtaCte
    {
        public DateTime FechaPase { get; set; }
        public DateTime FechaComprobante { get; set; }
        public DateTime FechaVencimiento { get; set; }
        public string Concepto { get; set; }
        public string IdCuenta { get; set; }
        public string NumeroComprobante { get; set; }
        public string IdTransaccion { get; set; }
        public decimal Debe { get; set; }
        public decimal Haber { get; set; }
        public decimal ImpD { get; set; }
        public decimal Saldo { get; set; }
        public decimal SaldoVencido { get; set; }
        public decimal SaldoAVencer { get; set; }
        public bool Vencido { get; set; }
        public string Tipo { get; set; }
        public bool TieneComp { get; set; }
        public decimal Cotizacion { get; set; }
        public int IdDivisa { get; set; }
        public int Orden { get; set; }
    }

}

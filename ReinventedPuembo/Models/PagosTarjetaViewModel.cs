namespace ReinventedPuembo.Models
{
    public class PagosTarjetaViewModel
    {

        public int? Id { get; set; }//id de registroPago

        public int IdRepresentante { get; set; }
        //public int ObligacionPendienteId { get; set; }
        public string Concepto { get; set; }

        public string? NumeroReferencia { get; set; }


        public decimal Valor { get; set; }


        public DateTime FechaTransaccion { get; set; }

        public int? EstadoPagoId { get; set; }
        public string EstadoPago { get; set; }

        public string DescripcionTarjeta { get; set; }
        public string EstadoTarjeta { get; set; }

        public string ObservacionTarjeta { get; set; }

        public string IdTransaccion { get; set; }
        public string codTransaccion { get; set; }

        public string Operador { get; set; }

    }
}

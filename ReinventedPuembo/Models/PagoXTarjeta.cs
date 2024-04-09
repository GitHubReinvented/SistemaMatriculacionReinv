using Domain;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ReinventedPuembo.Models
{
    public class PagoXTarjeta
    {
        public int[]  ObligacionPendienteId { get; set; }
      
        public string? NumeroReferencia { get; set; }

     
        public decimal Valor { get; set; }


        public DateTime FechaTransaccion { get; set; }

        public string? NombrePropietarioTarjeta { get; set; }
       
        public string? EstadoTarjeta { get; set; }
       
        public string? ObservacionTarjeta { get; set; }
      
        public string? DescripcionTarjeta { get; set; }
     
        public string? TokenTxTarjeta { get; set; }
       
        public string? idTxTarjeta { get; set; }
        
        public string? codTxTarjeta { get; set; }
        public string Operador { get; set; }
        public int EstadoCodigo { get; set; }

    }
}

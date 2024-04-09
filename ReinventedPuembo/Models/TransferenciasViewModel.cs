using Domain;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ReinventedPuembo.Models
{
    public class TransferenciasViewModel
    {
        public int? Id { get; set; }//id de registroPago
        
       public int IdRepresentante { get; set; }
        //public int ObligacionPendienteId { get; set; }
        public string Concepto { get; set; }

        public string? Estudiante { get; set; }
        public string? NumeroReferencia { get; set; }

       
        public decimal Valor { get; set; }


        public DateTime FechaTransaccion { get; set; }
       
        public int? CuentaDestinoId { get; set; }
        public string CuentaDestino { get; set; }
        public int? EstadoPagoId { get; set; }
        public string EstadoPago { get; set; }
        public string FotoTransferencia { get; set; }

       
    }
}

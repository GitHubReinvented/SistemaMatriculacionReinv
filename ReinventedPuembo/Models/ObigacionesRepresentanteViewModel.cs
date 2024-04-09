using Domain;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;

namespace ReinventedPuembo.Models
{
    public class ObigacionesRepresentanteViewModel
    {
        public int Id { get; set; }
        public int IdRepresentante { get; set; }
        public string Concepto { get; set; }
        public decimal Cuota { get; set; }

        public decimal Valor { get; set; }


        public decimal PorcentajeDescuento { get; set; }

        public decimal Descuento { get; set; }

        public decimal ValorFinal { get; set; }


        public decimal Abono { get; set; }

        public decimal Saldo { get; set; }

        public string Estado  { get; set; }
        public int EstadoId { get; set; }

        public string MedioPago { get; set; }   




    }
}

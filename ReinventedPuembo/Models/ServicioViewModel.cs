using Domain;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ReinventedPuembo.Models
{
    public class ServicioViewModel
    {
        public int TipoServicioId { get; set; }
        public string? Nombre { get; set; }
        public string? Descripcion { get; set; }
        public int Periodos { get; set; }
        public decimal CostoPeriodo { get; set; }
        public string? Link { get; set; }


    }
}

using Domain;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;

namespace ReinventedPuembo.Models
{
    public class RubroViewModel
    {
        public int idSucursal { get; set; }
        public int idRubro { get; set; }
        public int idCiclo { get; set; }
        public decimal? Costo { get; set; }
        public int CicloEscolarId { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }

    }
}

using Domain.Common;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
	public class Servicio : BaseDomainModel
	{
        [MaxLength(70)]
        public string? Descripcion { get; set; }
        public int? Periodos { get; set; }

		[Column(TypeName = "DECIMAL(8,2)")]
		public decimal? CostoPeriodo { get; set; }
        public bool? Estado { get; set; }
        [MaxLength(30)]
        public string? Nombre { get; set; }
        public string? Link { get; set; }

        public int TipoServicioId { get; set; }
        public TipoServicio? TipoServicio { get; set; }
		public List<ObligacionPendiente>? ObligacionPendiente { get; set; }
        

        public int SucursalId { get; set; }
        public Sucursal? Sucursal { get; set; }
    }
}

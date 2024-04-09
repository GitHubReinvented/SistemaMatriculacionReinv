using Domain.Common;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
	public class AplicaDescuento : BaseDomainModel
	{
		[Column(TypeName = "DECIMAL(5,2)")]
		public decimal? Porcentaje { get; set; }
		public byte Estado { get; set; }

		public int RubroId { get; set; }
        public Rubro? Rubro { get; set; }

		public int DescuentoId { get; set; }
		public Descuento? Descuento { get; set; }
	}
}

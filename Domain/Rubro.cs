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
	public class Rubro : BaseDomainModel
	{

        public Int16 TipoRubroId { get; set; }
		public TipoRubro? TipoRubro { get; set; }

        [Column(TypeName = "DECIMAL(8,2)")]
		public decimal? Costo { get; set; }


		public int CicloEscolarId { get; set; }
        public CicloEscolar? CicloEscolar { get; set; }

		public List<AplicaDescuento>? AplicaDescuento { get; set; }
		public List<ObligacionPendiente>? ObligacionPendiente { get; set; }

	}
}

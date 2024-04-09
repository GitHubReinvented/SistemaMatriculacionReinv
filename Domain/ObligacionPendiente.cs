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
	public class ObligacionPendiente : BaseDomainModel
	{
		[Column(TypeName = "DECIMAL(3,1)")]
		public decimal Cuota { get; set; }

		[Column(TypeName = "DECIMAL(8,2)")]
		public decimal Valor { get; set; }


		[Column(TypeName = "DECIMAL(5,2)")]
		public decimal PorcentajeDescuento { get; set; }

		[Column(TypeName = "DECIMAL(8,2)")]
		public decimal Descuento { get; set; }

		[Column(TypeName = "DECIMAL(8,2)")]
		public decimal ValorFinal { get; set; }


		[Column(TypeName = "DECIMAL(8,2)")]
		public decimal Abono { get; set; }

		[Column(TypeName = "DECIMAL(8,2)")]
		public decimal Saldo { get; set; }

		public int MatriculaId { get; set; }
        public Matricula? Matricula { get; set; }

		public int EstadoCuotaId { get; set; }
		public EstadoCuota? EstadoCuota { get; set; }

		public Nullable<int> ServicioId { get; set; }
		public Servicio? Servicio { get; set; }

		public int RubroId { get; set; }
		public Rubro? Rubro { get; set; }

		public int? RegistroPagosId { get; set; }

        public RegistroPagos? RegistroPagos { get; set;}

	}
}

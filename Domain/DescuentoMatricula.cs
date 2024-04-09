using Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
	public class DescuentoMatricula : BaseDomainModel
	{
        public int MatriculaId { get; set; }
        public Matricula? Matricula { get; set; }


		public int AplicaDescuentoId { get; set; }
		public AplicaDescuento? AplicaDescuento { get; set; }
	}
}

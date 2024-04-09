using Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
	public class AlumnoTransporte: BaseDomainModel
	{
        public int DireccionId { get; set; }
        public Direccion? Direccion { get; set; }

		public int? RutaId { get; set; }
		public Ruta? Ruta { get; set; }

		public int DiaId { get; set; }
		public Dia? Dia { get; set; }
	}
}

using Domain.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
	public class ObservacionMedica : BaseDomainModel
	{
		[MaxLength(200)]
        public string? Descripcion { get; set; }
        public bool? Estado { get; set; }

		public List<FichaObservacionMedica>? FichaObservacionMedica { get; set; }

	}
}

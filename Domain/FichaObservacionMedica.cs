using Domain.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
	public class FichaObservacionMedica : BaseDomainModel
	{
        public bool? Respuesta { get; set; }

        [MaxLength(200)]
        public string? Especificacion { get; set; }

        public int ObservacionMedicaId { get; set; }
		public ObservacionMedica? ObservacionMedica { get; set; }

		public int FichaMedicaId { get; set; }
		public FichaMedica? FichaMedica { get; set; }


	}
}

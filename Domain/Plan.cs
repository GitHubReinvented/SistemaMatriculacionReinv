using Domain.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
	public class Plan : BaseDomainModel
	{
		[MaxLength(70)]
        public string? Descripcion { get; set; }
        public bool? Estado { get; set; }

        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }

        public int? AreaAcademicaId { get; set; }
		public AreaAcademica? AreaAcademica { get; set; }

        public int? NivelAreaId { get; set; }
        public NivelArea? NivelArea { get; set; }


		public Habilidad? Habilidad { get; set; }
	}
}

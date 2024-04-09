using Domain.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
	public class Calificacion : BaseDomainModel
	{
        public int SubHabilidadId { get; set; }
        public SubHabilidad? SubHabilidad { get; set; }

		public int MatriculaId { get; set; }
		public Matricula? Matricula { get; set; }

		public int ValorCalificacionId { get; set; }
		public ValorCalificacion? ValorCalificacion { get; set; }

		[MaxLength(10)]
		public string? Valor { get; set; }
		[MaxLength(500)]
		public string? Justificacion { get; set; }
		[MaxLength(300)]
        public string? Observacion { get; set; }
    }
}

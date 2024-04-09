using Domain.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
	public class ValorCalificacion : BaseDomainModel
	{
        [MaxLength(10)]
        public string? Escala { get; set; }
        [MaxLength(30)]
        public string? Significado { get; set; }
        [MaxLength(200)]
        public string? Valor { get; set; }

        public int TipoCalificacionId { get; set; }
        public TipoCalificacion? TipoCalificacion { get; set; }
		public List<Calificacion>? Calificacion { get; set; }
    }
}

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
	public class Habilidad : BaseDomainModel
	{
        [MaxLength(120)]
        public string? Descripcion { get; set; }

        [Column(TypeName = "DECIMAL(5,2)")]
        public decimal? Porcentaje { get; set; }
        public bool? Estado { get; set; }
        public List<SubHabilidad>? SubHabilidad { get; set; }
	}
}

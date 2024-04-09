using Domain.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
	public class TipoCalificacion : BaseDomainModel
	{
        [MaxLength(20)]
        public string? Descripcion { get; set; }

        public List<ValorCalificacion>? ValorCalificacion { get; set; }
    }
}

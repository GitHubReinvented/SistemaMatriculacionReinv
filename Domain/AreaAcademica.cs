using Domain.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
	public class AreaAcademica : BaseDomainModel
	{
        [MaxLength(70)]
        public string? Descripcion { get; set; }
        public bool? Estado { get; set; }
        public List<NivelArea>? NivelArea { get; set; }
    }
}

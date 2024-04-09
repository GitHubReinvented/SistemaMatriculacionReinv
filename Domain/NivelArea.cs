using Domain.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
	public class NivelArea : BaseDomainModel
	{
		[MaxLength(30)]
		public string? Descripcion { get; set; }

		public List<Plan>? Plan { get; set; }

		public int NivelEscolarId { get; set; }
        public NivelEscolar? NivelEscolar { get; set; }

        public int AreaAcademicaId { get; set; }
        public AreaAcademica? AreaAcademica { get; set; }
    }
}

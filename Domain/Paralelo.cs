using Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
	public class Paralelo : BaseDomainModel
	{
        public char? Nemonico { get; set; }
		public short? Tutor { get; set; }
		public short? Asistente { get; set; }


        public int NivelEscolarId { get; set; }
        public NivelEscolar? NivelEscolar { get; set; }

		public int CicloEscolarId { get; set; }
		public CicloEscolar? CicloEscolar { get; set; }

	}
}

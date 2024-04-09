using Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
	public class Matricula : BaseDomainModel
	{
		public int PersonaId { get; set; }
		public Persona? Persona { get; set; }

		public int EstadoMatriculaId { get; set; }
		public List<EstadoMatricula>? EstadoMatricula { get; set; }

        public int NivelEscolarId { get; set; }
        public NivelEscolar? NivelEscolar { get; set; }

        public int CicloEscolarId { get; set; }
        public CicloEscolar? CicloEscolar { get; set; }


        public char? Nemonico { get; set; }
		public string? Foto { get; set; }

        public List<Calificacion>? Calificacion { get; set; }
        public List<DescuentoMatricula>? DescuentoMatricula { get; set; }
        public List<ObligacionPendiente>? ObligacionPendiente { get; set; }

        public List<FichaMedica>? FichaMedica { get; set; }
    }
}

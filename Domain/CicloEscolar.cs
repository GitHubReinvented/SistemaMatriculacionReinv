using Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
	public class CicloEscolar : BaseDomainModel
	{
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        public int NumeroPeriodos { get; set; }

        public bool Estado { get; set; }
        public int TipoPeriodoId { get; set; }
        public TipoPeriodo? TipoPeriodo { get; set; }

        public List<Matricula>? Matricula { get; set; }

        //public List<Paralelo>? Paralelo { get; set; }
		public List<Rubro>? Rubro { get; set; }

        public int SucursalId { get; set; }
        public Sucursal? Sucursal { get; set; }

    }
}

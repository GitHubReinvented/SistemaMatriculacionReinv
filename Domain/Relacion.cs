using Domain.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
	public class Relacion : BaseDomainModel
	{
        public int TipoRelacionId { get; set; }
        public TipoRelacion? TipoRelacion { get; set; }

		public int PersonaId { get; set; }
        [ForeignKey("PersonaId")]
        [InverseProperty("RelacionPersona")]
        public Persona? Persona { get; set; }


        public int Persona2Id { get; set; }
        [ForeignKey("Persona2Id")]
        [InverseProperty("RelacionPersona2")]
        public Persona? Persona2 { get; set; }
    }
}

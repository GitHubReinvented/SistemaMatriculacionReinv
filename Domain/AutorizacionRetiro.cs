using Domain.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
	public class AutorizacionRetiro : BaseDomainModel
	{
        [Key]
        public int Id { get; set; }
        public int PersonaId { get; set; }
        public Persona? Persona { get; set; }

        public DateTime? FechaRetiro { get; set; }
        [MaxLength(200)]
        public string? Motivo { get; set; }
        
        
    }
}

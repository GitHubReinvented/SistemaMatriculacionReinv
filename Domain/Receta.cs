using Domain.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
	public class Receta : BaseDomainModel
	{
        [MaxLength(70)]
        public string? Medicamento { get; set; }
        [MaxLength(100)]
        public string? Dosis { get; set; }
        public int HistorialMedicoId { get; set; }
        public HistorialMedico? HistorialMedico { get; set; }
	}
}

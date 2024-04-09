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
	public class HistorialMedico : BaseDomainModel
	{
        public int FichaMedicaId { get; set; }
        public FichaMedica? FichaMedica { get; set; }

        [Column(TypeName = "DECIMAL(6,2)")]
		public decimal Peso { get; set; }
        [Column(TypeName = "DECIMAL(6,2)")]
        public decimal Talla { get; set; }

		[Column(TypeName = "DECIMAL(4,2)")]
		public decimal Temperatura { get; set; }

		[Column(TypeName = "DECIMAL(4,2)")]
		public decimal Presion { get; set; }

		[Column(TypeName = "DECIMAL(4,2)")]
		public decimal Oxigenacion { get; set; }

        [MaxLength(500)]
		public string? Sintomas { get; set; }
        [MaxLength(500)]
        public string? Diagnostico { get; set; }
        [MaxLength(200)]
        public string? Alarmas { get; set; }

        public List<Receta>? Receta { get; set; }
    }
}

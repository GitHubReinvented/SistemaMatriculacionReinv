using Domain.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
	public class FichaMedica : BaseDomainModel
	{
        public int MatriculaId { get; set; }
        public Matricula? Matricula { get; set; }

        [MaxLength(70)]
        public string? NombresMedico { get; set; }
        [MaxLength(70)]
        public string? ApellidosMedico { get; set; }
        [MaxLength(120)]
        public string? DireccionMedico { get; set; }
        [MaxLength(70)]
        public string? ReferenciaDireccionMedico { get; set; }
        [MaxLength(10)]
        public string? CelularMedico { get; set; }
        [MaxLength(10)]
        public string? TelefonoMedico { get; set; }
        [MaxLength(300)]
        public string? Alergias { get; set; }
        [MaxLength(500)]
        public string? RestriccionesAlimenticias { get; set; }
        [MaxLength(500)]
        public string? NotasImportanteSalud { get; set; }

        public bool? AutorizaAntiinflamatorioAnalgesico { get; set; } = false;
        [MaxLength(70)]
        public string? SeguroMedico { get; set; }
        public List<FichaObservacionMedica>? FichaObservacionMedica { get; set; }
        public List<HistorialMedico>? HistorialMedico { get; set; }
	}
}

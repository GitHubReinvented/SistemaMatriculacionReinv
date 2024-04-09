using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public class JustificaNoSeguro
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public DateTime FechaRegistro { get; set; }
        [Required]
        [Column(TypeName = "VARCHAR")]
        [StringLength(100)]
        public string NombreRepresentante { get; set; }
        [Required]
        [Column(TypeName = "VARCHAR")]
        [StringLength(15)]
        public string TelefonoRepresentante { get; set; }
        [Required]
        [Column(TypeName = "VARCHAR")]
        [StringLength(20)]
        public string IdentificacionRepresentante { get; set; }
        [Required]
        [Column(TypeName = "VARCHAR")]
        [StringLength(300)]
        public string Justificacion { get; set; }
        public bool Estado { get; set; }//por aprobar false//true aprobado

    }
}

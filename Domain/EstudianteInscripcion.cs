using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public class EstudianteInscripcion
    {
        public int Id { get; set; }
        [Column(TypeName = "VARCHAR(70)")]
        public string Apellidos { get; set; }
        [Column(TypeName = "VARCHAR(70)")]
        public string Nombres { get; set; }
        public int TipoIdentificacionId { get; set; }
        [Column(TypeName = "VARCHAR(20)")]
        public string Identificacion { get; set; }
        public int Grado { get; set; }//o nivel escolar

        public int TipoEstudianteId { get; set; }
        

       public DateTime FechaNacimiento { get; set; }
        public string CedulaMadre { get; set; }
        [Column(TypeName = "VARCHAR(70)")]
        public string NombresMadre { get; set; }
        [Column(TypeName = "VARCHAR(70)")]
        public string ApellidosMadre { get; set; }
        [Column(TypeName = "VARCHAR(15)")]
        public string MovilMadre { get; set; }
        [Column(TypeName = "VARCHAR(70)")]
        public string EmailMadre { get; set; }
        [Column(TypeName = "VARCHAR(20)")]
        public string CedulaPadre { get; set; }
        [Column(TypeName = "VARCHAR(70)")]
        public string NombresPadre { get; set; }
        [Column(TypeName = "VARCHAR(70)")]
        public string ApellidosPadre { get; set;}
        [Column(TypeName = "VARCHAR(15)")]
        public string MovilPadre { get; set; }
        [Column(TypeName = "VARCHAR(70)")]
        public string Emailpadre { get; set; }
        [Column(TypeName = "VARCHAR(300)")]
        public string UsuarioId { get; set; }
        public int SucursalId { get; set; }
        [Column(TypeName = "DECIMAL(8,2")]
        public decimal? MatriculaDescuento { get; set; }
        [Column(TypeName = "VARCHAR(15)")]
        public string? TipoDescuentoMatricula { get; set; }
        [Column(TypeName = "DECIMAL(8,2)")]
        public decimal? MatriculaAbono { get; set; }
        [Column(TypeName = "DECIMAL(8,2")]
        public decimal? PensionDescuento { get; set; }
        [Column(TypeName = "VARCHAR(15)")]
        public string? TipoDescuentoPension  { get; set; }
        [Column(TypeName = "DECIMAL(8,2)")]
        public  decimal? PensionAbono { get; set; }

        public decimal? SeguroDescuento { get; set; }
        [Column(TypeName = "VARCHAR(15)")]
        public string? TipoDescuentoSeguro { get; set; }
        [Column(TypeName = "DECIMAL(8,2)")]
        public decimal? SeguroAbono { get; set; }

        public bool procesado { get; set; }






    }
}

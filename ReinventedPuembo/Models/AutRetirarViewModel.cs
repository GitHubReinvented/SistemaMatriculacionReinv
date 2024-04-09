using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ReinventedPuembo.Models
{
    public class AutRetirarViewModel
    {
        public int? id { get; set; }
        public int? idPersona2 { get; set; }
        public IFormFile? Avatar { get; set; }
      
        public string? Nombres { get; set; }
       
        public string? Apellidos { get; set; }
       
        public string? Identificacion { get; set; }
        public int TipoIdentificacionId { get; set; }
        
        public IFormFile? PDFIdentificacion { get; set; }
        public string? TelefonoMovil { get; set; }
       
        public int TipoRelacionId { get; set; }
        public string CedulaAlumno { get; set; }
        public int? SucursalId { get; set; }

    }
}

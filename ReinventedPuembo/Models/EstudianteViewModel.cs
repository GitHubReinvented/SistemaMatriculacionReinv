using Domain;
using System.ComponentModel.DataAnnotations;

namespace ReinventedPuembo.Models
{
    public class EstudianteViewModel
    {
        public int? TipoPersonaId { get; set; }
        public IFormFile? Avatar { get; set; }
        [Required(ErrorMessage = "Campo requerido")]
        public string? Nombres { get; set; }
        [Required(ErrorMessage = "Campo requerido")]
        public string? Apellidos { get; set; }
        [Required(ErrorMessage = "Campo requerido")]
        public int TipoIdentificacionId { get; set; }
        [StringLength(20)]
        [Required(ErrorMessage = "Campo requerido")]
        public string Identificacion { get; set; }
        public IFormFile? PDFIdentificacion { get; set; }

        public string? CallePrincipal { get; set; }
        public string? Numero { get; set; }
        public string? CalleSecundaria { get; set; }
        [Required(ErrorMessage = "Campo requerido")]
        public DateTime FechaNacimiento { get; set; }
        public decimal Peso { get; set; }
        public decimal Talla { get; set; }
        public int? NumeroDosis { get; set; }
        public string? EnfermedadCronica{ get; set; }
        public string? MedicacionContinua{ get; set; }
        public string? cirugia{ get; set; }
        public bool? Analgesico{ get; set; }
        public string? SeguroPrivado{ get; set; }
        public string? DatoAdicional{ get; set; }
        public string? NombreMedico { get; set; }
        public string? TelefonoMedico{ get; set; }
        public string? CelularMedico { get; set; }
        public string? DireccionMedico { get; set; }
        public string? alergias{ get; set; }
        public string? restriccionAlimento{ get; set; }
        public string? datoAdicional { get; set; }
        public int nivel { get; set; }

        public string? UsuarioRegistra { get; set; }
        public int CicloEscolarId { get;set; }

        public int SucursalId { get; set; }

        


    }
}

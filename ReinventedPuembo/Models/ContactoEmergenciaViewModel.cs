namespace ReinventedPuembo.Models
{
    public class ContactoEmergenciaViewModel
    {
        public int? id { get; set; }
        public int? idPersona2 { get; set; }
        public IFormFile? Avatar { get; set; }
        public string? Nombres { get; set; }
        public string? Apellidos { get; set; }
        public string? Identificacion { get; set; }
        public int TipoIdentificacionId { get; set; }
        public string? TelefonoMovil { get; set; }
        public int TipoRelacionId { get; set; }
        public bool? ContactoEmergencia { get; set; }
        public string CedulaAlumno { get; set; }

    }
}

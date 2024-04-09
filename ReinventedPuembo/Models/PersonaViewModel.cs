using Domain;

namespace ReinventedPuembo.Models
{
    public class PersonaViewModel
    {
        public int TipoIdentificacionId { get; set; }
        public string? Identificacion { get; set; }
        public string? Nombres { get; set; }
        public string? Apellidos { get; set; }
        public string? EmailPrincipal { get; set; }
        public string? EmailSecundario { get; set; }
        public string? TelefonoMovil { get; set; }
        public string? Avatar { get; set; } = "avatar-default.png";

        public int ProfesionId { get; set; }

        public int TipoPersonaId { get; set; }

        public string? Cargo { get; set; }
        public string? LugarTrabajo { get; set; }
        public bool AutorizadoRetirar { get; set; } = false;
        public DateTime FechaNacimiento { get; set; }
        public bool Representante { get; set; } = false;

    }
}

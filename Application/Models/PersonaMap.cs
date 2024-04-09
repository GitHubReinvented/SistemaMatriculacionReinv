using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Models
{
    public class PersonaMap
    {
        public int TipoIdentificacionId { get; set; }
        public string? Identificacion { get; set; }
        public string? Nombres { get; set; }
        public string? Apellidos { get; set; }
        public string? EmailPrincipal { get; set; }
        public string? EmailSecundario { get; set; }
        public string? TelefonoMovil { get; set; }
        public IFormFile? Avatar { get; set; } 

        public int ProfesionId { get; set; }

        public int TipoPersonaId { get; set; }

        public string? Cargo { get; set; }
        public string? LugarTrabajo { get; set; }
        public bool AutorizadoRetirar { get; set; } = false;
        public DateTime FechaNacimiento { get; set; }
        public bool Representante { get; set; } = false;
    }
}

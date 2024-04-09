using Microsoft.AspNetCore.Identity;
using System.Text.Json.Serialization;

namespace Domain
{
    public class Usuario : IdentityUser
    {
        public string? Nombres { get; set; }
        public string? Apellidos { get; set; }
		public string? Avatar { get; set; } = "avatar-default.png";

		public bool isActive { get; set; } = true;
        public List<UsuarioSucursal>? UsuarioSucursal { get; set; }
        public List<Persona>? Persona { get; set; }

	}
}
using Domain;

namespace ReinventedPuembo.Models
{
	public class UsuarioViewModel
	{
        public string? id { get; set; }
        public string? Avatar { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }
        public string? Nombres { get; set; }
        public string? Apellidos { get; set; }
        public string? Sucursal { get; set; }
        public int SucursalId { get; set; }
        public string? Rol { get; set; }
        public bool isActive { get; set; }
    }
}

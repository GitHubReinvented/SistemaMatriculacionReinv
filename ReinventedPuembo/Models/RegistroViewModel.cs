using System.ComponentModel.DataAnnotations;

namespace ReinventedPuembo.Models
{
    public class RegistroViewModel
    {
        public string? Nombre { get; set; }
        public string? Apellido { get; set; }
        public string? Cedula { get; set; }
        public string? Email { get; set; }
        public string? Telefono { get; set; }
        public string? Rol { get; set; }
        public string? Sucursal { get; set; }
    }
}

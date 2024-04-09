using System.ComponentModel.DataAnnotations;

namespace ReinventedPuembo.Models
{
    public class RecoveryPasswordViewModel
    {
        public string Email { get; set; }
        public string token {  get; set; }
        [Required(ErrorMessage = "La contraseña es requerida")]
        [MinLength(6, ErrorMessage = "La contraseña debe tener al menos 6 caracteres")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*\W).*$", ErrorMessage = "La contraseña debe contener al menos una mayúscula, un número y un caracter especial")]
        public string Password { get; set; }

        [Compare("Password", ErrorMessage = "Las contraseñas no coinciden")]
        public string Password2 { get; set; }

    }
}

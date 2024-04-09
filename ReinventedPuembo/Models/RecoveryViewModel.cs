using System.ComponentModel.DataAnnotations;

namespace ReinventedPuembo.Models
{
    public class RecoveryViewModel
    {
        [EmailAddress(ErrorMessage = "Ingrese una dirección de correo electrónico válida")]
        [Required(ErrorMessage = "Ingrese un email")]
        public string Email { get; set; }
    }
}

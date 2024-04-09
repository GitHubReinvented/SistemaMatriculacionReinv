using System.ComponentModel.DataAnnotations;

namespace ReinventedPuembo.Models
{
    public class DocumentoCrearDTO
    {
        [Required(ErrorMessage = "El campo nombre es requerido")]
        [MaxLength(70)]
        [MinLength(4)]
        public string doc_nombre { get; set; }
        public IFormFile plantilla { get; set; }
    }

}

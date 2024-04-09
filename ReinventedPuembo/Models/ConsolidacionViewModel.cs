using Domain;

namespace ReinventedPuembo.Models
{
    public class ConsolidacionViewModel
    {
        public int IdRepresentante { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string CedulaRepresentante { get; set; }
        public string NombreEstudiante { get; set; }
        public string ApellidoEstudiante { get; set; }
        public string Sucursal { get; set; }
        public string? JustificacionNoSeguro { get; set; }
        public String Estado { get; set; }
    }
}

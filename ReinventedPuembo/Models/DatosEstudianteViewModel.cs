using Domain;

namespace ReinventedPuembo.Models
{
    public class DatosEstudianteViewModel
    {
        public IEnumerable<Persona> Estudiantes { get; set; }
        public IEnumerable<FichaMedica> FichaMedicaEstudiante { get; set; }
        public IEnumerable<HistorialMedico> HistorialEstudiante { get; set; }
        public IEnumerable<FichaObservacionMedica> FichaObsMedicaEstudiante { get; set; }
        public IEnumerable<Direccion> DireccionEstudiante { get; set; }

    }
}

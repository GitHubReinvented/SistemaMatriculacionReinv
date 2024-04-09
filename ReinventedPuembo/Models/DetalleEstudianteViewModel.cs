using Domain;

namespace ReinventedPuembo.Models
{
    public class DetalleEstudianteViewModel
    {
        public Persona EstudiantesSeleccionado { get; set; }
        public List<HistorialMedico> HistorialMedico { get; set; }
        public List<FichaObservacionMedica> ObservacionesMedicas { get; set; }
        public Matricula MatriculaEstudiante { get; set; }
        public Direccion DireccionEstudiante { get; set; }
        public FichaMedica FichaEstudiante { get; set; }
    }
}

using Domain;

namespace ReinventedPuembo.Models
{
    public class ListaUsuariosViewModel
    {
        public IReadOnlyList<Matricula> Estudiantes { get; set; }
        public IReadOnlyList<Relacion> Padres { get; set; }
    }
}

using Domain;
using ReinventedPuembo.Models;

namespace ReinventedPuembo.Interfaces
{
    public interface IDocumento
    {
        Task<byte[]> ObtenerPlantilla(string nombrePlantilla);
        Task<string> GenerarPDFcontrato(string textoTopdf, string cedulaEstudiante, string codSucursal, string CicloEscolar);
        
        
    }
}

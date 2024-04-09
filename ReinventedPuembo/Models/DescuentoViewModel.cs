using Domain;

namespace ReinventedPuembo.Models
{
    public class DescuentoViewModel
    {
        public int RubroId { get; set; }
        public int DescuentoId { get; set; }
        public string? TipoDescuento { get; set; }
        public string? DescripcionDescuento { get; set; }
        public decimal? Porcentaje { get; set; }
    }
}

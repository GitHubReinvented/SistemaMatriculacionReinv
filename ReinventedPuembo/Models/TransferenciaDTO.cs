namespace ReinventedPuembo.Models
{
    public class TransferenciaDTO
    {
        public IFormFile? fotoTransferencia { get; set; }
        public string ObligacionPendienteId { get; set; }
        public string numeroReferencia { get; set; }
        public DateTime fechaTransferencia { get; set; }

        public decimal valor { get; set; }

        public int? cuentaDestino { get; set; }

        public int[] auxiliar { get; set; }

    }
}

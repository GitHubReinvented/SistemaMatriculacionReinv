namespace ReinventedPuembo.Models
{
    public class CuentasViewModel
    {
        public int CuentaId { get; set; }
        public int SucursalId { get; set; }
        public int TipoCuentaId { get; set; }
        public int BancoId { get; set; }
        public string? NumeroCuenta { get; set; }
        public bool? Estado { get; set; }
    }
}

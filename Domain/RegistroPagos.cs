using Domain.Common;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
	public class RegistroPagos: BaseDomainModel
	{
        public int TipoPagoId { get; set; }
        public TipoPago TipoPago { get; set; }
       

        [MaxLength(70)]
        public string? NumeroReferencia { get; set; }

		[Column(TypeName = "DECIMAL(8,2)")]
		public decimal Valor { get; set; }

        
        public DateTime FechaTransaccion { get; set; }
        
        public int? CuentaSucursalId { get; set; }
        public CuentaSucursal? CuentaSucursal { get; set; }
        public int? EstadoPagoId { get; set; }
        public EstadoPago? EstadoPago { get; set; }
    
        [MaxLength(70)]
        public string? NombrePropietarioTarjeta { get; set; }
        [MaxLength(200)]
        public string? EstadoTarjeta { get; set; }
        [MaxLength(200)]
        public string? ObservacionTarjeta { get; set; }
        [MaxLength(200)]
        public string? DescripcionTarjeta { get; set; }
        [MaxLength(255)]
        public string? TokenTxTarjeta { get; set; }
        [MaxLength(70)]
        public string? idTxTarjeta { get; set; }
        [MaxLength(70)]
        public string? codTx { get; set; }
        [MaxLength(70)]
        public string? Foto { get; set; }

        //public int ObligacionPendienteId { get; set; }
        public List<ObligacionPendiente>? ObligacionPendiente { get; set; }
    }
}

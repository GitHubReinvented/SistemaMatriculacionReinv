using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Common;

namespace Domain
{
    public class CuentaSucursal : BaseDomainModel
    {

        public int SucursalId { get; set; }
        public Sucursal? Sucursal { get; set; }

        public int TipoCuentaId { get; set; }
        public TipoCuenta? TipoCuenta { get; set; }

        public int BancoId { get; set; }
        public Banco? Banco { get; set; }

        [Required]
        [Column(TypeName = "VARCHAR")]
        [StringLength(30)]
        public string? NumeroCuenta { get; set; }
        public bool Estado { get; set; }

        public List<RegistroPagos>? RegistroPagos { get; set; }
    }
}

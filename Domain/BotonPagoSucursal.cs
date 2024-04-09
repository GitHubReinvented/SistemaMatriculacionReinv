using Domain.Common;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
	public class BotonPagoSucursal : BaseDomainModel
	{
        public int BotonPagoId { get; set; }
        public BotonPago? BotonPago { get; set; }

		public int SucursalId { get; set; }
		public Sucursal? Sucursal { get; set; }
        [Column(TypeName = "VARCHAR")]
        [StringLength(300)]
        public string TokenPruebas { get; set;}

        [Column(TypeName = "VARCHAR")]
        [StringLength(300)]
        public string TokenProduccion { get; set; }

    }
}

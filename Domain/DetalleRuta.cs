using Domain.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
	public class DetalleRuta : BaseDomainModel
	{

        public int? Parroquia { get; set; }

		[MaxLength(30)]
        public string? Barrio { get; set; }
        public byte Orden { get; set; }

		public int RutaId { get; set; }
		public Ruta? Ruta { get; set; }

	}
}

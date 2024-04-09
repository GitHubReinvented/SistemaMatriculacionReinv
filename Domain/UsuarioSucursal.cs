using Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
	public class UsuarioSucursal : BaseDomainModel
	{
        public string? UsuarioId { get; set; }
        public Usuario? Usuario { get; set; }

		public int SucursalId { get; set; }
		public Sucursal? Sucursal { get; set; }
	}
}

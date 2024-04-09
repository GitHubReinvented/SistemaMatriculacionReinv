using Domain.Common;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
	public class RolTarea : BaseDomainModel
	{
        public int TareaId { get; set; }
        public Tarea? Tarea { get; set; }

		public string? RolUsuarioId { get; set; }
		public RolUsuario? RolUsuario { get; set; }
	}
}

using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
	public class RolUsuario : IdentityRole
	{
        [MaxLength(100)]
        public string? Descripcion { get; set; }
        public List<RolTarea>? RolTarea { get; set; }
    }
}

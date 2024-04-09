﻿using Domain.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
	public class TipoEstudiante : BaseDomainModel
	{
		[MaxLength(30)]
        public string? Descripcion { get; set; }
		public List<Persona>? Persona { get; set; }

	}
}

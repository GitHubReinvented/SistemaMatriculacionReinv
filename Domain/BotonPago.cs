﻿using Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
	public class BotonPago : BaseDomainModel
	{
        public string? Descripcion { get; set; }
        public List<BotonPagoSucursal>? BotonPagoSucursal { get; set; }
	}
}

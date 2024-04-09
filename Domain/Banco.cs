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
    public class Banco : BaseDomainModel
    {

        [Column(TypeName = "VARCHAR")]
        [StringLength(70)]
        public string? Nombre { get; set; }
        public List<CuentaSucursal>? CuentaSucursal { get; set; }
    }
}


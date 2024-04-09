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
    public class EstadoPago
    {
        public int Id { get; set; }
        [Column(TypeName = "VARCHAR")]
        [StringLength(70)]
        [Required]
        public string Descripcion { get; set; }

        public List<RegistroPagos>? RegistroPagos { get; set; }
    }
}

using Domain.Common;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public class Documento : BaseDomainModel
    {
        [Key]
        public Int16 Id { get; set; }
        [Required]
        [Column(TypeName = "VARCHAR")]
        [StringLength(70)]
        public string Nombre { get; set; }
        [Required]
        
        public bool Estado { get; set; }
        [Required]
        public int SucursalId { get; set; }

    }
}

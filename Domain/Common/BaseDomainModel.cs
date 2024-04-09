using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Common
{
    public class BaseDomainModel
    {
        public int Id { get; set; }
        public DateTime? CreatedDate { get; set; }
        [Column(TypeName = "VARCHAR")]
        [StringLength(255)]
        public string? CreatedBy { get; set; }
        public DateTime? LastModifiedDate { get; set; }
        [Column(TypeName = "VARCHAR")]
        [StringLength(255)]
        public string? LastModifiedBy { get; set; }
    }
}

using Domain.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
	public class NivelEscolar : BaseDomainModel
	{
        [MaxLength(30)]
        public string? Descripcion { get; set; }
        public byte Orden { get; set; }
        [Column(TypeName = "DECIMAL(8,2)")]
        public decimal? CostoSeguroEstudiantil { get; set; }
        public List<Paralelo>? Paralelo { get; set; }
        public List<NivelArea>? NivelArea { get; set; }

        public List<Matricula>? Matricula { get; set; }
    }
}

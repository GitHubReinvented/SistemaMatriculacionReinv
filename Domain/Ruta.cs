using Domain.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
	public class Ruta : BaseDomainModel
	{
        [MaxLength(20)]
        public string? Descripcion { get; set; }
        public int TransportistaId { get; set; }
        public Transportista? Transportista { get; set; }

        public List<AlumnoTransporte>? AlumnoTransporte { get; set; }
        public List<DetalleRuta>? DetalleRuta { get; set; }
	}
}

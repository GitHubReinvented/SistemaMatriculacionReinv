using Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public class Dia : BaseDomainModel
    {
        public string? Nombre { get; set; }
        public List<AlumnoTransporte>? AlumnoTransporte { get; set; }
    }
}

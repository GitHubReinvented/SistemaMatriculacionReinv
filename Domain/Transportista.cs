using Domain.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
	public class Transportista : BaseDomainModel
	{
        [MaxLength(70)]
        public string? Nombres { get; set; }
        [MaxLength(70)]
        public string? Apellidos { get; set; }
        [MaxLength(70)]
        public string? Cedula { get; set; }
        [MaxLength(70)]
        public string? NumeroLicencia { get; set; }
        [MaxLength(20)]
        public string? FotoPersona { get; set; }
        [MaxLength(10)]
        public string? NumeroPlaca { get; set; }
        [MaxLength(10)]
        public string? NumeroPermisoMunicipal { get; set; }
        [MaxLength(10)]
        public string? Celular { get; set; }
        [MaxLength(20)]
        public string? FotoVehiculo { get; set; }
        public List<Ruta>? Ruta { get; set; }
    }
}

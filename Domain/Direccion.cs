using Domain.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
	public class Direccion : BaseDomainModel
	{
        public int TipoDireccionId { get; set; }
        public TipoDireccion? TipoDireccion { get; set; }

		public int PersonaId { get; set; }
		public Persona? Persona { get; set; }

        [MaxLength(70)]
        public string? CallePrincipal { get; set; }
        [MaxLength(10)]
        public string? Numero { get; set; } = "S/N";
        [MaxLength(70)]
        public string? CalleSecundaria { get; set; }
        [MaxLength(10)]
        public string? CodigoPostal { get; set; }
        [MaxLength(20)]
        public string? Latitud { get; set; }
        [MaxLength(20)]
        public string? Longitud { get; set; }
        [MaxLength(15)]
        public string? Telefono { get; set; }
        [MaxLength(70)]
        public string? Contacto { get; set; }
        public short Provincia { get; set; }
        public int Canton { get; set; }
        public int Parroquia { get; set; }
        [MaxLength(70)]
        public string? Referencia { get; set; }
        public string? linkGoogleMaps { get; set; }


        public List<AlumnoTransporte>? AlumnoTransporte { get; set; }
    }
}

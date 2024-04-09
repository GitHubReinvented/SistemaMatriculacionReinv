using Domain.Common;
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
	public class Persona : BaseDomainModel
	{
        public int TipoIdentificacionId { get; set; }
        public TipoIdentificacion TipoIdentificacion { get; set; }
        [Column(TypeName = "VARCHAR(20)")]
        
        public string Identificacion { get; set; }
        [Column(TypeName = "VARCHAR(70)")]
        
        public string Nombres { get; set; }
        [Column(TypeName = "VARCHAR(70)")]
       
        public string Apellidos { get; set; }
        [Column(TypeName = "VARCHAR(70)")]
        
        public string? EmailPrincipal { get; set; }
        [Column(TypeName = "VARCHAR(70)")]
        
        public string? EmailSecundario { get; set; }
        [Column(TypeName = "VARCHAR(15)")]
        
        public string? TelefonoMovil { get; set; }
        [Column(TypeName = "VARCHAR(25)")]
        
        public string Avatar { get; set; } = "avatar-default.png";
        [Column(TypeName = "VARCHAR(70)")]
        
        public string? Profesion { get; set; }
		//public Profesion? Profesion { get; set; }

		public int? TipoEstudianteId { get; set; }
		public TipoEstudiante? TipoEstudiante { get; set; }
        [Column(TypeName = "VARCHAR(70)")]
        
        public string? Cargo { get; set; }
        [Column(TypeName = "VARCHAR(70)")]
        
        public string? LugarTrabajo { get; set; }
        public bool? AutorizadoRetirar { get; set; } = false;
        public DateTime FechaNacimiento { get; set; }
		public bool Representante { get; set; } 
        [Column(TypeName = "VARCHAR(300)")]
        
        public string? UsuarioId { get; set; }
        public int? SucursalId { get; set; }
        public Sucursal? Sucursal { get; set; }
        public bool? ContactoEmergencia { get; set; }
        public bool? AutorizadoRecibir { get; set; }

        public Usuario? Usuario { get; set; }

		public List<AutorizacionRetiro>? AutorizacionRetiro { get; set; }

		public List<Relacion>? RelacionPersona { get; set; }
		public List<Relacion>? RelacionPersona2 { get; set; }

        public List<Matricula>? Matricula { get; set; }
		public List<Direccion>? Direccion { get; set; }
        //public FichaMedica? FichaMedica { get; set; }
       
    }
}

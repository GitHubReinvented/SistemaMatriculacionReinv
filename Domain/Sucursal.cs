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
    public class Sucursal : BaseDomainModel
    {
        [Column(TypeName = "VARCHAR")]
        [StringLength(70)]
        public string? Nombre { get; set; }
        [Column(TypeName = "VARCHAR")]
        [StringLength(70)]
        public string? Direccion { get; set; }
        [Column(TypeName = "VARCHAR")]
        [StringLength(15)]
        public string? Telefono { get; set; }
        [Column(TypeName = "VARCHAR")]
        [StringLength(70)]
        public string? Email { get; set; }
        [StringLength(70)]
        public string? RazonSocial { get; set; }
        [StringLength(15)]
        public string? RUC { get; set; }
        [StringLength(70)]
        public string? EmailFacturas { get; set; }
        public string? color1 { get; set; }
        public string? color2 { get; set; }
        public string? color3 { get; set; }
        public string? color4 { get; set; }
        public List<UsuarioSucursal>? UsuarioSucursal { get; set; }
		public List<BotonPagoSucursal>? BotonPagoSucursal { get; set; }
        public List<CicloEscolar>? CicloEscolar { get; set; } 
        public List<CuentaSucursal>? CuentaSucursal { get; set; }
        public List<Servicio>? Servicios { get; set; }
        public List<Persona>? Personas { get; set; }


        public List<Documento> Documento { get; set; }
    }
}

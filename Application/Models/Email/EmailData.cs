using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Models.Email
{
    public class EmailData
    {
        public LinkedResource? HeaderImage { get; set; }
        public string? HtmlContent { get; set; }
        public LinkedResource? FooterImage { get; set; }
        public LinkedResource? Attachment { get; set; }
    }

    public class LinkedResource
    {
        public string? ContentId { get; set; }
        public string? ContentPath { get; set; }
        public string? ContentType { get; set; }
        public byte[]? ContentBytes { get; set; }
    }

    public class EmailConfirmed
    {
        public string? linkConfirmacion { get; set; }
        public string? name { get; set; }
        public string? userName { get; set; }
        public string? email { get; set; }
        public string? password { get; set; }
        public int? sucursal { get; set; }

    }

    public class EmailChangePassword
    {
        public string? passwordChangeLink { get; set; }
        public string? userName { get; set; }
        public string? email { get; set; }
    }

    public class JustificacionSeguro
    {
        public string? Nombre { get; set; }
        public string? Cedula { get; set; }
        public string? Telefono { get; set; }
        public string? Justificacion { get; set; }
    }
}

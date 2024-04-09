namespace ReinventedPuembo.Models
{
    public class RepresentantesViewModel
    {
        public IFormFile? AvatarPadre { get; set; }
        public string NombresPadre { get; set; }
        public string ApellidosPadre { get; set; }
        public string? EmailPrincipalPadre { get; set; }
        public int TipoIdentificacionIdPadre { get; set; }
        public string? IdentificacionPadre { get; set; }
        public IFormFile? PDFIdentificacionPadre { get; set; }

        public string? ProfesionPadre { get; set; }
        public string? LugarTrabajoPadre { get; set; }
        public string? CelularPadre { get; set; }
        public string? CorreoPadre { get; set; }
        public string? CargoPadre { get; set; }
        public bool? RepresentantePadre { get; set; }

        public IFormFile? AvatarMadre { get; set; }
        public string NombresMadre { get; set; }
        public string ApellidosMadre { get; set; }
        public string? EmailPrincipalMadre { get; set; }
        public int TipoIdentificacionIdMadre { get; set; }
        public IFormFile? PDFIdentificacionMadre { get; set; }

        public string? IdentificacionMadre { get; set; }
        public string? ProfesionMadre { get; set; }
        public string? LugarTrabajoMadre { get; set; }
        public string? CelularMadre { get; set; }
        public string? CorreoMadre { get; set; }
        public string? CargoMadre { get; set; }
        public bool? RepresentanteMadre { get; set; }

        public string? CedulaHijo { get; set; }
        public int? SucursalId { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;

namespace ReinventedPuembo.Models
{
    public class TransporteViewModel
    {
        public int idEstudiante { get; set; }
        public string? Referencia1 { get; set; }
        public string? Latitud { get; set; }
        public string? Longitud { get; set; }
        public string? linkGoogleMaps1 { get; set; }
        //Nuevo domicilio
        public string? CallePrincipal2 { get; set; }
        public string? Numero2 { get; set; }
        public string? CalleSecundaria2 { get; set; }
        public string? Referencia2 { get; set; }
        public string? Latitud2 { get; set; }
        public string? Longitud2 { get; set; }
        public string? linkGoogleMaps2 { get; set; }



        //Información Rastreo persona 1
        public string? NombresRastreo1 { get; set; }
        public string? ApellidosRastreo1 { get; set; }
        public string? EmailPrincipal1 { get; set; }
        public string? Identificacion1 { get; set; }
        public string? TelefonoMovil1 { get; set; }
        public int TipoRelacionId1 { get; set; }


        //Información Rastreo persona 2
        public string? NombresRastreo2 { get; set; }
        public string? ApellidosRastreo2 { get; set; }
        public string? EmailPrincipal2 { get; set; }
        public string? Identificacion2 { get; set; }
        public string? TelefonoMovil2 { get; set; }
        public int TipoRelacionId2 { get; set; }


        //Información Rastreo persona 3



        public string? NombresRastreo3 { get; set; }
        public string? ApellidosRastreo3 { get; set; }
        public string? Identificacion3 { get; set; }
        public string? EmailPrincipal3 { get; set; }
        public string? TelefonoMovil3 { get; set; }
        public int TipoRelacionId3 { get; set; }



        //Personas autorizadas a recibir al estudiante



        public string? NombresAutorizado1 { get; set; }
        public string? ApellidosAutorizado1 { get; set; }
        public string? IdentificacionAutorizado1 { get; set; }
        public string? EmailAutorizado1 { get; set; }
        public string? TelefonoMovilAutorizado1 { get; set; }
        public IFormFile? AvatarAutorizado1 { get; set; }
        public int TipoRelacionAutorizado1 { get; set; }


        //Persona autorizadas a recibir al estudiante 2



        public string? NombresAutorizado2 { get; set; }
        public string? ApellidosAutorizado2 { get; set; }
        public string? IdentificacionAutorizado2 { get; set; }
        public string? EmailAutorizado2 { get; set; }
        public string? TelefonoMovilAutorizado2 { get; set; }
        public IFormFile? AvatarAutorizado2 { get; set; }
        public int TipoRelacionAutorizado2 { get; set; }


        //Persona autorizadas a recibir al estudiante 3



        public string? NombresAutorizado3 { get; set; }
        public string? ApellidosAutorizado3 { get; set; }
        public string? IdentificacionAutorizado3 { get; set; }
        public string? EmailAutorizado3 { get; set; }
        public string? TelefonoMovilAutorizado3 { get; set; }
        public IFormFile? AvatarAutorizado3 { get; set; }
        public int TipoRelacionAutorizado3 { get; set; }



        //Persona autorizadas a recibir al estudiante 4
        public string? NombresAutorizado4 { get; set; }
        public string? ApellidosAutorizado4 { get; set; }
        public string? IdentificacionAutorizado4 { get; set; }
        public string? EmailAutorizado4 { get; set; }
        public string? TelefonoMovilAutorizado4 { get; set; }
        public IFormFile? AvatarAutorizado4 { get; set; }
        public int TipoRelacionAutorizado4 { get; set; }

    }
}

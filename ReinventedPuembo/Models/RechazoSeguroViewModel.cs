namespace ReinventedPuembo.Models
{
    public class RechazoSeguroViewModel
    {

        public int Id { get; set; }
        public DateTime FechaRegistro { get; set; }
        
        public string NombreRepresentante { get; set; }
        
        public string TelefonoRepresentante { get; set; }
        
        public string IdentificacionRepresentante { get; set; }
        
        public string Justificacion { get; set; }
        public bool Estado { get; set; }//por aprobar false//true aprobado
        public string  EstadoDescripcion { get; set; }//por aprobar false//trrue aprobado

    }
}

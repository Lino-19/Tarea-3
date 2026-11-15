namespace GestorExcursiones.API.Models.Dtos
{
    public class ClientCreateDto
    {
        public string Nombre { get; set; } = string.Empty;
        public string Apellido { get; set; } = string.Empty;
        public string DocumentoIdentificacion { get; set; } = string.Empty;
        public string Telefono { get; set; } = string.Empty;
        public string CorreoElectronico { get; set; } = string.Empty;
    }
}
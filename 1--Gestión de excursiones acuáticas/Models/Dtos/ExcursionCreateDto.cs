namespace GestorExcursiones.API.Models.Dtos
{
    public class ExcursionCreateDto
    {
        public string Nombre { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public DateTime Fecha { get; set; }
        public decimal Precio { get; set; }
        public int MaxParticipantes { get; set; }
    }
}
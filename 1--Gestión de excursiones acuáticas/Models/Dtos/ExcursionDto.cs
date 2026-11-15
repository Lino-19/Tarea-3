namespace GestorExcursiones.API.Models.Dtos
{
    public class ExcursionDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public DateTime Fecha { get; set; }
        public decimal Precio { get; set; }
        public int MaxParticipantes { get; set; }
    }
}
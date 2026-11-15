namespace GestorExcursiones.Application.Dtos;

public class UpdateReservaDto
{
    public int Id { get; set; }
    public int CantidadParticipantes { get; set; }
    public string Estado { get; set; } = "Pendiente";
    public decimal MontoPagado { get; set; }
}

namespace GestorExcursiones.Application.Dtos;

public class CreateReservaDto
{
    public int ClienteId { get; set; }
    public int ExcursionId { get; set; }
    public int CantidadParticipantes { get; set; }
    public decimal MontoPagado { get; set; }
}

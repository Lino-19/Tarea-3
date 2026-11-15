namespace GestorExcursiones.Application.Dtos;

public class ReservaDto : DtoBase
{
    public int ClienteId { get; set; }
    public string ClienteNombre { get; set; } = string.Empty;
    public int ExcursionId { get; set; }
    public string ExcursionNombre { get; set; } = string.Empty;
    public DateTime FechaReserva { get; set; }
    public int CantidadParticipantes { get; set; }
    public string Estado { get; set; } = string.Empty;
    public decimal MontoTotal { get; set; }
    public decimal MontoPagado { get; set; }
    public decimal SaldoPendiente { get; set; }
    public string EstadoPago { get; set; } = string.Empty;
}

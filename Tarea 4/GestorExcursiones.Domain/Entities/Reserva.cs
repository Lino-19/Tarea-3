using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using GestorExcursiones.Domain.Core;

namespace GestorExcursiones.Domain.Entities
{
    public class Reserva : BaseEntity
    {
        [Required] public int ClienteId { get; set; }
        public Cliente? Cliente { get; set; }
        [Required] public int ExcursionId { get; set; }
        public Excursion? Excursion { get; set; }
        public DateTime FechaReserva { get; set; } = DateTime.Now;
        [Range(1, int.MaxValue)] public int CantidadParticipantes { get; set; }
        public string Estado { get; set; } = "Pendiente";
        public decimal MontoTotal { get; set; }
        public decimal MontoPagado { get; set; }
        [NotMapped] public decimal SaldoPendiente => MontoTotal - MontoPagado;
        [NotMapped] public string EstadoPago => MontoPagado <= 0 ? "Pendiente" :
            MontoPagado < MontoTotal ? "Parcialmente Pagada" : "Pagada";
    }
}

using System.ComponentModel.DataAnnotations;
using GestorExcursiones.Domain.Core;

namespace GestorExcursiones.Domain.Entities
{
    public class Excursion : BaseEntity
    {
        [Required] public string Nombre { get; set; } = string.Empty;
        [Required] public string Descripcion { get; set; } = string.Empty;
        public DateTime Fecha { get; set; }
        public decimal Precio { get; set; }
        public int MaxParticipantes { get; set; }
        public ICollection<Reserva> Reservas { get; set; } = new List<Reserva>();
    }
}

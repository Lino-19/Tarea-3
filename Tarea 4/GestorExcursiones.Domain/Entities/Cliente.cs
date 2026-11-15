using System.ComponentModel.DataAnnotations;
using GestorExcursiones.Domain.Core;

namespace GestorExcursiones.Domain.Entities
{
    public class Cliente : BaseEntity
    {
        [Required] public string Nombre { get; set; } = string.Empty;
        [Required] public string Apellido { get; set; } = string.Empty;
        [Required] public string DocumentoIdentificacion { get; set; } = string.Empty;
        public string Telefono { get; set; } = string.Empty;
        [EmailAddress] public string CorreoElectronico { get; set; } = string.Empty;
    }
}

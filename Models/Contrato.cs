using System.ComponentModel.DataAnnotations;

namespace INMOBILIARIA_JosiasTolaba.Models
{
    public class Contrato : IValidatableObject
    {
        [Key]
        public int IdContrato { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        public int MontoMensual { get; set; }
        public InmuebleDto? Propiedad { get; set; }
        public InquilinoDto? Habitante { get; set; }
        public int? QuienCreo { get; set; }
        public int? QuienElimino { get; set; }
        public bool Estado { get; set; }
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (FechaInicio > FechaFin)
            {
                yield return new ValidationResult(
                    "La fecha de inicio no puede ser mayor que la fecha de fin",
                    new[] { nameof(FechaInicio), nameof(FechaFin) }
                );
            }
        }
    }
}

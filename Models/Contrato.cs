using System.ComponentModel.DataAnnotations;

namespace INMOBILIARIA_JosiasTolaba.Models
{
    public class Contrato
    {
        [Key]
        public int IdContrato { get; set; }

        public DateTime FechaInicio { get; set; }

        public DateTime FechaFin { get; set; }

        public int MontoMensual { get; set; }

        public InmuebleDto? Propiedad { get; set; }
        public InquilinoDto? Habitante { get; set; }
        
        public bool Estado { get; set; }
    }
}

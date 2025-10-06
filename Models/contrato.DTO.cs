using System.ComponentModel.DataAnnotations;

namespace INMOBILIARIA_JosiasTolaba.Models
{
    public class ContratoDTO
    {
        [Key]
        public int IdContrato { get; set; }
        public InquilinoDto? Habitante { get; set; }
    }
}
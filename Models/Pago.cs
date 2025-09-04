using System.ComponentModel.DataAnnotations;

namespace INMOBILIARIA_JosiasTolaba.Models
{
    public class Pago
    {
        [Key]
        public int IdPago { get; set; }
        
        [Required(ErrorMessage = "El FechaPago es obligatorio")]
        public required DateTime FechaPago { get; set; }
        
        [Required(ErrorMessage = "El Monto es obligatorio")]
        public required decimal Monto { get; set; }

        [Required(ErrorMessage = "El MesCorrespondiente es obligatorio")]
        public required char MesCorrespondiente { get; set; }
        
        [Required(ErrorMessage = "El NumeroPago es obligatorio")]
        public required string NumeroPago { get; set; }

        [Required(ErrorMessage = "El Concepto es obligatorio")]
        public required string Concepto { get; set; }

        [Required(ErrorMessage = "El IdContrato es obligatorio")]
        public required int IdContrato { get; set; }

        [Required(ErrorMessage = "El Estado es obligatorio")]
        public required bool Estado { get; set; }

        public Pago() {}
    }
}
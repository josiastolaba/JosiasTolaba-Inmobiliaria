using System.ComponentModel.DataAnnotations;

namespace INMOBILIARIA_JosiasTolaba.Models
{
    public class Usuario
    {
        [Key]
        public int IdUsuario { get; set; } 
        
        [Required(ErrorMessage = "El nombre es obligatorio")]
        [StringLength(100, ErrorMessage = "El nombre no puede exceder los 100 caracteres")]
        public required string Nombre { get; set; }
        
        [Required(ErrorMessage = "El apellido es obligatorio")]
        [StringLength(100, ErrorMessage = "El apellido no puede exceder los 100 caracteres")]
        public required string Apellido { get; set; }

        [Required(ErrorMessage = "El Dni es obligatorio")]
        [StringLength(8, ErrorMessage = "El Dni no puede exceder los 8 caracteres")]
        public required string Dni { get; set; }
        
        public enum TipoRol
        {
            Administrador,
            Empleado
        }

        [Required(ErrorMessage = "El Rol es obligatorio")]
        public TipoRol Rol { get; set; }

        [Required(ErrorMessage = "El email es obligatorio")]
        [EmailAddress(ErrorMessage = "Formato de email inválido")]
        public required string Email { get; set; }
        
        public required string Contrasena { get; set; }

        public required bool Estado { get; set; }

        public Usuario() {}

    }
}

using System.ComponentModel.DataAnnotations;

namespace INMOBILIARIA_JosiasTolaba.Models
{
    public class Inmueble
    {
        [Key]
        public int IdInmueble { get; set; }
        [Required(ErrorMessage = "La Direccion es obligatoria")]
        [StringLength(200, ErrorMessage = "La Direccion no puede exceder los 200 caracteres")]
        public string Direccion { get; set; }
        [Required(ErrorMessage = "El Tipo es obligatorio")]
        public int IdTipo { get; set; }
        public int IdPropietario { get; set; }
        public enum TipoUso
        {
            Comercial,
            Residencial
        }
        [Required(ErrorMessage = "El Uso es obligatorio")]
        public TipoUso Uso { get; set; } 
        [Required(ErrorMessage = "La Latitud es obligatorio")]
        public decimal Latitud { get; set; }
        [Required(ErrorMessage = "La Longitud es obligatorio")]
        public decimal Longitud { get; set; }
        [Required(ErrorMessage = "El Precio es obligatorio")]
        public decimal Precio { get; set; }
        [Required(ErrorMessage = "El Ambiente es obligatorio")]
        public int Ambiente { get; set; }
        public string? Portada { get; set; }
        public IFormFile? PortadaFile { get; set; }
        public IList<Imagen>? Fotos { get; set; } = new List<Imagen>();
        public bool Estado { get; set; }
        
        public Inmueble() { }
    }
}
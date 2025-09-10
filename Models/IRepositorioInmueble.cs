
namespace INMOBILIARIA_JosiasTolaba.Models
{
    public interface IRepositorioInmueble : IRepositorio<Inmueble>
    {
        int DarDeBaja(int IdInmueble);
        IList<Inmueble> ListarInmuebles();
        Inmueble InmuebleId(int IdInmueble);
    }
}

namespace INMOBILIARIA_JosiasTolaba.Models
{
    public interface IRepositorioInmueble : IRepositorio<Inmueble>
    {
        List<Inmueble> buscar(string dato);
        int DarDeBaja(int IdInmueble);
        IList<Inmueble> ListarInmuebles();
        Inmueble InmuebleId(int IdInmueble);
    }
}
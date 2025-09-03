
namespace INMOBILIARIA_JosiasTolaba.Models
{
    public interface IRepositorioInmueble : IRepositorio<Inmueble>
    {
        IList<Inmueble> ListarInmuebles();
        int DarDeBaja(int IdInmueble);
        Inmueble InmuebleId(int IdInmueble);
    }
    public interface IRepositorio : IRepositorio<Inmueble>
    {
        IList<Propietario> ListarPropietarios();
	}
}
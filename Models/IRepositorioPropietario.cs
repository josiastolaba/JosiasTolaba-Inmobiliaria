
namespace INMOBILIARIA_JosiasTolaba.Models
{
	public interface IRepositorioPropietario : IRepositorio<Propietario>
	{
        int DarDeBaja(int idPropietario);
        IList<Propietario> ListarPropietarios();
		Propietario PropietarioId(int IdPropietario);
		
	}
}
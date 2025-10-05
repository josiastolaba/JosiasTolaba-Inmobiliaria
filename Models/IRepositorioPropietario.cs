
namespace INMOBILIARIA_JosiasTolaba.Models
{
	public interface IRepositorioPropietario : IRepositorio<Propietario>
	{
		int DarDeBaja(int idPropietario);
		IList<Propietario> ListarPropietarios();
		Propietario PropietarioId(int IdPropietario);
		List<Propietario> buscar(string dato);

		IList<Propietario> obtenerPaginados(int offset, int limit);

        int contar();

		bool existeDni(string dni);
		bool existeOtroDni(string dni, int idPropietario);
	}
}
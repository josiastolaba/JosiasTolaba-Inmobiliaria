
namespace INMOBILIARIA_JosiasTolaba.Models
{
	public interface IRepositorioInquilino : IRepositorio<Inquilino>
	{
		int DarDeBaja(int IdInquilino);
		IList<Inquilino> ListarInquilinos();
		Inquilino InquilinoId(int IdInquilino);

		List<Inquilino> buscar(String datos);

		IList<Inquilino> obtenerPaginados(int offset, int limit);

        int contar();

		bool existeDni(string dni);
		bool existeOtroDni(string dni, int idPropietario);
	}
}
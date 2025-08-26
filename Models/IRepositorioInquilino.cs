
namespace INMOBILIARIA_JosiasTolaba.Models
{
	public interface IRepositorioInquilino : IRepositorio<Inquilino>
	{
		int DarDeBaja(int IdInquilino);
		IList<Inquilino> ListarInquilinos();
		Inquilino InquilinoId(int IdInquilino);
	}
}
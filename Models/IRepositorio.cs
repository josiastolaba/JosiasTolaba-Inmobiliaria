
namespace INMOBILIARIA_JosiasTolaba.Models
{
	public interface IRepositorio<T>
	{
		int Alta(T p);
		int Baja(int id);
		int Modificacion(T p);
	}
}

namespace INMOBILIARIA_JosiasTolaba.Models
{
        public interface IRepositorioUsuario : IRepositorio<Usuario>
        {
                int DarDeBaja(int idUsuario);
                IList<Usuario> ListarUsuarios();

                List<Usuario> buscar(string dato);
                Usuario UsuarioId(int IdUsuario);

                IList<Usuario> obtenerPaginados(int offset, int limit);

                int contar();
		
	}
}
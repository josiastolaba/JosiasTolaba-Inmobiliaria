
namespace INMOBILIARIA_JosiasTolaba.Models
{
        public interface IRepositorioUsuario : IRepositorio<Usuario>
        {
                int DarDeBaja(int idUsuario);
                IList<Usuario> ListarUsuarios();
                Usuario UsuarioId(int IdUsuario);
                Usuario ObtenerPorEmail(string email);
	}
}
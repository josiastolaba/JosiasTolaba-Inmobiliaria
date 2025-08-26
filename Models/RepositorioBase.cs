
namespace INMOBILIARIA_JosiasTolaba.Models
{
	public abstract class RepositorioBase
	{
		protected readonly IConfiguration configuration;
		protected readonly string connectionString;

		protected RepositorioBase(IConfiguration configuration)
		{
			this.configuration = configuration;
			connectionString = configuration.GetConnectionString("MySql")?? throw new Exception("Cadena de conexión 'MySql' no encontrada");
			//connectionString = configuration["ConnectionStrings:MySql"];
		}
	}
}
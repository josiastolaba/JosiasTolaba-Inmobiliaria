using MySql.Data.MySqlClient;

namespace INMOBILIARIA_JosiasTolaba.Models
{
    public class RepositorioImagen : RepositorioBase, IRepositorioImagen
    {
        public RepositorioImagen(IConfiguration configuration) : base(configuration)
        {
        }
        public int Alta(Imagen i)
        {
            int res = -1;
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                string query = $@"INSERT INTO imagen (
                {nameof(Imagen.IdImagen)},
                {nameof(Imagen.IdInmueble)},
                {nameof(Imagen.Url)})
                VALUES (@IdImagen, @IdInmueble, @Url);
                SELECT LAST_INSERT_ID();";
                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@IdImagen", i.IdImagen);
                    command.Parameters.AddWithValue("@IdInmueble", i.IdInmueble);
                    command.Parameters.AddWithValue("@Url", i.Url);
                    connection.Open();
                    res = Convert.ToInt32(command.ExecuteScalar());
                    connection.Close();
                }
            }
            return res;
        }
        public int Baja(int id)
        {
            int res = -1;
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                string query = $@"DELETE FROM imagen WHERE {nameof(Imagen.IdImagen)}=@IdImagen";
                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@IdImagen", id);
                    connection.Open();
                    res = command.ExecuteNonQuery();
                    connection.Close();
                }
            }
            return res;
        }
        public int Modificacion(Imagen i)
        {
            int res = -1;
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                string query = $@"UPDATE inmueble SET
                {nameof(Imagen.IdImagen)}=@Direccion,
                {nameof(Imagen.IdInmueble)}=@Tipo,
                {nameof(Imagen.Url)}=@IdPropietario,
                WHERE {nameof(Imagen.IdImagen)}=@IdImagen";
                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@IdImagen", i.IdImagen);
                    command.Parameters.AddWithValue("@IdInmueble", i.IdInmueble);
                    command.Parameters.AddWithValue("@Url", i.Url);
                    connection.Open();
                    res = command.ExecuteNonQuery();
                    connection.Close();
                }
            }
            return res;
        }
        public Imagen? ObtenerPorId(int IdImagen)
        {
            Imagen? res = null;
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                string query = @$"
					SELECT 
						{nameof(Imagen.IdImagen)}, 
						{nameof(Imagen.IdInmueble)}, 
						{nameof(Imagen.Url)} 
					FROM imagenes
					WHERE {nameof(Imagen.IdImagen)}=@IdImagen";
                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@IdImagen", IdImagen);
                    connection.Open();
                    var reader = command.ExecuteReader();
                    if (reader.Read())
                    {
                        res = new Imagen();
                        res.IdImagen = reader.GetInt32(nameof(Imagen.IdImagen));
                        res.IdInmueble = reader.GetInt32(nameof(Imagen.IdInmueble));
                        res.Url = reader.GetString(nameof(Imagen.Url));
                    }
                    connection.Close();
                }
            }
            return res;
        }
        public IList<Imagen> BuscarPorInmueble(int IdInmueble)
		{
			List<Imagen> res = new List<Imagen>();
            using (MySqlConnection connection = new MySqlConnection(connectionString))
			{
				string query = @$"
					SELECT 
						{nameof(Imagen.IdImagen)}, 
						{nameof(Imagen.IdInmueble)}, 
						{nameof(Imagen.Url)} 
					FROM Imagenes
					WHERE {nameof(Imagen.IdInmueble)}=@IdInmueble";
                using (MySqlCommand command = new MySqlCommand(query, connection))
				{
					command.Parameters.AddWithValue("@IdInmueble", IdInmueble);
					connection.Open();
					var reader = command.ExecuteReader();
					while (reader.Read())
					{
						res.Add(new Imagen
						{
							IdImagen = reader.GetInt32(nameof(Imagen.IdImagen)),
							IdInmueble = reader.GetInt32(nameof(Imagen.IdInmueble)),
							Url = reader.GetString(nameof(Imagen.Url)),
						});
					}
					connection.Close();
				}
			}
			return res;
		}
    }
}
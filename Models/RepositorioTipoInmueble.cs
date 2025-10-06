using MySql.Data.MySqlClient;

namespace INMOBILIARIA_JosiasTolaba.Models
{
    public class RepositorioTipoInmueble : RepositorioBase, IRepositorioTipoInmueble
    {
        public RepositorioTipoInmueble(IConfiguration configuration) : base(configuration)
        {
        }

        public int Alta(TipoInmueble entidad)
        {
            int res = -1;
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                string query = $"INSERT INTO tipo_inmueble (Nombre, Estado) " +
                    $"VALUES (@nombre, @estado);" +
                    "SELECT LAST_INSERT_ID();";
                using (var command = new MySqlCommand(query, connection))
                {
                    entidad.Estado = true;
                    command.Parameters.AddWithValue("@Nombre", entidad.Nombre);
                    command.Parameters.AddWithValue("@Estado", entidad.Estado);
                    connection.Open();
                    res = Convert.ToInt32(command.ExecuteScalar());
                    connection.Close();
                }
            }
            return res;
        }
        public int Baja(int idTipo)
        {
            int res = -1;
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                string query = $@"DELETE FROM tipo_inmueble 
                WHERE {nameof(TipoInmueble.IdTipo)} = @IdTipo";
                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@IdTipo", idTipo);
                    connection.Open();
                    res = command.ExecuteNonQuery();
                }
            }
            return res;
        }
        public int Modificacion(TipoInmueble entidad)
        {
            int res = -1;
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                string query = $@"UPDATE tipo_inmueble SET
                    {nameof(TipoInmueble.Nombre)}=@Nombre,
                    WHERE {nameof(TipoInmueble.IdTipo)}=@IdTipo";

                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Nombre", entidad.Nombre);
                    connection.Open();
                    res = command.ExecuteNonQuery();
                }
            }
            return res;
        }
        public IList<TipoInmueble> ListarTipoInmueble()
        {
            IList<TipoInmueble> res = new List<TipoInmueble>();
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                string query = $@"SELECT * FROM tipo_inmueble WHERE Estado = true;";
                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    connection.Open();
                    var reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        TipoInmueble tp = new TipoInmueble
                        {
                            IdTipo = reader.GetInt32(nameof(TipoInmueble.IdTipo)),
                            Nombre = reader.GetString(nameof(TipoInmueble.Nombre)),
                            Estado = reader.GetBoolean(nameof(TipoInmueble.Estado))
                        };
                        res.Add(tp);
                    }
                    connection.Close();
                }
            }
            return res;
        }
        public TipoInmueble TipoInmuebleId(int IdTipo)
        {
            TipoInmueble res = null;
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                string query = $@"SELECT * FROM tipo_inmueble WHERE IdTipo = @IdTipo;";

                using (MySqlCommand command = new MySqlCommand(query, connection))

                {
                    command.Parameters.AddWithValue("@IdTipo", IdTipo);
                    connection.Open();
                    var reader = command.ExecuteReader();
                    if (reader.Read())
                    {
                        res = new TipoInmueble
                        {
                            IdTipo = reader.GetInt32(nameof(TipoInmueble.IdTipo)),
                            Nombre = reader.GetString(nameof(TipoInmueble.Nombre)),
                            Estado = reader.GetBoolean(nameof(TipoInmueble.Estado))
                        };
                    }
                    connection.Close();
                }
            }
            return res;
        }
        public int DarDeBaja(int IdTipo)
        {
            int res = -1;
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                string query = $@"UPDATE tipo_inmueble 
                    SET Estado = 0 
                    WHERE {nameof(IdTipo)}=@IdTipo";

                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@IdTipo", IdTipo);
                    connection.Open();
                    res = command.ExecuteNonQuery();
                }
            }
            return res;
        }
        public List<TipoInmueble> buscar(string dato)
        {
            var lista = new List<TipoInmueble>();

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                string query = @"SELECT IdTipo, Nombre, Estado
                                FROM tipo_inmueble 
                                WHERE Nombre LIKE @dato
                                LIMIT 10";

                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@dato", "%" + dato + "%");
                    connection.Open();

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var p = new TipoInmueble
                            {
                                IdTipo = reader.GetInt32("IdTipo"),
                                Nombre = reader.GetString("Nombre"),
                                Estado = reader.GetBoolean("Estado")
                            };
                            lista.Add(p);
                        }
                    }
                }
            }
            return lista;
        }
    }
}
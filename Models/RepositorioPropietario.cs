using MySql.Data.MySqlClient;

namespace INMOBILIARIA_JosiasTolaba.Models
{
    public class RepositorioPropietario : RepositorioBase, IRepositorioPropietario
    {
        public RepositorioPropietario(IConfiguration configuration) : base(configuration)
        {
        }

        /*METODOS PARA BUSCAR*/

        public List<Propietario> buscar(string dato)
        {
            var lista = new List<Propietario>();

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                string query = @"SELECT IdPropietario, Nombre, Apellido, Dni, Telefono, Email, Estado
                         FROM propietario 
                         WHERE Nombre LIKE @dato OR Dni LIKE @dato
                         LIMIT 10";

                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@dato", "%" + dato + "%");
                    connection.Open();

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var p = new Propietario
                            {
                                IdPropietario = reader.GetInt32("IdPropietario"),
                                Nombre = reader.GetString("Nombre"),
                                Apellido = reader.GetString("Apellido"),
                                Dni = reader.GetString("Dni"),
                                Telefono = reader.GetString("Telefono"),
                                Email = reader.GetString("Email"),
                                Estado = reader.GetBoolean("Estado")
                            };
                            lista.Add(p);
                        }
                    }
                }
            }
            return lista;
        }

    public IList<Propietario> obtenerPaginados(int offset, int limit)
        {
            IList<Propietario> res = new List<Propietario>();
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                string query = @"SELECT * FROM propietario
                WHERE Estado = true
                ORDER BY IdPropietario
                LIMIT @limit OFFSET @offset;";

                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@limit", limit);
                    command.Parameters.AddWithValue("@offset", offset);
                    connection.Open();
                    var reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        Propietario p = new Propietario
                        {
                            IdPropietario = reader.GetInt32(nameof(Propietario.IdPropietario)),
                            Nombre = reader.GetString(nameof(Propietario.Nombre)),
                            Apellido = reader.GetString(nameof(Propietario.Apellido)),
                            Dni = reader.GetString(nameof(Propietario.Dni)),
                            Email = reader.GetString(nameof(Propietario.Email)),
                            Telefono = reader.GetString(nameof(Propietario.Telefono)),
                            Estado = reader.GetBoolean(nameof(Propietario.Estado))
                        };
                        res.Add(p);
                    }
                }
            }
            return res;
        }

        public int contar()
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                string query = "SELECT COUNT(*) FROM propietario WHERE Estado = true;";

                using (var command = new MySqlCommand(query, connection))
                {
                    connection.Open();
                    return Convert.ToInt32(command.ExecuteScalar());
                }
            }
        }



        public int Alta(Propietario p)
        {
            int res = -1;
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                string query = $@"INSERT INTO propietario (
                    {nameof(Propietario.Nombre)},
                    {nameof(Propietario.Apellido)},
                    {nameof(Propietario.Dni)},
                    {nameof(Propietario.Telefono)},
                    {nameof(Propietario.Email)},
                    {nameof(Propietario.Estado)})
                    VALUES (@Nombre, @Apellido, @Dni, @Telefono, @Email, @Estado);
                    SELECT LAST_INSERT_ID();";

                using (var command = new MySqlCommand(query, connection))
                {
                    p.Estado = true;
                    command.Parameters.AddWithValue("@Nombre", p.Nombre);
                    command.Parameters.AddWithValue("@Apellido", p.Apellido);
                    command.Parameters.AddWithValue("@Dni", p.Dni);
                    command.Parameters.AddWithValue("@Telefono", p.Telefono);
                    command.Parameters.AddWithValue("@Email", p.Email);
                    command.Parameters.AddWithValue("@Estado", p.Estado);

                    connection.Open();
                    res = Convert.ToInt32(command.ExecuteScalar()); // devuelve el último ID insertado
                }
            }
            return res;
        }

        public int Baja(int id)
        {
            int res = -1;
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                string query = $@"DELETE FROM propietario 
                WHERE {nameof(Propietario.IdPropietario)} = @IdPropietario";
                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@IdPropietario", id);
                    connection.Open();
                    res = command.ExecuteNonQuery();
                }
            }
            return res;
        }

        public int Modificacion(Propietario p)
        {
            int res = -1;
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                string query = $@"UPDATE propietario SET
                    {nameof(Propietario.Nombre)}=@Nombre,
                    {nameof(Propietario.Apellido)}=@Apellido,
                    {nameof(Propietario.Dni)}=@Dni,
                    {nameof(Propietario.Telefono)}=@Telefono,
                    {nameof(Propietario.Email)}=@Email
                    WHERE {nameof(Propietario.IdPropietario)}=@IdPropietario";

                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@IdPropietario", p.IdPropietario);
                    command.Parameters.AddWithValue("@Nombre", p.Nombre);
                    command.Parameters.AddWithValue("@Apellido", p.Apellido);
                    command.Parameters.AddWithValue("@Dni", p.Dni);
                    command.Parameters.AddWithValue("@Telefono", p.Telefono);
                    command.Parameters.AddWithValue("@Email", p.Email);
                    connection.Open();
                    res = command.ExecuteNonQuery();
                }
            }
            return res;
        }

        public IList<Propietario> ListarPropietarios()
        {
            IList<Propietario> res = new List<Propietario>();
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                string query = $@"SELECT * FROM propietario WHERE Estado = true;";

                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    connection.Open();
                    var reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        Propietario p = new Propietario
                        {
                            IdPropietario = reader.GetInt32(nameof(Propietario.IdPropietario)),
                            Nombre = reader.GetString(nameof(Propietario.Nombre)),
                            Apellido = reader.GetString(nameof(Propietario.Apellido)),
                            Dni = reader.GetString(nameof(Propietario.Dni)),
                            Email = reader.GetString(nameof(Propietario.Email)),
                            Telefono = reader.GetString(nameof(Propietario.Telefono)),
                            Estado = reader.GetBoolean(nameof(Propietario.Estado))
                        };
                        res.Add(p);
                    }
                    connection.Close();
                }
            }
            return res;
        }
        public Propietario PropietarioId(int IdPropietario)
        {
            Propietario res = null;
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                string query = $@"SELECT * FROM propietario WHERE IdPropietario = @IdPropietario;";

                using (MySqlCommand command = new MySqlCommand(query, connection))

                {
                    command.Parameters.AddWithValue("@IdPropietario", IdPropietario);
                    connection.Open();
                    var reader = command.ExecuteReader();
                    if (reader.Read())
                    {
                        res = new Propietario
                        {
                            IdPropietario = reader.GetInt32(nameof(Propietario.IdPropietario)),
                            Nombre = reader.GetString(nameof(Propietario.Nombre)),
                            Apellido = reader.GetString(nameof(Propietario.Apellido)),
                            Dni = reader.GetString(nameof(Propietario.Dni)),
                            Email = reader.GetString(nameof(Propietario.Email)),
                            Telefono = reader.GetString(nameof(Propietario.Telefono)),
                            Estado = reader.GetBoolean(nameof(Propietario.Estado))
                        };
                    }
                    connection.Close();
                }
            }
            return res;
        }

        public bool existeDni(string dni)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                string query = "SELECT COUNT(*) FROM propietario WHERE Dni = @Dni";
                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@dni", dni);
                    connection.Open();
                    int count = Convert.ToInt32(command.ExecuteScalar());
                    return count > 0;
                }
            }
        }

        public bool existeOtroDni(string dni, int idPropietario)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                string query = @"SELECT COUNT(*) FROM propietario
                WHERE Dni = @dni AND IdPropietario <> @id";

                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@dni", dni);
                    command.Parameters.AddWithValue("@id", idPropietario);
                    connection.Open();
                    int count = Convert.ToInt32(command.ExecuteScalar());
                    return count > 0;
                }
            }
        }
        public int DarDeBaja(int IdPropietario)
        {

            int res = -1;
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                string query = $@"UPDATE propietario 
                    SET Estado = 0 
                    WHERE {nameof(IdPropietario)}=@IdPropietario";

                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@IdPropietario", IdPropietario);
                    connection.Open();
                    res = command.ExecuteNonQuery();
                }
            }
            return res;
        }

    }
}

using MySql.Data.MySqlClient;

namespace INMOBILIARIA_JosiasTolaba.Models
{
    public class RepositorioInquilino : RepositorioBase, IRepositorioInquilino
    {
        public RepositorioInquilino(IConfiguration configuration) : base(configuration)
        {
        }

        public List<Inquilino> buscar(string dato)
        {
            var lista = new List<Inquilino>();

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                string query = @"SELECT IdInquilino, Nombre, Apellido, Dni, Telefono, Email, Estado
                         FROM inquilino 
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
                            var p = new Inquilino
                            {
                                IdInquilino = reader.GetInt32("IdInquilino"),
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

public IList<Inquilino> obtenerPaginados(int offset, int limit)
        {
            IList<Inquilino> res = new List<Inquilino>();
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                string query = @"SELECT * FROM inquilino
                WHERE Estado = true
                ORDER BY IdInquilino
                LIMIT @limit OFFSET @offset;";

                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@limit", limit);
                    command.Parameters.AddWithValue("@offset", offset);
                    connection.Open();
                    var reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                       Inquilino p = new Inquilino
                        {
                            IdInquilino = reader.GetInt32(nameof(Inquilino.IdInquilino)),
                            Nombre = reader.GetString(nameof(Inquilino.Nombre)),
                            Apellido = reader.GetString(nameof(Inquilino.Apellido)),
                            Dni = reader.GetString(nameof(Inquilino.Dni)),
                            Email = reader.GetString(nameof(Inquilino.Email)),
                            Telefono = reader.GetString(nameof(Inquilino.Telefono)),
                            Estado = reader.GetBoolean(nameof(Inquilino.Estado))
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
                string query = "SELECT COUNT(*) FROM inquilino WHERE Estado = true;";

                using (var command = new MySqlCommand(query, connection))
                {
                    connection.Open();
                    return Convert.ToInt32(command.ExecuteScalar());
                }
            }
        }

          public bool existeDni(string dni)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                string query = "SELECT COUNT(*) FROM inquilino WHERE Dni = @Dni";
                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@dni", dni);
                    connection.Open();
                    int count = Convert.ToInt32(command.ExecuteScalar());
                    return count > 0;
                }
            }
        }

        public bool existeOtroDni(string dni, int idInquilino)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                string query = @"SELECT COUNT(*) FROM inquilino
                WHERE Dni = @dni AND IdInquilino <> @id";

                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@dni", dni);
                    command.Parameters.AddWithValue("@id", idInquilino);
                    connection.Open();
                    int count = Convert.ToInt32(command.ExecuteScalar());
                    return count > 0;
                }
            }
        }
        public int Alta(Inquilino i)
        {
            int res = -1;
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                string query = $@"INSERT INTO inquilino (
                {nameof(Inquilino.Nombre)},
                {nameof(Inquilino.Apellido)},
                {nameof(Propietario.Dni)},
                {nameof(Inquilino.Telefono)},
                {nameof(Inquilino.Email)},
                {nameof(Inquilino.Estado)})
                VALUES (@Nombre, @Apellido, @Dni, @Telefono, @Email, @Estado);
                SELECT LAST_INSERT_ID();";
                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    i.Estado = true;
                    command.Parameters.AddWithValue("@Nombre", i.Nombre);
                    command.Parameters.AddWithValue("@Apellido", i.Apellido);
                    command.Parameters.AddWithValue("@DNI", i.Dni);
                    command.Parameters.AddWithValue("@Telefono", i.Telefono);
                    command.Parameters.AddWithValue("@Email", i.Email);
                    command.Parameters.AddWithValue("@Estado", i.Estado);
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
                string query = $@"DELETE FROM inquilino WHERE {nameof(Inquilino.IdInquilino)}=@IdInquilino";
                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@IdInquilino", id);
                    connection.Open();
                    res = command.ExecuteNonQuery();
                    connection.Close();
                }
            }
            return res;
        }

        public int Modificacion(Inquilino i)
        {
            int res = -1;
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                string query = $@"UPDATE inquilino SET
                {nameof(Inquilino.Nombre)}=@Nombre,
                {nameof(Inquilino.Apellido)}=@Apellido,
                {nameof(Inquilino.Dni)}=@Dni,
                {nameof(Inquilino.Telefono)}=@Telefono,
                {nameof(Inquilino.Email)}=@Email
                WHERE {nameof(Inquilino.IdInquilino)}=@IdInquilino";

                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@IdInquilino", i.IdInquilino);
                    command.Parameters.AddWithValue("@Nombre", i.Nombre);
                    command.Parameters.AddWithValue("@Apellido", i.Apellido);
                    command.Parameters.AddWithValue("@Dni", i.Dni);
                    command.Parameters.AddWithValue("@Telefono", i.Telefono);
                    command.Parameters.AddWithValue("@Email", i.Email);
                    connection.Open();
                    res = command.ExecuteNonQuery();
                    connection.Close();
                }
            }
            return res;
        }
        public IList<Inquilino> ListarInquilinos()
        {
            IList<Inquilino> res = new List<Inquilino>();
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                string query = $@"SELECT * FROM inquilino WHERE Estado = true;";

                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    connection.Open();
                    var reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        res.Add(new Inquilino
                        {
                            IdInquilino = reader.GetInt32(nameof(Inquilino.IdInquilino)),
                            Nombre = reader.GetString(nameof(Inquilino.Nombre)),
                            Apellido = reader.GetString(nameof(Inquilino.Apellido)),
                            Dni = reader.GetString(nameof(Inquilino.Dni)),
                            Email = reader.GetString(nameof(Inquilino.Email)),
                            Telefono = reader.GetString(nameof(Inquilino.Telefono)),
                            Estado = reader.GetBoolean(nameof(Inquilino.Estado))
                        });
                    }
                }
            }
            return res;
        }
        public Inquilino InquilinoId(int IdInquilino)
        {
            Inquilino res = null;
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                string query = $@"SELECT * FROM inquilino WHERE IdInquilino = @IdInquilino;";

                using (MySqlCommand command = new MySqlCommand(query, connection))

                {
                    command.Parameters.AddWithValue("@IdInquilino", IdInquilino);
                    connection.Open();
                    var reader = command.ExecuteReader();
                    if (reader.Read())
                    {
                        res = new Inquilino
                        {
                            IdInquilino = reader.GetInt32(nameof(Inquilino.IdInquilino)),
                            Nombre = reader.GetString(nameof(Inquilino.Nombre)),
                            Apellido = reader.GetString(nameof(Inquilino.Apellido)),
                            Dni = reader.GetString(nameof(Inquilino.Dni)),
                            Email = reader.GetString(nameof(Inquilino.Email)),
                            Telefono = reader.GetString(nameof(Inquilino.Telefono)),
                            Estado = reader.GetBoolean(nameof(Inquilino.Estado))
                        };
                    }
                    connection.Close();
                }
            }
            return res;
        }
        public int DarDeBaja(int IdInquilino)
        {
            int res = -1;
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                string query = $@"UPDATE inquilino 
                    SET Estado = 0 
                    WHERE {nameof(IdInquilino)}=@IdInquilino";

                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@IdInquilino", IdInquilino);
                    connection.Open();
                    res = command.ExecuteNonQuery();
                }
            }
            return res;
        }
    }
}
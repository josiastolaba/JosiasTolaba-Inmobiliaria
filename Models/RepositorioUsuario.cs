using MySql.Data.MySqlClient;

namespace INMOBILIARIA_JosiasTolaba.Models
{
    public class RepositorioUsuario : RepositorioBase, IRepositorioUsuario
    {
        public RepositorioUsuario(IConfiguration configuration) : base(configuration)
        {

        }

        public List<Usuario> buscar(String dato)
        {
            var lista = new List<Usuario>();

    using (MySqlConnection connection = new MySqlConnection(connectionString))
    {
        string query = @"SELECT IdUsuario, Nombre, Apellido, Dni, Contrasena, Rol, Email, Estado
                         FROM usuario
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
                            var p = new Usuario
                            {
                                IdUsuario = reader.GetInt32("IdUsuario"),
                                Nombre = reader.GetString("Nombre"),
                                Apellido = reader.GetString("Apellido"),
                                Contrasena = reader.GetString("Contrasena"),
                                Dni = reader.GetString("Dni"),
                                Email = reader.GetString("Email"),
                                Estado = reader.GetBoolean("Estado"),
                                Rol = Enum.Parse<Usuario.TipoRol>(
                            reader.GetString("Rol"), // trae "adminstrador" o "empleado"
                            true                     // true = ignoreCase
                        )
                            };
                            lista.Add(p);
                         }
                }
         }
    }
    return lista;
        }

        public IList<Usuario> obtenerPaginados(int offset, int limit)
        {
            IList<Usuario> res = new List<Usuario>();
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                string query = @"SELECT * FROM usuario 
                WHERE Estado = true
                ORDER BY IdUsuario
                LIMIT @limit OFFSET @offset;";

                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@limit", limit);
                    command.Parameters.AddWithValue("@offset", offset);
                    connection.Open();
                    var reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        Usuario p = new Usuario
                        {
                            IdUsuario = reader.GetInt32(nameof(Usuario.IdUsuario)),
                            Nombre = reader.GetString(nameof(Usuario.Nombre)),
                            Apellido = reader.GetString(nameof(Usuario.Apellido)),
                            Dni = reader.GetString(nameof(Usuario.Dni)),
                            Email = reader.GetString(nameof(Usuario.Email)),
                            Contrasena = reader.GetString(nameof(Usuario.Contrasena)),
                            Rol = Enum.Parse<Usuario.TipoRol>(reader.GetString(nameof(Usuario.Rol))),
                            Estado = reader.GetBoolean(nameof(Usuario.Estado))
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
                string query = "SELECT COUNT(*) FROM usuario WHERE Estado = true;";

                using (var command = new MySqlCommand(query, connection))
                {
                    connection.Open();
                    return Convert.ToInt32(command.ExecuteScalar());
                }
            }
        }
        public int Alta(Usuario u)
        {
            int res = -1;
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                string query = $@"INSERT INTO usuario (
                    {nameof(Usuario.Nombre)},
                    {nameof(Usuario.Apellido)},
                    {nameof(Usuario.Dni)},
                    {nameof(Usuario.Rol)},
                    {nameof(Usuario.Email)},
                    {nameof(Usuario.Contrasena)},
                    {nameof(Usuario.Estado)})
                    VALUES (@Nombre, @Apellido, @Dni, @Rol, @Email, @Contrasena, @Estado);
                    SELECT LAST_INSERT_ID();";

                using (var command = new MySqlCommand(query, connection))
                {
                    u.Estado = true;
                    command.Parameters.AddWithValue("@Nombre", u.Nombre);
                    command.Parameters.AddWithValue("@Apellido", u.Apellido);
                    command.Parameters.AddWithValue("@Dni", u.Dni);
                    command.Parameters.AddWithValue("@Rol", u.Rol.ToString());
                    command.Parameters.AddWithValue("@Email", u.Email);
                    command.Parameters.AddWithValue("@Contrasena", u.Contrasena);
                    command.Parameters.AddWithValue("@Estado", u.Estado);

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
                string query = $@"DELETE FROM usuario 
                WHERE {nameof(Propietario.IdPropietario)} = @IdUsuario";
                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@IdUsuario", id);
                    connection.Open();
                    res = command.ExecuteNonQuery();
                }
            }
            return res;
        }
        public int Modificacion(Usuario u)
        {
            int res = -1;
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                string query = $@"UPDATE usuario SET
                    {nameof(Usuario.Nombre)}=@Nombre,
                    {nameof(Usuario.Apellido)}=@Apellido,
                    {nameof(Usuario.Dni)}=@Dni,
                    {nameof(Usuario.Rol)}=@Rol,
                    {nameof(Usuario.Email)}=@Email,
                    {nameof(Usuario.Contrasena)}=@Contrasena
                    WHERE {nameof(Usuario.IdUsuario)}=@IdUsuario";

                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@IdUsuario", u.IdUsuario);
                    command.Parameters.AddWithValue("@Nombre", u.Nombre);
                    command.Parameters.AddWithValue("@Apellido", u.Apellido);
                    command.Parameters.AddWithValue("@Dni", u.Dni);
                    command.Parameters.AddWithValue("@Rol", u.Rol.ToString());
                    command.Parameters.AddWithValue("@Email", u.Email);
                    command.Parameters.AddWithValue("@Contrasena", u.Contrasena);
                    connection.Open();
                    res = command.ExecuteNonQuery();
                }
            }
            return res;
        }
        public IList<Usuario> ListarUsuarios()
        {
            IList<Usuario> res = new List<Usuario>();
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                string query = $@"SELECT * FROM usuario WHERE Estado = true;";

                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    connection.Open();
                    var reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        Usuario p = new Usuario
                        {
                            IdUsuario = reader.GetInt32(nameof(Usuario.IdUsuario)),
                            Nombre = reader.GetString(nameof(Usuario.Nombre)),
                            Apellido = reader.GetString(nameof(Usuario.Apellido)),
                            Dni = reader.GetString(nameof(Usuario.Dni)),
                            Email = reader.GetString(nameof(Usuario.Email)),
                            Contrasena = reader.GetString(nameof(Usuario.Contrasena)),
                            Rol = Enum.Parse<Usuario.TipoRol>(reader.GetString(nameof(Usuario.Rol))),
                            Estado = reader.GetBoolean(nameof(Usuario.Estado))
                        };
                        res.Add(p);
                    }
                    connection.Close();
                }
            }
            return res;
        }
        public Usuario UsuarioId(int IdUsuario)
        {
            Usuario res = null;
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                string query = $@"SELECT * FROM usuario WHERE IdUsuario = @IdUsuario;";

                using (MySqlCommand command = new MySqlCommand(query, connection))

                {
                    command.Parameters.AddWithValue("@IdUsuario", IdUsuario);
                    connection.Open();
                    var reader = command.ExecuteReader();
                    if (reader.Read())
                    {
                        res = new Usuario
                        {
                            IdUsuario = reader.GetInt32(nameof(Usuario.IdUsuario)),
                            Nombre = reader.GetString(nameof(Usuario.Nombre)),
                            Apellido = reader.GetString(nameof(Usuario.Apellido)),
                            Dni = reader.GetString(nameof(Usuario.Dni)),
                            Email = reader.GetString(nameof(Usuario.Email)),
                            Contrasena = reader.GetString(nameof(Usuario.Contrasena)),
                            Rol = Enum.Parse<Usuario.TipoRol>(reader.GetString(nameof(Usuario.Rol))),
                            Estado = reader.GetBoolean(nameof(Usuario.Estado))
                        };
                    }
                    connection.Close();
                }
            }
            return res;
        }
        public int DarDeBaja(int IdUsuario)
        {
            int res = -1;
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                string query = $@"UPDATE usuario 
                    SET Estado = 0 
                    WHERE {nameof(IdUsuario)}=@IdUsuario";

                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@IdUsuario", IdUsuario);
                    connection.Open();
                    res = command.ExecuteNonQuery();
                }
            }
            return res;
        }
    }
}
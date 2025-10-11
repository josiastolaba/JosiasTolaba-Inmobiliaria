using System.Data;
using MySql.Data.MySqlClient;

namespace INMOBILIARIA_JosiasTolaba.Models
{
    public class RepositorioInmueble : RepositorioBase, IRepositorioInmueble
    {
        public RepositorioInmueble(IConfiguration configuration) : base(configuration)
        {
        }
        public List<Inmueble> buscar(string dato)
        {
            var lista = new List<Inmueble>();
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                string query = @"SELECT * 
                                FROM inmueble
                                WHERE Direccion LIKE @dato
                                OR Uso LIKE @dato
                                OR Precio LIKE @dato
                                LIMIT 10";
                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@dato", "%" + dato + "%");
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var i = new Inmueble
                            {
                                IdInmueble = reader.GetInt32(nameof(Inmueble.IdInmueble)),
                                Direccion = reader.GetString(nameof(Inmueble.Direccion)),
                                IdTipo = reader.GetInt32(nameof(Inmueble.IdTipo)),
                                IdPropietario = reader.GetInt32(nameof(Inmueble.IdPropietario)),
                                Uso = Enum.Parse<Inmueble.TipoUso>(reader.GetString(nameof(Inmueble.Uso))),
                                Latitud = reader.GetDecimal(nameof(Inmueble.Latitud)),
                                Longitud = reader.GetDecimal(nameof(Inmueble.Longitud)),
                                Ambiente = reader.GetInt32(nameof(Inmueble.Ambiente)),
                                Precio = reader.GetDecimal(nameof(Inmueble.Precio)),
                                Estado = reader.GetBoolean(nameof(Inmueble.Estado))
                            };
                            lista.Add(i);
                        }
                    }
                }
            }
            return lista;
        }

       public IList<Inmueble> buscarAvanzadoInmuebles(int IdPropietario, string direccion)
{
    var lista = new List<Inmueble>();
    using (var connection = new MySqlConnection(connectionString))
    {
        string query = @"
            SELECT m.IdInmueble, m.Direccion, m.IdTipo, m.Estado AS EstadoInmueble,
                   p.IdPropietario, p.Nombre, p.Apellido, p.Dni, p.Telefono, p.Email, p.Estado AS EstadoPropietario
            FROM Inmueble m
            JOIN Propietario p ON m.IdPropietario = p.IdPropietario
            WHERE (@direccion IS NULL OR m.Direccion LIKE CONCAT('%', @direccion, '%'))
              AND (@IdPropietario IS NULL 
                   OR p.Nombre LIKE CONCAT('%', @IdPropietario, '%') 
                   OR p.Apellido LIKE CONCAT('%', @IdPropietario, '%')
                   OR p.DNI LIKE CONCAT('%', @IdPropietario, '%'))
            ORDER BY m.Direccion";

        using (var command = new MySqlCommand(query, connection))
        {
            command.Parameters.AddWithValue("@direccion", (object)direccion ?? DBNull.Value);
            command.Parameters.AddWithValue("@IdPropietario", (object)IdPropietario ?? DBNull.Value);

            connection.Open();
            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    var inmueble = new Inmueble
                    {
                        IdInmueble = reader.GetInt32("IdInmueble"),
                        Direccion = reader.GetString("Direccion"),
                        IdTipo = reader.GetInt32("IdTipo"),
                        Estado = reader.GetBoolean("EstadoInmueble"),

                        Propietario = new Propietario
                        {
                            IdPropietario = reader.GetInt32("IdPropietario"),
                            Nombre = reader.GetString("Nombre"),
                            Apellido = reader.GetString("Apellido"),
                            Dni = reader.GetString("Dni"),
                            Telefono = reader.GetString("Telefono"),
                            Email = reader.GetString("Email"),
                            Estado = reader.GetBoolean("EstadoPropietario")
                        }
                    };

                    lista.Add(inmueble);
                }
            }
        }
    }
    return lista;
}


public IList<Inmueble> Disponibilidad(DateTime fechaDesde, DateTime fechaHasta)
{
    var lista = new List<Inmueble>();

    using (MySqlConnection connection = new MySqlConnection(connectionString))
    {
        string query = @"
            SELECT i.IdInmueble, i.Direccion, i.Precio, i.Estado AS EstadoInmueble,
                   p.IdPropietario, p.Nombre AS PropietarioNombre, p.Apellido AS PropietarioApellido, 
                   p.Dni AS PropietarioDni, p.Telefono AS PropietarioTelefono, p.Email AS PropietarioEmail
            FROM Inmueble i
            JOIN Propietario p ON i.IdPropietario = p.IdPropietario
            WHERE i.IdInmueble NOT IN (
                SELECT r.IdInmueble 
                FROM Reserva r
                WHERE r.Estado = true
                  AND r.FechaDesde <= @fechaHasta
                  AND r.FechaHasta >= @fechaDesde
            )
            ORDER BY i.Direccion";

        using (var command = new MySqlCommand(query, connection))
        {
            command.Parameters.AddWithValue("@fechaDesde", fechaDesde);
            command.Parameters.AddWithValue("@fechaHasta", fechaHasta);

            connection.Open();
            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    var inmueble = new Inmueble
                    {
                        IdInmueble = reader.GetInt32("IdInmueble"),
                        Direccion = reader.GetString("Direccion"),
                        Precio = reader.GetDecimal("Precio"),
                        Estado = reader.GetBoolean("EstadoInmueble"),

                        Propietario = new Propietario
                        {
                            IdPropietario = reader.GetInt32("IdPropietario"),
                            Nombre = reader.GetString("PropietarioNombre"),
                            Apellido = reader.GetString("PropietarioApellido"),
                            Dni = reader.GetString("PropietarioDni"),
                            Telefono = reader.GetString("PropietarioTelefono"),
                            Email = reader.GetString("PropietarioEmail"),
                            Estado = true 
                        }
                    };

                    lista.Add(inmueble);
                }
            }
        }
    }

    return lista;
}





        public List<Inmueble> inmueblesPorPropietario(int IdPropietario)
        {

            var lista = new List<Inmueble>();
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                string query = @"SELECT IdInmueble, Direccion, Precio, IdPropietario 
                FROM Inmueble
                WHERE IdPropietario = @IdPropietario";

                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@IdPropietario", IdPropietario);
                    connection.Open();

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var i = new Inmueble
                            {
                                IdInmueble = reader.GetInt32(nameof(Inmueble.IdInmueble)),
                                Direccion = reader.GetString(nameof(Inmueble.Direccion)),
                                Precio = reader.GetDecimal(nameof(Inmueble.Precio))
                            };
                            lista.Add(i);
                        }
                    }
                }
                return lista;
            }
        }


         public IList<Inmueble> obtenerPaginados(int offset, int limit)
        {
            IList<Inmueble> res = new List<Inmueble>();
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                string query = @"SELECT * FROM inmueble
                                WHERE Estado = true
                                ORDER BY IdInmueble
                                LIMIT @limit OFFSET @offset;";
                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@limit", limit);
                    command.Parameters.AddWithValue("@offset", offset);
                    connection.Open();
                    var reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        Inmueble p = new Inmueble
                        {
                            IdInmueble = reader.GetInt32(nameof(Inmueble.IdInmueble)),
                            Direccion = reader.GetString(nameof(Inmueble.Direccion)),
                            IdTipo = reader.GetInt32(nameof(Inmueble.IdTipo)),
                            IdPropietario = reader.GetInt32(nameof(Inmueble.IdPropietario)),
                            Uso = Enum.Parse<Inmueble.TipoUso>(reader.GetString(nameof(Inmueble.Uso))),
                            Latitud = reader.GetDecimal(nameof(Inmueble.Latitud)),
                            Longitud = reader.GetDecimal(nameof(Inmueble.Longitud)),
                            Ambiente = reader.GetInt32(nameof(Inmueble.Ambiente)),
                            Precio = reader.GetDecimal(nameof(Inmueble.Precio)),
                            Estado = reader.GetBoolean(nameof(Inmueble.Estado))
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
                string query = "SELECT COUNT(*) FROM inmueble WHERE Estado = true;";
                using (var command = new MySqlCommand(query, connection))
                {
                    connection.Open();
                    return Convert.ToInt32(command.ExecuteScalar());
                }
            }
        }
        public int Alta(Inmueble i)
        {
            int res = -1;
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                string query = $@"INSERT INTO inmueble (
                {nameof(Inmueble.Direccion)},
                {nameof(Inmueble.IdTipo)},
                {nameof(Inmueble.IdPropietario)},
                {nameof(Inmueble.Uso)},
                {nameof(Inmueble.Latitud)},
                {nameof(Inmueble.Longitud)},
                {nameof(Inmueble.Precio)},
                {nameof(Inmueble.Ambiente)},
                {nameof(Inmueble.Estado)})
                VALUES (@Direccion, @IdTipo, @IdPropietario, @Uso, @Latitud, @Longitud, @Precio, @Ambiente, @Estado);
                SELECT LAST_INSERT_ID();";
                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    i.Estado = true;
                    command.Parameters.AddWithValue("@Direccion", i.Direccion);
                    command.Parameters.AddWithValue("@IdTipo", i.IdTipo);
                    command.Parameters.AddWithValue("@IdPropietario", i.IdPropietario);
                    command.Parameters.AddWithValue("@Uso", i.Uso.ToString());
                    command.Parameters.AddWithValue("@Latitud", i.Latitud);
                    command.Parameters.AddWithValue("@Longitud", i.Longitud);
                    command.Parameters.AddWithValue("@Precio", i.Precio);
                    command.Parameters.AddWithValue("@Ambiente", i.Ambiente);
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
                string query = $@"DELETE FROM inmueble WHERE {nameof(Inmueble.IdInmueble)}=@IdInmueble";
                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@IdInmueble", id);
                    connection.Open();
                    res = command.ExecuteNonQuery();
                    connection.Close();
                }
            }
            return res;
        }
        public int Modificacion(Inmueble i)
        {
            int res = -1;
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                string query = $@"UPDATE inmueble SET
                {nameof(Inmueble.Direccion)}=@Direccion,
                {nameof(Inmueble.IdTipo)}=@IdTipo,
                {nameof(Inmueble.IdPropietario)}=@IdPropietario,
                {nameof(Inmueble.Uso)}=@Uso,
                {nameof(Inmueble.Latitud)}=@Latitud,
                {nameof(Inmueble.Longitud)}=@Longitud,
                {nameof(Inmueble.Precio)}=@Precio,
                {nameof(Inmueble.Ambiente)}=@Ambiente
                WHERE {nameof(Inmueble.IdInmueble)}=@IdInmueble";

                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@IdInmueble", i.IdInmueble);
                    command.Parameters.AddWithValue("@Direccion", i.Direccion);
                    command.Parameters.AddWithValue("@IdTipo", i.IdTipo);
                    command.Parameters.AddWithValue("@IdPropietario", i.IdPropietario);
                    command.Parameters.AddWithValue("@Uso", i.Uso.ToString());
                    command.Parameters.AddWithValue("@Latitud", i.Latitud);
                    command.Parameters.AddWithValue("@Longitud", i.Longitud);
                    command.Parameters.AddWithValue("@Precio", i.Precio);
                    command.Parameters.AddWithValue("@Ambiente", i.Ambiente);
                    connection.Open();
                    res = command.ExecuteNonQuery();
                    connection.Close();
                }
            }
            return res;
        }
        public IList<Inmueble> ListarInmuebles()
        {
            IList<Inmueble> res = new List<Inmueble>();
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                string query = $@"SELECT * FROM inmueble WHERE Estado = true;";

                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    connection.Open();
                    var reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        Inmueble p = new Inmueble
                        {
                            IdInmueble = reader.GetInt32(nameof(Inmueble.IdInmueble)),
                            Direccion = reader.GetString(nameof(Inmueble.Direccion)),
                            IdTipo = reader.GetInt32(nameof(Inmueble.IdTipo)),
                            IdPropietario = reader.GetInt32(nameof(Inmueble.IdPropietario)),
                            Uso = Enum.Parse<Inmueble.TipoUso>(reader.GetString(nameof(Inmueble.Uso))),
                            Latitud = reader.GetDecimal(nameof(Inmueble.Latitud)),
                            Longitud = reader.GetDecimal(nameof(Inmueble.Longitud)),
                            Ambiente = reader.GetInt32(nameof(Inmueble.Ambiente)),
                            Precio = reader.GetDecimal(nameof(Inmueble.Precio)),
                            PortadaUrl = reader[nameof(Inmueble.PortadaUrl)] == DBNull.Value ? null : reader.GetString(nameof(Inmueble.PortadaUrl)),
                            Estado = reader.GetBoolean(nameof(Inmueble.Estado))
                        };
                        res.Add(p);
                    }
                    connection.Close();
                }
            }
            return res;
        }
        public Inmueble InmuebleId(int IdInmueble)
        {
            Inmueble res = null;
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                string query = $@"SELECT i.*,p.*, t.Nombre AS Tipo
                                FROM inmueble i
                                JOIN propietario p ON i.IdPropietario = p.IdPropietario
                                JOIN tipo_inmueble t ON i.IdTipo = t.IdTipo
                                WHERE IdInmueble = @IdInmueble;";

                using (MySqlCommand command = new MySqlCommand(query, connection))

                {
                    command.Parameters.AddWithValue("@IdInmueble", IdInmueble);
                    connection.Open();
                    var reader = command.ExecuteReader();
                    if (reader.Read())
                    {
                        res = new Inmueble
                        {
                            IdInmueble = reader.GetInt32(nameof(Inmueble.IdInmueble)),
                            Direccion = reader.GetString(nameof(Inmueble.Direccion)),
                            IdTipo = reader.GetInt32(nameof(Inmueble.IdTipo)),
                            IdPropietario = reader.GetInt32(nameof(Inmueble.IdPropietario)),
                            Uso = Enum.Parse<Inmueble.TipoUso>(reader.GetString(nameof(Inmueble.Uso))),
                            Latitud = reader.GetDecimal(nameof(Inmueble.Latitud)),
                            Longitud = reader.GetDecimal(nameof(Inmueble.Longitud)),
                            Precio = reader.GetDecimal(nameof(Inmueble.Precio)),
                            Ambiente = reader.GetInt32(nameof(Inmueble.Ambiente)),
                            PortadaUrl = reader[nameof(Inmueble.PortadaUrl)] == DBNull.Value ? null : reader.GetString(nameof(Inmueble.PortadaUrl)),
                            Estado = reader.GetBoolean(nameof(Inmueble.Estado))
                        };
                    }
                    connection.Close();
                }
            }
            return res;
        }
        
        public List<Inmueble> BuscarPorDireccion(string texto)
{
    var lista = new List<Inmueble>();
    using (MySqlConnection connection = new MySqlConnection(connectionString))
    {
        string query = @"SELECT IdInmueble, Direccion 
                         FROM inmueble 
                         WHERE Direccion LIKE @texto
                         LIMIT 10;";

        using (var command = new MySqlCommand(query, connection))
        {
            command.Parameters.AddWithValue("@texto", "%" + texto + "%");
            connection.Open();

            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    lista.Add(new Inmueble
                    {
                        IdInmueble = reader.GetInt32("IdInmueble"),
                        Direccion = reader.GetString("Direccion")
                    });
                }
            }
        }
    }
    return lista;
}

        public int DarDeBaja(int IdInmueble)
        {
            int res = -1;
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                string query = $@"UPDATE inmueble
                    SET Estado = 0 
                    WHERE {nameof(IdInmueble)}=@IdInmueble";

                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@IdInmueble", IdInmueble);
                    connection.Open();
                    res = command.ExecuteNonQuery();
                }
            }
            return res;
        }
        public int ModificarPortada(int id, string url)
		{
			int res = -1;
			using (MySqlConnection connection = new MySqlConnection(connectionString))
			{
				string query = @"
					UPDATE inmueble SET
					PortadaUrl=@portada
					WHERE IdInmueble = @IdInmueble";
				using (MySqlCommand command = new MySqlCommand(query, connection))
				{
					command.Parameters.AddWithValue("@portada", String.IsNullOrEmpty(url) ? DBNull.Value : url);
					command.Parameters.AddWithValue("@IdInmueble", id);
					command.CommandType = CommandType.Text;
					connection.Open();
					res = command.ExecuteNonQuery();
					connection.Close();
				}
			}
			return res;
		}
        
    }
}
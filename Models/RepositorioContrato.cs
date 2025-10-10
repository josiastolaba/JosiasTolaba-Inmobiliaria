using MySql.Data.MySqlClient;

namespace INMOBILIARIA_JosiasTolaba.Models
{
    public class RepositorioContrato : RepositorioBase, IRepositorioContrato
    {
        public RepositorioContrato(IConfiguration configuration) : base(configuration)
        {
        }

        public List<Contrato> buscar(string dato)
        {
            var lista = new List<Contrato>();
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                string query = @"SELECT c.IdContrato, c.FechaInicio, c.FechaFin, c.MontoMensual, c.Estado,
                                i.IdInquilino, i.Nombre AS InquilinoNombre, i.Apellido AS InquilinoApellido, i.Dni AS InquilinoDni,
                                m.IdInmueble, m.Direccion AS InmuebleDireccion, m.IdTipo AS InmuebleTipo
                                FROM contrato c
                                JOIN inquilino i ON c.IdInquilino = i.IdInquilino
                                JOIN inmueble m ON c.IdInmueble = m.IdInmueble 
                                WHERE i.Nombre LIKE @dato
                                OR i.Apellido LIKE @dato
                                OR m.IdTipo LIKE @dato
                                OR i.Dni LIKE @dato
                                LIMIT 10";
                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@dato", "%" + dato + "%");
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var c = new Contrato
                            {
                                IdContrato = reader.GetInt32("IdContrato"),
                                FechaInicio = reader.GetDateTime("FechaInicio"),
                                FechaFin = reader.GetDateTime("FechaFin"),
                                MontoMensual = reader.GetInt32("MontoMensual"),
                                //QuienCreo = reader.GetInt32("QuienCreo"),
                                //QuienElimino = reader.GetInt32("QuienElimino"),
                                Estado = reader.GetBoolean("Estado"),
                                Habitante = new InquilinoDto
                                {
                                    IdInquilino = reader.GetInt32("IdInquilino"),
                                    Nombre = reader.GetString("InquilinoNombre"),
                                    Apellido = reader.GetString("InquilinoApellido"),
                                    Dni = reader.GetString("InquilinoDni")
                                },
                                Propiedad = new InmuebleDto
                                {
                                    IdInmueble = reader.GetInt32("IdInmueble"),
                                    Direccion = reader.GetString("InmuebleDireccion"),
                                    IdTipo = reader.GetInt32("InmuebleTipo")
                                }
                            };
                            lista.Add(c);
                        }
                    }
                }
            }
            return lista;
        }

        public List<Contrato> contratosPorInquilino(int IdInquilino)
        {
            var lista = new List<Contrato>();
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                string query = @"
            SELECT 
                c.IdContrato, c.FechaInicio, c.FechaFin, c.MontoMensual, c.Estado,
                i.IdInquilino, i.Nombre AS InquilinoNombre, i.Apellido AS InquilinoApellido, i.Dni AS InquilinoDni,
                inm.IdInmueble, inm.Direccion AS InmuebleDireccion, inm.IdTipo AS InmuebleTipo
            FROM contrato c
            JOIN inquilino i ON c.IdInquilino = i.IdInquilino
            JOIN inmueble inm ON c.IdInmueble = inm.IdInmueble
                WHERE c.IdInquilino = @IdInquilino";

                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@IdInquilino", IdInquilino);
                    connection.Open();

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Contrato c = new Contrato
                            {
                                IdContrato = reader.GetInt32(nameof(Contrato.IdContrato)),
                                FechaInicio = reader.GetDateTime(nameof(Contrato.FechaInicio)),
                                FechaFin = reader.GetDateTime(nameof(Contrato.FechaFin)),
                                MontoMensual = reader.GetInt32(nameof(Contrato.MontoMensual)),
                                Habitante = new InquilinoDto
                                {
                                    IdInquilino = reader.GetInt32("IdInquilino"),
                                    Nombre = reader.GetString("InquilinoNombre"),
                                    Apellido = reader.GetString("InquilinoApellido"),
                                    Dni = reader.GetString("InquilinoDni")
                                },
                                Propiedad = new InmuebleDto
                                {
                                    IdInmueble = reader.GetInt32("IdInmueble"),
                                    Direccion = reader.GetString("InmuebleDireccion"),
                                    IdTipo = reader.GetInt32("InmuebleTipo")
                                },
                                Estado = reader.GetBoolean(nameof(Contrato.Estado))
                            };
                            lista.Add(c);
                        }
                    }
                }
            }
            return lista;
        }

        public int Alta(Contrato p)
        {
            int res = -1;
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                string query = @"
                    INSERT INTO contrato (FechaInicio, FechaFin, MontoMensual, IdInquilino, IdInmueble, QuienCreo, QuienElimino, Estado)
                    VALUES (@FechaInicio, @FechaFin, @MontoMensual, @IdInquilino, @IdInmueble, @QuienCreo, @QuienElimino, @Estado);
                    SELECT LAST_INSERT_ID();";

                using (var command = new MySqlCommand(query, connection))
                {
                    p.Estado = true;
                    command.Parameters.AddWithValue("@FechaInicio", p.FechaInicio);
                    command.Parameters.AddWithValue("@FechaFin", p.FechaFin);
                    command.Parameters.AddWithValue("@MontoMensual", p.MontoMensual);
                    command.Parameters.AddWithValue("@IdInquilino", p.Habitante.IdInquilino);
                    command.Parameters.AddWithValue("@IdInmueble", p.Propiedad.IdInmueble);
                    command.Parameters.AddWithValue("@QuienCreo", DBNull.Value);
                    command.Parameters.AddWithValue("@QuienElimino", DBNull.Value);
                    command.Parameters.AddWithValue("@Estado", p.Estado);
                    connection.Open();
                    res = Convert.ToInt32(command.ExecuteScalar());
                }
            }
            return res;
        }

        public IList<Contrato> obtenerPaginados(int offset, int limit)
        {
            IList<Contrato> res = new List<Contrato>();
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
             String query = @"
            SELECT 
                c.IdContrato, c.FechaInicio, c.FechaFin, c.MontoMensual, c.Estado,
                i.IdInquilino, i.Nombre AS InquilinoNombre, i.Apellido AS InquilinoApellido, i.Dni AS InquilinoDni,
                inm.IdInmueble, inm.Direccion AS InmuebleDireccion, inm.IdTipo AS InmuebleTipo
            FROM contrato c
            JOIN inquilino i ON c.IdInquilino = i.IdInquilino
            JOIN inmueble inm ON c.IdInmueble = inm.IdInmueble
            WHERE c.Estado = TRUE
            ORDER BY c.IdContrato
            LIMIT @limit OFFSET @offset";

                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@limit", limit);
                    command.Parameters.AddWithValue("@offset", offset);
                    connection.Open();
                    var reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        Contrato c = new Contrato
                        {
                            IdContrato = reader.GetInt32(nameof(Contrato.IdContrato)),
                            FechaInicio = reader.GetDateTime(nameof(Contrato.FechaInicio)),
                            FechaFin = reader.GetDateTime(nameof(Contrato.FechaFin)),
                            MontoMensual = reader.GetInt32(nameof(Contrato.MontoMensual)),
                            Habitante = new InquilinoDto
                            {
                                IdInquilino = reader.GetInt32("IdInquilino"),
                                Nombre = reader.GetString("InquilinoNombre"),
                                Apellido = reader.GetString("InquilinoApellido"),
                                Dni = reader.GetString("InquilinoDni")
                            },
                            Propiedad = new InmuebleDto
                            {
                                IdInmueble = reader.GetInt32("IdInmueble"),
                                Direccion = reader.GetString("InmuebleDireccion"),
                                IdTipo = reader.GetInt32("InmuebleTipo")
                            },
                            Estado = reader.GetBoolean(nameof(Contrato.Estado))
                        };
                        res.Add(c);
                    }
                }
            }
            return res;
        }

        public int contar()
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                string query = "SELECT COUNT(*) FROM contrato WHERE Estado = true;";

                using (var command = new MySqlCommand(query, connection))
                {
                    connection.Open();
                    return Convert.ToInt32(command.ExecuteScalar());
                }
            }
        }

        public int DarDeBaja(int IdContrato)
        {
            int res = -1;
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                string query = $@"UPDATE contrato 
                    SET Estado = 0 
                    WHERE IdContrato = @IdContrato";

                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@IdContrato", IdContrato);
                    connection.Open();
                    res = command.ExecuteNonQuery();
                }
            }
            return res;
        }

        public int Baja(int IdContrato)
        {
            int res = -1;
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                string query = @"DELETE FROM contrato WHERE IdContrato = @IdContrato";

                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@IdContrato", IdContrato);
                    connection.Open();
                    res = command.ExecuteNonQuery();
                }
            }
            return res;
        }

        public int Modificacion(Contrato c)
        {
            int res = -1;
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                string query = @"
                    UPDATE contrato SET
                        FechaInicio = @FechaInicio,
                        FechaFin = @FechaFin,
                        MontoMensual = @MontoMensual,
                        IdInquilino = @IdInquilino,
                        IdInmueble = @IdInmueble
                    WHERE IdContrato = @IdContrato";

                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@FechaInicio", c.FechaInicio);
                    command.Parameters.AddWithValue("@FechaFin", c.FechaFin);
                    command.Parameters.AddWithValue("@MontoMensual", c.MontoMensual);
                    command.Parameters.AddWithValue("@IdInquilino", c.Habitante.IdInquilino);
                    command.Parameters.AddWithValue("@IdInmueble", c.Propiedad.IdInmueble);
                    //command.Parameters.AddWithValue("@QuienCreo", c.QuienCreo);
                    //command.Parameters.AddWithValue("@QuienElimino", c.QuienElimino);
                    command.Parameters.AddWithValue("@IdContrato", c.IdContrato);

                    connection.Open();
                    res = command.ExecuteNonQuery();
                }
            }
            return res;
        }

        public IList<Contrato> ListarContratos()
        {
            IList<Contrato> res = new List<Contrato>();
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                string query = @"
                    SELECT 
                        c.IdContrato, c.FechaInicio, c.FechaFin, c.MontoMensual, c.Estado,
                        i.IdInquilino, i.Nombre AS InquilinoNombre, i.Apellido AS InquilinoApellido, i.Dni AS InquilinoDni,
                        m.IdInmueble, m.Direccion AS InmuebleDireccion, m.IdTipo AS InmuebleTipo
                    FROM contrato c
                    JOIN inquilino i ON c.IdInquilino = i.IdInquilino
                    JOIN inmueble m ON c.IdInmueble = m.IdInmueble
                    WHERE c.Estado = true;";

                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    connection.Open();
                    var reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        Contrato c = new Contrato
                        {
                            IdContrato = reader.GetInt32("IdContrato"),
                            FechaInicio = reader.GetDateTime("FechaInicio"),
                            FechaFin = reader.GetDateTime("FechaFin"),
                            MontoMensual = reader.GetInt32("MontoMensual"),
                            //QuienCreo = reader.GetInt32("QuienCreo"),
                            //QuienElimino = reader.GetInt32("QuienElimino"),
                            Estado = reader.GetBoolean("Estado"),
                            Habitante = new InquilinoDto
                            {
                                IdInquilino = reader.GetInt32("IdInquilino"),
                                Nombre = reader.GetString("InquilinoNombre"),
                                Apellido = reader.GetString("InquilinoApellido"),
                                Dni = reader.GetString("InquilinoDni")
                            },
                            Propiedad = new InmuebleDto
                            {
                                IdInmueble = reader.GetInt32("IdInmueble"),
                                Direccion = reader.GetString("InmuebleDireccion"),
                                IdTipo = reader.GetInt32("InmuebleTipo")
                            }
                        };
                        res.Add(c);
                    }
                    connection.Close();
                }
            }
            return res;
        }

    public Contrato IdContrato(int IdContrato)
{
    Contrato res = null;

    using (MySqlConnection connection = new MySqlConnection(connectionString))
    {
        string query = @"
            SELECT 
                c.IdContrato, c.FechaInicio, c.FechaFin, c.MontoMensual, c.Estado,
                i.IdInquilino, i.Nombre AS InquilinoNombre, i.Apellido AS InquilinoApellido, i.Dni AS InquilinoDni,
                m.IdInmueble, m.Direccion AS InmuebleDireccion, m.IdTipo,
                t.Nombre AS InmuebleTipoNombre
            FROM contrato c
            JOIN inquilino i ON c.IdInquilino = i.IdInquilino
            JOIN inmueble m ON c.IdInmueble = m.IdInmueble
            JOIN tipo_inmueble t ON m.IdTipo = t.IdTipo
            WHERE c.IdContrato = @IdContrato;";

        using (MySqlCommand command = new MySqlCommand(query, connection))
        {
            command.Parameters.AddWithValue("@IdContrato", IdContrato);

            connection.Open();

            using (var reader = command.ExecuteReader())
            {
                if (reader.Read())
                {
                    res = new Contrato
                    {
                        IdContrato = reader.GetInt32("IdContrato"),
                        FechaInicio = reader.GetDateTime("FechaInicio"),
                        FechaFin = reader.GetDateTime("FechaFin"),
                        MontoMensual = reader.GetInt32("MontoMensual"),
                        Estado = reader.GetBoolean("Estado"),

                        Habitante = new InquilinoDto
                        {
                            IdInquilino = reader.GetInt32("IdInquilino"),
                            Nombre = reader.IsDBNull(reader.GetOrdinal("InquilinoNombre")) ? "Sin nombre" : reader.GetString("InquilinoNombre"),
                            Apellido = reader.IsDBNull(reader.GetOrdinal("InquilinoApellido")) ? "Sin apellido" : reader.GetString("InquilinoApellido"),
                            Dni = reader.IsDBNull(reader.GetOrdinal("InquilinoDni")) ? "Sin DNI" : reader.GetString("InquilinoDni")
                        },

                        Propiedad = new InmuebleDto
                        {
                            IdInmueble = reader.GetInt32("IdInmueble"),
                            Direccion = reader.IsDBNull(reader.GetOrdinal("InmuebleDireccion")) ? "Sin dirección" : reader.GetString("InmuebleDireccion"),
                            IdTipo = reader.GetInt32("IdTipo"),
                            TipoNombre = reader.IsDBNull(reader.GetOrdinal("InmuebleTipoNombre")) ? "Sin tipo" : reader.GetString("InmuebleTipoNombre")
                        }
                    };
                }
            }

            connection.Close();
        }
    }

    return res;
}

        public IList<Contrato> ListarPorInquilino(int idInquilino)
        {
            IList<Contrato> res = new List<Contrato>();
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                string query = @"
                    SELECT 
                        c.IdContrato, c.FechaInicio, c.FechaFin, c.MontoMensual,
                        i.IdInquilino, i.Nombre AS InquilinoNombre, i.Apellido AS InquilinoApellido, i.Dni AS InquilinoDni,
                        m.IdInmueble, m.Dirección AS InmuebleDireccion, m.Tipo AS InmuebleTipo
                    FROM contrato c
                    JOIN inquilino i ON c.IdInquilino = i.IdInquilino
                    JOIN inmueble m ON c.IdInmueble = m.IdInmueble
                    WHERE c.IdInquilino = @idInquilino;";

                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@idInquilino", idInquilino);
                    connection.Open();
                    var reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        Contrato c = new Contrato
                        {
                            IdContrato = reader.GetInt32("IdContrato"),
                            FechaInicio = reader.GetDateTime("FechaInicio"),
                            FechaFin = reader.GetDateTime("FechaFin"),
                            MontoMensual = reader.GetInt32("MontoMensual"),
                            Habitante = new InquilinoDto
                            {
                                IdInquilino = reader.GetInt32("IdInquilino"),
                                Nombre = reader.GetString("InquilinoNombre"),
                                Apellido = reader.GetString("InquilinoApellido"),
                                Dni = reader.GetString("InquilinoDni")
                            },
                            Propiedad = new InmuebleDto
                            {
                                IdInmueble = reader.GetInt32("IdInmueble"),
                                Direccion = reader.GetString("InmuebleDireccion"),
                                IdTipo = reader.GetInt32("InmuebleTipo")
                            }
                        };
                        res.Add(c);
                    }
                    connection.Close();
                }
            }
            return res;
        }
    }
}

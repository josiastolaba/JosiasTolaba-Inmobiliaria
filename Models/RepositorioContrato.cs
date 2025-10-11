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

        public int Alta(Contrato c)
        {
            int res = -1;
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        // Inserta Contrato
                        string queryContrato = @"
                            INSERT INTO contrato (FechaInicio, FechaFin, MontoMensual, IdInquilino, IdInmueble, QuienCreo, QuienElimino, Estado)
                            VALUES (@FechaInicio, @FechaFin, @MontoMensual, @IdInquilino, @IdInmueble, @QuienCreo, @QuienElimino, @Estado);
                            SELECT LAST_INSERT_ID();";
                        using (var cmdContrato = new MySqlCommand(queryContrato, connection, transaction))
                        {
                            c.Estado = true;
                            cmdContrato.Parameters.AddWithValue("@FechaInicio", c.FechaInicio);
                            cmdContrato.Parameters.AddWithValue("@FechaFin", c.FechaFin);
                            cmdContrato.Parameters.AddWithValue("@MontoMensual", c.MontoMensual);
                            cmdContrato.Parameters.AddWithValue("@IdInquilino", c.Habitante.IdInquilino);
                            cmdContrato.Parameters.AddWithValue("@IdInmueble", c.Propiedad.IdInmueble);
                            cmdContrato.Parameters.AddWithValue("@QuienCreo", c.QuienCreo);
                            cmdContrato.Parameters.AddWithValue("@QuienElimino", DBNull.Value);
                            cmdContrato.Parameters.AddWithValue("@Estado", c.Estado);
                            res = Convert.ToInt32(cmdContrato.ExecuteScalar()); // IdContrato generado
                        }
                        // Inserta Reserva usando IdContrato
                        string queryReserva = @"
                            INSERT INTO reserva (IdInmueble, IdContrato, FechaDesde, FechaHasta, Estado)
                            VALUES (@IdInmueble, @IdContrato, @FechaDesde, @FechaHasta, @Estado)";
                        using (var cmdReserva = new MySqlCommand(queryReserva, connection, transaction))
                        {
                            cmdReserva.Parameters.AddWithValue("@IdInmueble", c.Propiedad.IdInmueble);
                            cmdReserva.Parameters.AddWithValue("@IdContrato", res);
                            cmdReserva.Parameters.AddWithValue("@FechaDesde", c.FechaInicio);
                            cmdReserva.Parameters.AddWithValue("@FechaHasta", c.FechaFin);
                            cmdReserva.Parameters.AddWithValue("@Estado", true);
                            cmdReserva.ExecuteNonQuery();
                        }
                        // Commit de la transacción
                        transaction.Commit();
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
            return res;
        }

        public IList<Contrato> buscarAvanzadoContratos(DateTime? fechaDesde, DateTime? fechaHasta, int? idInquilino)
{
    var lista = new List<Contrato>();
    using (var connection = new MySqlConnection(connectionString))
    {
        string query = @"
            SELECT c.IdContrato, c.FechaInicio, c.FechaFin, c.MontoMensual, c.Estado,
                   i.IdInquilino, i.Nombre, i.Apellido
            FROM Contrato c
            JOIN Inquilino i ON c.IdInquilino = i.IdInquilino
            WHERE (@fechaDesde IS NULL OR c.FechaInicio >= @fechaDesde)
              AND (@fechaHasta IS NULL OR c.FechaFin <= @fechaHasta)
              AND (@idInquilino IS NULL OR i.IdInquilino = @idInquilino)
            ORDER BY c.FechaInicio DESC";

        using (var command = new MySqlCommand(query, connection))
        {
            command.Parameters.AddWithValue("@fechaDesde", (object)fechaDesde ?? DBNull.Value);
            command.Parameters.AddWithValue("@fechaHasta", (object)fechaHasta ?? DBNull.Value);
            command.Parameters.AddWithValue("@idInquilino", (object)idInquilino ?? DBNull.Value);

            connection.Open();
            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    var contrato = new Contrato
                    {
                        IdContrato = reader.GetInt32("IdContrato"),
                        FechaInicio = reader.GetDateTime("FechaInicio"),
                        FechaFin = reader.GetDateTime("FechaFin"),
                        MontoMensual = reader.GetInt32("MontoMensual"),
                        Estado = reader.GetBoolean("Estado"),
                        Habitante = new InquilinoDto
                        {
                            IdInquilino = reader.GetInt32("IdInquilino"),
                            Nombre = reader.GetString("Nombre"),
                            Apellido = reader.GetString("Apellido")
                        }
                    };
                    lista.Add(contrato);
                }
            }
        }
    }
    return lista;
}

        

        public IList<Contrato> obtenerPaginados(int offset, int limit)
        {
            IList<Contrato> res = new List<Contrato>();
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
             String query = @"
            SELECT 
                c.IdContrato, c.FechaInicio, c.FechaFin, c.MontoMensual, c.Estado,c.QuienCreo,c.QuienElimino,
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
                            QuienCreo = reader.IsDBNull(reader.GetOrdinal(nameof(Contrato.QuienCreo)))
                                ? (int?)null
                                : reader.GetInt32(reader.GetOrdinal(nameof(Contrato.QuienCreo))),

                            QuienElimino = reader.IsDBNull(reader.GetOrdinal(nameof(Contrato.QuienElimino)))
                                ? (int?)null
                                : reader.GetInt32(reader.GetOrdinal(nameof(Contrato.QuienElimino))),
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

        public int DarDeBaja(int IdContrato, int QuienElimino)
        {
            int res = -1;
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                string query = $@"UPDATE contrato 
                    SET Estado = 0, QuienElimino = @QuienElimino
                    WHERE IdContrato = @IdContrato";

                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@IdContrato", IdContrato);
                    command.Parameters.AddWithValue("@QuienElimino", QuienElimino);
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

        public List<Contrato> contratoPorInmueble(int IdInmueble)
        {
            Console.WriteLine($"Buscando contratos del inmueble {IdInmueble}"); 
            var lista = new List<Contrato>();
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                string query = @"SELECT c.IdContrato, c.IdInmueble, c.fechaInicio, c.fechaFin, c.MontoMensual, c.Estado, i.Nombre AS InquilinoNombre,
                i.Apellido AS InquilinoApellido 
                FROM contrato c
                JOIN inquilino i ON c.IdInquilino = i.IdInquilino
                WHERE IdInmueble = @IdInmueble";

                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@IdInmueble", IdInmueble);
                    connection.Open();

                    using (var reader = command.ExecuteReader())
                    {
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
                                //IdInquilino = reader.GetInt32("IdInquilino"),
                                Nombre = reader.GetString("InquilinoNombre"),
                                Apellido = reader.GetString("InquilinoApellido"),
                                //Dni = reader.GetString("InquilinoDni")
                            },

                                Propiedad = new InmuebleDto
                                {
                                    IdInmueble = reader.GetInt32("IdInmueble"),
                                    //Direccion = reader.GetString("InmuebleDireccion"),
                                    //IdTipo = reader.GetInt32("InmuebleTipo")
                                }
                            };
                            lista.Add(c);
                        }
                    }
                }
                return lista;
            }
        }

        public IList<Contrato> ListarContratos()
        {
            IList<Contrato> res = new List<Contrato>();
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                string query = @"
                    SELECT 
                        c.IdContrato, c.FechaInicio, c.FechaFin, c.MontoMensual, c.Estado, c.QuienCreo, c.QuienElimino,
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
                            QuienCreo = reader.IsDBNull(reader.GetOrdinal(nameof(Contrato.QuienCreo)))
                                ? (int?)null
                                : reader.GetInt32(reader.GetOrdinal(nameof(Contrato.QuienCreo))),

                            QuienElimino = reader.IsDBNull(reader.GetOrdinal(nameof(Contrato.QuienElimino)))
                                ? (int?)null
                                : reader.GetInt32(reader.GetOrdinal(nameof(Contrato.QuienElimino))),
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
        public bool TerminarContrato(int idContrato, int idUsuario)
        {
            bool resultado = false;
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        DarDeBaja(idContrato, idUsuario);
                        string query = @"UPDATE reserva SET Estado = 0 WHERE IdContrato = @IdContrato";
                        using (var cmdReserva = new MySqlCommand(query, connection, transaction))
                        {
                            cmdReserva.Parameters.AddWithValue("@IdContrato", idContrato);
                            cmdReserva.ExecuteNonQuery();
                        }
                        transaction.Commit();
                        resultado = true;
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
            return resultado;
        }
    }
}

using MySql.Data.MySqlClient;

namespace INMOBILIARIA_JosiasTolaba.Models
{
    public class RepositorioPago : RepositorioBase, IRepositorioPago
    {
        public RepositorioPago(IConfiguration configuration) : base(configuration)
        {
        }

       public IList<Pago> buscarAvanzado(DateTime? fechaDesde, DateTime? fechaHasta, string filtroInquilino)
{
    var lista = new List<Pago>();
    using (MySqlConnection connection = new MySqlConnection(connectionString))
    {
        string query = @"
            SELECT p.IdPago, p.FechaPago, p.Monto, p.Mes,
                   p.NumeroPago, p.Concepto, p.Estado, p.IdContrato,
                   c.IdContrato, i.IdInquilino, i.Nombre, i.Apellido
            FROM Pago p
            JOIN Contrato c ON p.IdContrato = c.IdContrato
            JOIN Inquilino i ON c.IdInquilino = i.IdInquilino
            WHERE (@fechaDesde IS NULL OR p.FechaPago >= @fechaDesde)
              AND (@fechaHasta IS NULL OR p.FechaPago <= @fechaHasta)
              AND (@filtroInquilino IS NULL OR i.Nombre LIKE @filtroInquilino OR i.Apellido LIKE @filtroInquilino OR i.Dni LIKE @filtroInquilino)
            ORDER BY p.FechaPago DESC";

        using (var command = new MySqlCommand(query, connection))
        {
            command.Parameters.AddWithValue("@fechaDesde", (object)fechaDesde ?? DBNull.Value);
            command.Parameters.AddWithValue("@fechaHasta", (object)fechaHasta ?? DBNull.Value);
            command.Parameters.AddWithValue("@filtroInquilino", string.IsNullOrWhiteSpace(filtroInquilino) ? DBNull.Value : "%" + filtroInquilino + "%");

            connection.Open();
            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    var pago = new Pago
                    {
                        IdPago = reader.GetInt32("IdPago"),
                        FechaPago = reader.GetDateTime("FechaPago"),
                        Monto = reader.GetDecimal("Monto"),
                        Mes = reader.GetDateTime("Mes"),
                        NumeroPago = reader.GetString("NumeroPago"),
                        Concepto = reader.GetString("Concepto"),
                        Estado = reader.GetBoolean("Estado"),
                        IdContrato = reader.GetInt32("IdContrato"),
                        Contrato = new ContratoDTO
                        {
                            IdContrato = reader.GetInt32("IdContrato"),
                            Habitante = new InquilinoDto
                            {
                                IdInquilino = reader.GetInt32("IdInquilino"),
                                Nombre = reader.GetString("Nombre"),
                                Apellido = reader.GetString("Apellido")
                            }
                        }
                    };
                    lista.Add(pago);
                }
            }
        }
    }
    return lista;
}


        public IList<Pago> obtenerPaginados(int offset, int limit)
        {
            IList<Pago> res = new List<Pago>();
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                string query = @"SELECT * FROM pago 
                                WHERE Estado = true
                                ORDER BY IdPago
                                LIMIT @limit OFFSET @offset;";
                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@limit", limit);
                    command.Parameters.AddWithValue("@offset", offset);
                    connection.Open();
                    var reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                       Pago p = new Pago
                        {
                            IdPago = reader.GetInt32(nameof(Pago.IdPago)),
                            FechaPago = reader.GetDateTime(nameof(Pago.FechaPago)),
                            Monto = reader.GetDecimal(nameof(Pago.Monto)),
                            Mes = reader.GetDateTime(nameof(Pago.Mes)),
                            NumeroPago = reader.GetString(nameof(Pago.NumeroPago)),
                            Concepto = reader.GetString(nameof(Pago.Concepto)),
                            IdContrato = reader.GetInt32(nameof(Pago.IdContrato)),
                            //QuienCreo = reader.GetInt32(nameof(Pago.QuienCreo)),
                            //QuienElimino = reader.GetInt32(nameof(Pago.QuienElimino)),
                            Estado = reader.GetBoolean(nameof(Pago.Estado))
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
        public int Alta(Pago p)
        {
            int res = -1;
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                string query = $@"INSERT INTO pago (
                    {nameof(Pago.FechaPago)},
                    {nameof(Pago.Monto)},
                    {nameof(Pago.Mes)},
                    {nameof(Pago.NumeroPago)},
                    {nameof(Pago.Concepto)},
                    {nameof(Pago.IdContrato)},
                    {nameof(Pago.QuienCreo)},
                    {nameof(Pago.QuienElimino)},
                    {nameof(Pago.Estado)})
                    VALUES (@FechaPago, @Monto, @Mes, @NumeroPago, @Concepto, @IdContrato, @QuienCreo, @QuienElimino, @Estado);
                    SELECT LAST_INSERT_ID();";

                using (var command = new MySqlCommand(query, connection))
                {
                    p.Estado = true;
                    command.Parameters.AddWithValue("@FechaPago", p.FechaPago);
                    command.Parameters.AddWithValue("@Monto", p.Monto);
                    command.Parameters.AddWithValue("@Mes", p.Mes);
                    command.Parameters.AddWithValue("@NumeroPago", p.NumeroPago);
                    command.Parameters.AddWithValue("@Concepto", p.Concepto);
                    command.Parameters.AddWithValue("@IdContrato", p.IdContrato);
                    command.Parameters.AddWithValue("@QuienCreo", p.QuienCreo);
                    command.Parameters.AddWithValue("@QuienElimino", DBNull.Value);//p.QuienElimino);
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
                string query = $@"DELETE FROM pago WHERE {nameof(Pago.IdPago)}=@IdPago";
                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@IdPago", id);
                    connection.Open();
                    res = command.ExecuteNonQuery();
                    connection.Close();
                }
            }
            return res;
        }
        public int Modificacion(Pago p)
        {
            int res = -1;
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                string query = $@"UPDATE pago SET
                    {nameof(Pago.FechaPago)}=@FechaPago,
                    {nameof(Pago.Monto)}=@Monto,
                    {nameof(Pago.Mes)}=@Mes,
                    {nameof(Pago.NumeroPago)}=@NumeroPago,
                    {nameof(Pago.Concepto)}=@Concepto,
                    {nameof(Pago.IdContrato)}=@IdContrato
                    WHERE {nameof(Pago.IdPago)}=@IdPago";
                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@IdPago", p.IdPago);
                    command.Parameters.AddWithValue("@FechaPago", p.FechaPago);
                    command.Parameters.AddWithValue("@Monto", p.Monto);
                    command.Parameters.AddWithValue("@Mes", p.Mes);
                    command.Parameters.AddWithValue("@NumeroPago", p.NumeroPago);
                    command.Parameters.AddWithValue("@Concepto", p.Concepto);
                    command.Parameters.AddWithValue("@IdContrato", p.IdContrato);
                    //command.Parameters.AddWithValue("@QuienCreo", p.QuienCreo);
                    //command.Parameters.AddWithValue("@QuienElimino", p.QuienElimino);
                    //{nameof(Pago.QuienCreo)}=@QuienCreo,
                    //{nameof(Pago.QuienElimino)}=@QuienElimino,
                    connection.Open();
                    res = command.ExecuteNonQuery();
                    connection.Close();
                }
            }
            return res;
        }

        public List<Pago> pagoPorContrato(int IdContrato)
{
    Console.WriteLine($"Buscando pagos del contrato {IdContrato}");
    var lista = new List<Pago>();

    using (var connection = new MySqlConnection(connectionString))
    {
        string query = @"
            SELECT p.IdPago, p.FechaPago, p.Monto, p.Mes, p.NumeroPago, p.Concepto, p.IdContrato, p.Estado,
                c.FechaInicio AS ContratoFechaInicio,
                c.FechaFin AS ContratoFechaFin,
                c.MontoMensual AS ContratoMonto,
                i.Nombre AS InquilinoNombre,
                i.Apellido AS InquilinoApellido
            FROM pago p
            INNER JOIN contrato c ON p.IdContrato = c.IdContrato
            INNER JOIN inquilino i ON c.IdInquilino = i.IdInquilino
            WHERE p.IdContrato = @IdContrato
            ORDER BY p.FechaPago ASC;
        ";

        using (var command = new MySqlCommand(query, connection))
        {
            command.Parameters.AddWithValue("@IdContrato", IdContrato);
            connection.Open();

            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    var pago = new Pago
                    {
                        IdPago = reader.GetInt32("IdPago"),
                        FechaPago = reader.GetDateTime("FechaPago"),
                        Monto = reader.GetDecimal("Monto"),
                        Mes = reader.GetDateTime("Mes"),
                        NumeroPago = reader.GetString("NumeroPago"),
                        Concepto = reader.GetString("Concepto"),
                        IdContrato = reader.GetInt32("IdContrato"),
                        Estado = reader.GetBoolean("Estado"),

                        Contrato = new ContratoDTO
                        {
                            IdContrato = reader.GetInt32("IdContrato"),

                            Habitante = new InquilinoDto
                            {
                                Nombre = reader.GetString("InquilinoNombre"),
                                Apellido = reader.GetString("InquilinoApellido")
                            }
                        }
                    };

                    lista.Add(pago);
                }
            }
        }
    }

    return lista;
}


        public IList<Pago> ListarPagos()
        {
            IList<Pago> res = new List<Pago>();
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                string query = $@"SELECT * FROM pago WHERE Estado = true;";

                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    connection.Open();
                    var reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        Pago p = new Pago
                        {
                            IdPago = reader.GetInt32(nameof(Pago.IdPago)),
                            FechaPago = reader.GetDateTime(nameof(Pago.FechaPago)),
                            Monto = reader.GetDecimal(nameof(Pago.Monto)),
                            Mes = reader.GetDateTime(nameof(Pago.Mes)),
                            NumeroPago = reader.GetString(nameof(Pago.NumeroPago)),
                            Concepto = reader.GetString(nameof(Pago.Concepto)),
                            IdContrato = reader.GetInt32(nameof(Pago.IdContrato)),
                            QuienCreo = reader.GetInt32(nameof(Pago.QuienCreo)),
                            QuienElimino = reader.GetInt32(nameof(Pago.QuienElimino)),
                            Estado = reader.GetBoolean(nameof(Pago.Estado))
                        };
                        res.Add(p);
                    }
                    connection.Close();
                }
            }
            return res;
        }

        public Pago PagoId(int IdPago)
        {
            Pago? p = null;
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                string query = $@"SELECT * FROM pago WHERE {nameof(Pago.IdPago)}=@IdPago";
                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@IdPago", IdPago);
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            p = new Pago
                            {
                                IdPago = reader.GetInt32(nameof(Pago.IdPago)),
                                FechaPago = reader.GetDateTime(nameof(Pago.FechaPago)),
                                Monto = reader.GetDecimal(nameof(Pago.Monto)),
                                Mes = reader.GetDateTime(nameof(Pago.Mes)),
                                NumeroPago = reader.GetString(nameof(Pago.NumeroPago)),
                                Concepto = reader.GetString(nameof(Pago.Concepto)),
                                IdContrato = reader.GetInt32(nameof(Pago.IdContrato)),
                                //QuienCreo = reader.GetInt32(nameof(Pago.QuienCreo)),
                                //QuienElimino = reader.GetInt32(nameof(Pago.QuienElimino)),
                                Estado = reader.GetBoolean(nameof(Pago.Estado))
                            };
                        }
                    }
                    connection.Close();
                }
            }
            return p;
        }
        public int DarDeBaja(int IdPago, int QuienElimino)
        {
            int res = -1;
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                string query = $@"UPDATE pago 
                    SET Estado = 0 , QuienElimino = @QuienElimino
                    WHERE {nameof(IdPago)}=@IdPago";
                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@IdPago", IdPago);
                    command.Parameters.AddWithValue("@QuienElimino", QuienElimino);
                    connection.Open();
                    res = command.ExecuteNonQuery();
                }
            }
            return res;
        }
    }
}
using MySql.Data.MySqlClient;

namespace INMOBILIARIA_JosiasTolaba.Models
{
    public class RepositorioPago : RepositorioBase, IRepositorioPago
    {
        public RepositorioPago(IConfiguration configuration) : base(configuration)
        {
        }
        public List<Pago> buscar(string dato)
        {
            var lista = new List<Pago>();
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                string query = @"SELECT * 
                                FROM pago
                                WHERE Monto LIKE @dato
                                OR FechaPago LIKE @dato
                                OR Mes LIKE @dato
                                OR NumeroPago LIKE @dato
                                LIMIT 10";
            using (var command = new MySqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@dato", "%" + dato + "%");
                connection.Open();
            using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var i = new Pago
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
                        lista.Add(i);
                    }
                }
            }
            }
            return lista;
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
                    command.Parameters.AddWithValue("@QuienCreo", DBNull.Value);//p.QuienCreo);
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
                            //QuienCreo = reader.GetInt32(nameof(Pago.QuienCreo)),
                            //QuienElimino = reader.GetInt32(nameof(Pago.QuienElimino)),
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
        public int DarDeBaja(int IdPago)
        {
            int res = -1;
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                string query = $@"UPDATE pago 
                    SET Estado = 0 
                    WHERE {nameof(IdPago)}=@IdPago";
                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@IdPago", IdPago);
                    connection.Open();
                    res = command.ExecuteNonQuery();
                }
            }
            return res;
        }
    }
}
using MySql.Data.MySqlClient;

namespace INMOBILIARIA_JosiasTolaba.Models
{
    public class RepositorioInmueble : RepositorioBase, IRepositorioInmueble
    {
        public RepositorioInmueble(IConfiguration configuration) : base(configuration)
        {
        }
        public int Alta(Inmueble i)
        {
            int res = -1;
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                string query = $@"INSERT INTO inmueble (
                {nameof(Inmueble.Direccion)},
                {nameof(Inmueble.Tipo)},
                {nameof(Inmueble.IdPropietario)},
                {nameof(Inmueble.Uso)},
                {nameof(Inmueble.Latitud)},
                {nameof(Inmueble.Longitud)},
                {nameof(Inmueble.Precio)},
                {nameof(Inmueble.Ambiente)},
                {nameof(Inmueble.Estado)})
                VALUES (@Direccion, @Tipo, @IdPropietario, @Uso, @Latitud, @Longitud, @Precio, @Ambiente, @Estado);
                SELECT LAST_INSERT_ID();";
                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    i.Estado = true;
                    command.Parameters.AddWithValue("@Direccion", i.Direccion);
                    command.Parameters.AddWithValue("@Tipo", i.Tipo);
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
                {nameof(Inmueble.Tipo)}=@Tipo,
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
                    command.Parameters.AddWithValue("@Tipo", i.Tipo);
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
                            Tipo = reader.GetString(nameof(Inmueble.Tipo)),
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
                string query = $@"SELECT i.*,p.* FROM inmueble i
                                JOIN propietario p ON i.IdPropietario = p.IdPropietario
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
                            Tipo = reader.GetString(nameof(Inmueble.Tipo)),
                            IdPropietario = reader.GetInt32(nameof(Inmueble.IdPropietario)),
                            Uso = Enum.Parse<Inmueble.TipoUso>(reader.GetString(nameof(Inmueble.Uso))),
                            Latitud = reader.GetDecimal(nameof(Inmueble.Latitud)),
                            Longitud = reader.GetDecimal(nameof(Inmueble.Longitud)),
                            Precio = reader.GetDecimal(nameof(Inmueble.Precio)),
                            Ambiente = reader.GetInt32(nameof(Inmueble.Ambiente)),
                            Estado = reader.GetBoolean(nameof(Inmueble.Estado))
                        };
                    }
                    connection.Close();
                }
            }
            return res;
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
    }
}
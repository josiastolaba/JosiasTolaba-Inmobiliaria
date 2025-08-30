using MySql.Data.MySqlClient;

namespace INMOBILIARIA_JosiasTolaba.Models.DTOs
{
    public class RepositorioContrato : RepositorioBase, IRepositorioContrato
    {
        public RepositorioContrato(IConfiguration configuration) : base(configuration)
        {

        }

        public Contrato DarDeAlta()
        {
            return new Contrato();
        }

        public int Insertar(Contrato p)
        {
            int IdGenerado = 0;

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                string solicitud = $@"INSERT INTO Contratos (
                                {nameof(Contrato.FechaInicio)},
                                {nameof(Contrato.FechaFin)},
                                {nameof(Contrato.MontoMensual)},
                                {nameof(Contrato.Habitante)}),
                                {nameof(Contrato.Propiedad)},
                                VALUES (@FechaInicio, @FechaFin, @MontoMensual, @IdInmueble, @IdInquilino);
                                SELECT LAS_INSERT_ID();";

                using (var command = new MySqlCommand(solicitud, connection))
                {
                    p.Estado = true;
                    command.Parameters.AddWithValue("@FechaInicio", p.FechaInicio);
                    command.Parameters.AddWithValue("@FechaInicio", p.FechaFin);
                    command.Parameters.AddWithValue("@FechaInicio", p.MontoMensual);
                    command.Parameters.AddWithValue("@FechaInicio", p.Habitante);
                    command.Parameters.AddWithValue("@FechaInicio", p.FechaInicio);

                    connection.Open();
                    IdGenerado = Convert.ToInt32(command.ExecuteScalar());
                }
            }
            return IdGenerado;
        }

        public int DarDeBaja(int IdContrato)
        {
            int res = -1;
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                string query = $@"UPADTE contrato
                set Estado = 0
                WHERE {nameof(IdContrato)}=@IdContrato";

                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@IdContrato", IdContrato);
                    res = command.ExecuteNonQuery();
                }
            }
            return res;
        }

        public int Modificar(Contrato c)
        {
            int res = -1;
            using (MySqlConnection connection = new MySqlConnection(connectionString)) {
                string query = $@"UPDATE contrato SET
                {nameof(Contrato.FechaInicio)} =@FechaInicio,
                {nameof(Contrato.FechaFin)}=@FechaFin,
                {nameof(Contrato.MontoMensual)}=@MontoMensual";

                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@FechaInicio", c.FechaInicio);
                    command.Parameters.AddWithValue("FechaFin", c.FechaFin);
                    command.Parameters.AddWithValue("MontoMensual", c.MontoMensual);
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
                string query = $@"SELECT c.IdContrato, c.FechaInicio, c.FechaFin, c.MontoMensual, c.Estado,
                   i.IdInquilino, i.Nombre AS inquilino.Nombre, i.Apellido AS inquilino.Apellido,
                   m.IdInmueble, m.Direccion AS inmueble.Direccion
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
                            IdContrato = reader.GetInt32(nameof(Contrato.IdContrato)),
                            FechaInicio = reader.GetDateTime(nameof(Contrato.FechaInicio)),
                            FechaFin = reader.GetDateTime(nameof(Contrato.FechaFin)),
                            MontoMensual = reader.GetInt32(nameof(Contrato.MontoMensual)),
                            Estado = reader.GetBoolean(nameof(Contrato.Estado)),
                            Habitante = new InquilinoDto
                            {
                                IdInquilino = reader.GetInt32(nameof(Inquilino.IdInquilino)),
                                Nombre = reader.GetString(nameof(Inquilino.Nombre)),
                                Apellido = reader.GetString(nameof(Inquilino.Apellido)),
                                Dni = reader.GetString(nameof(Inquilino.Dni))

                            },

                            Propiedad = new InmuebleDto
                            {
                                IdInmueble = reader.GetInt32("IdInmueble"),
                                Direccion = reader.GetString("InmuebleDireccion"),
                                Tipo = reader.GetString("InmuebleTipo")
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
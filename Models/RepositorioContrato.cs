using MySql.Data.MySqlClient;

namespace INMOBILIARIA_JosiasTolaba.Models
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
    }
}
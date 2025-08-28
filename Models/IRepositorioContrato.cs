namespace INMOBILIARIA_JosiasTolaba.Models
{
    public interface IRepositorioContrato : IRepositorio<Contrato>
    {
        Contrato DarDeAlta();
        int DarDeBaja(int IdContrato);
        IList<Contrato> listarContratos();

        IList<Contrato> buscarPorFechas(DateTime FechaInicio, DateTime FechaFin);

        IList<Contrato> buscarPorInquilino(int IdInquilino);
    }
}
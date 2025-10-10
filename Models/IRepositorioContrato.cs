namespace INMOBILIARIA_JosiasTolaba.Models
{
    public interface IRepositorioContrato : IRepositorio<Contrato>
    {
        List<Contrato> buscar(string dato);
        int DarDeBaja(int IdContrato);
        IList<Contrato> ListarContratos();
        Contrato IdContrato(int IdContrato);

        List<Contrato> contratosPorInquilino(int IdInquilino);

        IList<Contrato> obtenerPaginados(int offset, int limit);

        int contar();
    }
}
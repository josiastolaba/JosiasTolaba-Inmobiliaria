namespace INMOBILIARIA_JosiasTolaba.Models
{
    public interface IRepositorioContrato : IRepositorio<Contrato>
    {
        List<Contrato> buscar(string dato);
        int DarDeBaja(int IdContrato, int QuienElimino);
        IList<Contrato> ListarContratos();
        Contrato IdContrato(int IdContrato);

        List<Contrato> contratosPorInquilino(int IdInquilino);

        List<Contrato> contratoPorInmueble(int IdInmueble);

        IList<Contrato> obtenerPaginados(int offset, int limit);

        int contar();
    }
}
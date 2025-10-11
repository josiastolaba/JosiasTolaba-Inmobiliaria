namespace INMOBILIARIA_JosiasTolaba.Models
{
    public interface IRepositorioPago : IRepositorio<Pago>
    {
       // List<Pago> buscar(string dato);
        int DarDeBaja(int IdPago, int QuienElimino);
        IList<Pago> ListarPagos();
        Pago PagoId(int IdPago);

        List<Pago> pagoPorContrato(int IdContrato);

        IList<Pago> buscarAvanzado(DateTime? fechaDesde, DateTime? fechaHasta, string filtroInquilino);

        IList<Pago> obtenerPaginados(int offset, int limit);

        int contar();
    }
}
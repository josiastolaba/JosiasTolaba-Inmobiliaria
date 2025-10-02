namespace INMOBILIARIA_JosiasTolaba.Models
{
    public interface IRepositorioPago : IRepositorio<Pago>
    {
        List<Pago> buscar(string dato);
        int DarDeBaja(int IdPago);
        IList<Pago> ListarPagos();
        Pago PagoId(int IdPago);
    }
}
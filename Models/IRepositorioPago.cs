namespace INMOBILIARIA_JosiasTolaba.Models
{
    public interface IRepositorioPago : IRepositorio<Pago>
    {
        int DarDeBaja(int IdPago);
        IList<Pago> ListarPagos();
        Pago PagoId(int IdPago);
    }
}
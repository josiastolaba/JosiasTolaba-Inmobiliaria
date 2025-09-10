namespace INMOBILIARIA_JosiasTolaba.Models
{
    public interface IRepositorioPago : IRepositorio<Pago>
    {
        IList<Pago> ListarPagos();
        Pago PagoId(int IdPago);
        int DarDeBaja(int IdPago);
    }
}
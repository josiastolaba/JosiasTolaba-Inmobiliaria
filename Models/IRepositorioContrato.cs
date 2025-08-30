namespace INMOBILIARIA_JosiasTolaba.Models
{
    public interface IRepositorioContrato : IRepositorio<Contrato>
    {
        int DarDeBaja(int IdContrato);
        IList<Contrato> ListarContratos();

        Contrato IdContrato(int IdContrato);
    }
}
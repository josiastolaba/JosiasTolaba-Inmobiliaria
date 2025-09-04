namespace INMOBILIARIA_JosiasTolaba.Models
{
    public interface IRepositorioContrato : IRepositorio<Contrato>
    {

        IList<Contrato> ListarContratos();

        Contrato IdContrato(int IdContrato);
    }
}
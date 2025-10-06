namespace INMOBILIARIA_JosiasTolaba.Models
{
    public interface IRepositorioTipoInmueble : IRepositorio<TipoInmueble>
    {
        int DarDeBaja(int IdTipo);
        IList<TipoInmueble> ListarTipoInmueble();
        TipoInmueble TipoInmuebleId(int IdTipo);
        List<TipoInmueble> buscar(String datos);
    }
}
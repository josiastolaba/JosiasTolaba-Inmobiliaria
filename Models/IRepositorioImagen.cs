namespace INMOBILIARIA_JosiasTolaba.Models
{
    public interface IRepositorioImagen : IRepositorio<Imagen>
    {
        IList<Imagen> BuscarPorInmueble(int IdInmueble);
    }
}
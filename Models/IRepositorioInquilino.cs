using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace INMOBILIARIA_JosiasTolaba.Models
{
	public interface IRepositorioInquilino : IRepositorio<Inquilino>
	{
		IList<Inquilino> ListarInquilinos();
		Inquilino InquilinoId(int IdInquilino);
	}
}
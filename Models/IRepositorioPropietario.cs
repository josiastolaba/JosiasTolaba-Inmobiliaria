using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace INMOBILIARIA_JosiasTolaba.Models
{
	public interface IRepositorioPropietario : IRepositorio<Propietario>
	{
		IList<Propietario> ListarPropietarios();
		Propietario PropietarioId(int IdPropietario);
		
	}
}
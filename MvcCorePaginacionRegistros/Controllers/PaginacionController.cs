using Microsoft.AspNetCore.Mvc;
using MvcCorePaginacionRegistros.Models;
using MvcCorePaginacionRegistros.Repositories;

namespace MvcCorePaginacionRegistros.Controllers
{
    public class PaginacionController : Controller
    {
        private RepositoryHospital repo;

        public PaginacionController(RepositoryHospital repo)
        {
            this.repo = repo;
        }

        public async Task<IActionResult> RegistroVistaDepartamento(int? posicion)
        {
            if(posicion == null)
            {
                posicion = 1;
            }
            int numRegistros = await this.repo.GetNumeroRegistrosVistaDepartamentosAsync();
            //PRIMERO = 1
            //ULTIMO = 4
            //ANTERIOR = posicion - 1
            //SIGUIENTE = posicion + 1
            int siguiente = posicion.Value + 1;
            if(siguiente > numRegistros)
            {
                siguiente = numRegistros;
            }
            int anterior = posicion.Value - 1;
            if(anterior < 1)
            {
                anterior = 1;
            }
            ViewData["ULTIMO"] = numRegistros;
            ViewData["SIGUIENTE"] = siguiente;
            ViewData["ANTERIOR"] = anterior;
            VistaDepartamento departamento = await this.repo.GetVistaDepartamentoAsync(posicion.Value);
            return View(departamento);
        }

        public async Task<IActionResult> GrupoVistaDepartamentos(int? posicion)
        {
            if(posicion == null)
            {
                posicion = 1;
            }
            //LO SIGUIENTE SERA QUE DEBEMOS DIBUJAR LOS NUMEROS
            //DE PAGINA EN LOS LINKS
            //<a href='grupodepts?posicion=1'>Pagina 1</a>
            //<a href='grupodepts?posicion=3'>Pagina 2</a>
            //<a href='grupodepts?posicion=5'>Pagina 3</a>
            //NECESITAMOS UNA VARIABLE PARA EL NUMERO DE PAGINA
            //VOY A REALIZAR EL DIBUJO DESDE AQUI, NO DESDE RAZOR
            int numRegistros =
                await this.repo.GetNumeroRegistrosVistaDepartamentosAsync();
            ViewData["NUMREGISTROS"] = numRegistros;
            List<VistaDepartamento> departamentos = await this.repo.GetGrupoVistaDepartamentosAsync(posicion.Value);
            return View(departamentos);
        }

        public IActionResult Index()
        {
            return View();
        }
    }
}

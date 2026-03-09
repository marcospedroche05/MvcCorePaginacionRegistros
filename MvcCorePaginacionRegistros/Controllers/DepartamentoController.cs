using Microsoft.AspNetCore.Mvc;
using MvcCorePaginacionRegistros.Models;
using MvcCorePaginacionRegistros.Repositories;

namespace MvcCorePaginacionRegistros.Controllers
{
    public class DepartamentoController : Controller
    {
        private RepositoryHospital repo;

        public DepartamentoController(RepositoryHospital repo)
        {
            this.repo = repo;
        }

        public async Task<IActionResult> Details(int iddepartamento, int? posicion)
        {
            if (posicion == null)
            {
                posicion = 1;
            }
            int numRegistros = await this.repo.GetRegistrosEmpleadosDepartamentoAsync(iddepartamento);
            //PRIMERO = 1
            //ULTIMO = 4
            //ANTERIOR = posicion - 1
            //SIGUIENTE = posicion + 1
            int siguiente = posicion.Value + 1;
            if (siguiente > numRegistros)
            {
                siguiente = numRegistros;
            }
            int anterior = posicion.Value - 1;
            if (anterior < 1)
            {
                anterior = 1;
            }
            ViewData["ULTIMO"] = numRegistros;
            ViewData["SIGUIENTE"] = siguiente;
            ViewData["ANTERIOR"] = anterior;
            Empleado empleado = await this.repo.GetEmpleadoDepartamentoAsync(posicion.Value, iddepartamento);
            ViewData["EMPLEADO"] = empleado;
            ViewData["POSICION"] = posicion;
            Departamento dept = await this.repo.FindDepartamentoAsync(iddepartamento);
            return View(dept);
        }

        public IActionResult Index()
        {
            return View();
        }
    }
}

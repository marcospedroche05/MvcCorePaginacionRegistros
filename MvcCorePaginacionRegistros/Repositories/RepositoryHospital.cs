using Microsoft.EntityFrameworkCore;
using MvcCorePaginacionRegistros.Data;
using MvcCorePaginacionRegistros.Models;
using System.Diagnostics.Metrics;

#region VIEWS

//ALTER VIEW V_DEPARTAMENTOS_INDIVIDUAL
//AS
//	select cast(ROW_NUMBER() OVER (ORDER BY DEPT_NO) as int) AS POSICION
//	, DEPT_NO, DNOMBRE, LOC FROM DEPT
//GO

#endregion

namespace MvcCorePaginacionRegistros.Repositories
{
    public class RepositoryHospital
    {
        private HospitalContext context;
        public RepositoryHospital(HospitalContext context)
        {
            this.context = context;
        }

        public async Task<int> GetNumeroRegistrosVistaDepartamentosAsync()
        {
            return await this.context.VistaDepartamentos.CountAsync();
        }
        public async Task<VistaDepartamento>
            GetVistaDepartamentoAsync(int posicion)
        {
            VistaDepartamento departamento = 
                await this.context.VistaDepartamentos.Where(x => x.Posicion == posicion).FirstOrDefaultAsync();
            return departamento;
        }

        public async Task<List<VistaDepartamento>> GetGrupoVistaDepartamentosAsync(int posicion)
        {
            var consulta = from datos in this.context.VistaDepartamentos
                           where datos.Posicion >= posicion && datos.Posicion < (posicion + 2)
                           select datos;
            return await consulta.ToListAsync();
        }
    }
}

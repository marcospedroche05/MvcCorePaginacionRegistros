using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Data.SqlClient;
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

//CREATE VIEW V_GRUPO_EMPLEADOS
//AS
//	SELECT CAST(ROW_NUMBER() OVER (ORDER BY APELLIDO) AS int) AS POSICION
//	, EMP_NO, APELLIDO, OFICIO, SALARIO, DEPT_NO FROM EMP;
//GO

#endregion

#region STORED PROCEDURES

//CREATE PROCEDURE SP_GRUPO_EMPLEADOS
//(@posicion int)
//AS
//	SELECT EMP_NO, APELLIDO, OFICIO, SALARIO, DEPT_NO FROM V_GRUPO_EMPLEADOS
//	WHERE POSICION >= @posicion AND POSICION < (@posicion + 3)
//GO

//CREATE PROCEDURE SP_GRUPO_EMPLEADOS_OFICIO
//(@posicion int, @oficio NVARCHAR(50))
//AS
//	SELECT EMP_NO, APELLIDO, OFICIO, SALARIO, DEPT_NO FROM
//	(SELECT CAST(ROW_NUMBER() OVER (ORDER BY APELLIDO) AS int) 
//	AS POSICION, EMP_NO, APELLIDO, OFICIO, SALARIO, DEPT_NO 
//	FROM EMP
//	WHERE OFICIO = @oficio) QUERY 
//	WHERE ( QUERY.POSICION >= @posicion AND QUERY.POSICION < (@posicion + 3))
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

        public async Task<List<Departamento>> GetGrupoDepartamentosAsync(int posicion)
        {
            string sql = "SP_GRUPO_DEPARTAMENTOS @posicion";
            SqlParameter pamPosicion = new SqlParameter("@posicion", posicion);
            var consulta = this.context.Departamentos.FromSqlRaw(sql, pamPosicion);
            return await consulta.ToListAsync();
        }

        public async Task<int> GetEmpleadosCountAsync()
        {
            return await this.context.Empleados.CountAsync();
        }

        public async Task<List<Empleado>> GetGrupoEmpleadosAsync(int posicion)
        {
            string sql = "SP_GRUPO_EMPLEADOS @posicion";
            SqlParameter pamPosicion = new SqlParameter("@posicion", posicion);
            var consulta = this.context.Empleados.FromSqlRaw(sql, pamPosicion);
            return await consulta.ToListAsync();
        }

        public async Task<int> GetEmpleadosOficioCountAsync(string oficio)
        {
            return await this.context.Empleados.Where(x => x.Oficio == oficio).CountAsync();
        }

        public async Task<List<Empleado>> GetGrupoEmpleadosOficioAsync(int posicion, string oficio)
        {
            string sql = "SP_GRUPO_EMPLEADOS_OFICIO @posicion, @oficio";
            SqlParameter pamPosicion = new SqlParameter("@posicion", posicion);
            SqlParameter pamOficio = new SqlParameter("@oficio", oficio);
            var consulta = this.context.Empleados.FromSqlRaw(sql, pamPosicion, pamOficio);
            return await consulta.ToListAsync();
        }

        public async Task<ModelEmpleadosOficio> GetGrupoEmpleadosOficioOutAsync(int posicion, string oficio)
        {
            string sql = "SP_GRUPO_EMPLEADOS_OFICIO_OUT @posicion, @oficio, @registros out";
            SqlParameter pamPosicion = new SqlParameter("@posicion", posicion);
            SqlParameter pamOficio = new SqlParameter("@oficio", oficio);
            SqlParameter pamRegistros = new SqlParameter("@registros", 0);
            pamRegistros.Direction = System.Data.ParameterDirection.Output;
            pamRegistros.DbType = System.Data.DbType.Int32;
            var consulta = this.context.Empleados.FromSqlRaw(sql, pamPosicion, pamOficio, pamRegistros);
            List<Empleado> empleados = await consulta.ToListAsync();
            //HASTA QUE NO HEMOS EXTRAIDO LOS EMPLEADOS
            //NO SE LIBERAN LOS PARAMETROS DE SALIDA
            int registros = (int)pamRegistros.Value;
            return new ModelEmpleadosOficio
            {
                Empleados = empleados,
                NumeroRegistros = registros
            };
        }
    }
}

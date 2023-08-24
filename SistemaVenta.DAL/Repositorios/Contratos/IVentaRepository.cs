using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SistemaVenta.Model;
namespace SistemaVenta.DAL.Repositorios.Contratos
{
    public interface IVentaRepository : IgenericRepository<Venta>
    {
        Task<Venta> Registrar(Venta Modelo);
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SistemaVenta.DAL.DBContext;
using SistemaVenta.DAL.Repositorios.Contratos;
using SistemaVenta.Model;

namespace SistemaVenta.DAL.Repositorios
{
    public class VentaRepository : GenericRepository<Venta>,IVentaRepository
    {
        private readonly DbventaContext _dbcontext;

        public VentaRepository(DbventaContext dbcontext) : base(dbcontext) 
        {
            _dbcontext = dbcontext;
        }

        public async Task<Venta> Registrar(Venta modelo)
        {
            Venta ventaGenerada = new Venta();

            using (var transaction = _dbcontext.Database.BeginTransaction())
            {
                try
                {
                    foreach (DetalleVenta dv in modelo.DetalleVenta)
                    {
                        Producto productoEncontrado = _dbcontext.Productos.Where(p => p.IdProducto == dv.IdProducto).First();

                        productoEncontrado.Stock = productoEncontrado.Stock - dv.Cantidad;
                        _dbcontext.Productos.Update(productoEncontrado);
                    }
                    await _dbcontext.SaveChangesAsync();

                    NumeroDocumento correlativo = _dbcontext.NumeroDocumentos.First();
                    correlativo.UltimoNumero = correlativo.UltimoNumero + 1;

                    _dbcontext.NumeroDocumentos.Update(correlativo);
                    await _dbcontext.SaveChangesAsync();

                    int CantidadDigitos = 4;
                    string ceros = string.Concat(Enumerable.Repeat("0", CantidadDigitos));
                    string numeroVenta = ceros +correlativo.UltimoNumero.ToString();

                    numeroVenta = numeroVenta.Substring(numeroVenta.Length - CantidadDigitos,CantidadDigitos);

                    modelo.NumeroDocumento = numeroVenta;
                    await _dbcontext.Venta.AddAsync(modelo);
                    await _dbcontext.SaveChangesAsync();

                    ventaGenerada = modelo;

                    transaction.Commit();
                }
                catch
                {
                    transaction.Rollback();
                    throw;

                }
                return ventaGenerada;
            }
        }
    }
}

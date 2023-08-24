using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SistemaVenta.BLL.Servicios.Contrato;
using SistemaVenta.DAL.Repositorios.Contratos;
using SistemaVenta.DTO;
using SistemaVenta.Model;

namespace SistemaVenta.BLL.Servicios
{
    public class ProductoService:IProductoService
    {
        private readonly IgenericRepository<Producto> _productoRepositorio;
        private readonly IMapper _mapper;

        public ProductoService(IgenericRepository<Producto> productoRepositorio, IMapper mapper)
        {
            _productoRepositorio = productoRepositorio;
            _mapper = mapper;
        }


        public async Task<List<ProductoDTO>> Lista()
        {
            try
            {
                var queryProducto = await _productoRepositorio.Consultar();
                var listaProducto = queryProducto.Include(cat => cat.IdCategoriaNavigation).ToList();

                return _mapper.Map<List<ProductoDTO>>(listaProducto.ToList());
            }
            catch
            {
                throw;
            }
        }
        public async Task<ProductoDTO> Crear(ProductoDTO modelo)
        {
            try
            {
                var productoCreado = await _productoRepositorio.Crear(_mapper.Map<Producto>(modelo));
                if (productoCreado.IdProducto == 0)
                    throw new TaskCanceledException("No a sido posible crear el producto");

                return _mapper.Map<ProductoDTO>(productoCreado);

            }
            catch
            {
                throw;
            }
        }

        public async Task<bool> Editar(ProductoDTO modelo)
        {
            try
            {
                var productoModelo = _mapper.Map<Producto>(modelo);
                var productoEncontrado = await _productoRepositorio.Obtener(u => u.IdProducto == productoModelo.IdProducto);

                if (productoEncontrado == null)
                    throw new TaskCanceledException("El producto no a sido encontrado");

                productoEncontrado.Nombre = productoModelo.Nombre;
                productoEncontrado.IdCategoria = productoModelo.IdCategoria;
                productoEncontrado.Stock = productoModelo.Stock;
                productoEncontrado.Precio = productoModelo.Precio;
                productoEncontrado.EsActivo = productoModelo.EsActivo;

                bool respuesta = await _productoRepositorio.Editar(productoEncontrado);


                if (!respuesta)
                    throw new TaskCanceledException("No a sido posible editar el producto");

                return respuesta;

            }
            catch
            {
                throw;
            }
        }

        public async Task<bool> Eliminar(int id)
        {
            try
            {
               var productoEncontrado = await _productoRepositorio.Obtener(p => p.IdProducto == id);

                if (productoEncontrado == null)
                    throw new TaskCanceledException("El producto no a sido encontrado");

                bool respuesta = await _productoRepositorio.Eliminar(productoEncontrado);

                if (!respuesta)
                    throw new TaskCanceledException("No a sido posible eliminar el producto");

                return respuesta;
            }
            catch
            {
                throw;
            }
        }

 
    }
}

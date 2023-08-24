using System;
using System.Collections.Generic;
using System.Globalization;
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
    public class VentaService:IVentaService
    {
        private readonly IVentaRepository _ventaRepositorio;
        private readonly IgenericRepository<DetalleVenta> _detalleVentaRepositorio;
        private readonly IMapper _mapper;

        public VentaService(IVentaRepository ventaRepositorio,
            IgenericRepository<DetalleVenta> detalleVentaRepositorio, 
            IMapper mapper)
        {
            _ventaRepositorio = ventaRepositorio;
            _detalleVentaRepositorio = detalleVentaRepositorio;
            _mapper = mapper;
        }

        public async Task<VentaDTO> Registrar(VentaDTO modelo)
        {
            try
            {
                var ventaGenerada = await _ventaRepositorio.Registrar(_mapper.Map<Venta>(modelo));

                if(ventaGenerada.IdVenta ==0)
                    throw new TaskCanceledException("No a sido posible crear la venta");

                return _mapper.Map<VentaDTO>(ventaGenerada);
            }
            catch
            {
                throw;
            }
        }


        public async Task<List<VentaDTO>> Historial(string buscarPor, string numeroVenta, string fechaInicio, string fechaFin)
        {
            IQueryable<Venta> query = await _ventaRepositorio.Consultar();
            var ListarResultado = new List<Venta>();


            try
            {
                if(buscarPor == "fecha")
                {
                    DateTime fech_Inicio = DateTime.ParseExact(fechaInicio, "dd/MM/yyyy", new CultureInfo("es-ES"));
                    DateTime fech_Fin = DateTime.ParseExact(fechaFin, "dd/MM/yyyy", new CultureInfo("es-ES"));

                    ListarResultado = await query.Where(v =>
                    v.FechaRegistro.Value.Date >=fech_Inicio.Date &&
                    v.FechaRegistro.Value.Date <= fech_Fin.Date 
                    ).Include(dv => dv.DetalleVenta)
                    .ThenInclude(p => p.IdProductoNavigation)
                    .ToListAsync();
                }
                else
                {
                    ListarResultado = await query.Where(v => v.NumeroDocumento == numeroVenta 
               ).Include(dv => dv.DetalleVenta)
               .ThenInclude(p => p.IdProductoNavigation)
               .ToListAsync();
                }

            }
            catch
            {
                throw;
            }
            return _mapper.Map<List<VentaDTO>>(ListarResultado);
        }


        public async Task<List<ReporteDTO>> Reporte(string fechaInicio, string fechaFin)
        {
            IQueryable<DetalleVenta> query = await _detalleVentaRepositorio.Consultar();
            var ListaResultado = new List<DetalleVenta>();
            try
            {
                DateTime fech_Inicio = DateTime.ParseExact(fechaInicio, "dd/MM/yyyy", new CultureInfo("es-ES"));
                DateTime fech_Fin = DateTime.ParseExact(fechaFin, "dd/MM/yyyy", new CultureInfo("es-ES"));

                ListaResultado = await query
                    .Include(p => p.IdProductoNavigation)
                    .Include(v => v.IdVentaNavigation)
                    .Where(dv =>
                        dv.IdVentaNavigation.FechaRegistro.Value.Date >=fech_Inicio.Date &&
                        dv.IdVentaNavigation.FechaRegistro.Value.Date <= fech_Fin.Date 
                    ).ToListAsync();
            }
            catch
            {
                throw;
            }
            return _mapper.Map<List<ReporteDTO>>(ListaResultado);
        }
    }
}

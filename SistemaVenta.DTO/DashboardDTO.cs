using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaVenta.DTO
{
   public class DashboardDTO
    {
        public int TotalVentas { get; set; }    

        public string? TotalIngresos { get; set; }
        public int TotalProductos { get; set; }

        public List<VentaSemanaDTO> VentasUltimaSemana { get; set; }
    }
}

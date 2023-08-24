using System;
using System.Collections.Generic;

namespace SistemaVenta.Model;

public partial class Rol
{
    public int IdRol { get; set; }

    public string? Nombre { get; set; }

    public DateTime? FechaRegistro { get; set; }

    public virtual ICollection<MenuRol> MenuRols { get; } = new List<MenuRol>();

    public virtual ICollection<Usuario> Usuarios { get; } = new List<Usuario>();
}

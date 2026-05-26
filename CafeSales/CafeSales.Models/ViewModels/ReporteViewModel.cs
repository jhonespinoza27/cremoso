using System.ComponentModel.DataAnnotations;

namespace CafeSales.Models.ViewModels;

public class ReporteVentasViewModel
{
    [Display(Name = "Fecha Inicio")]
    [DataType(DataType.Date)]
    public DateTime FechaInicio { get; set; } = DateTime.Now.AddMonths(-1);

    [Display(Name = "Fecha Fin")]
    [DataType(DataType.Date)]
    public DateTime FechaFin { get; set; } = DateTime.Now;

    public List<VentaReporteItem> Ventas { get; set; } = new();
    public decimal TotalGeneral { get; set; }
    public int TotalVentas { get; set; }
}

public class VentaReporteItem
{
    public int VentaId { get; set; }
    public DateTime Fecha { get; set; }
    public string Cliente { get; set; } = string.Empty;
    public string Vendedor { get; set; } = string.Empty;
    public decimal Total { get; set; }
    public int CantidadProductos { get; set; }
}

public class ReporteProductosViewModel
{
    [Display(Name = "Fecha Inicio")]
    [DataType(DataType.Date)]
    public DateTime FechaInicio { get; set; } = DateTime.Now.AddMonths(-1);

    [Display(Name = "Fecha Fin")]
    [DataType(DataType.Date)]
    public DateTime FechaFin { get; set; } = DateTime.Now;

    public List<ProductoReporteItem> Productos { get; set; } = new();
}

public class ProductoReporteItem
{
    public string Nombre { get; set; } = string.Empty;
    public string Categoria { get; set; } = string.Empty;
    public int CantidadVendida { get; set; }
    public decimal Ingresos { get; set; }
}

public class ReporteVendedoresViewModel
{
    [Display(Name = "Fecha Inicio")]
    [DataType(DataType.Date)]
    public DateTime FechaInicio { get; set; } = DateTime.Now.AddMonths(-1);

    [Display(Name = "Fecha Fin")]
    [DataType(DataType.Date)]
    public DateTime FechaFin { get; set; } = DateTime.Now;

    public List<VendedorReporteItem> Vendedores { get; set; } = new();
}

public class VendedorReporteItem
{
    public string NombreVendedor { get; set; } = string.Empty;
    public int TotalVentas { get; set; }
    public decimal TotalIngresos { get; set; }
}

public class UsuarioViewModel
{
    public int UsuarioId { get; set; }

    [Required(ErrorMessage = "El nombre de usuario es obligatorio.")]
    [StringLength(100)]
    [Display(Name = "Usuario")]
    public string NombreUsuario { get; set; } = string.Empty;

    [StringLength(200)]
    [Display(Name = "Nombre Completo")]
    public string? NombreCompleto { get; set; }

    [Required(ErrorMessage = "La contraseña es obligatoria.")]
    [DataType(DataType.Password)]
    [MinLength(6, ErrorMessage = "La contraseña debe tener al menos 6 caracteres.")]
    [Display(Name = "Contraseña")]
    public string? Password { get; set; }

    [Required(ErrorMessage = "El rol es obligatorio.")]
    [Display(Name = "Rol")]
    public int RolId { get; set; }

    public bool Activo { get; set; } = true;
}

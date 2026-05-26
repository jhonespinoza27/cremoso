namespace CafeSales.Models.ViewModels;

public class DashboardViewModel
{
    public int VentasHoy { get; set; }
    public decimal IngresosHoy { get; set; }
    public decimal IngresosMes { get; set; }
    public int ProductosBajoStock { get; set; }
    public int TotalClientes { get; set; }
    public int TotalProductos { get; set; }
    public List<ProductoPopularViewModel> ProductosPopulares { get; set; } = new();
    public List<VentaRecienteViewModel> VentasRecientes { get; set; } = new();
}

public class ProductoPopularViewModel
{
    public string Nombre { get; set; } = string.Empty;
    public int CantidadVendida { get; set; }
    public decimal Ingresos { get; set; }
}

public class VentaRecienteViewModel
{
    public int VentaId { get; set; }
    public string Cliente { get; set; } = string.Empty;
    public string Vendedor { get; set; } = string.Empty;
    public DateTime Fecha { get; set; }
    public decimal Total { get; set; }
}

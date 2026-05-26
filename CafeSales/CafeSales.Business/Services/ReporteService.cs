using CafeSales.Business.Interfaces;
using CafeSales.Data.Context;
using CafeSales.Models.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace CafeSales.Business.Services;

public class ReporteService : IReporteService
{
    private readonly CafeDbContext _context;
    public ReporteService(CafeDbContext context) => _context = context;

    public async Task<DashboardViewModel> ObtenerDashboardAsync()
    {
        var hoy = DateTime.Today;
        var inicioMes = new DateTime(hoy.Year, hoy.Month, 1);

        var ventasHoy = await _context.Ventas
            .Where(v => v.Fecha.Date == hoy)
            .ToListAsync();

        var ventasMes = await _context.Ventas
            .Where(v => v.Fecha >= inicioMes)
            .ToListAsync();

        var model = new DashboardViewModel
        {
            VentasHoy = ventasHoy.Count,
            IngresosHoy = ventasHoy.Sum(v => v.Total),
            IngresosMes = ventasMes.Sum(v => v.Total),
            ProductosBajoStock = await _context.Productos.CountAsync(p => p.Activo && p.Stock <= 10),
            TotalClientes = await _context.Clientes.CountAsync(c => c.Activo),
            TotalProductos = await _context.Productos.CountAsync(p => p.Activo),
            ProductosPopulares = await _context.DetalleVentas
                .Include(d => d.Producto)
                .Include(d => d.Venta)
                .Where(d => d.Venta.Fecha >= inicioMes)
                .GroupBy(d => d.Producto.Nombre)
                .Select(g => new ProductoPopularViewModel
                {
                    Nombre = g.Key,
                    CantidadVendida = g.Sum(d => d.Cantidad),
                    Ingresos = g.Sum(d => d.Cantidad * d.PrecioUnit)
                })
                .OrderByDescending(p => p.CantidadVendida)
                .Take(5)
                .ToListAsync(),
            VentasRecientes = await _context.Ventas
                .Include(v => v.Cliente)
                .Include(v => v.Usuario)
                .OrderByDescending(v => v.Fecha)
                .Take(10)
                .Select(v => new VentaRecienteViewModel
                {
                    VentaId = v.VentaId,
                    Cliente = v.Cliente.Nombre,
                    Vendedor = v.Usuario.NombreCompleto ?? v.Usuario.NombreUsuario,
                    Fecha = v.Fecha,
                    Total = v.Total
                })
                .ToListAsync()
        };

        return model;
    }

    public async Task<ReporteVentasViewModel> ObtenerVentasPorPeriodoAsync(DateTime fechaInicio, DateTime fechaFin)
    {
        var ventas = await _context.Ventas
            .Include(v => v.Cliente)
            .Include(v => v.Usuario)
            .Include(v => v.Detalles)
            .Where(v => v.Fecha >= fechaInicio && v.Fecha <= fechaFin.AddDays(1))
            .OrderByDescending(v => v.Fecha)
            .Select(v => new VentaReporteItem
            {
                VentaId = v.VentaId,
                Fecha = v.Fecha,
                Cliente = v.Cliente.Nombre,
                Vendedor = v.Usuario.NombreCompleto ?? v.Usuario.NombreUsuario,
                Total = v.Total,
                CantidadProductos = v.Detalles.Sum(d => d.Cantidad)
            })
            .ToListAsync();

        return new ReporteVentasViewModel
        {
            FechaInicio = fechaInicio,
            FechaFin = fechaFin,
            Ventas = ventas,
            TotalGeneral = ventas.Sum(v => v.Total),
            TotalVentas = ventas.Count
        };
    }

    public async Task<ReporteProductosViewModel> ObtenerProductosMasVendidosAsync(DateTime fechaInicio, DateTime fechaFin)
    {
        var productos = await _context.DetalleVentas
            .Include(d => d.Producto).ThenInclude(p => p.Categoria)
            .Include(d => d.Venta)
            .Where(d => d.Venta.Fecha >= fechaInicio && d.Venta.Fecha <= fechaFin.AddDays(1))
            .GroupBy(d => new { d.Producto.Nombre, Categoria = d.Producto.Categoria.Nombre })
            .Select(g => new ProductoReporteItem
            {
                Nombre = g.Key.Nombre,
                Categoria = g.Key.Categoria,
                CantidadVendida = g.Sum(d => d.Cantidad),
                Ingresos = g.Sum(d => d.Cantidad * d.PrecioUnit)
            })
            .OrderByDescending(p => p.CantidadVendida)
            .ToListAsync();

        return new ReporteProductosViewModel
        {
            FechaInicio = fechaInicio,
            FechaFin = fechaFin,
            Productos = productos
        };
    }

    public async Task<ReporteVendedoresViewModel> ObtenerVentasPorVendedorAsync(DateTime fechaInicio, DateTime fechaFin)
    {
        var vendedores = await _context.Ventas
            .Include(v => v.Usuario)
            .Where(v => v.Fecha >= fechaInicio && v.Fecha <= fechaFin.AddDays(1))
            .GroupBy(v => v.Usuario.NombreCompleto ?? v.Usuario.NombreUsuario)
            .Select(g => new VendedorReporteItem
            {
                NombreVendedor = g.Key,
                TotalVentas = g.Count(),
                TotalIngresos = g.Sum(v => v.Total)
            })
            .OrderByDescending(v => v.TotalIngresos)
            .ToListAsync();

        return new ReporteVendedoresViewModel
        {
            FechaInicio = fechaInicio,
            FechaFin = fechaFin,
            Vendedores = vendedores
        };
    }
}

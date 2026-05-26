using CafeSales.Business.Services;
using CafeSales.Data.Context;
using CafeSales.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace CafeSales.Tests;

public class ReporteServiceTests
{
    private static async Task<CafeDbContext> CrearContextoConVentasAsync()
    {
        var context = await SqlServerTestDatabase.CreateEmptyContextAsync();
        var rol = new Rol { Nombre = "Administrador" };
        var categoria = new Categoria { Nombre = "Bebidas" };
        var latte = new Producto
        {
            Nombre = "Latte",
            CategoriaId = 1,
            Categoria = categoria,
            Precio = 5m,
            Stock = 8,
            Activo = true
        };
        var te = new Producto
        {
            Nombre = "Te",
            CategoriaId = 1,
            Categoria = categoria,
            Precio = 4m,
            Stock = 20,
            Activo = true
        };
        var cliente = new Cliente { Nombre = "Cliente General", Activo = true };
        var usuario = new Usuario
        {
            NombreUsuario = "admin",
            NombreCompleto = "Administrador",
            PasswordHash = "hash",
            RolId = 1,
            Activo = true
        };
        var hoy = DateTime.Today.AddHours(10);
        var ventaHoy = new Venta
        {
            ClienteId = 1,
            Cliente = cliente,
            UsuarioId = 1,
            Usuario = usuario,
            Fecha = hoy,
            Total = 15m
        };
        var ventaMes = new Venta
        {
            ClienteId = 1,
            Cliente = cliente,
            UsuarioId = 1,
            Usuario = usuario,
            Fecha = DateTime.Today.AddHours(12),
            Total = 4m
        };
        ventaHoy.Detalles.Add(new DetalleVenta
        {
            VentaId = 1,
            Venta = ventaHoy,
            ProductoId = 1,
            Producto = latte,
            Cantidad = 3,
            PrecioUnit = 5m
        });
        ventaMes.Detalles.Add(new DetalleVenta
        {
            VentaId = 2,
            Venta = ventaMes,
            ProductoId = 2,
            Producto = te,
            Cantidad = 1,
            PrecioUnit = 4m
        });
        context.AddRange(rol, categoria, latte, te, cliente, usuario, ventaHoy, ventaMes);
        await context.SaveChangesAsync();
        return context;
    }

    [Fact]
    public async Task ObtenerDashboard_DebeCalcularIndicadoresYVentasRecientes()
    {
        await using var context = await CrearContextoConVentasAsync();
        var service = new ReporteService(context);

        var resultado = await service.ObtenerDashboardAsync();

        Assert.Equal(2, resultado.VentasHoy);
        Assert.Equal(19m, resultado.IngresosHoy);
        Assert.Equal(19m, resultado.IngresosMes);
        Assert.Equal(1, resultado.ProductosBajoStock);
        Assert.Single(resultado.VentasRecientes, v => v.VentaId == 1);
    }

    [Fact]
    public async Task ObtenerVentasPorPeriodo_DebeTotalizarVentasYProductos()
    {
        await using var context = await CrearContextoConVentasAsync();
        var service = new ReporteService(context);

        var resultado = await service.ObtenerVentasPorPeriodoAsync(
            DateTime.Today.AddDays(-2), DateTime.Today);

        Assert.Equal(2, resultado.TotalVentas);
        Assert.Equal(19m, resultado.TotalGeneral);
        Assert.Equal(3, resultado.Ventas.Single(v => v.VentaId == 1).CantidadProductos);
    }

    [Fact]
    public async Task ObtenerProductosMasVendidos_DebeAgruparYOrdenarPorCantidad()
    {
        await using var context = await CrearContextoConVentasAsync();
        var service = new ReporteService(context);

        var resultado = await service.ObtenerProductosMasVendidosAsync(
            DateTime.Today.AddDays(-2), DateTime.Today);

        Assert.Equal("Latte", resultado.Productos[0].Nombre);
        Assert.Equal(3, resultado.Productos[0].CantidadVendida);
        Assert.Equal(15m, resultado.Productos[0].Ingresos);
    }

    [Fact]
    public async Task ObtenerVentasPorVendedor_DebeAgruparIngresosDelUsuario()
    {
        await using var context = await CrearContextoConVentasAsync();
        var service = new ReporteService(context);

        var resultado = await service.ObtenerVentasPorVendedorAsync(
            DateTime.Today.AddDays(-2), DateTime.Today);

        var vendedor = Assert.Single(resultado.Vendedores);
        Assert.Equal("Administrador", vendedor.NombreVendedor);
        Assert.Equal(2, vendedor.TotalVentas);
        Assert.Equal(19m, vendedor.TotalIngresos);
    }
}

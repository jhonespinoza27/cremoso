using CafeSales.Business.Services;
using CafeSales.Data.Context;
using CafeSales.Models.Entities;
using CafeSales.Models.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace CafeSales.Tests;

public class VentaServiceTests
{
    private static async Task<CafeDbContext> CrearContextoAsync()
    {
        return await SqlServerTestDatabase.CreateEmptyContextAsync();
    }

    private static async Task SembrarVentasAsync(CafeDbContext context)
    {
        context.Roles.Add(new Rol { Nombre = "Vendedor" });
        context.Categorias.Add(new Categoria { Nombre = "Bebidas" });
        context.Clientes.AddRange(
            new Cliente { Nombre = "General" },
            new Cliente { Nombre = "Empresa" });
        context.Usuarios.Add(new Usuario
        {
            NombreUsuario = "vendedor",
            PasswordHash = "hash",
            RolId = 1
        });
        context.Productos.Add(new Producto
        {
            Nombre = "Latte",
            Precio = 5.50m,
            Stock = 10,
            CategoriaId = 1,
            Activo = true
        });
        context.Ventas.AddRange(
            new Venta
            {
                ClienteId = 1,
                UsuarioId = 1,
                Fecha = new DateTime(2026, 5, 10),
                Total = 5.50m,
                Detalles = [new DetalleVenta { ProductoId = 1, Cantidad = 1, PrecioUnit = 5.50m }]
            },
            new Venta
            {
                ClienteId = 2,
                UsuarioId = 1,
                Fecha = new DateTime(2026, 5, 12),
                Total = 11.00m,
                Detalles = [new DetalleVenta { ProductoId = 1, Cantidad = 2, PrecioUnit = 5.50m }]
            });
        await context.SaveChangesAsync();
    }

    [Fact]
    public async Task ObtenerTodos_DebeRetornarVentasOrdenadasConDetalle()
    {
        await using var context = await CrearContextoAsync();
        await SembrarVentasAsync(context);
        var service = new VentaService(context);

        var resultado = (await service.ObtenerTodosAsync()).ToList();

        Assert.Equal([2, 1], resultado.Select(v => v.VentaId));
        Assert.Equal("Empresa", resultado[0].Cliente.Nombre);
        Assert.Equal("Latte", resultado[0].Detalles.Single().Producto.Nombre);
    }

    [Fact]
    public async Task Buscar_DebeFiltrarPorPeriodoYCliente()
    {
        await using var context = await CrearContextoAsync();
        await SembrarVentasAsync(context);
        var service = new VentaService(context);

        var resultado = await service.BuscarAsync(
            new DateTime(2026, 5, 11), new DateTime(2026, 5, 12), 2);

        var venta = Assert.Single(resultado);
        Assert.Equal(2, venta.VentaId);
    }

    [Fact]
    public async Task ObtenerPorId_DebeRetornarDetalleCompleto()
    {
        await using var context = await CrearContextoAsync();
        await SembrarVentasAsync(context);
        var service = new VentaService(context);

        var resultado = await service.ObtenerPorIdAsync(1);

        Assert.NotNull(resultado);
        Assert.Equal("General", resultado.Cliente.Nombre);
        Assert.Equal(1, resultado.Detalles.Single().Cantidad);
    }

    [Fact]
    public async Task RegistrarVenta_DebeUsarPrecioRegistradoYReducirStock()
    {
        await using var context = await CrearContextoAsync();
        context.Roles.Add(new Rol { Nombre = "Vendedor" });
        context.Categorias.Add(new Categoria { Nombre = "Bebidas" });
        context.Clientes.Add(new Cliente { Nombre = "General" });
        context.Usuarios.Add(new Usuario { NombreUsuario = "vendedor", PasswordHash = "hash", RolId = 1 });
        context.Productos.Add(new Producto
        {
            Nombre = "Latte",
            Precio = 5.50m,
            Stock = 10,
            CategoriaId = 1,
            Activo = true
        });
        await context.SaveChangesAsync();
        var service = new VentaService(context);
        var model = new VentaViewModel
        {
            ClienteId = 1,
            Detalles = [new DetalleVentaViewModel { ProductoId = 1, Cantidad = 2, PrecioUnit = 0.01m }]
        };

        var venta = await service.RegistrarVentaAsync(model, 1);

        Assert.Equal(11.00m, venta.Total);
        Assert.Equal(5.50m, venta.Detalles.Single().PrecioUnit);
        Assert.Equal(8, (await context.Productos.FindAsync(1))!.Stock);
    }

    [Fact]
    public async Task RegistrarVenta_DebeRechazarCantidadNoValida()
    {
        await using var context = await CrearContextoAsync();
        context.Categorias.Add(new Categoria { Nombre = "Bebidas" });
        context.Productos.Add(new Producto
        {
            Nombre = "Latte",
            Precio = 5.50m,
            Stock = 10,
            CategoriaId = 1,
            Activo = true
        });
        await context.SaveChangesAsync();
        var service = new VentaService(context);
        var model = new VentaViewModel
        {
            ClienteId = 1,
            Detalles = [new DetalleVentaViewModel { ProductoId = 1, Cantidad = -1 }]
        };

        await Assert.ThrowsAsync<InvalidOperationException>(
            () => service.RegistrarVentaAsync(model, 1));

        Assert.Equal(10, (await context.Productos.FindAsync(1))!.Stock);
    }

    [Fact]
    public async Task RegistrarVenta_DebeRechazarProductoInactivo()
    {
        await using var context = await CrearContextoAsync();
        context.Categorias.Add(new Categoria { Nombre = "Bebidas" });
        context.Productos.Add(new Producto
        {
            Nombre = "Latte",
            Precio = 5.50m,
            Stock = 10,
            CategoriaId = 1,
            Activo = false
        });
        await context.SaveChangesAsync();
        var service = new VentaService(context);
        var model = new VentaViewModel
        {
            ClienteId = 1,
            Detalles = [new DetalleVentaViewModel { ProductoId = 1, Cantidad = 1 }]
        };

        await Assert.ThrowsAsync<InvalidOperationException>(
            () => service.RegistrarVentaAsync(model, 1));

        Assert.Empty(context.Ventas);
    }

    [Fact]
    public async Task RegistrarVenta_DebeRechazarStockInsuficiente()
    {
        await using var context = await CrearContextoAsync();
        context.Categorias.Add(new Categoria { Nombre = "Bebidas" });
        context.Productos.Add(new Producto
        {
            Nombre = "Latte",
            Precio = 5.50m,
            Stock = 1,
            CategoriaId = 1,
            Activo = true
        });
        await context.SaveChangesAsync();
        var service = new VentaService(context);
        var model = new VentaViewModel
        {
            ClienteId = 1,
            Detalles = [new DetalleVentaViewModel { ProductoId = 1, Cantidad = 2 }]
        };

        await Assert.ThrowsAsync<InvalidOperationException>(
            () => service.RegistrarVentaAsync(model, 1));

        Assert.Equal(1, (await context.Productos.FindAsync(1))!.Stock);
    }
}

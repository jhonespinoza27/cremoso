using CafeSales.Business.Services;
using CafeSales.Data.Context;
using CafeSales.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace CafeSales.Tests;

public class ProductoServiceTests
{
    private static async Task<CafeDbContext> GetSqlServerContextAsync()
    {
        return await SqlServerTestDatabase.CreateEmptyContextAsync();
    }

    [Fact]
    public async Task CrearProducto_DebeAgregarAlContexto()
    {
        // Arrange
        await using var ctx = await GetSqlServerContextAsync();
        ctx.Categorias.Add(new Categoria { Nombre = "Bebidas" });
        await ctx.SaveChangesAsync();
        var svc = new ProductoService(ctx);
        var prod = new Producto { Nombre = "Café", Precio = 3.5m, Stock = 100, CategoriaId = 1 };

        // Act
        await svc.CrearAsync(prod);

        // Assert
        Assert.Equal(1, ctx.Productos.Count());
    }

    [Fact]
    public async Task EliminarProducto_DebePonerseBajaLogica()
    {
        // Arrange
        await using var ctx = await GetSqlServerContextAsync();
        ctx.Categorias.Add(new Categoria { Nombre = "Bebidas" });
        ctx.Productos.Add(new Producto { Nombre = "Té", Precio = 2m, Stock = 50, CategoriaId = 1, Activo = true });
        await ctx.SaveChangesAsync();
        var svc = new ProductoService(ctx);

        // Act
        await svc.EliminarAsync(1);

        // Assert
        Assert.False(ctx.Productos.First().Activo);
    }

    [Fact]
    public async Task ObtenerTodos_DebeRetornarSoloActivos()
    {
        // Arrange
        await using var ctx = await GetSqlServerContextAsync();
        ctx.Categorias.Add(new Categoria { Nombre = "Bebidas" });
        ctx.Productos.Add(new Producto { Nombre = "Café", Precio = 3m, Stock = 10, CategoriaId = 1, Activo = true });
        ctx.Productos.Add(new Producto { Nombre = "Té Inactivo", Precio = 2m, Stock = 5, CategoriaId = 1, Activo = false });
        await ctx.SaveChangesAsync();
        var svc = new ProductoService(ctx);

        // Act
        var result = await svc.ObtenerTodosAsync();

        // Assert
        Assert.Single(result);
        Assert.Equal("Café", result.First().Nombre);
    }

    [Fact]
    public async Task BuscarProducto_DebeRetornarCoincidencias()
    {
        // Arrange
        await using var ctx = await GetSqlServerContextAsync();
        ctx.Categorias.Add(new Categoria { Nombre = "Bebidas" });
        ctx.Productos.Add(new Producto { Nombre = "Café Americano", Precio = 3m, Stock = 10, CategoriaId = 1, Activo = true });
        ctx.Productos.Add(new Producto { Nombre = "Croissant", Precio = 4m, Stock = 5, CategoriaId = 1, Activo = true });
        await ctx.SaveChangesAsync();
        var svc = new ProductoService(ctx);

        // Act
        var result = await svc.BuscarAsync("Café", null);

        // Assert
        Assert.Single(result);
    }

    [Fact]
    public async Task ActualizarProducto_DebeModificarDatos()
    {
        // Arrange
        await using var ctx = await GetSqlServerContextAsync();
        ctx.Categorias.Add(new Categoria { Nombre = "Bebidas" });
        ctx.Productos.Add(new Producto { Nombre = "Café", Precio = 3m, Stock = 10, CategoriaId = 1, Activo = true });
        await ctx.SaveChangesAsync();
        var svc = new ProductoService(ctx);

        // Act
        var prod = await svc.ObtenerPorIdAsync(1);
        prod!.Precio = 5m;
        await svc.ActualizarAsync(prod);

        // Assert
        var updated = await ctx.Productos.FindAsync(1);
        Assert.Equal(5m, updated!.Precio);
    }

    [Fact]
    public async Task ObtenerPorId_DebeRetornarProductoConCategoria()
    {
        await using var ctx = await GetSqlServerContextAsync();
        ctx.Categorias.Add(new Categoria { Nombre = "Bebidas" });
        ctx.Productos.Add(new Producto { Nombre = "Latte", Precio = 5m, Stock = 10, CategoriaId = 1 });
        await ctx.SaveChangesAsync();
        var svc = new ProductoService(ctx);

        var resultado = await svc.ObtenerPorIdAsync(1);

        Assert.NotNull(resultado);
        Assert.Equal("Bebidas", resultado.Categoria.Nombre);
    }

    [Fact]
    public async Task BuscarPorCategoria_DebeRetornarSoloProductosDeLaCategoria()
    {
        await using var ctx = await GetSqlServerContextAsync();
        ctx.Categorias.AddRange(
            new Categoria { Nombre = "Bebidas" },
            new Categoria { Nombre = "Postres" });
        ctx.Productos.AddRange(
            new Producto { Nombre = "Latte", Precio = 5m, Stock = 10, CategoriaId = 1 },
            new Producto { Nombre = "Tarta", Precio = 6m, Stock = 3, CategoriaId = 2 });
        await ctx.SaveChangesAsync();
        var svc = new ProductoService(ctx);

        var resultado = await svc.BuscarAsync(null, 2);

        Assert.Single(resultado);
        Assert.Equal("Tarta", resultado.Single().Nombre);
    }

    [Fact]
    public async Task EliminarProducto_IdInexistente_NoDebeLanzarExcepcion()
    {
        await using var ctx = await GetSqlServerContextAsync();
        var svc = new ProductoService(ctx);

        var excepcion = await Record.ExceptionAsync(() => svc.EliminarAsync(999));

        Assert.Null(excepcion);
    }
}

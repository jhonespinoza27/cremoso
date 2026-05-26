using CafeSales.Business.Interfaces;
using CafeSales.Data.Context;
using CafeSales.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace CafeSales.Business.Services;

public class ProductoService : IProductoService
{
    private readonly CafeDbContext _context;
    public ProductoService(CafeDbContext context) => _context = context;

    public async Task<IEnumerable<Producto>> ObtenerTodosAsync()
        => await _context.Productos
               .Include(p => p.Categoria)
               .Where(p => p.Activo)
               .OrderBy(p => p.Nombre)
               .ToListAsync();

    public async Task<IEnumerable<Producto>> BuscarAsync(string? busqueda, int? categoriaId)
    {
        var query = _context.Productos
            .Include(p => p.Categoria)
            .Where(p => p.Activo);

        if (!string.IsNullOrWhiteSpace(busqueda))
            query = query.Where(p => p.Nombre.Contains(busqueda) || (p.Descripcion != null && p.Descripcion.Contains(busqueda)));

        if (categoriaId.HasValue && categoriaId.Value > 0)
            query = query.Where(p => p.CategoriaId == categoriaId.Value);

        return await query.OrderBy(p => p.Nombre).ToListAsync();
    }

    public async Task<Producto?> ObtenerPorIdAsync(int id)
        => await _context.Productos
               .Include(p => p.Categoria)
               .FirstOrDefaultAsync(p => p.ProductoId == id);

    public async Task CrearAsync(Producto producto)
    {
        _context.Productos.Add(producto);
        await _context.SaveChangesAsync();
    }

    public async Task ActualizarAsync(Producto producto)
    {
        _context.Productos.Update(producto);
        await _context.SaveChangesAsync();
    }

    public async Task EliminarAsync(int id)
    {
        var p = await _context.Productos.FindAsync(id);
        if (p != null) { p.Activo = false; }  // Baja lógica
        await _context.SaveChangesAsync();
    }
}

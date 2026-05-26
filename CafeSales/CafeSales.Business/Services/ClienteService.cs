using CafeSales.Business.Interfaces;
using CafeSales.Data.Context;
using CafeSales.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace CafeSales.Business.Services;

public class ClienteService : IClienteService
{
    private readonly CafeDbContext _context;
    public ClienteService(CafeDbContext context) => _context = context;

    public async Task<IEnumerable<Cliente>> ObtenerTodosAsync()
        => await _context.Clientes
               .Where(c => c.Activo)
               .OrderBy(c => c.Nombre)
               .ToListAsync();

    public async Task<IEnumerable<Cliente>> BuscarAsync(string? busqueda)
    {
        var query = _context.Clientes.Where(c => c.Activo);

        if (!string.IsNullOrWhiteSpace(busqueda))
            query = query.Where(c =>
                c.Nombre.Contains(busqueda) ||
                (c.Documento != null && c.Documento.Contains(busqueda)) ||
                (c.Correo != null && c.Correo.Contains(busqueda)));

        return await query.OrderBy(c => c.Nombre).ToListAsync();
    }

    public async Task<Cliente?> ObtenerPorIdAsync(int id)
        => await _context.Clientes.FindAsync(id);

    public async Task CrearAsync(Cliente cliente)
    {
        _context.Clientes.Add(cliente);
        await _context.SaveChangesAsync();
    }

    public async Task ActualizarAsync(Cliente cliente)
    {
        _context.Clientes.Update(cliente);
        await _context.SaveChangesAsync();
    }

    public async Task EliminarAsync(int id)
    {
        var c = await _context.Clientes.FindAsync(id);
        if (c != null) { c.Activo = false; }
        await _context.SaveChangesAsync();
    }
}

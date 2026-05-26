using CafeSales.Business.Interfaces;
using CafeSales.Data.Context;
using CafeSales.Models.Entities;
using CafeSales.Models.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace CafeSales.Business.Services;

public class VentaService : IVentaService
{
    private readonly CafeDbContext _context;
    public VentaService(CafeDbContext context) => _context = context;

    public async Task<IEnumerable<Venta>> ObtenerTodosAsync()
        => await _context.Ventas
               .Include(v => v.Cliente)
               .Include(v => v.Usuario)
               .Include(v => v.Detalles).ThenInclude(d => d.Producto)
               .OrderByDescending(v => v.Fecha)
               .ToListAsync();

    public async Task<IEnumerable<Venta>> BuscarAsync(DateTime? fechaInicio, DateTime? fechaFin, int? clienteId)
    {
        var query = _context.Ventas
            .Include(v => v.Cliente)
            .Include(v => v.Usuario)
            .Include(v => v.Detalles).ThenInclude(d => d.Producto)
            .AsQueryable();

        if (fechaInicio.HasValue)
            query = query.Where(v => v.Fecha >= fechaInicio.Value);
        if (fechaFin.HasValue)
            query = query.Where(v => v.Fecha <= fechaFin.Value.AddDays(1));
        if (clienteId.HasValue && clienteId.Value > 0)
            query = query.Where(v => v.ClienteId == clienteId.Value);

        return await query.OrderByDescending(v => v.Fecha).ToListAsync();
    }

    public async Task<Venta?> ObtenerPorIdAsync(int id)
        => await _context.Ventas
               .Include(v => v.Cliente)
               .Include(v => v.Usuario)
               .Include(v => v.Detalles).ThenInclude(d => d.Producto)
               .FirstOrDefaultAsync(v => v.VentaId == id);

    public async Task<Venta> RegistrarVentaAsync(VentaViewModel model, int usuarioId)
    {
        await using var transaction = _context.Database.IsRelational()
            ? await _context.Database.BeginTransactionAsync()
            : null;
        try
        {
            var venta = new Venta
            {
                ClienteId = model.ClienteId,
                UsuarioId = usuarioId,
                Fecha = DateTime.Now,
                Observacion = model.Observacion
            };

            foreach (var det in model.Detalles)
            {
                if (det.Cantidad <= 0)
                    throw new InvalidOperationException("La cantidad de cada producto debe ser mayor a 0.");

                var prod = await _context.Productos
                    .FirstOrDefaultAsync(p => p.ProductoId == det.ProductoId && p.Activo);
                if (prod == null)
                    throw new InvalidOperationException($"El producto con ID {det.ProductoId} no existe o no está disponible.");
                if (prod.Stock < det.Cantidad)
                    throw new InvalidOperationException($"Stock insuficiente para {prod.Nombre}. Disponible: {prod.Stock}");

                prod.Stock -= det.Cantidad;
                venta.Detalles.Add(new DetalleVenta
                {
                    ProductoId = det.ProductoId,
                    Cantidad = det.Cantidad,
                    PrecioUnit = prod.Precio
                });
                venta.Total += det.Cantidad * prod.Precio;
            }

            _context.Ventas.Add(venta);
            await _context.SaveChangesAsync();
            if (transaction != null)
                await transaction.CommitAsync();

            return venta;
        }
        catch
        {
            if (transaction != null)
                await transaction.RollbackAsync();
            throw;
        }
    }
}

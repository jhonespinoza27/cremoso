using CafeSales.Models.Entities;

namespace CafeSales.Business.Interfaces;

public interface IProductoService
{
    Task<IEnumerable<Producto>> ObtenerTodosAsync();
    Task<IEnumerable<Producto>> BuscarAsync(string? busqueda, int? categoriaId);
    Task<Producto?> ObtenerPorIdAsync(int id);
    Task CrearAsync(Producto producto);
    Task ActualizarAsync(Producto producto);
    Task EliminarAsync(int id);
}

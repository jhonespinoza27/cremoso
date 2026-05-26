using CafeSales.Models.Entities;

namespace CafeSales.Business.Interfaces;

public interface IClienteService
{
    Task<IEnumerable<Cliente>> ObtenerTodosAsync();
    Task<IEnumerable<Cliente>> BuscarAsync(string? busqueda);
    Task<Cliente?> ObtenerPorIdAsync(int id);
    Task CrearAsync(Cliente cliente);
    Task ActualizarAsync(Cliente cliente);
    Task EliminarAsync(int id);
}

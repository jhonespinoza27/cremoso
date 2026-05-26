using CafeSales.Models.Entities;
using CafeSales.Models.ViewModels;

namespace CafeSales.Business.Interfaces;

public interface IVentaService
{
    Task<IEnumerable<Venta>> ObtenerTodosAsync();
    Task<IEnumerable<Venta>> BuscarAsync(DateTime? fechaInicio, DateTime? fechaFin, int? clienteId);
    Task<Venta?> ObtenerPorIdAsync(int id);
    Task<Venta> RegistrarVentaAsync(VentaViewModel model, int usuarioId);
}

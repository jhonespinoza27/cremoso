using CafeSales.Models.Entities;
using CafeSales.Models.ViewModels;

namespace CafeSales.Business.Interfaces;

public interface IUsuarioService
{
    Task<IEnumerable<Usuario>> ObtenerTodosAsync();
    Task<Usuario?> ObtenerPorIdAsync(int id);
    Task<Usuario?> ValidarCredencialesAsync(string nombreUsuario, string password);
    Task CrearAsync(UsuarioViewModel model);
    Task ActualizarAsync(UsuarioViewModel model);
    Task EliminarAsync(int id);
}

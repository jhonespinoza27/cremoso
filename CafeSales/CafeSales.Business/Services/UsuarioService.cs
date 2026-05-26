using CafeSales.Business.Interfaces;
using CafeSales.Data.Context;
using CafeSales.Models.Entities;
using CafeSales.Models.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace CafeSales.Business.Services;

public class UsuarioService : IUsuarioService
{
    private readonly CafeDbContext _context;
    public UsuarioService(CafeDbContext context) => _context = context;

    public async Task<IEnumerable<Usuario>> ObtenerTodosAsync()
        => await _context.Usuarios
               .Include(u => u.Rol)
               .OrderBy(u => u.NombreUsuario)
               .ToListAsync();

    public async Task<Usuario?> ObtenerPorIdAsync(int id)
        => await _context.Usuarios
               .Include(u => u.Rol)
               .FirstOrDefaultAsync(u => u.UsuarioId == id);

    public async Task<Usuario?> ValidarCredencialesAsync(string nombreUsuario, string password)
    {
        var usuario = await _context.Usuarios
            .Include(u => u.Rol)
            .FirstOrDefaultAsync(u => u.NombreUsuario == nombreUsuario && u.Activo);

        if (usuario == null)
            return null;

        if (!BCrypt.Net.BCrypt.Verify(password, usuario.PasswordHash))
            return null;

        return usuario;
    }

    public async Task CrearAsync(UsuarioViewModel model)
    {
        var usuario = new Usuario
        {
            NombreUsuario = model.NombreUsuario,
            NombreCompleto = model.NombreCompleto,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.Password),
            RolId = model.RolId,
            Activo = true,
            FechaCreacion = DateTime.Now
        };

        _context.Usuarios.Add(usuario);
        await _context.SaveChangesAsync();
    }

    public async Task ActualizarAsync(UsuarioViewModel model)
    {
        var usuario = await _context.Usuarios.FindAsync(model.UsuarioId);
        if (usuario == null) return;

        usuario.NombreUsuario = model.NombreUsuario;
        usuario.NombreCompleto = model.NombreCompleto;
        usuario.RolId = model.RolId;
        usuario.Activo = model.Activo;

        if (!string.IsNullOrWhiteSpace(model.Password))
            usuario.PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.Password);

        await _context.SaveChangesAsync();
    }

    public async Task EliminarAsync(int id)
    {
        var u = await _context.Usuarios.FindAsync(id);
        if (u != null) { u.Activo = false; }
        await _context.SaveChangesAsync();
    }
}

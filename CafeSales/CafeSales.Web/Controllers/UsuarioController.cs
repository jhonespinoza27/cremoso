using CafeSales.Business.Interfaces;
using CafeSales.Data.Context;
using CafeSales.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace CafeSales.Web.Controllers;

[Authorize(Roles = "Administrador")]
public class UsuarioController : Controller
{
    private readonly IUsuarioService _svc;
    private readonly CafeDbContext _context;

    public UsuarioController(IUsuarioService svc, CafeDbContext context)
    {
        _svc = svc;
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var usuarios = await _svc.ObtenerTodosAsync();
        return View(usuarios);
    }

    public async Task<IActionResult> Crear()
    {
        ViewBag.Roles = new SelectList(await _context.Roles.ToListAsync(), "RolId", "Nombre");
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Crear(UsuarioViewModel model)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.Roles = new SelectList(await _context.Roles.ToListAsync(), "RolId", "Nombre", model.RolId);
            return View(model);
        }
        await _svc.CrearAsync(model);
        TempData["Exito"] = "Usuario creado correctamente.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Editar(int id)
    {
        var usuario = await _svc.ObtenerPorIdAsync(id);
        if (usuario == null) return NotFound();

        var model = new UsuarioViewModel
        {
            UsuarioId = usuario.UsuarioId,
            NombreUsuario = usuario.NombreUsuario,
            NombreCompleto = usuario.NombreCompleto,
            RolId = usuario.RolId,
            Activo = usuario.Activo
        };

        ViewBag.Roles = new SelectList(await _context.Roles.ToListAsync(), "RolId", "Nombre", model.RolId);
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Editar(UsuarioViewModel model)
    {
        // Cuando editamos, la contraseña es opcional
        if (string.IsNullOrWhiteSpace(model.Password))
        {
            ModelState.Remove("Password");
        }

        if (!ModelState.IsValid)
        {
            ViewBag.Roles = new SelectList(await _context.Roles.ToListAsync(), "RolId", "Nombre", model.RolId);
            return View(model);
        }
        await _svc.ActualizarAsync(model);
        TempData["Exito"] = "Usuario actualizado correctamente.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Eliminar(int id)
    {
        await _svc.EliminarAsync(id);
        TempData["Exito"] = "Usuario desactivado correctamente.";
        return RedirectToAction(nameof(Index));
    }
}

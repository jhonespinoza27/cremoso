using CafeSales.Business.Interfaces;
using CafeSales.Models.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CafeSales.Web.Controllers;

[Authorize]
public class ClienteController : Controller
{
    private readonly IClienteService _svc;

    public ClienteController(IClienteService svc) => _svc = svc;

    public async Task<IActionResult> Index(string? busqueda)
    {
        var clientes = await _svc.BuscarAsync(busqueda);
        ViewBag.Busqueda = busqueda;
        return View(clientes);
    }

    public async Task<IActionResult> Detalles(int id)
    {
        var cliente = await _svc.ObtenerPorIdAsync(id);
        if (cliente == null) return NotFound();
        return View(cliente);
    }

    public IActionResult Crear() => View();

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Crear(Cliente cliente)
    {
        if (!ModelState.IsValid) return View(cliente);
        await _svc.CrearAsync(cliente);
        TempData["Exito"] = "Cliente registrado correctamente.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Editar(int id)
    {
        var cliente = await _svc.ObtenerPorIdAsync(id);
        if (cliente == null) return NotFound();
        return View(cliente);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Editar(Cliente cliente)
    {
        if (!ModelState.IsValid) return View(cliente);
        await _svc.ActualizarAsync(cliente);
        TempData["Exito"] = "Cliente actualizado correctamente.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> Eliminar(int id)
    {
        await _svc.EliminarAsync(id);
        TempData["Exito"] = "Cliente eliminado correctamente.";
        return RedirectToAction(nameof(Index));
    }
}

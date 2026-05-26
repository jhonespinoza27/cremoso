using CafeSales.Business.Interfaces;
using CafeSales.Data.Context;
using CafeSales.Models.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace CafeSales.Web.Controllers;

[Authorize]
public class ProductoController : Controller
{
    private readonly IProductoService _svc;
    private readonly CafeDbContext _context;

    public ProductoController(IProductoService svc, CafeDbContext context)
    {
        _svc = svc;
        _context = context;
    }

    public async Task<IActionResult> Index(string? busqueda, int? categoriaId)
    {
        var productos = await _svc.BuscarAsync(busqueda, categoriaId);
        ViewBag.Categorias = new SelectList(await _context.Categorias.OrderBy(c => c.Nombre).ToListAsync(), "CategoriaId", "Nombre", categoriaId);
        ViewBag.Busqueda = busqueda;
        ViewBag.CategoriaId = categoriaId;
        return View(productos);
    }

    public async Task<IActionResult> Detalles(int id)
    {
        var producto = await _svc.ObtenerPorIdAsync(id);
        if (producto == null) return NotFound();
        return View(producto);
    }

    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> Crear()
    {
        ViewBag.Categorias = new SelectList(await _context.Categorias.OrderBy(c => c.Nombre).ToListAsync(), "CategoriaId", "Nombre");
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> Crear(Producto producto)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.Categorias = new SelectList(await _context.Categorias.OrderBy(c => c.Nombre).ToListAsync(), "CategoriaId", "Nombre", producto.CategoriaId);
            return View(producto);
        }
        await _svc.CrearAsync(producto);
        TempData["Exito"] = "Producto creado correctamente.";
        return RedirectToAction(nameof(Index));
    }

    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> Editar(int id)
    {
        var producto = await _svc.ObtenerPorIdAsync(id);
        if (producto == null) return NotFound();
        ViewBag.Categorias = new SelectList(await _context.Categorias.OrderBy(c => c.Nombre).ToListAsync(), "CategoriaId", "Nombre", producto.CategoriaId);
        return View(producto);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> Editar(Producto producto)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.Categorias = new SelectList(await _context.Categorias.OrderBy(c => c.Nombre).ToListAsync(), "CategoriaId", "Nombre", producto.CategoriaId);
            return View(producto);
        }
        await _svc.ActualizarAsync(producto);
        TempData["Exito"] = "Producto actualizado correctamente.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> Eliminar(int id)
    {
        await _svc.EliminarAsync(id);
        TempData["Exito"] = "Producto eliminado correctamente.";
        return RedirectToAction(nameof(Index));
    }
}

using CafeSales.Business.Interfaces;
using CafeSales.Data.Context;
using CafeSales.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace CafeSales.Web.Controllers;

[Authorize]
public class VentaController : Controller
{
    private readonly IVentaService _ventaSvc;
    private readonly IProductoService _productoSvc;
    private readonly IClienteService _clienteSvc;
    private readonly CafeDbContext _context;

    public VentaController(IVentaService ventaSvc, IProductoService productoSvc,
        IClienteService clienteSvc, CafeDbContext context)
    {
        _ventaSvc = ventaSvc;
        _productoSvc = productoSvc;
        _clienteSvc = clienteSvc;
        _context = context;
    }

    public async Task<IActionResult> Index(DateTime? fechaInicio, DateTime? fechaFin, int? clienteId)
    {
        var ventas = await _ventaSvc.BuscarAsync(fechaInicio, fechaFin, clienteId);
        ViewBag.Clientes = new SelectList(await _clienteSvc.ObtenerTodosAsync(), "ClienteId", "Nombre", clienteId);
        ViewBag.FechaInicio = fechaInicio?.ToString("yyyy-MM-dd");
        ViewBag.FechaFin = fechaFin?.ToString("yyyy-MM-dd");
        ViewBag.ClienteId = clienteId;
        return View(ventas);
    }

    public async Task<IActionResult> Detalles(int id)
    {
        var venta = await _ventaSvc.ObtenerPorIdAsync(id);
        if (venta == null) return NotFound();
        return View(venta);
    }

    public async Task<IActionResult> Registrar()
    {
        ViewBag.Clientes = new SelectList(await _clienteSvc.ObtenerTodosAsync(), "ClienteId", "Nombre");
        ViewBag.Productos = await _productoSvc.ObtenerTodosAsync();
        return View(new VentaViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Registrar([FromForm] VentaViewModel model)
    {
        if (model.Detalles == null || model.Detalles.Count == 0)
        {
            ModelState.AddModelError("", "Debe agregar al menos un producto a la venta.");
        }

        if (!ModelState.IsValid)
        {
            await PrepararRegistroAsync(model.ClienteId);
            return View(model);
        }

        try
        {
            var usuarioId = int.Parse(User.FindFirst("UsuarioId")!.Value);
            await _ventaSvc.RegistrarVentaAsync(model, usuarioId);
            TempData["Exito"] = "Venta registrada correctamente.";
            return RedirectToAction(nameof(Index));
        }
        catch (InvalidOperationException ex)
        {
            ModelState.AddModelError("", ex.Message);
            await PrepararRegistroAsync(model.ClienteId);
            return View(model);
        }
    }

    [HttpGet]
    public async Task<IActionResult> ObtenerProductos()
    {
        var productos = await _context.Productos
            .Where(p => p.Activo && p.Stock > 0)
            .Select(p => new { p.ProductoId, p.Nombre, p.Precio, p.Stock })
            .OrderBy(p => p.Nombre)
            .ToListAsync();
        return Json(productos);
    }

    private async Task PrepararRegistroAsync(int? clienteId = null)
    {
        ViewBag.Clientes = new SelectList(await _clienteSvc.ObtenerTodosAsync(), "ClienteId", "Nombre", clienteId);
        ViewBag.Productos = await _productoSvc.ObtenerTodosAsync();
    }
}

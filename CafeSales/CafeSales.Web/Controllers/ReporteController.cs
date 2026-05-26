using CafeSales.Business.Interfaces;
using CafeSales.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CafeSales.Web.Controllers;

[Authorize(Roles = "Administrador")]
public class ReporteController : Controller
{
    private readonly IReporteService _svc;

    public ReporteController(IReporteService svc) => _svc = svc;

    public IActionResult Index() => View();

    public async Task<IActionResult> PorPeriodo(DateTime? fechaInicio, DateTime? fechaFin)
    {
        var fi = fechaInicio ?? DateTime.Now.AddMonths(-1);
        var ff = fechaFin ?? DateTime.Now;
        var model = await _svc.ObtenerVentasPorPeriodoAsync(fi, ff);
        return View(model);
    }

    public async Task<IActionResult> ProductosMasVendidos(DateTime? fechaInicio, DateTime? fechaFin)
    {
        var fi = fechaInicio ?? DateTime.Now.AddMonths(-1);
        var ff = fechaFin ?? DateTime.Now;
        var model = await _svc.ObtenerProductosMasVendidosAsync(fi, ff);
        return View(model);
    }

    public async Task<IActionResult> PorVendedor(DateTime? fechaInicio, DateTime? fechaFin)
    {
        var fi = fechaInicio ?? DateTime.Now.AddMonths(-1);
        var ff = fechaFin ?? DateTime.Now;
        var model = await _svc.ObtenerVentasPorVendedorAsync(fi, ff);
        return View(model);
    }
}

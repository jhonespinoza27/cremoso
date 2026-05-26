using CafeSales.Business.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CafeSales.Web.Controllers;

[Authorize]
public class DashboardController : Controller
{
    private readonly IReporteService _reporteService;

    public DashboardController(IReporteService reporteService)
    {
        _reporteService = reporteService;
    }

    public async Task<IActionResult> Index()
    {
        var model = await _reporteService.ObtenerDashboardAsync();
        return View(model);
    }
}

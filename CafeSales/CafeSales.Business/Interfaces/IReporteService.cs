using CafeSales.Models.ViewModels;

namespace CafeSales.Business.Interfaces;

public interface IReporteService
{
    Task<DashboardViewModel> ObtenerDashboardAsync();
    Task<ReporteVentasViewModel> ObtenerVentasPorPeriodoAsync(DateTime fechaInicio, DateTime fechaFin);
    Task<ReporteProductosViewModel> ObtenerProductosMasVendidosAsync(DateTime fechaInicio, DateTime fechaFin);
    Task<ReporteVendedoresViewModel> ObtenerVentasPorVendedorAsync(DateTime fechaInicio, DateTime fechaFin);
}

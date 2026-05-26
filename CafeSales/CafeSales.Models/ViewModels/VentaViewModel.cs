using System.ComponentModel.DataAnnotations;

namespace CafeSales.Models.ViewModels;

public class VentaViewModel
{
    [Required(ErrorMessage = "El cliente es obligatorio.")]
    [Range(1, int.MaxValue, ErrorMessage = "Seleccione un cliente válido.")]
    [Display(Name = "Cliente")]
    public int ClienteId { get; set; }

    [StringLength(500)]
    [Display(Name = "Observación")]
    public string? Observacion { get; set; }

    public List<DetalleVentaViewModel> Detalles { get; set; } = new();
}

public class DetalleVentaViewModel
{
    [Range(1, int.MaxValue, ErrorMessage = "Seleccione un producto válido.")]
    public int ProductoId { get; set; }
    public string NombreProducto { get; set; } = string.Empty;

    [Range(1, int.MaxValue, ErrorMessage = "La cantidad debe ser mayor a 0.")]
    public int Cantidad { get; set; }

    public decimal PrecioUnit { get; set; }
    public decimal Subtotal => Cantidad * PrecioUnit;
}

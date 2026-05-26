using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CafeSales.Models.Entities;

public class DetalleVenta
{
    public int DetalleId { get; set; }

    [Display(Name = "Venta")]
    public int VentaId { get; set; }

    [Required(ErrorMessage = "El producto es obligatorio.")]
    [Display(Name = "Producto")]
    public int ProductoId { get; set; }

    [Required(ErrorMessage = "La cantidad es obligatoria.")]
    [Range(1, int.MaxValue, ErrorMessage = "La cantidad debe ser mayor a 0.")]
    public int Cantidad { get; set; }

    [Required]
    [Display(Name = "Precio Unitario")]
    [DisplayFormat(DataFormatString = "{0:C2}")]
    public decimal PrecioUnit { get; set; }

    [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
    [DisplayFormat(DataFormatString = "{0:C2}")]
    public decimal? Subtotal { get; set; }

    // Navegación
    public Venta Venta { get; set; } = null!;
    public Producto Producto { get; set; } = null!;
}

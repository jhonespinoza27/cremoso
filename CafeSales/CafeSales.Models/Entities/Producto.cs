using System.ComponentModel.DataAnnotations;

namespace CafeSales.Models.Entities;

public class Producto
{
    public int ProductoId { get; set; }

    [Required(ErrorMessage = "El nombre del producto es obligatorio.")]
    [StringLength(200)]
    public string Nombre { get; set; } = string.Empty;

    [StringLength(500)]
    [Display(Name = "Descripción")]
    public string? Descripcion { get; set; }

    [Required(ErrorMessage = "El precio es obligatorio.")]
    [Range(0.01, 99999.99, ErrorMessage = "El precio debe ser mayor a 0.")]
    [DisplayFormat(DataFormatString = "{0:C2}")]
    public decimal Precio { get; set; }

    [Required(ErrorMessage = "El stock es obligatorio.")]
    [Range(0, int.MaxValue, ErrorMessage = "El stock no puede ser negativo.")]
    public int Stock { get; set; }

    [Required(ErrorMessage = "La categoría es obligatoria.")]
    [Display(Name = "Categoría")]
    public int CategoriaId { get; set; }

    public bool Activo { get; set; } = true;

    // Navegación
    public Categoria Categoria { get; set; } = null!;
    public ICollection<DetalleVenta> Detalles { get; set; } = new List<DetalleVenta>();
}

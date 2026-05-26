using System.ComponentModel.DataAnnotations;

namespace CafeSales.Models.Entities;

public class Venta
{
    public int VentaId { get; set; }

    [Required(ErrorMessage = "El cliente es obligatorio.")]
    [Display(Name = "Cliente")]
    public int ClienteId { get; set; }

    [Display(Name = "Vendedor")]
    public int UsuarioId { get; set; }

    public DateTime Fecha { get; set; } = DateTime.Now;

    [DisplayFormat(DataFormatString = "{0:C2}")]
    public decimal Total { get; set; }

    [StringLength(500)]
    [Display(Name = "Observación")]
    public string? Observacion { get; set; }

    // Navegación
    public Cliente Cliente { get; set; } = null!;
    public Usuario Usuario { get; set; } = null!;
    public ICollection<DetalleVenta> Detalles { get; set; } = new List<DetalleVenta>();
}

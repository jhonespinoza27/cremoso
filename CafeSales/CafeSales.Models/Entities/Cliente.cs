using System.ComponentModel.DataAnnotations;

namespace CafeSales.Models.Entities;

public class Cliente
{
    public int ClienteId { get; set; }

    [Required(ErrorMessage = "El nombre del cliente es obligatorio.")]
    [StringLength(200)]
    public string Nombre { get; set; } = string.Empty;

    [StringLength(20)]
    [Display(Name = "DNI / RUC")]
    public string? Documento { get; set; }

    [StringLength(20)]
    [Display(Name = "Teléfono")]
    public string? Telefono { get; set; }

    [StringLength(150)]
    [EmailAddress(ErrorMessage = "Ingrese un correo electrónico válido.")]
    public string? Correo { get; set; }

    [StringLength(300)]
    [Display(Name = "Dirección")]
    public string? Direccion { get; set; }

    public bool Activo { get; set; } = true;

    // Navegación
    public ICollection<Venta> Ventas { get; set; } = new List<Venta>();
}

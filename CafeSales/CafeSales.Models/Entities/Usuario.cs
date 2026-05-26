using System.ComponentModel.DataAnnotations;

namespace CafeSales.Models.Entities;

public class Usuario
{
    public int UsuarioId { get; set; }

    [Required(ErrorMessage = "El nombre de usuario es obligatorio.")]
    [StringLength(100)]
    public string NombreUsuario { get; set; } = string.Empty;

    [Required]
    [StringLength(256)]
    public string PasswordHash { get; set; } = string.Empty;

    [StringLength(200)]
    [Display(Name = "Nombre Completo")]
    public string? NombreCompleto { get; set; }

    [Required(ErrorMessage = "El rol es obligatorio.")]
    [Display(Name = "Rol")]
    public int RolId { get; set; }

    public bool Activo { get; set; } = true;

    [Display(Name = "Fecha de Creación")]
    public DateTime FechaCreacion { get; set; } = DateTime.Now;

    // Navegación
    public Rol Rol { get; set; } = null!;
    public ICollection<Venta> Ventas { get; set; } = new List<Venta>();
}

using System.ComponentModel.DataAnnotations;

namespace CafeSales.Models.Entities;

public class Rol
{
    public int RolId { get; set; }

    [Required(ErrorMessage = "El nombre del rol es obligatorio.")]
    [StringLength(50)]
    public string Nombre { get; set; } = string.Empty;

    // Navegación
    public ICollection<Usuario> Usuarios { get; set; } = new List<Usuario>();
}

using System.ComponentModel.DataAnnotations;

namespace CafeSales.Models.Entities;

public class Categoria
{
    public int CategoriaId { get; set; }

    [Required(ErrorMessage = "El nombre de la categoría es obligatorio.")]
    [StringLength(100)]
    public string Nombre { get; set; } = string.Empty;

    // Navegación
    public ICollection<Producto> Productos { get; set; } = new List<Producto>();
}

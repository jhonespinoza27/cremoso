using CafeSales.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace CafeSales.Data.Context;

public class CafeDbContext : DbContext
{
    public CafeDbContext(DbContextOptions<CafeDbContext> options)
        : base(options) { }

    public DbSet<Producto> Productos { get; set; }
    public DbSet<Cliente> Clientes { get; set; }
    public DbSet<Venta> Ventas { get; set; }
    public DbSet<DetalleVenta> DetalleVentas { get; set; }
    public DbSet<Usuario> Usuarios { get; set; }
    public DbSet<Rol> Roles { get; set; }
    public DbSet<Categoria> Categorias { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // ──── Configuración de tablas ────
        modelBuilder.Entity<Rol>(entity =>
        {
            entity.HasKey(e => e.RolId);
            entity.HasIndex(e => e.Nombre).IsUnique();
        });

        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.HasKey(e => e.UsuarioId);
            entity.HasIndex(e => e.NombreUsuario).IsUnique();
            entity.HasOne(e => e.Rol)
                  .WithMany(r => r.Usuarios)
                  .HasForeignKey(e => e.RolId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Categoria>(entity =>
        {
            entity.HasKey(e => e.CategoriaId);
            entity.HasIndex(e => e.Nombre).IsUnique();
        });

        modelBuilder.Entity<Producto>(entity =>
        {
            entity.HasKey(e => e.ProductoId);
            entity.Property(e => e.Precio).HasColumnType("decimal(10,2)");
            entity.HasOne(e => e.Categoria)
                  .WithMany(c => c.Productos)
                  .HasForeignKey(e => e.CategoriaId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Cliente>(entity =>
        {
            entity.HasKey(e => e.ClienteId);
        });

        modelBuilder.Entity<Venta>(entity =>
        {
            entity.HasKey(e => e.VentaId);
            entity.Property(e => e.Total).HasColumnType("decimal(10,2)");
            entity.HasOne(e => e.Cliente)
                  .WithMany(c => c.Ventas)
                  .HasForeignKey(e => e.ClienteId)
                  .OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(e => e.Usuario)
                  .WithMany(u => u.Ventas)
                  .HasForeignKey(e => e.UsuarioId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<DetalleVenta>(entity =>
        {
            entity.HasKey(e => e.DetalleId);
            entity.Property(e => e.PrecioUnit).HasColumnType("decimal(10,2)");
            entity.Property(e => e.Subtotal)
                  .HasColumnType("decimal(21,4)")
                  .HasComputedColumnSql("[Cantidad] * [PrecioUnit]");
            entity.HasOne(e => e.Venta)
                  .WithMany(v => v.Detalles)
                  .HasForeignKey(e => e.VentaId)
                  .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(e => e.Producto)
                  .WithMany(p => p.Detalles)
                  .HasForeignKey(e => e.ProductoId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        // ──── Datos semilla ────
        // Roles
        modelBuilder.Entity<Rol>().HasData(
            new Rol { RolId = 1, Nombre = "Administrador" },
            new Rol { RolId = 2, Nombre = "Vendedor" }
        );

        // Categorías de cafetería
        modelBuilder.Entity<Categoria>().HasData(
            new Categoria { CategoriaId = 1, Nombre = "Café" },
            new Categoria { CategoriaId = 2, Nombre = "Té e Infusiones" },
            new Categoria { CategoriaId = 3, Nombre = "Bebidas Frías" },
            new Categoria { CategoriaId = 4, Nombre = "Panadería" },
            new Categoria { CategoriaId = 5, Nombre = "Postres" },
            new Categoria { CategoriaId = 6, Nombre = "Snacks" }
        );

        // Usuario administrador por defecto (password: Admin123!)
        // BCrypt hash generado para "Admin123!"
        modelBuilder.Entity<Usuario>().HasData(
            new Usuario
            {
                UsuarioId = 1,
                NombreUsuario = "admin",
                PasswordHash = "$2a$11$1mbo37LKlFsyb6/chL/E0.A4slbSlgUcmuQ0Moq3s6RRylHzQuORa",
                NombreCompleto = "Administrador del Sistema",
                RolId = 1,
                Activo = true,
                FechaCreacion = new DateTime(2026, 1, 1)
            }
        );

        // Cliente genérico
        modelBuilder.Entity<Cliente>().HasData(
            new Cliente
            {
                ClienteId = 1,
                Nombre = "Cliente General",
                Documento = "00000000",
                Activo = true
            }
        );

        // Productos de ejemplo
        modelBuilder.Entity<Producto>().HasData(
            new Producto { ProductoId = 1, Nombre = "Café Americano", Descripcion = "Café negro filtrado", Precio = 3.50m, Stock = 100, CategoriaId = 1, Activo = true },
            new Producto { ProductoId = 2, Nombre = "Cappuccino", Descripcion = "Espresso con leche espumada", Precio = 5.00m, Stock = 80, CategoriaId = 1, Activo = true },
            new Producto { ProductoId = 3, Nombre = "Latte", Descripcion = "Café con leche cremosa", Precio = 5.50m, Stock = 80, CategoriaId = 1, Activo = true },
            new Producto { ProductoId = 4, Nombre = "Espresso", Descripcion = "Shot de café concentrado", Precio = 2.50m, Stock = 120, CategoriaId = 1, Activo = true },
            new Producto { ProductoId = 5, Nombre = "Té Verde", Descripcion = "Té verde japonés", Precio = 3.00m, Stock = 60, CategoriaId = 2, Activo = true },
            new Producto { ProductoId = 6, Nombre = "Frappé de Mocha", Descripcion = "Bebida fría de café y chocolate", Precio = 7.00m, Stock = 50, CategoriaId = 3, Activo = true },
            new Producto { ProductoId = 7, Nombre = "Croissant", Descripcion = "Croissant de mantequilla artesanal", Precio = 3.50m, Stock = 40, CategoriaId = 4, Activo = true },
            new Producto { ProductoId = 8, Nombre = "Cheesecake", Descripcion = "Tarta de queso con frutos rojos", Precio = 6.50m, Stock = 25, CategoriaId = 5, Activo = true },
            new Producto { ProductoId = 9, Nombre = "Muffin de Arándanos", Descripcion = "Muffin casero con arándanos frescos", Precio = 4.00m, Stock = 35, CategoriaId = 4, Activo = true },
            new Producto { ProductoId = 10, Nombre = "Galletas de Avena", Descripcion = "Paquete de galletas artesanales", Precio = 3.00m, Stock = 45, CategoriaId = 6, Activo = true }
        );
    }
}

using CafeSales.Business.Services;
using CafeSales.Data.Context;
using CafeSales.Models.Entities;
using CafeSales.Models.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace CafeSales.Tests;

public class UsuarioServiceTests
{
    private static async Task<CafeDbContext> CrearContextoConSemillaAsync()
    {
        return await SqlServerTestDatabase.CreateSeededContextAsync();
    }

    [Fact]
    public async Task ObtenerTodos_DebeOrdenarUsuariosEIncluirRol()
    {
        await using var context = await CrearContextoConSemillaAsync();
        context.Usuarios.Add(new Usuario
        {
            NombreUsuario = "vendedor",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("Venta123!"),
            RolId = 2,
            Activo = true
        });
        await context.SaveChangesAsync();
        var service = new UsuarioService(context);

        var resultado = (await service.ObtenerTodosAsync()).ToList();

        Assert.Equal(["admin", "vendedor"], resultado.Select(u => u.NombreUsuario));
        Assert.Equal("Administrador", resultado[0].Rol.Nombre);
        Assert.Equal("Vendedor", resultado[1].Rol.Nombre);
    }

    [Fact]
    public async Task ObtenerPorId_DebeRetornarUsuarioConRol()
    {
        await using var context = await CrearContextoConSemillaAsync();
        var service = new UsuarioService(context);

        var resultado = await service.ObtenerPorIdAsync(1);

        Assert.NotNull(resultado);
        Assert.Equal("admin", resultado.NombreUsuario);
        Assert.Equal("Administrador", resultado.Rol.Nombre);
    }

    [Fact]
    public async Task ObtenerPorId_IdInexistente_DebeRetornarNull()
    {
        await using var context = await CrearContextoConSemillaAsync();
        var service = new UsuarioService(context);

        var resultado = await service.ObtenerPorIdAsync(999);

        Assert.Null(resultado);
    }

    [Fact]
    public async Task ValidarCredenciales_DebeAutenticarAdministradorSemilla()
    {
        await using var context = await CrearContextoConSemillaAsync();
        var service = new UsuarioService(context);

        var usuario = await service.ValidarCredencialesAsync("admin", "Admin123!");

        Assert.NotNull(usuario);
        Assert.Equal("Administrador", usuario.Rol.Nombre);
    }

    [Theory]
    [InlineData("admin", "Incorrecta")]
    [InlineData("no-existe", "Admin123!")]
    public async Task ValidarCredenciales_DatosIncorrectos_DebeRetornarNull(
        string nombreUsuario, string password)
    {
        await using var context = await CrearContextoConSemillaAsync();
        var service = new UsuarioService(context);

        var resultado = await service.ValidarCredencialesAsync(nombreUsuario, password);

        Assert.Null(resultado);
    }

    [Fact]
    public async Task ValidarCredenciales_UsuarioInactivo_DebeRetornarNull()
    {
        await using var context = await CrearContextoConSemillaAsync();
        var admin = await context.Usuarios.FindAsync(1);
        admin!.Activo = false;
        await context.SaveChangesAsync();
        var service = new UsuarioService(context);

        var resultado = await service.ValidarCredencialesAsync("admin", "Admin123!");

        Assert.Null(resultado);
    }

    [Fact]
    public async Task Crear_DebeGuardarUsuarioActivoConPasswordHasheada()
    {
        await using var context = await CrearContextoConSemillaAsync();
        var service = new UsuarioService(context);
        var model = new UsuarioViewModel
        {
            NombreUsuario = "rosa",
            NombreCompleto = "Rosa Huaman",
            Password = "Rosa123!",
            RolId = 2
        };

        await service.CrearAsync(model);

        var guardado = await context.Usuarios.SingleAsync(u => u.NombreUsuario == "rosa");
        Assert.True(guardado.Activo);
        Assert.True(BCrypt.Net.BCrypt.Verify("Rosa123!", guardado.PasswordHash));
    }

    [Fact]
    public async Task Actualizar_SinPassword_DebeMantenerHashYModificarDatos()
    {
        await using var context = await CrearContextoConSemillaAsync();
        var original = (await context.Usuarios.FindAsync(1))!.PasswordHash;
        var service = new UsuarioService(context);
        var model = new UsuarioViewModel
        {
            UsuarioId = 1,
            NombreUsuario = "administrador",
            NombreCompleto = "Admin Actualizado",
            Password = " ",
            RolId = 1,
            Activo = true
        };

        await service.ActualizarAsync(model);

        var actualizado = await context.Usuarios.FindAsync(1);
        Assert.Equal("administrador", actualizado!.NombreUsuario);
        Assert.Equal("Admin Actualizado", actualizado.NombreCompleto);
        Assert.Equal(original, actualizado.PasswordHash);
    }

    [Fact]
    public async Task Actualizar_ConPassword_DebeCambiarCredencial()
    {
        await using var context = await CrearContextoConSemillaAsync();
        var service = new UsuarioService(context);
        var model = new UsuarioViewModel
        {
            UsuarioId = 1,
            NombreUsuario = "admin",
            NombreCompleto = "Administrador del Sistema",
            Password = "NuevaClave123!",
            RolId = 1,
            Activo = true
        };

        await service.ActualizarAsync(model);

        var actualizado = await context.Usuarios.FindAsync(1);
        Assert.True(BCrypt.Net.BCrypt.Verify("NuevaClave123!", actualizado!.PasswordHash));
    }

    [Fact]
    public async Task Actualizar_IdInexistente_NoDebeLanzarExcepcion()
    {
        await using var context = await CrearContextoConSemillaAsync();
        var service = new UsuarioService(context);
        var model = new UsuarioViewModel
        {
            UsuarioId = 999,
            NombreUsuario = "no-existe",
            Password = "Clave123!",
            RolId = 2,
            Activo = true
        };

        var excepcion = await Record.ExceptionAsync(() => service.ActualizarAsync(model));

        Assert.Null(excepcion);
        Assert.Single(context.Usuarios);
    }

    [Fact]
    public async Task Eliminar_DebeRealizarBajaLogica()
    {
        await using var context = await CrearContextoConSemillaAsync();
        var service = new UsuarioService(context);

        await service.EliminarAsync(1);

        var admin = await context.Usuarios.FindAsync(1);
        Assert.False(admin!.Activo);
        Assert.Equal("admin", admin.NombreUsuario);
    }

    [Fact]
    public async Task Eliminar_IdInexistente_NoDebeLanzarExcepcion()
    {
        await using var context = await CrearContextoConSemillaAsync();
        var service = new UsuarioService(context);

        var excepcion = await Record.ExceptionAsync(() => service.EliminarAsync(999));

        Assert.Null(excepcion);
    }
}

using CafeSales.Business.Services;
using CafeSales.Data.Context;
using CafeSales.Models.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;




//using CafeSales.Business.Services;
//using CafeSales.Data.Context;
//using CafeSales.Models.Entities;
//using Microsoft.EntityFrameworkCore;
//using Xunit;

namespace CafeSales.Tests;

public class ClienteServiceTests
{
    // Crea una base SQL Server limpia y agrega los datos requeridos por cada prueba.
    private async Task<CafeDbContext> CrearContextoConDatos(string _)
    {
        var context = await SqlServerTestDatabase.CreateEmptyContextAsync();

        // Seed: datos exactos de tu tabla Clientes
        context.Clientes.AddRange(
            new Cliente
            {
                Nombre = "Cliente General",
                Documento = "00000000",
                Telefono = null,
                Correo = null,
                Direccion = null,
                Activo = true
            },
            new Cliente
            {
                Nombre = "jhon",
                Documento = "71294392",
                Telefono = "967310540",
                Correo = "jeverjhon.27@gmail.com",
                Direccion = "huamanga",
                Activo = true
            },
            new Cliente
            {
                Nombre = "Maria Lopez",
                Documento = "74581236",
                Telefono = "987654321",
                Correo = "maria@gmail.com",
                Direccion = "Huamanga",
                Activo = true
            },
            new Cliente
            {
                Nombre = "Carlos Ramos",
                Documento = "70112233",
                Telefono = "965874123",
                Correo = "carlos@gmail.com",
                Direccion = "Huamanga",
                Activo = true
            },
            new Cliente
            {
                Nombre = "Ana Torres",
                Documento = "78945612",
                Telefono = "912345678",
                Correo = "ana@gmail.com",
                Direccion = "Huamanga",
                Activo = true
            },
            new Cliente
            {
                Nombre = "Luis Quispe",
                Documento = "73698521",
                Telefono = "998877665",
                Correo = "luis@gmail.com",
                Direccion = "Huamanga",
                Activo = true
            }
        );

        await context.SaveChangesAsync();
        return context;
    }

    // ════════════════════════════════════════════════════════════════════════
    // ObtenerTodosAsync
    // ════════════════════════════════════════════════════════════════════════

    [Fact]
    public async Task ObtenerTodos_DebeRetornar6ClientesActivos()
    {
        // Arrange
        using var context = await CrearContextoConDatos(
            nameof(ObtenerTodos_DebeRetornar6ClientesActivos));
        var service = new ClienteService(context);

        // Act
        var resultado = await service.ObtenerTodosAsync();

        // Assert
        Assert.Equal(6, resultado.Count());
    }

    [Fact]
    public async Task ObtenerTodos_DebeRetornarOrdenadosPorNombre()
    {
        // Arrange
        using var context = await CrearContextoConDatos(
            nameof(ObtenerTodos_DebeRetornarOrdenadosPorNombre));
        var service = new ClienteService(context);

        // Act
        var resultado = (await service.ObtenerTodosAsync()).ToList();

        // Assert — Orden alfabético A → Z
        Assert.Equal("Ana Torres", resultado[0].Nombre);
        Assert.Equal("Carlos Ramos", resultado[1].Nombre);
        Assert.Equal("Cliente General", resultado[2].Nombre);
        Assert.Equal("jhon", resultado[3].Nombre);
        Assert.Equal("Luis Quispe", resultado[4].Nombre);
        Assert.Equal("Maria Lopez", resultado[5].Nombre);
    }

    [Fact]
    public async Task ObtenerTodos_TodosDebenEstarActivos()
    {
        // Arrange
        using var context = await CrearContextoConDatos(
            nameof(ObtenerTodos_TodosDebenEstarActivos));
        var service = new ClienteService(context);

        // Act
        var resultado = await service.ObtenerTodosAsync();

        // Assert
        Assert.All(resultado, c => Assert.True(c.Activo));
    }

    // ════════════════════════════════════════════════════════════════════════
    // BuscarAsync — por Nombre
    // ════════════════════════════════════════════════════════════════════════

    [Fact]
    public async Task Buscar_PorNombre_Maria_DebeRetornar1Resultado()
    {
        // Arrange
        using var context = await CrearContextoConDatos(
            nameof(Buscar_PorNombre_Maria_DebeRetornar1Resultado));
        var service = new ClienteService(context);

        // Act
        var resultado = await service.BuscarAsync("Maria");

        // Assert
        Assert.Single(resultado);
        Assert.Equal("Maria Lopez", resultado.First().Nombre);
    }

    [Fact]
    public async Task Buscar_PorNombreParcial_Lopez_DebeRetornarMaria()
    {
        // Arrange
        using var context = await CrearContextoConDatos(
            nameof(Buscar_PorNombreParcial_Lopez_DebeRetornarMaria));
        var service = new ClienteService(context);

        // Act
        var resultado = await service.BuscarAsync("Lopez");

        // Assert
        Assert.Single(resultado);
        Assert.Equal("74581236", resultado.First().Documento);
    }

    // ════════════════════════════════════════════════════════════════════════
    // BuscarAsync — por Documento
    // ════════════════════════════════════════════════════════════════════════

    [Fact]
    public async Task Buscar_PorDocumento_71294392_DebeRetornarJhon()
    {
        // Arrange
        using var context = await CrearContextoConDatos(
            nameof(Buscar_PorDocumento_71294392_DebeRetornarJhon));
        var service = new ClienteService(context);

        // Act
        var resultado = await service.BuscarAsync("71294392");

        // Assert
        Assert.Single(resultado);
        Assert.Equal("jhon", resultado.First().Nombre);
    }

    [Fact]
    public async Task Buscar_PorDocumentoGeneral_00000000_DebeRetornarClienteGeneral()
    {
        // Arrange
        using var context = await CrearContextoConDatos(
            nameof(Buscar_PorDocumentoGeneral_00000000_DebeRetornarClienteGeneral));
        var service = new ClienteService(context);

        // Act
        var resultado = await service.BuscarAsync("00000000");

        // Assert
        Assert.Single(resultado);
        Assert.Equal("Cliente General", resultado.First().Nombre);
    }

    [Fact]
    public async Task Buscar_PorDocumentoParcial_736_DebeRetornarLuis()
    {
        // Arrange
        using var context = await CrearContextoConDatos(
            nameof(Buscar_PorDocumentoParcial_736_DebeRetornarLuis));
        var service = new ClienteService(context);

        // Act
        var resultado = await service.BuscarAsync("736");

        // Assert
        Assert.Single(resultado);
        Assert.Equal("Luis Quispe", resultado.First().Nombre);
    }

    // ════════════════════════════════════════════════════════════════════════
    // BuscarAsync — por Correo
    // ════════════════════════════════════════════════════════════════════════

    [Fact]
    public async Task Buscar_PorCorreo_JhonGmail_DebeRetornarJhon()
    {
        // Arrange
        using var context = await CrearContextoConDatos(
            nameof(Buscar_PorCorreo_JhonGmail_DebeRetornarJhon));
        var service = new ClienteService(context);

        // Act
        var resultado = await service.BuscarAsync("jeverjhon.27@gmail.com");

        // Assert
        Assert.Single(resultado);
        Assert.Equal("jhon", resultado.First().Nombre);
    }

    [Fact]
    public async Task Buscar_PorCorreoParcial_gmail_DebeRetornar5Clientes()
    {
        // Arrange
        using var context = await CrearContextoConDatos(
            nameof(Buscar_PorCorreoParcial_gmail_DebeRetornar5Clientes));
        var service = new ClienteService(context);

        // Act
        // jhon, Maria, Carlos, Ana, Luis tienen @gmail.com
        // Cliente General NO tiene correo (NULL)
        var resultado = await service.BuscarAsync("gmail.com");

        // Assert
        Assert.Equal(5, resultado.Count());
    }

    // ════════════════════════════════════════════════════════════════════════
    // BuscarAsync — sin filtro / vacío
    // ════════════════════════════════════════════════════════════════════════

    [Fact]
    public async Task Buscar_SinFiltroNull_DebeRetornarLos6Clientes()
    {
        // Arrange
        using var context = await CrearContextoConDatos(
            nameof(Buscar_SinFiltroNull_DebeRetornarLos6Clientes));
        var service = new ClienteService(context);

        // Act
        var resultado = await service.BuscarAsync(null);

        // Assert
        Assert.Equal(6, resultado.Count());
    }

    [Fact]
    public async Task Buscar_ConEspaciosEnBlanco_DebeRetornarLos6Clientes()
    {
        // Arrange
        using var context = await CrearContextoConDatos(
            nameof(Buscar_ConEspaciosEnBlanco_DebeRetornarLos6Clientes));
        var service = new ClienteService(context);

        // Act
        var resultado = await service.BuscarAsync("   ");

        // Assert
        Assert.Equal(6, resultado.Count());
    }

    // ════════════════════════════════════════════════════════════════════════
    // ObtenerPorIdAsync
    // ════════════════════════════════════════════════════════════════════════

    [Fact]
    public async Task ObtenerPorId_Id2_DebeRetornarJhon()
    {
        // Arrange
        using var context = await CrearContextoConDatos(
            nameof(ObtenerPorId_Id2_DebeRetornarJhon));
        var service = new ClienteService(context);

        // Act
        var resultado = await service.ObtenerPorIdAsync(2);

        // Assert
        Assert.NotNull(resultado);
        Assert.Equal("jhon", resultado!.Nombre);
        Assert.Equal("jeverjhon.27@gmail.com", resultado.Correo);
        Assert.Equal("967310540", resultado.Telefono);
    }

    [Fact]
    public async Task ObtenerPorId_Id1_DebeRetornarClienteGeneral()
    {
        // Arrange
        using var context = await CrearContextoConDatos(
            nameof(ObtenerPorId_Id1_DebeRetornarClienteGeneral));
        var service = new ClienteService(context);

        // Act
        var resultado = await service.ObtenerPorIdAsync(1);

        // Assert
        Assert.NotNull(resultado);
        Assert.Equal("Cliente General", resultado!.Nombre);
        Assert.Null(resultado.Correo);      // NULL en tu BD
        Assert.Null(resultado.Telefono);    // NULL en tu BD
        Assert.Null(resultado.Direccion);   // NULL en tu BD
    }

    [Fact]
    public async Task ObtenerPorId_IdInexistente_DebeRetornarNull()
    {
        // Arrange
        using var context = await CrearContextoConDatos(
            nameof(ObtenerPorId_IdInexistente_DebeRetornarNull));
        var service = new ClienteService(context);

        // Act
        var resultado = await service.ObtenerPorIdAsync(999);

        // Assert
        Assert.Null(resultado);
    }

    // ════════════════════════════════════════════════════════════════════════
    // CrearAsync
    // ════════════════════════════════════════════════════════════════════════

    [Fact]
    public async Task Crear_NuevoCliente_DebePasarDe6A7Registros()
    {
        // Arrange
        using var context = await CrearContextoConDatos(
            nameof(Crear_NuevoCliente_DebePasarDe6A7Registros));
        var service = new ClienteService(context);

        var nuevoCliente = new Cliente
        {
            Nombre = "Rosa Huaman",
            Documento = "76543210",
            Telefono = "911223344",
            Correo = "rosa@gmail.com",
            Direccion = "Huamanga",
            Activo = true
        };

        // Act
        await service.CrearAsync(nuevoCliente);

        // Assert
        Assert.Equal(7, await context.Clientes.CountAsync());
        var guardado = await context.Clientes.FindAsync(7);
        Assert.Equal("Rosa Huaman", guardado!.Nombre);
    }

    // ════════════════════════════════════════════════════════════════════════
    // ActualizarAsync
    // ════════════════════════════════════════════════════════════════════════

    [Fact]
    public async Task Actualizar_CorreoDeJhon_DebeQuedarActualizado()
    {
        // Arrange
        using var context = await CrearContextoConDatos(
            nameof(Actualizar_CorreoDeJhon_DebeQuedarActualizado));
        var service = new ClienteService(context);

        var jhon = await context.Clientes.FindAsync(2);

        // Act
        jhon!.Correo = "jhon.nuevo@gmail.com";
        await service.ActualizarAsync(jhon);

        // Assert
        var actualizado = await context.Clientes.FindAsync(2);
        Assert.Equal("jhon.nuevo@gmail.com", actualizado!.Correo);
    }

    [Fact]
    public async Task Actualizar_TelefonoDeAna_DebeQuedarActualizado()
    {
        // Arrange
        using var context = await CrearContextoConDatos(
            nameof(Actualizar_TelefonoDeAna_DebeQuedarActualizado));
        var service = new ClienteService(context);

        var ana = await context.Clientes.FindAsync(5);

        // Act
        ana!.Telefono = "999000111";
        await service.ActualizarAsync(ana);

        // Assert
        var actualizado = await context.Clientes.FindAsync(5);
        Assert.Equal("999000111", actualizado!.Telefono);
    }

    // ════════════════════════════════════════════════════════════════════════
    // EliminarAsync (Baja lógica)
    // ════════════════════════════════════════════════════════════════════════

    [Fact]
    public async Task Eliminar_Carlos_DebePonerActivoEnFalse()
    {
        // Arrange
        using var context = await CrearContextoConDatos(
            nameof(Eliminar_Carlos_DebePonerActivoEnFalse));
        var service = new ClienteService(context);

        // Act
        await service.EliminarAsync(4); // Carlos Ramos

        // Assert
        var carlos = await context.Clientes.FindAsync(4);
        Assert.NotNull(carlos);
        Assert.False(carlos!.Activo);           // Baja lógica ✅
        Assert.Equal("Carlos Ramos", carlos.Nombre); // Sigue existiendo en BD
    }

    [Fact]
    public async Task Eliminar_Luis_DebeDesaparecerDeObtenerTodos()
    {
        // Arrange
        using var context = await CrearContextoConDatos(
            nameof(Eliminar_Luis_DebeDesaparecerDeObtenerTodos));
        var service = new ClienteService(context);

        // Act
        await service.EliminarAsync(6); // Luis Quispe
        var resultado = await service.ObtenerTodosAsync();

        // Assert — ObtenerTodos solo retorna activos
        Assert.Equal(5, resultado.Count());
        Assert.DoesNotContain(resultado, c => c.Nombre == "Luis Quispe");
    }

    [Fact]
    public async Task Eliminar_IdInexistente_NoDebeLanzarExcepcion()
    {
        // Arrange
        using var context = await CrearContextoConDatos(
            nameof(Eliminar_IdInexistente_NoDebeLanzarExcepcion));
        var service = new ClienteService(context);

        // Act & Assert
        var excepcion = await Record.ExceptionAsync(() => service.EliminarAsync(999));
        Assert.Null(excepcion);
    }
}


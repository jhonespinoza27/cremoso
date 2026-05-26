using CafeSales.Data.Context;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace CafeSales.Tests;

internal static class SqlServerTestDatabase
{
    private const string DefaultConnectionString =
        "Server=JHON;Database=CafeSalesDB_Tests;Trusted_Connection=True;TrustServerCertificate=True;";

    private static string ConnectionString
    {
        get
        {
            var connectionString = Environment.GetEnvironmentVariable("CAFE_TEST_CONNECTION_STRING")
                ?? DefaultConnectionString;
            var builder = new SqlConnectionStringBuilder(connectionString);

            if (string.IsNullOrWhiteSpace(builder.InitialCatalog)
                || !builder.InitialCatalog.Contains("Test", StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException(
                    "CAFE_TEST_CONNECTION_STRING debe apuntar a una base aislada cuyo nombre contenga 'Test'.");
            }

            return connectionString;
        }
    }

    public static async Task<CafeDbContext> CreateSeededContextAsync()
    {
        var options = new DbContextOptionsBuilder<CafeDbContext>()
            .UseSqlServer(ConnectionString)
            .Options;
        var context = new CafeDbContext(options);

        await context.Database.EnsureDeletedAsync();
        await context.Database.EnsureCreatedAsync();

        return context;
    }

    public static async Task<CafeDbContext> CreateEmptyContextAsync()
    {
        var context = await CreateSeededContextAsync();

        await context.DetalleVentas.ExecuteDeleteAsync();
        await context.Ventas.ExecuteDeleteAsync();
        await context.Productos.ExecuteDeleteAsync();
        await context.Clientes.ExecuteDeleteAsync();
        await context.Usuarios.ExecuteDeleteAsync();
        await context.Categorias.ExecuteDeleteAsync();
        await context.Roles.ExecuteDeleteAsync();
        // These tables have no model seed rows; on a never-populated table SQL Server
        // uses the reseed value for the first insert rather than incrementing it.
        await context.Database.ExecuteSqlRawAsync("DBCC CHECKIDENT ('DetalleVentas', RESEED, 1);");
        await context.Database.ExecuteSqlRawAsync("DBCC CHECKIDENT ('Ventas', RESEED, 1);");
        await context.Database.ExecuteSqlRawAsync("DBCC CHECKIDENT ('Productos', RESEED, 0);");
        await context.Database.ExecuteSqlRawAsync("DBCC CHECKIDENT ('Clientes', RESEED, 0);");
        await context.Database.ExecuteSqlRawAsync("DBCC CHECKIDENT ('Usuarios', RESEED, 0);");
        await context.Database.ExecuteSqlRawAsync("DBCC CHECKIDENT ('Categorias', RESEED, 0);");
        await context.Database.ExecuteSqlRawAsync("DBCC CHECKIDENT ('Roles', RESEED, 0);");

        return context;
    }
}

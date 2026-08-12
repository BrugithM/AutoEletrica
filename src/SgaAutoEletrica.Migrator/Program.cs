using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SgaAutoEletrica.Application;
using SgaAutoEletrica.Infrastructure;
using SgaAutoEletrica.Infrastructure.Persistence;
using SgaAutoEletrica.Infrastructure.Persistence.Context;

var services = new ServiceCollection();

var dbPath = Path.Combine(
    Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
    "SgaAutoEletrica",
    "sga_dev.db");

Directory.CreateDirectory(Path.GetDirectoryName(dbPath)!);

services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite($"Data Source={dbPath}"));

services.AddInfrastructure();
services.AddLogging();
services.AddApplication();

var serviceProvider = services.BuildServiceProvider();

using var context = serviceProvider.GetRequiredService<AppDbContext>();

Console.WriteLine($"Banco: {dbPath}");
Console.WriteLine("Aplicando migrações...");
context.Database.Migrate();
Console.WriteLine("Banco atualizado.");

Console.WriteLine("Inserindo dados de teste...");
await DataSeeder.SeedAsync(context);

Console.WriteLine("Pronto! Pressione qualquer tecla para sair...");
Console.ReadKey();
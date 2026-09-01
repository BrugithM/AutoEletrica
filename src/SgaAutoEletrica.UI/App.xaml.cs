using System.IO;
using System.Windows;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SgaAutoEletrica.Application;
using SgaAutoEletrica.Application.Common.Interfaces;
using SgaAutoEletrica.Infrastructure;
using SgaAutoEletrica.Infrastructure.Persistence.Context;

namespace SgaAutoEletrica.UI;

public partial class App : System.Windows.Application
{
    public static IServiceProvider ServiceProvider { get; private set; } = null!;

    public App()
    {
        DispatcherUnhandledException += App_DispatcherUnhandledException;
        AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
    }

    private void App_DispatcherUnhandledException(object sender,
    System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
    {
        MessageBox.Show(
            $"Erro na interface:\n\n{e.Exception.Message}\n\n{e.Exception.StackTrace}",
            "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
        e.Handled = true;
    }

    private void CurrentDomain_UnhandledException(object sender,
        UnhandledExceptionEventArgs e)
    {
        var ex = e.ExceptionObject as Exception;
        MessageBox.Show(
            $"Erro fatal:\n\n{ex?.Message}\n\n{ex?.StackTrace}",
            "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
    }
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        try
        {
            var services = new ServiceCollection();

            var dbPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "SgaAutoEletrica",
            "sga_dev.db"
            );

            Directory.CreateDirectory(Path.GetDirectoryName(dbPath)!);

            Console.WriteLine($"Banco: {dbPath}");
            Console.WriteLine($"Existe? {File.Exists(dbPath)}");

            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlite($"Data Source={dbPath}"),
                ServiceLifetime.Transient);

            services.AddInfrastructure();
            services.AddLogging();
            services.AddApplication();

            services.AddTransient<MainWindow>();

            services.AddTransient<ViewModels.Dashboard.DashboardViewModel>();
            services.AddTransient<Views.Dashboard.DashboardView>();

            services.AddTransient<ViewModels.Clientes.ListaClientesViewModel>();
            services.AddTransient<Views.Clientes.ListaClientesView>();

            services.AddTransient<ViewModels.Veiculos.ListaVeiculosViewModel>();
            services.AddTransient<Views.Veiculos.ListaVeiculosView>();

            services.AddTransient<ViewModels.Pecas.ListaPecasViewModel>();
            services.AddTransient<Views.Pecas.ListaPecasView>();

            services.AddTransient<ViewModels.Servicos.ListaServicosViewModel>();
            services.AddTransient<Views.Servicos.ListaServicosView>();

            services.AddTransient<ViewModels.Fornecedores.ListaFornecedoresViewModel>();
            services.AddTransient<Views.Fornecedores.ListaFornecedoresView>();

            services.AddTransient<ViewModels.OrdensServico.ListaOSViewModel>();
            services.AddTransient<Views.OrdensServico.ListaOSView>();

            services.AddTransient<ViewModels.OrdensServico.ListaNotasFiscaisViewModel>();
            services.AddTransient<Views.OrdensServico.ListaNotasFiscaisView>();

            services.AddTransient<ViewModels.Fornecedores.CriarNotaFiscalEntradaViewModel>();

            services.AddTransient<ViewModels.Buscas.BuscarProdutosViewModel>();
            services.AddTransient<Views.Buscas.BuscarProdutosView>();

            services.AddTransient<ViewModels.Buscas.BuscarNotasEmitidasViewModel>();
            services.AddTransient<Views.Buscas.BuscarNotasEmitidasView>();

            services.AddTransient<ViewModels.Buscas.BuscarNotasEntradaViewModel>();
            services.AddTransient<Views.Buscas.BuscarNotasEntradaView>();

            services.AddTransient<ViewModels.Configuracoes.ConfiguracoesImpressoraViewModel>();
            services.AddTransient<Views.Configuracoes.ConfiguracoesImpressoraView>();

            services.AddTransient<ViewModels.Configuracoes.BackupViewModel>();
            services.AddTransient<Views.Configuracoes.BackupView>();

            ServiceProvider = services.BuildServiceProvider();

            using var scope = ServiceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            context.Database.Migrate();

            var mainWindow = ServiceProvider.GetRequiredService<MainWindow>();
            mainWindow.Show();
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                $"Erro ao iniciar:\n\n{ex.Message}\n\nInner: {ex.InnerException?.Message}\n\nStack: {ex.StackTrace}",
                "Erro",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }
    }

   protected override void OnExit(ExitEventArgs e)
    {
        base.OnExit(e);

        try
        {
            using var scope = ServiceProvider.CreateScope();
            var backupService = scope.ServiceProvider.GetService<IBackupService>();
            backupService?.RealizarBackupAutomatico().GetAwaiter().GetResult();
        }
        catch
        {
        }
        Environment.Exit(0);
    }
}
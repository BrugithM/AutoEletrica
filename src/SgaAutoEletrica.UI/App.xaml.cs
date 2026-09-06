using System.IO;
using System.Windows;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SgaAutoEletrica.Application;
using SgaAutoEletrica.Application.Common.Interfaces;
using SgaAutoEletrica.Infrastructure;
using SgaAutoEletrica.Infrastructure.Persistence.Context;
using SgaAutoEletrica.UI.Views;

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

            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlite($"Data Source={dbPath}"));

            services.AddInfrastructure();
            services.AddLogging();
            services.AddApplication();

            services.AddTransient<MainWindow>();
            services.AddTransient<LoginWindow>();
            services.AddTransient<ViewModels.LoginViewModel>();

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

            services.AddTransient<ViewModels.Configuracoes.ConfiguracaoEmpresaViewModel>();
            services.AddTransient<Views.Configuracoes.ConfiguracaoEmpresaView>();

            ServiceProvider = services.BuildServiceProvider();

            // Configura o banco
            using (var scope = ServiceProvider.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                // Fecha qualquer conexão pendente
                context.Database.CloseConnection();

                // Abre a conexão e aplica o PRAGMA
                context.Database.OpenConnection();
                context.Database.ExecuteSqlRaw("PRAGMA journal_mode=WAL;");
                context.Database.CloseConnection();

                // Aplica migrações
                context.Database.Migrate();
            }

            // Abre o login
            var loginWindow = ServiceProvider.GetRequiredService<LoginWindow>();
            System.Windows.Application.Current.MainWindow = loginWindow;
            loginWindow.Show();
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
            var context = scope.ServiceProvider.GetService<AppDbContext>();
            context?.Database.CloseConnection();

            var backupService = scope.ServiceProvider.GetService<IBackupService>();
            _ = Task.Run(async () =>
            {
                try
                {
                    await backupService?.RealizarBackupAutomatico()!;
                }
                catch
                {
                    // Silencioso
                }
            }).Wait(2000);
        }
        catch
        {
            // Silencioso
        }

        Environment.Exit(0);
    }
}
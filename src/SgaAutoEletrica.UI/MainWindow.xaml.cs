using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using SgaAutoEletrica.UI.Views.Dashboard;
using SgaAutoEletrica.UI.Views.Clientes;
using SgaAutoEletrica.UI.Views.Veiculos;
using SgaAutoEletrica.UI.Views.Pecas;
using SgaAutoEletrica.UI.Views.Servicos;
using SgaAutoEletrica.UI.Views.Fornecedores;
using SgaAutoEletrica.UI.Views.OrdensServico;

namespace SgaAutoEletrica.UI;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        NavegarParaDashboard();
    }

    private void BtnDashboard_Click(object sender, RoutedEventArgs e)
        => NavegarParaDashboard();

    private void BtnClientes_Click(object sender, RoutedEventArgs e)
    {
        var view = App.ServiceProvider.GetRequiredService<ListaClientesView>();
        ContentArea.Content = view;
    }

    private void BtnVeiculos_Click(object sender, RoutedEventArgs e)
    {
        var view = App.ServiceProvider.GetRequiredService<ListaVeiculosView>();
        ContentArea.Content = view;
    }
    private void BtnPecas_Click(object sender, RoutedEventArgs e)
    {
        var view = App.ServiceProvider.GetRequiredService<ListaPecasView>();
        ContentArea.Content = view;
    }

    private void BtnServicos_Click(object sender, RoutedEventArgs e)
    {
        var view = App.ServiceProvider.GetRequiredService<ListaServicosView>();
        ContentArea.Content = view;
    }

    private void BtnFornecedores_Click(object sender, RoutedEventArgs e)
{
    var view = App.ServiceProvider.GetRequiredService<ListaFornecedoresView>();
    ContentArea.Content = view;
}

    private void BtnOS_Click(object sender, RoutedEventArgs e)
{
    var view = App.ServiceProvider.GetRequiredService<ListaOSView>();
    ContentArea.Content = view;
}

private void BtnNotasFiscais_Click(object sender, RoutedEventArgs e)
{
    var view = App.ServiceProvider.GetRequiredService<SgaAutoEletrica.UI.Views.OrdensServico.ListaNotasFiscaisView>();
    ContentArea.Content = view;
}

private void BtnBuscarProdutos_Click(object sender, RoutedEventArgs e)
{
    var view = App.ServiceProvider.GetRequiredService<SgaAutoEletrica.UI.Views.Buscas.BuscarProdutosView>();
    ContentArea.Content = view;
}

private void BtnBuscarNotasEmitidas_Click(object sender, RoutedEventArgs e)
{
    var view = App.ServiceProvider.GetRequiredService<SgaAutoEletrica.UI.Views.Buscas.BuscarNotasEmitidasView>();
    ContentArea.Content = view;
}
private void BtnBuscarNotasEntrada_Click(object sender, RoutedEventArgs e)
{
    var view = App.ServiceProvider.GetRequiredService<SgaAutoEletrica.UI.Views.Buscas.BuscarNotasEntradaView>();
    ContentArea.Content = view;
}

private void BtnConfigEmpresa_Click(object sender, RoutedEventArgs e)
{
    var view = App.ServiceProvider.GetRequiredService<SgaAutoEletrica.UI.Views.Configuracoes.ConfiguracaoEmpresaView>();
    ContentArea.Content = view;
}

    private void NavegarParaDashboard()
    {
        try
        {
            var dashboardView = App.ServiceProvider.GetRequiredService<DashboardView>();
            ContentArea.Content = dashboardView;
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                $"Erro ao abrir Dashboard:\n\n{ex.Message}\n\nInner: {ex.InnerException?.Message}",
                "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            ShowPlaceholder("Erro ao carregar Dashboard. Verifique o banco de dados.");
        }
    }

    private void ShowPlaceholder(string text)
    {
        ContentArea.Content = new System.Windows.Controls.TextBlock
        {
            Text = text,
            FontSize = 28,
            FontWeight = FontWeights.Bold,
            Foreground = new System.Windows.Media.SolidColorBrush(
                System.Windows.Media.Color.FromRgb(44, 62, 80)),
            VerticalAlignment = VerticalAlignment.Center,
            HorizontalAlignment = HorizontalAlignment.Center,
            TextAlignment = TextAlignment.Center
        };
    }

    private void BtnConfiguracoes_Click(object sender, RoutedEventArgs e)
{
    var view = App.ServiceProvider.GetRequiredService<SgaAutoEletrica.UI.Views.Configuracoes.ConfiguracoesImpressoraView>();
    ContentArea.Content = view;
}

private void BtnBackup_Click(object sender, RoutedEventArgs e)
{
    var view = App.ServiceProvider.GetRequiredService<SgaAutoEletrica.UI.Views.Configuracoes.BackupView>();
    ContentArea.Content = view;
}
}
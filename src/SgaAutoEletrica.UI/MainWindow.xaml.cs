using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using SgaAutoEletrica.UI.Views.Dashboard;

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
        => ShowPlaceholder("Tela de Clientes - em breve");

    private void BtnVeiculos_Click(object sender, RoutedEventArgs e)
        => ShowPlaceholder("Tela de Veículos - em breve");

    private void BtnPecas_Click(object sender, RoutedEventArgs e)
        => ShowPlaceholder("Tela de Peças - em breve");

    private void BtnServicos_Click(object sender, RoutedEventArgs e)
        => ShowPlaceholder("Tela de Serviços - em breve");

    private void BtnFornecedores_Click(object sender, RoutedEventArgs e)
        => ShowPlaceholder("Tela de Fornecedores - em breve");

    private void BtnOS_Click(object sender, RoutedEventArgs e)
        => ShowPlaceholder("Tela de Ordens de Serviço - em breve");

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
}
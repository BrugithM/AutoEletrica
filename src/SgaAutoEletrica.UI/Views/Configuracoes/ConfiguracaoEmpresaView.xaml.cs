using System.Windows;
using System.Windows.Controls;
using SgaAutoEletrica.UI.ViewModels.Configuracoes;

namespace SgaAutoEletrica.UI.Views.Configuracoes;

public partial class ConfiguracaoEmpresaView : UserControl
{
    private readonly ConfiguracaoEmpresaViewModel _viewModel;

    public ConfiguracaoEmpresaView(ConfiguracaoEmpresaViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        DataContext = viewModel;
        Loaded += ConfiguracaoEmpresaView_Loaded;
    }

    private async void ConfiguracaoEmpresaView_Loaded(object sender, RoutedEventArgs e)
    {
        await _viewModel.CarregarAsync();
    }
}
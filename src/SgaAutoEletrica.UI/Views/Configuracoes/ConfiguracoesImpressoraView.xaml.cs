using System.Windows;
using System.Windows.Controls;
using SgaAutoEletrica.UI.ViewModels.Configuracoes;

namespace SgaAutoEletrica.UI.Views.Configuracoes;

public partial class ConfiguracoesImpressoraView : UserControl
{
    private readonly ConfiguracoesImpressoraViewModel _viewModel;

    public ConfiguracoesImpressoraView(ConfiguracoesImpressoraViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        DataContext = viewModel;
        Loaded += ConfiguracoesImpressoraView_Loaded;
    }

    private async void ConfiguracoesImpressoraView_Loaded(object sender, RoutedEventArgs e)
    {
        await _viewModel.CarregarAsync();
    }
}
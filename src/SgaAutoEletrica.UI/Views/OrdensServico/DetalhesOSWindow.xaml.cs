using System.Windows;
using MediatR;
using SgaAutoEletrica.UI.ViewModels.OrdensServico;

namespace SgaAutoEletrica.UI.Views.OrdensServico;

public partial class DetalhesOSWindow : Window
{
    private readonly DetalhesOSViewModel _viewModel;

    public DetalhesOSWindow(IMediator mediator, Guid osId)
    {
        InitializeComponent();
        _viewModel = new DetalhesOSViewModel(mediator, osId);
        DataContext = _viewModel;
        Loaded += DetalhesOSWindow_Loaded;
    }

    private async void DetalhesOSWindow_Loaded(object sender, RoutedEventArgs e)
    {
        await _viewModel.CarregarDadosAsync();
    }
}
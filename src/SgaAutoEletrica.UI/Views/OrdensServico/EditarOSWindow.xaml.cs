using System.Windows;
using MediatR;
using SgaAutoEletrica.UI.ViewModels.OrdensServico;

namespace SgaAutoEletrica.UI.Views.OrdensServico;

public partial class EditarOSWindow : Window
{
    private readonly EditarOSViewModel _viewModel;

    public EditarOSWindow(IMediator mediator, Guid osId)
    {
        InitializeComponent();
        _viewModel = new EditarOSViewModel(mediator, osId);
        DataContext = _viewModel;
        Loaded += EditarOSWindow_Loaded;
    }

    private async void EditarOSWindow_Loaded(object sender, RoutedEventArgs e)
    {
        await _viewModel.CarregarDadosAsync();
    }

    private void BtnFechar_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }
}
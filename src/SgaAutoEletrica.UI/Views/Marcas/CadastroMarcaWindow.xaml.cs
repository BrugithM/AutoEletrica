using System.Windows;
using MediatR;
using SgaAutoEletrica.UI.ViewModels.Marcas;

namespace SgaAutoEletrica.UI.Views.Marcas;

public partial class CadastroMarcaWindow : Window
{
    private readonly CadastroMarcaViewModel _viewModel;

    public CadastroMarcaWindow(IMediator mediator, int? marcaId = null)
    {
        InitializeComponent();
        _viewModel = new CadastroMarcaViewModel(mediator, marcaId);
        DataContext = _viewModel;
    }

    private async void BtnSalvar_Click(object sender, RoutedEventArgs e)
    {
        var sucesso = await _viewModel.SalvarAsync();
        if (sucesso)
            DialogResult = true;
    }

    private void BtnLimpar_Click(object sender, RoutedEventArgs e)
    {
        _viewModel.Limpar();
    }
}
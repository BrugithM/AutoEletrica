using System.Windows;
using MediatR;
using SgaAutoEletrica.UI.ViewModels.Clientes;

namespace SgaAutoEletrica.UI.Views.Clientes;

public partial class CadastroClienteWindow : Window
{
    private readonly CadastroClienteViewModel _viewModel;

    public CadastroClienteWindow(IMediator mediator, Guid? clienteId = null)
    {
        InitializeComponent();
        _viewModel = new CadastroClienteViewModel(mediator, clienteId);
        DataContext = _viewModel;
    }

    private async void BtnSalvar_Click(object sender, RoutedEventArgs e)
    {
        var sucesso = await _viewModel.SalvarAsync();
        if (sucesso)
            DialogResult = true;
    }

    private void BtnCancelar_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
    }
}
using System.Windows;
using MediatR;
using SgaAutoEletrica.UI.ViewModels.Fornecedores;

namespace SgaAutoEletrica.UI.Views.Fornecedores;

public partial class CadastroFornecedorWindow : Window
{
    private readonly CadastroFornecedorViewModel _viewModel;

    public CadastroFornecedorWindow(IMediator mediator, Guid? fornecedorId = null)
    {
        InitializeComponent();
        _viewModel = new CadastroFornecedorViewModel(mediator, fornecedorId);
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
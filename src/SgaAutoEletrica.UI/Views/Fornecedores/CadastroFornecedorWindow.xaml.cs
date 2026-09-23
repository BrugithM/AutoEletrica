using System.Windows;
using MediatR;
using SgaAutoEletrica.Domain.ValueObjects;
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

    private void BtnLimpar_Click(object sender, RoutedEventArgs e)
    {
        _viewModel.Limpar();
    }

    private void TxtCnpj_LostFocus(object sender, RoutedEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(_viewModel.Cnpj)) return;

        try
        {
            var cnpj = new Cnpj(_viewModel.Cnpj);
            _viewModel.Cnpj = cnpj.Formatado();
            TxtCnpj.GetBindingExpression(System.Windows.Controls.TextBox.TextProperty)?.UpdateTarget();
        }
        catch
        {
            // CNPJ inválido — deixa como está
        }
    }

    private void TxtTelefone_LostFocus(object sender, RoutedEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(_viewModel.Telefone)) return;

        try
        {
            var tel = new Telefone(_viewModel.Telefone);
            _viewModel.Telefone = tel.Formatado();
            TxtTelefone.GetBindingExpression(System.Windows.Controls.TextBox.TextProperty)?.UpdateTarget();
        }
        catch
        {
            // Telefone inválido — deixa como está
        }
    }
}
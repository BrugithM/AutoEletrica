using System.Windows;
using System.Windows.Controls;
using MediatR;
using SgaAutoEletrica.Domain.ValueObjects;
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
        Loaded += CadastroClienteWindow_Loaded;
    }

    private async void CadastroClienteWindow_Loaded(object sender, RoutedEventArgs e)
    {
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

    private void TxtCpfCnpj_LostFocus(object sender, RoutedEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(_viewModel.CpfCnpj)) return;

        try
        {
            var cpfCnpj = CpfCnpj.Criar(_viewModel.CpfCnpj);
            _viewModel.CpfCnpj = cpfCnpj?.Formatado() ?? string.Empty;
            TxtCpfCnpj.GetBindingExpression(TextBox.TextProperty)?.UpdateTarget();
        }
        catch
        {
            // valor inválido: deixa como está, o usuário corrige
        }
    }

    private void TxtTelefone_LostFocus(object sender, RoutedEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(_viewModel.Telefone)) return;

        try
        {
            var tel = Telefone.Criar(_viewModel.Telefone);
            _viewModel.Telefone = tel?.Formatado() ?? string.Empty;
            TxtTelefone.GetBindingExpression(TextBox.TextProperty)?.UpdateTarget();
        }
        catch
        {
            // valor inválido: deixa como está
        }
    }
}
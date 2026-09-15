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

    private void TxtCpf_LostFocus(object sender, RoutedEventArgs e)
{
    if (string.IsNullOrWhiteSpace(_viewModel.Cpf)) return;

    try
    {
        var cpf = new SgaAutoEletrica.Domain.ValueObjects.Cpf(_viewModel.Cpf);
        _viewModel.Cpf = cpf.Formatado();
        OnPropertyChanged(nameof(_viewModel.Cpf));
    }
    catch
    {
    }
}

private void TxtTelefone_LostFocus(object sender, RoutedEventArgs e)
{
    if (string.IsNullOrWhiteSpace(_viewModel.Telefone)) return;

    try
    {
        var tel = new SgaAutoEletrica.Domain.ValueObjects.Telefone(_viewModel.Telefone);
        _viewModel.Telefone = tel.Formatado();
        OnPropertyChanged(nameof(_viewModel.Telefone));
    }
    catch
    {
    }
}

private void OnPropertyChanged(string propertyName)
{
    var binding = TxtCpf.GetBindingExpression(System.Windows.Controls.TextBox.TextProperty);
    binding?.UpdateTarget();
    var binding2 = TxtTelefone.GetBindingExpression(System.Windows.Controls.TextBox.TextProperty);
    binding2?.UpdateTarget();
}
}
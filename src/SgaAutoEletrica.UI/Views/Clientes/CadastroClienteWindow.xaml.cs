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
        // Nada — o carregamento já acontece no construtor do ViewModel
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
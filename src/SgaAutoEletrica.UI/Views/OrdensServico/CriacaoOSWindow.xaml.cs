using System.Windows;
using MediatR;
using SgaAutoEletrica.UI.ViewModels.OrdensServico;

namespace SgaAutoEletrica.UI.Views.OrdensServico;

public partial class CriacaoOSWindow : Window
{
    private readonly CriacaoOSViewModel _viewModel;

    public CriacaoOSWindow(IMediator mediator, Guid? clienteIdPreSelecionado = null, Guid? veiculoIdPreSelecionado = null)
    {
        InitializeComponent();
        _viewModel = new CriacaoOSViewModel(mediator, clienteIdPreSelecionado, veiculoIdPreSelecionado);
        DataContext = _viewModel;
        Loaded += CriacaoOSWindow_Loaded;
    }

    private async void CriacaoOSWindow_Loaded(object sender, RoutedEventArgs e)
    {
        await _viewModel.CarregarDadosAsync();
    }

    private void BtnAdicionarPeca_Click(object sender, RoutedEventArgs e)
    {
        _viewModel.AdicionarPeca();
    }

    private void BtnAdicionarServico_Click(object sender, RoutedEventArgs e)
    {
        _viewModel.AdicionarServico();
    }

    private async void BtnEmitirOrcamento_Click(object sender, RoutedEventArgs e)
    {
        var sucesso = await _viewModel.SalvarAsync(false);
        if (sucesso)
            DialogResult = true;
    }

    private async void BtnAprovarIniciar_Click(object sender, RoutedEventArgs e)
    {
        var sucesso = await _viewModel.SalvarAsync(true);
        if (sucesso)
            DialogResult = true;
    }
}
using System.Windows;
using MediatR;
using SgaAutoEletrica.UI.ViewModels.Servicos;

namespace SgaAutoEletrica.UI.Views.Servicos;

public partial class CadastroServicoWindow : Window
{
    private readonly CadastroServicoViewModel _viewModel;

    public CadastroServicoWindow(IMediator mediator, Guid? servicoId = null)
    {
        InitializeComponent();
        _viewModel = new CadastroServicoViewModel(mediator, servicoId);
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
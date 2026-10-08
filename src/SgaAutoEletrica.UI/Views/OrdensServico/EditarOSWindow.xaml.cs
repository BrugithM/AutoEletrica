using System.Windows;
using MediatR;
using SgaAutoEletrica.UI.ViewModels.OrdensServico;
using System.Windows.Controls;

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

    private async void BtnAdicionarPeca_Click(object sender, RoutedEventArgs e)
    {
        await _viewModel.AdicionarPecaAsync();
    }

    private async void BtnAdicionarServico_Click(object sender, RoutedEventArgs e)
    {
        await _viewModel.AdicionarServicoAsync();
    }

    private async void BtnSalvar_Click(object sender, RoutedEventArgs e)
    {
        var sucesso = await _viewModel.SalvarAsync();
        if (sucesso)
            DialogResult = true;
    }

    private void PecasGrid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
    {
        if (e.EditAction == DataGridEditAction.Commit)
        {
            var item = e.Row.Item as ItemPecaOSTemporario;
            if (item != null)
            {
                Dispatcher.BeginInvoke(new Action(async () => await _viewModel.SalvarPrecoPecaAsync(item)),
                    System.Windows.Threading.DispatcherPriority.Background);
            }
        }
    }

    private void ServicosGrid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
    {
        if (e.EditAction == DataGridEditAction.Commit)
        {
            var item = e.Row.Item as ItemServicoOSTemporario;
            if (item != null)
            {
                Dispatcher.BeginInvoke(new Action(async () => await _viewModel.SalvarPrecoServicoAsync(item)),
                    System.Windows.Threading.DispatcherPriority.Background);
            }
        }
    }
}
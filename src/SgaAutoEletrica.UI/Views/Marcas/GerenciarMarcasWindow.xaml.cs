using System.Windows;
using MediatR;
using SgaAutoEletrica.Application.Features.Marcas.Commands;
using SgaAutoEletrica.UI.ViewModels.Marcas;

namespace SgaAutoEletrica.UI.Views.Marcas;

public partial class GerenciarMarcasWindow : Window
{
    private readonly IMediator _mediator;
    private readonly GerenciarMarcasViewModel _viewModel;

    public GerenciarMarcasWindow(IMediator mediator)
    {
        InitializeComponent();
        _mediator = mediator;
        _viewModel = new GerenciarMarcasViewModel(mediator);
        DataContext = _viewModel;
        Loaded += GerenciarMarcasWindow_Loaded;
    }

    private async void GerenciarMarcasWindow_Loaded(object sender, RoutedEventArgs e)
    {
        await _viewModel.CarregarAsync();
    }

    private void BtnNovaMarca_Click(object sender, RoutedEventArgs e)
    {
        var dialog = new CadastroMarcaWindow(_mediator);
        if (dialog.ShowDialog() == true)
        {
            _ = _viewModel.CarregarAsync();
        }
    }

    private async void BtnExcluirMarca_Click(object sender, RoutedEventArgs e)
    {
        if (sender is not System.Windows.Controls.Button btn) return;
        if (btn.Tag is not int marcaId) return;

        var marca = _viewModel.Marcas.FirstOrDefault(m => m.Id == marcaId);
        if (marca == null) return;

        var confirmacao = MessageBox.Show(
            $"Deseja realmente excluir a marca '{marca.Nome}'?\n\nAs peças vinculadas ficarão sem marca.",
            "Confirmar Exclusão",
            MessageBoxButton.YesNo,
            MessageBoxImage.Warning);

        if (confirmacao == MessageBoxResult.Yes)
        {
            try
            {
                await _mediator.Send(new ExcluirMarcaCommand { Id = marcaId });
                await _viewModel.CarregarAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro: {ex.Message}", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }

    private void BtnFechar_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }
}
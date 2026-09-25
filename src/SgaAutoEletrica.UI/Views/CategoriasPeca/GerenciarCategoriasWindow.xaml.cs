using System.Windows;
using MediatR;
using SgaAutoEletrica.Application.Features.CategoriasPeca.Commands;
using SgaAutoEletrica.UI.ViewModels.CategoriasPeca;

namespace SgaAutoEletrica.UI.Views.CategoriasPeca;

public partial class GerenciarCategoriasWindow : Window
{
    private readonly IMediator _mediator;
    private readonly GerenciarCategoriasViewModel _viewModel;

    public GerenciarCategoriasWindow(IMediator mediator)
    {
        InitializeComponent();
        _mediator = mediator;
        _viewModel = new GerenciarCategoriasViewModel(mediator);
        DataContext = _viewModel;
        Loaded += GerenciarCategoriasWindow_Loaded;
    }

    private async void GerenciarCategoriasWindow_Loaded(object sender, RoutedEventArgs e)
    {
        await _viewModel.CarregarAsync();
    }

    private void BtnNovaCategoria_Click(object sender, RoutedEventArgs e)
    {
        var dialog = new CadastroCategoriaWindow(_mediator);
        if (dialog.ShowDialog() == true)
        {
            _ = _viewModel.CarregarAsync();
        }
    }

    private async void BtnExcluirCategoria_Click(object sender, RoutedEventArgs e)
    {
        if (sender is not System.Windows.Controls.Button btn) return;
        if (btn.Tag is not Guid categoriaId) return;

        var categoria = _viewModel.Categorias.FirstOrDefault(c => c.Id == categoriaId);
        if (categoria == null) return;

        var confirmacao = MessageBox.Show(
            $"Deseja realmente excluir a categoria '{categoria.Nome}'?\n\nAs peças vinculadas ficarão sem categoria.",
            "Confirmar Exclusão",
            MessageBoxButton.YesNo,
            MessageBoxImage.Warning);

        if (confirmacao == MessageBoxResult.Yes)
        {
            try
            {
                await _mediator.Send(new ExcluirCategoriaPecaCommand { Id = categoriaId });
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
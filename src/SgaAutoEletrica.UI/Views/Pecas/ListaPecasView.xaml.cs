using System.Windows;
using System.Windows.Controls;
using SgaAutoEletrica.UI.ViewModels.Pecas;

namespace SgaAutoEletrica.UI.Views.Pecas;

public partial class ListaPecasView : UserControl
{
    private readonly ListaPecasViewModel _viewModel;

    public ListaPecasView(ListaPecasViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        DataContext = viewModel;
        Loaded += ListaPecasView_Loaded;
    }

    private async void ListaPecasView_Loaded(object sender, System.Windows.RoutedEventArgs e)
    {
        await _viewModel.CarregarDadosAuxiliaresAsync();
        await _viewModel.BuscarAsync();
    }

    private void BtnImprimirEtiqueta_Click(object sender, RoutedEventArgs e)
    {
        if (_viewModel.PecaSelecionada == null)
        {
            MessageBox.Show("Selecione uma peça para imprimir a etiqueta.", "Aviso",
                MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        var dialog = new ImprimirEtiquetaWindow(_viewModel.PecaSelecionada);
        dialog.ShowDialog();
    }
}
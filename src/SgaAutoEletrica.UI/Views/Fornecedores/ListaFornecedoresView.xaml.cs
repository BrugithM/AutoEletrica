using System.Windows;
using System.Windows.Controls;
using MediatR;
using SgaAutoEletrica.UI.ViewModels.Fornecedores;

namespace SgaAutoEletrica.UI.Views.Fornecedores;

public partial class ListaFornecedoresView : UserControl
{
    private readonly ListaFornecedoresViewModel _viewModel;
    private readonly IMediator _mediator;

    public ListaFornecedoresView(ListaFornecedoresViewModel viewModel, IMediator mediator)
    {
        InitializeComponent();
        _viewModel = viewModel;
        _mediator = mediator;
        DataContext = viewModel;
        Loaded += ListaFornecedoresView_Loaded;
    }

    private async void ListaFornecedoresView_Loaded(object sender, RoutedEventArgs e)
    {
        await _viewModel.BuscarAsync();
    }

    private void BtnNovaNFEntrada_Click(object sender, RoutedEventArgs e)
{
    var dialog = new CriarNotaFiscalEntradaWindow(_mediator);
    dialog.ShowDialog();
}
}
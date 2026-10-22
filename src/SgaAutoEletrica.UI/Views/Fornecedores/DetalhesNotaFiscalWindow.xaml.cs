using System.Windows;
using MediatR;
using SgaAutoEletrica.UI.ViewModels.Fornecedores;

namespace SgaAutoEletrica.UI.Views.Fornecedores;

public partial class DetalhesNotaFiscalWindow : Window
{
    private readonly DetalhesNotaFiscalViewModel _viewModel;

    public DetalhesNotaFiscalWindow(IMediator mediator, Guid notaId)
    {
        InitializeComponent(); 
        _viewModel = new DetalhesNotaFiscalViewModel(mediator, notaId);
        DataContext = _viewModel;
        Loaded += async (s, e) => await _viewModel.CarregarDadosAsync();
    }
}
using System.Windows;
using MediatR;
using SgaAutoEletrica.UI.ViewModels.MarcasVeiculo;
using Microsoft.Extensions.DependencyInjection;

namespace SgaAutoEletrica.UI.Views.MarcasVeiculo;

public partial class ListaMarcasVeiculoView : Window
{
    private readonly ListaMarcasVeiculoViewModel _viewModel;

    public ListaMarcasVeiculoView(IMediator mediator)
    {
        InitializeComponent();
        
        _viewModel = new ListaMarcasVeiculoViewModel(mediator);
        DataContext = _viewModel;
        
        Loaded += async (s, e) => await _viewModel.CarregarAsync();
    }
}
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using MediatR;
using SgaAutoEletrica.Application.Features.Buscas.DTOs;
using SgaAutoEletrica.Application.Features.Buscas.Queries;

namespace SgaAutoEletrica.UI.ViewModels.Buscas;

public class BuscarProdutosViewModel : INotifyPropertyChanged
{
    private readonly IMediator _mediator;

    public ObservableCollection<PecaBuscaDTO> Resultados { get; } = new();

    public string CodigoBarras { get; set; } = string.Empty;
    public string CodigoPeca { get; set; } = string.Empty;
    public string Marca { get; set; } = string.Empty;
    public string IdPeca { get; set; } = string.Empty;

    public ICommand BuscarCommand { get; }
    public ICommand LimparCommand { get; }

    public BuscarProdutosViewModel(IMediator mediator)
    {
        _mediator = mediator;
        BuscarCommand = new RelayCommand(async _ => await BuscarAsync());
        LimparCommand = new RelayCommand(async _ =>
        {
            CodigoBarras = string.Empty;
            CodigoPeca = string.Empty;
            Marca = string.Empty;
            IdPeca = string.Empty;
            OnPropertyChanged(nameof(CodigoBarras));
            OnPropertyChanged(nameof(CodigoPeca));
            OnPropertyChanged(nameof(Marca));
            OnPropertyChanged(nameof(IdPeca));
            await BuscarAsync();
        });
    }

    public async Task BuscarAsync()
    {
        Resultados.Clear();
        var query = new BuscarProdutosQuery
        {
            CodigoBarras = string.IsNullOrWhiteSpace(CodigoBarras) ? null : CodigoBarras,
            CodigoPeca = string.IsNullOrWhiteSpace(CodigoPeca) ? null : CodigoPeca,
            Marca = string.IsNullOrWhiteSpace(Marca) ? null : Marca
        };

        var resultado = await _mediator.Send(query);
        foreach (var p in resultado)
            Resultados.Add(p);
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}
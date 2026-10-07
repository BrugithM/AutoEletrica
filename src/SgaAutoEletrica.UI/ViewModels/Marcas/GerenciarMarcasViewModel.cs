using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using MediatR;
using SgaAutoEletrica.Application.Features.Marcas.DTOs;
using SgaAutoEletrica.Application.Features.Marcas.Queries;

namespace SgaAutoEletrica.UI.ViewModels.Marcas;

public class GerenciarMarcasViewModel : INotifyPropertyChanged
{
    private readonly IMediator _mediator;

    public ObservableCollection<MarcaDTO> Marcas { get; } = new();

    public GerenciarMarcasViewModel(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task CarregarAsync()
    {
        Marcas.Clear();
        var resultado = await _mediator.Send(new ListarMarcasQuery());
        foreach (var m in resultado)
            Marcas.Add(m);
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using MediatR;
using SgaAutoEletrica.Application.Features.OrdensServico.DTOs;
using SgaAutoEletrica.Application.Features.OrdensServico.Queries;

namespace SgaAutoEletrica.UI.ViewModels.OrdensServico;

public class ListaNotasFiscaisViewModel : INotifyPropertyChanged
{
    private readonly IMediator _mediator;

    public ObservableCollection<NotaFiscalSaidaDTO> Notas { get; } = new();

    public ListaNotasFiscaisViewModel(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task CarregarAsync()
    {
        Notas.Clear();
        var resultado = await _mediator.Send(new ListarNotasFiscaisSaidaQuery());
        foreach (var nf in resultado)
            Notas.Add(nf);
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using MediatR;
using SgaAutoEletrica.Application.Features.CategoriasPeca.DTOs;
using SgaAutoEletrica.Application.Features.CategoriasPeca.Queries;

namespace SgaAutoEletrica.UI.ViewModels.CategoriasPeca;

public class GerenciarCategoriasViewModel : INotifyPropertyChanged
{
    private readonly IMediator _mediator;

    public ObservableCollection<CategoriaPecaDTO> Categorias { get; } = new();

    public GerenciarCategoriasViewModel(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task CarregarAsync()
    {
        Categorias.Clear();
        var resultado = await _mediator.Send(new ListarCategoriasPecaQuery());
        foreach (var cat in resultado)
            Categorias.Add(cat);
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using MediatR;
using SgaAutoEletrica.Application.Features.Fornecedores.DTOs;
using SgaAutoEletrica.Application.Features.Fornecedores.Queries;

namespace SgaAutoEletrica.UI.ViewModels.Fornecedores;

public class DetalhesNotaFiscalViewModel : INotifyPropertyChanged
{
    private readonly IMediator _mediator;
    private readonly Guid _notaId;

    public string Titulo { get; private set; } = "Detalhes da NF";
    public string Numero { get; private set; } = string.Empty;
    public string Fornecedor { get; private set; } = string.Empty;
    public string DataEntrada { get; private set; } = string.Empty;
    public string Observacao { get; private set; } = string.Empty;
    public decimal ValorTotal { get; private set; }

    public ObservableCollection<ItemNotaEntradaDetalheDTO> Itens { get; } = new();
    public ICommand EditarCommand { get; }

    public DetalhesNotaFiscalViewModel(IMediator mediator, Guid notaId)
    {
        _mediator = mediator;
        _notaId = notaId;
        EditarCommand = new RelayCommand(async _ => await EditarAsync());
    }

    public async Task CarregarDadosAsync()
    {
        var nf = await _mediator.Send(new ObterNotaFiscalEntradaPorIdQuery { Id = _notaId });
        if (nf == null) return;

        Titulo = $"NF Nº {nf.Numero}";
        Numero = nf.Numero;
        Fornecedor = nf.NomeFornecedor;
        DataEntrada = nf.DataEntrada.ToLocalTime().ToString("dd/MM/yyyy");
        Observacao = nf.Observacao ?? "";
        ValorTotal = nf.ValorTotal;

        Itens.Clear();
        foreach (var item in nf.Itens) Itens.Add(item);

        OnPropertyChanged(nameof(Titulo));
        OnPropertyChanged(nameof(Numero));
        OnPropertyChanged(nameof(Fornecedor));
        OnPropertyChanged(nameof(DataEntrada));
        OnPropertyChanged(nameof(Observacao));
        OnPropertyChanged(nameof(ValorTotal));
    }

    private async Task EditarAsync()
    {
        var dialog = new Views.Fornecedores.CriarNotaFiscalEntradaWindow(_mediator, _notaId);
        
        if (dialog.ShowDialog() == true)
        {
            await CarregarDadosAsync();
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}
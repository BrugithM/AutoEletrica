using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using MediatR;
using SgaAutoEletrica.Application.Features.Buscas.DTOs;
using SgaAutoEletrica.Application.Features.Buscas.Queries;

namespace SgaAutoEletrica.UI.ViewModels.Buscas;

public class BuscarNotasEntradaViewModel : INotifyPropertyChanged
{
    private readonly IMediator _mediator;

    public ObservableCollection<NotaFiscalEntradaResumoDTO> Resultados { get; } = new();

    public string NomeFornecedor { get; set; } = string.Empty;
    public string CnpjFornecedor { get; set; } = string.Empty;
    public string CodigoProduto { get; set; } = string.Empty;
    public string NomeProduto { get; set; } = string.Empty;
    public DateTime? DataInicio { get; set; }
    public DateTime? DataFim { get; set; }

    public ICommand BuscarCommand { get; }
    public ICommand LimparCommand { get; }

    public BuscarNotasEntradaViewModel(IMediator mediator)
    {
        _mediator = mediator;
        BuscarCommand = new RelayCommand(async _ => await BuscarAsync());
        LimparCommand = new RelayCommand(_ =>
        {
            NomeFornecedor = string.Empty;
            CnpjFornecedor = string.Empty;
            CodigoProduto = string.Empty;
            NomeProduto = string.Empty;
            DataInicio = null;
            DataFim = null;
            OnPropertyChanged(nameof(NomeFornecedor));
            OnPropertyChanged(nameof(CnpjFornecedor));
            OnPropertyChanged(nameof(CodigoProduto));
            OnPropertyChanged(nameof(NomeProduto));
            OnPropertyChanged(nameof(DataInicio));
            OnPropertyChanged(nameof(DataFim));
            Resultados.Clear();
        });
    }

    public async Task BuscarAsync()
    {
        Resultados.Clear();
        var query = new BuscarNotasEntradaQuery
        {
            NomeFornecedor = string.IsNullOrWhiteSpace(NomeFornecedor) ? null : NomeFornecedor,
            CnpjFornecedor = string.IsNullOrWhiteSpace(CnpjFornecedor) ? null : CnpjFornecedor,
            CodigoProduto = string.IsNullOrWhiteSpace(CodigoProduto) ? null : CodigoProduto,
            NomeProduto = string.IsNullOrWhiteSpace(NomeProduto) ? null : NomeProduto,
            DataInicio = DataInicio,
            DataFim = DataFim
        };

        var resultado = await _mediator.Send(query);
        foreach (var nf in resultado)
            Resultados.Add(nf);
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}
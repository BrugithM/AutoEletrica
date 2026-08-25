using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using MediatR;
using SgaAutoEletrica.Application.Features.Buscas.DTOs;
using SgaAutoEletrica.Application.Features.Buscas.Queries;

namespace SgaAutoEletrica.UI.ViewModels.Buscas;

public class BuscarNotasEmitidasViewModel : INotifyPropertyChanged
{
    private readonly IMediator _mediator;

    public ObservableCollection<NotaFiscalSaidaResumoDTO> Resultados { get; } = new();

    public string Placa { get; set; } = string.Empty;
    public string NomeCliente { get; set; } = string.Empty;
    public string NomePeca { get; set; } = string.Empty;
    public string Observacao { get; set; } = string.Empty;
    public DateTime? DataInicio { get; set; }
    public DateTime? DataFim { get; set; }

    public ICommand BuscarCommand { get; }
    public ICommand LimparCommand { get; }

    public BuscarNotasEmitidasViewModel(IMediator mediator)
    {
        _mediator = mediator;
        BuscarCommand = new RelayCommand(async _ => await BuscarAsync());
        LimparCommand = new RelayCommand(async _ =>
        {
            Placa = string.Empty; NomeCliente = string.Empty; NomePeca = string.Empty;
            Observacao = string.Empty; DataInicio = null; DataFim = null;
            OnPropertyChanged(nameof(Placa)); OnPropertyChanged(nameof(NomeCliente));
            OnPropertyChanged(nameof(NomePeca)); OnPropertyChanged(nameof(Observacao));
            OnPropertyChanged(nameof(DataInicio)); OnPropertyChanged(nameof(DataFim));
            Resultados.Clear();
        });
    }

    public async Task BuscarAsync()
    {
        Resultados.Clear();
        var query = new BuscarNotasEmitidasQuery
        {
            Placa = string.IsNullOrWhiteSpace(Placa) ? null : Placa,
            NomeCliente = string.IsNullOrWhiteSpace(NomeCliente) ? null : NomeCliente,
            NomePeca = string.IsNullOrWhiteSpace(NomePeca) ? null : NomePeca,
            Observacao = string.IsNullOrWhiteSpace(Observacao) ? null : Observacao,
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
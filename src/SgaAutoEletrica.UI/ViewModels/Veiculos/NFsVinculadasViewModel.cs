using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using SgaAutoEletrica.Application.Features.Buscas.DTOs;
using SgaAutoEletrica.Application.Features.Buscas.Queries;

namespace SgaAutoEletrica.UI.ViewModels.Veiculos;

public class NFsVinculadasViewModel : INotifyPropertyChanged
{
    private readonly IMediator _mediator;
    private readonly Guid _veiculoId;

    public ObservableCollection<NotaFiscalSaidaResumoDTO> Notas { get; } = new();

    private NotaFiscalSaidaResumoDTO? _notaSelecionada;
    public NotaFiscalSaidaResumoDTO? NotaSelecionada
    {
        get => _notaSelecionada;
        set { _notaSelecionada = value; OnPropertyChanged(); OnPropertyChanged(nameof(TemNotaSelecionada)); }
    }

    public bool TemNotaSelecionada => NotaSelecionada != null;

    private string _titulo = string.Empty;
    public string Titulo
    {
        get => _titulo;
        private set { _titulo = value; OnPropertyChanged(); }
    }

    public int TotalNotas => Notas.Count;
    public decimal TotalGeral => Notas.Sum(n => n.ValorTotal);

    public ICommand VerDetalhesCommand { get; }
    public ICommand ImprimirNFCommand { get; }
    public ICommand FecharCommand { get; }

    public NFsVinculadasViewModel(IMediator mediator, Guid veiculoId, string placaVeiculo)
    {
        _mediator = mediator;
        _veiculoId = veiculoId;
        Titulo = $"Notas Fiscais do Veículo - Placa: {placaVeiculo}";

        VerDetalhesCommand = new RelayCommand(_ => VerDetalhes(), _ => TemNotaSelecionada);
        ImprimirNFCommand = new RelayCommand(_ => ImprimirNF(), _ => TemNotaSelecionada);
    }

    public async Task CarregarAsync()
    {
        try
        {
            Notas.Clear();
            var resultado = await _mediator.Send(new ListarNotasFiscaisPorVeiculoQuery { VeiculoId = _veiculoId });
            foreach (var nf in resultado)
                Notas.Add(nf);

            OnPropertyChanged(nameof(TotalNotas));
            OnPropertyChanged(nameof(TotalGeral));
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Erro: {ex.Message}", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    public void VerDetalhes()
    {
        if (NotaSelecionada == null) return;
        var dialog = new Views.Buscas.DetalhesNotaFiscalSaidaWindow(
            App.ServiceProvider.GetRequiredService<IMediator>(),
            NotaSelecionada.Id);
        dialog.ShowDialog();
    }

    public void ImprimirNF()
    {
        if (NotaSelecionada == null) return;

        try
        {
            var impressao = App.ServiceProvider.GetRequiredService<SgaAutoEletrica.Application.Common.Interfaces.IImpressaoService>();
            var nf = _mediator.Send(new ObterNotaFiscalSaidaPorIdQuery { Id = NotaSelecionada.Id })
                .GetAwaiter().GetResult();

            if (nf != null)
            {
                var osDto = new SgaAutoEletrica.Application.Features.OrdensServico.DTOs.OrdemServicoDetalheDTO
                {
                    NomeCliente = nf.NomeCliente,
                    TelefoneCliente = nf.TelefoneCliente,
                    PlacaVeiculo = nf.PlacaVeiculo,
                    ModeloVeiculo = nf.ModeloVeiculo,
                    MarcaVeiculo = nf.MarcaVeiculo,
                    ValorTotal = nf.ValorTotal,
                    Observacao = nf.Observacao,
                    ItensPeca = nf.Itens.Select(i => new SgaAutoEletrica.Application.Features.OrdensServico.DTOs.ItemPecaOSDTO
                    {
                        NomePeca = i.Descricao,
                        Quantidade = i.Quantidade,
                        PrecoUnitario = i.ValorUnitario,
                        ValorTotal = i.ValorTotal
                    }).ToList()
                };

                impressao.ImprimirNotaFiscal(osDto, nf.Numero);
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Erro ao imprimir: {ex.Message}", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}
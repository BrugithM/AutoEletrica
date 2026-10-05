using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using SgaAutoEletrica.Application.Common.Interfaces;
using SgaAutoEletrica.Application.Features.OrdensServico.DTOs;
using SgaAutoEletrica.Application.Features.OrdensServico.Queries;

namespace SgaAutoEletrica.UI.ViewModels.Veiculos;

public class OSVinculadasViewModel : INotifyPropertyChanged
{
    private readonly IMediator _mediator;
    private readonly Guid _veiculoId;

    public ObservableCollection<OrdemServicoResumoDTO> Ordens { get; } = new();

    private OrdemServicoResumoDTO? _osSelecionada;
    public OrdemServicoResumoDTO? OsSelecionada
    {
        get => _osSelecionada;
        set { _osSelecionada = value; OnPropertyChanged(); OnPropertyChanged(nameof(TemOsSelecionada)); }
    }

    public bool TemOsSelecionada => OsSelecionada != null;

    private string _titulo = string.Empty;
    public string Titulo
    {
        get => _titulo;
        private set { _titulo = value; OnPropertyChanged(); }
    }

    public int TotalOS => Ordens.Count;

    public ICommand VerDetalhesCommand { get; }
    public ICommand ImprimirOSCommand { get; }

    public OSVinculadasViewModel(IMediator mediator, Guid veiculoId, string placaVeiculo)
    {
        _mediator = mediator;
        _veiculoId = veiculoId;
        Titulo = $"Ordens de Serviço do Veículo - Placa: {placaVeiculo}";

        VerDetalhesCommand = new RelayCommand(_ => VerDetalhes(), _ => TemOsSelecionada);
        ImprimirOSCommand = new RelayCommand(_ => ImprimirOS(), _ => TemOsSelecionada);
    }

    public async Task CarregarAsync()
    {
        try
        {
            Ordens.Clear();
            var resultado = await _mediator.Send(new ListarOSPorVeiculoQuery { VeiculoId = _veiculoId });
            foreach (var os in resultado)
                Ordens.Add(os);

            OnPropertyChanged(nameof(TotalOS));
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Erro: {ex.Message}", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    public void VerDetalhes()
    {
        if (OsSelecionada == null) return;
        var dialog = new Views.OrdensServico.DetalhesOSWindow(
            App.ServiceProvider.GetRequiredService<IMediator>(),
            OsSelecionada.Id);
        dialog.ShowDialog();
        _ = CarregarAsync();
    }

    private void ImprimirOS()
    {
        if (OsSelecionada == null) return;

        try
        {
            var impressao = App.ServiceProvider.GetRequiredService<IImpressaoService>();
            var os = _mediator.Send(new ObterOSPorIdQuery { Id = OsSelecionada.Id })
                .GetAwaiter().GetResult();

            if (os != null)
            {
                impressao.ImprimirOS(os);
                MessageBox.Show("OS enviada para impressão.", "Sucesso",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Erro ao imprimir: {ex.Message}", "Erro",
                MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}
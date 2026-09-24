using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using SgaAutoEletrica.Application.Common.Interfaces;
using SgaAutoEletrica.Application.Features.OrdensServico.Commands;
using SgaAutoEletrica.Application.Features.OrdensServico.DTOs;
using SgaAutoEletrica.Application.Features.OrdensServico.Queries;
using SgaAutoEletrica.Domain.Enums;

namespace SgaAutoEletrica.UI.ViewModels.OrdensServico;

public class DetalhesOSViewModel : INotifyPropertyChanged
{
    private readonly IMediator _mediator;
    private readonly Guid _osId;

    public ObservableCollection<ItemPecaOSDTO> PecasNaOS { get; } = new();
    public ObservableCollection<ItemServicoOSDTO> ServicosNaOS { get; } = new();

    private string _titulo = string.Empty;
    public string Titulo
    {
        get => _titulo;
        private set { _titulo = value; OnPropertyChanged(); }
    }

    private string _numeroOS = string.Empty;
    public string NumeroOS
    {
        get => _numeroOS;
        private set { _numeroOS = value; OnPropertyChanged(); }
    }

    private string _dataAbertura = string.Empty;
    public string DataAbertura
    {
        get => _dataAbertura;
        private set { _dataAbertura = value; OnPropertyChanged(); }
    }

    private string _infoCliente = string.Empty;
    public string InfoCliente
    {
        get => _infoCliente;
        private set { _infoCliente = value; OnPropertyChanged(); }
    }

    private string _infoVeiculo = string.Empty;
    public string InfoVeiculo
    {
        get => _infoVeiculo;
        private set { _infoVeiculo = value; OnPropertyChanged(); }
    }

    private string _statusAtual = string.Empty;
    public string StatusAtual
    {
        get => _statusAtual;
        private set { _statusAtual = value; OnPropertyChanged(); }
    }

    private string _observacao = string.Empty;
    public string Observacao
    {
        get => _observacao;
        private set { _observacao = value; OnPropertyChanged(); }
    }

    private decimal _valorTotalPecas;
    public decimal ValorTotalPecas
    {
        get => _valorTotalPecas;
        private set { _valorTotalPecas = value; OnPropertyChanged(); }
    }

    private decimal _valorTotalServicos;
    public decimal ValorTotalServicos
    {
        get => _valorTotalServicos;
        private set { _valorTotalServicos = value; OnPropertyChanged(); }
    }

    private decimal _desconto;
    public decimal Desconto
    {
        get => _desconto;
        private set { _desconto = value; OnPropertyChanged(); }
    }

    private decimal _valorTotal;
    public decimal ValorTotal
    {
        get => _valorTotal;
        private set { _valorTotal = value; OnPropertyChanged(); }
    }

    private StatusOS _status;
    public StatusOS Status
    {
        get => _status;
        private set { _status = value; OnPropertyChanged(); }
    }

    // Habilitação dos botões
    public bool PodeIniciar => Status == StatusOS.Aberta || Status == StatusOS.AguardandoPecas;
    public bool PodeAguardarPecas => Status == StatusOS.EmAndamento;
    public bool PodeFinalizar => Status != StatusOS.Finalizada && Status != StatusOS.Cancelada;
    public bool PodeCancelar => Status != StatusOS.Finalizada && Status != StatusOS.Cancelada;
    public bool PodeEditar => Status == StatusOS.Aberta || Status == StatusOS.EmAndamento;
    public bool PodeGerarNF => Status == StatusOS.Finalizada;
    public bool PodeImprimir => true;

    public ICommand ImprimirOSCommand { get; }
    public ICommand ImprimirCupomCommand { get; }
    public ICommand ImprimirNFCommand { get; }
    public ICommand GerarNFCommand { get; }
    public ICommand EditarOSCommand { get; }
    public ICommand IniciarServicoCommand { get; }
    public ICommand AguardarPecasCommand { get; }
    public ICommand CancelarOSCommand { get; }
    public ICommand FinalizarCommand { get; }

    public DetalhesOSViewModel(IMediator mediator, Guid osId)
    {
        _mediator = mediator;
        _osId = osId;

        ImprimirOSCommand = new RelayCommand(_ => ImprimirOS());
        ImprimirCupomCommand = new RelayCommand(_ => ImprimirCupom());
        ImprimirNFCommand = new RelayCommand(_ => ImprimirNF());
        GerarNFCommand = new RelayCommand(_ => GerarNF(), _ => PodeGerarNF);
        EditarOSCommand = new RelayCommand(_ => EditarOS(), _ => PodeEditar);
        IniciarServicoCommand = new RelayCommand(async _ => await AlterarStatusAsync(StatusOS.EmAndamento), _ => PodeIniciar);
        AguardarPecasCommand = new RelayCommand(async _ => await AlterarStatusAsync(StatusOS.AguardandoPecas), _ => PodeAguardarPecas);
        CancelarOSCommand = new RelayCommand(async _ => await CancelarAsync(), _ => PodeCancelar);
        FinalizarCommand = new RelayCommand(async _ => await FinalizarAsync(), _ => PodeFinalizar);
    }

    public async Task CarregarDadosAsync()
    {
        var os = await _mediator.Send(new ObterOSPorIdQuery { Id = _osId });
        if (os == null)
        {
            MessageBox.Show("OS não encontrada.", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }

        Titulo = $"Detalhes da OS Nº {os.Numero}";
        NumeroOS = os.Numero.ToString();
        DataAbertura = os.DataAbertura.ToString("dd/MM/yyyy HH:mm");
        InfoCliente = $"Cliente: {os.NomeCliente} - CPF: {os.TelefoneCliente}";  // ajuste
        InfoCliente = $"Cliente: {os.NomeCliente} (Tel: {os.TelefoneCliente})";
        InfoVeiculo = $"Veículo: {os.MarcaVeiculo} {os.ModeloVeiculo} - Placa: {os.PlacaVeiculo}";
        StatusAtual = os.Status.ToString();
        Status = os.Status;
        Observacao = os.Observacao ?? "";
        ValorTotalPecas = os.ValorTotalPecas;
        ValorTotalServicos = os.ValorTotalServicos;
        Desconto = os.Desconto;
        ValorTotal = os.ValorTotal;

        PecasNaOS.Clear();
        foreach (var peca in os.ItensPeca)
            PecasNaOS.Add(peca);

        ServicosNaOS.Clear();
        foreach (var servico in os.ItensServico)
            ServicosNaOS.Add(servico);

        NotificarBotoes();
    }

    private void NotificarBotoes()
    {
        OnPropertyChanged(nameof(PodeIniciar));
        OnPropertyChanged(nameof(PodeAguardarPecas));
        OnPropertyChanged(nameof(PodeFinalizar));
        OnPropertyChanged(nameof(PodeCancelar));
        OnPropertyChanged(nameof(PodeEditar));
        OnPropertyChanged(nameof(PodeGerarNF));
        OnPropertyChanged(nameof(PodeImprimir));
    }

    private async Task AlterarStatusAsync(StatusOS novoStatus)
    {
        try
        {
            await _mediator.Send(new AlterarStatusOSCommand
            {
                OrdemServicoId = _osId,
                NovoStatus = novoStatus
            });
            await CarregarDadosAsync();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Erro: {ex.Message}", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private async Task FinalizarAsync()
    {
        var confirmacao = MessageBox.Show("Deseja finalizar esta OS?", "Confirmar", 
            MessageBoxButton.YesNo, MessageBoxImage.Question);
        if (confirmacao == MessageBoxResult.Yes)
            await AlterarStatusAsync(StatusOS.Finalizada);
    }

    private async Task CancelarAsync()
    {
        var dialog = new Views.OrdensServico.CancelarOSWindow();
        if (dialog.ShowDialog() == true)
            await AlterarStatusAsyncComMotivo(StatusOS.Cancelada, dialog.Motivo);
    }

    private async Task AlterarStatusAsyncComMotivo(StatusOS status, string motivo)
    {
        try
        {
            await _mediator.Send(new AlterarStatusOSCommand
            {
                OrdemServicoId = _osId,
                NovoStatus = status,
                MotivoCancelamento = motivo
            });
            await CarregarDadosAsync();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Erro: {ex.Message}", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void EditarOS()
    {
        var dialog = new Views.OrdensServico.EditarOSWindow(_mediator, _osId);
        if (dialog.ShowDialog() == true)
        {
            CarregarDadosAsync().GetAwaiter().GetResult();
        }
    }

    private void ImprimirOS()
    {
        try
        {
            var impressao = App.ServiceProvider.GetRequiredService<IImpressaoService>();
            var os = _mediator.Send(new ObterOSPorIdQuery { Id = _osId }).GetAwaiter().GetResult();
            if (os != null)
                impressao.ImprimirOS(os);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Erro: {ex.Message}", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void ImprimirCupom()
    {
        try
        {
            var impressao = App.ServiceProvider.GetRequiredService<IImpressaoService>();
            var os = _mediator.Send(new ObterOSPorIdQuery { Id = _osId }).GetAwaiter().GetResult();
            if (os != null)
                impressao.ImprimirCupomFiscal(os);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Erro: {ex.Message}", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void ImprimirNF()
    {
        try
        {
            var impressao = App.ServiceProvider.GetRequiredService<IImpressaoService>();
            var os = _mediator.Send(new ObterOSPorIdQuery { Id = _osId }).GetAwaiter().GetResult();
            if (os != null)
                impressao.ImprimirNotaFiscal(os, "NF-" + os.Numero);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Erro: {ex.Message}", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void GerarNF()
    {
        var dialog = new Views.OrdensServico.GerarNFWindow();
        if (dialog.ShowDialog() == true)
        {
            try
            {
                _mediator.Send(new GerarNotaFiscalSaidaCommand
                {
                    OrdemServicoId = _osId,
                    NumeroNota = dialog.NumeroNota
                }).GetAwaiter().GetResult();

                MessageBox.Show("Nota Fiscal gerada com sucesso!", "Sucesso", MessageBoxButton.OK, MessageBoxImage.Information);
                CarregarDadosAsync().GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro: {ex.Message}", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}
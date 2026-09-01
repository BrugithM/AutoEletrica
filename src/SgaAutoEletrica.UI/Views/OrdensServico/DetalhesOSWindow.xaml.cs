using System.Windows;
using MediatR;
using SgaAutoEletrica.Application.Common.Interfaces;
using SgaAutoEletrica.Application.Features.OrdensServico.Commands;
using SgaAutoEletrica.Application.Features.OrdensServico.DTOs;
using SgaAutoEletrica.Application.Features.OrdensServico.Queries;
using Microsoft.Extensions.DependencyInjection;
using SgaAutoEletrica.Domain.Enums;

namespace SgaAutoEletrica.UI.Views.OrdensServico;

public partial class DetalhesOSWindow : Window
{
    private readonly IMediator _mediator;
    private readonly Guid _osId;

    public DetalhesOSWindow(IMediator mediator, Guid osId)
    {
        InitializeComponent();
        _mediator = mediator;
        _osId = osId;
        Loaded += DetalhesOSWindow_Loaded;
    }

    private async void DetalhesOSWindow_Loaded(object sender, RoutedEventArgs e)
    {
        await CarregarDadosAsync();
    }

    private async Task CarregarDadosAsync()
    {
        var os = await _mediator.Send(new ObterOSPorIdQuery { Id = _osId });
        if (os == null)
        {
            MessageBox.Show("OS não encontrada.", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            Close();
            return;
        }

        TxtTitulo.Text = $"OS Nº {os.Numero}";
        TxtCliente.Text = $"Cliente: {os.NomeCliente}";
        TxtVeiculo.Text = $"Veículo: {os.MarcaVeiculo} {os.ModeloVeiculo} - {os.PlacaVeiculo}";
        TxtStatus.Text = $"Status: {os.Status}";
        TxtObservacao.Text = $"Obs: {os.Observacao ?? "Nenhuma"}";
        TxtTotal.Text = $"Total: {os.ValorTotal:C2}";

        var itens = new List<dynamic>();
        foreach (var peca in os.ItensPeca)
            itens.Add(new { Tipo = "Peça", Descricao = peca.NomePeca, Quantidade = peca.Quantidade.ToString(), Valor = peca.ValorTotal });
        foreach (var servico in os.ItensServico)
            itens.Add(new { Tipo = "Serviço", Descricao = servico.NomeServico, Quantidade = "1", Valor = servico.PrecoUnitario });

        GridItens.ItemsSource = itens;

        AtualizarBotoes(os.Status);
    }

    private void AtualizarBotoes(StatusOS status)
    {
        BtnIniciar.IsEnabled = status == StatusOS.Aberta || status == StatusOS.AguardandoPecas;
        BtnAguardarPecas.IsEnabled = status == StatusOS.EmAndamento;
        BtnFinalizar.IsEnabled = status != StatusOS.Finalizada && status != StatusOS.Cancelada;
        BtnEditarOS.IsEnabled = status == StatusOS.Aberta || status == StatusOS.EmAndamento;
        BtnGerarNF.IsEnabled = status == StatusOS.Finalizada;
        BtnCancelar.IsEnabled = status != StatusOS.Finalizada && status != StatusOS.Cancelada;
    }

    private async void BtnIniciar_Click(object sender, RoutedEventArgs e)
    {
        await AlterarStatusAsync(StatusOS.EmAndamento);
    }

    private async void BtnAguardarPecas_Click(object sender, RoutedEventArgs e)
    {
        await AlterarStatusAsync(StatusOS.AguardandoPecas);
    }

    private async void BtnFinalizar_Click(object sender, RoutedEventArgs e)
    {
        var confirmacao = MessageBox.Show("Finalizar esta OS?", "Confirmar", MessageBoxButton.YesNo, MessageBoxImage.Question);
        if (confirmacao == MessageBoxResult.Yes)
            await AlterarStatusAsync(StatusOS.Finalizada);
    }

    private async void BtnCancelar_Click(object sender, RoutedEventArgs e)
    {
        var dialog = new CancelarOSWindow();
        if (dialog.ShowDialog() == true)
            await AlterarStatusAsync(StatusOS.Cancelada, dialog.Motivo);
    }

    private async Task AlterarStatusAsync(StatusOS novoStatus, string? motivo = null)
    {
        try
        {
            await _mediator.Send(new AlterarStatusOSCommand
            {
                OrdemServicoId = _osId,
                NovoStatus = novoStatus,
                MotivoCancelamento = motivo
            });
            await CarregarDadosAsync();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Erro: {ex.Message}", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void BtnFechar_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }

    private async void BtnGerarNF_Click(object sender, RoutedEventArgs e)
    {
        var dialog = new GerarNFWindow();
        if (dialog.ShowDialog() == true)
        {
            try
            {
                await _mediator.Send(new GerarNotaFiscalSaidaCommand
                {
                    OrdemServicoId = _osId,
                    NumeroNota = dialog.NumeroNota
                });
                MessageBox.Show("Nota Fiscal gerada com sucesso!", "Sucesso", MessageBoxButton.OK, MessageBoxImage.Information);
                BtnGerarNF.IsEnabled = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro: {ex.Message}", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }

    private void BtnImprimirOS_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            var os = ObterOSAsync().GetAwaiter().GetResult();
            if (os != null)
            {
                var impressaoService = App.ServiceProvider.GetRequiredService<IImpressaoService>();
                impressaoService.ImprimirOS(os);
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Erro ao imprimir: {ex.Message}", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void BtnImprimirCupom_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            var os = ObterOSAsync().GetAwaiter().GetResult();
            if (os != null)
            {
                var impressaoService = App.ServiceProvider.GetRequiredService<IImpressaoService>();
                impressaoService.ImprimirCupomFiscal(os);
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Erro ao imprimir cupom: {ex.Message}", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private async Task<OrdemServicoDetalheDTO?> ObterOSAsync()
    {
        return await _mediator.Send(new ObterOSPorIdQuery { Id = _osId });
    }
    private void BtnEditarOS_Click(object sender, RoutedEventArgs e)
    {
        var dialog = new EditarOSWindow(_mediator, _osId);
        dialog.ShowDialog();
        CarregarDadosAsync().GetAwaiter().GetResult();
    }
}
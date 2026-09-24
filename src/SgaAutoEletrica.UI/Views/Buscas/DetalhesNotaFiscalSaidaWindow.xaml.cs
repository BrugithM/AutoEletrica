using System.Text;
using System.Windows;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using SgaAutoEletrica.Application.Features.Buscas.Queries;

namespace SgaAutoEletrica.UI.Views.Buscas;

public partial class DetalhesNotaFiscalSaidaWindow : Window
{
    private readonly IMediator _mediator;
    private readonly Guid _nfId;

    public DetalhesNotaFiscalSaidaWindow(IMediator mediator, Guid nfId)
    {
        InitializeComponent();
        _mediator = mediator;
        _nfId = nfId;
        Loaded += DetalhesNotaFiscalSaidaWindow_Loaded;
    }

    private async void DetalhesNotaFiscalSaidaWindow_Loaded(object sender, RoutedEventArgs e)
    {
        var nf = await _mediator.Send(new ObterNotaFiscalSaidaPorIdQuery { Id = _nfId });
        if (nf == null)
        {
            MessageBox.Show("Nota Fiscal não encontrada.", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            Close();
            return;
        }

        // Busca dados da empresa
        var empresa = await _mediator.Send(new SgaAutoEletrica.Application.Features.Configuracoes.Queries.ObterConfiguracaoEmpresaQuery());

        var sb = new StringBuilder();
        sb.AppendLine("========================================");
        sb.AppendLine("           NOTA FISCAL");
        sb.AppendLine("========================================");

        if (empresa != null)
        {
            sb.AppendLine(empresa.NomeEmpresa);
            sb.AppendLine($"CNPJ: {empresa.Cnpj}");
            sb.AppendLine($"Tel: {empresa.Telefone}");
            if (!string.IsNullOrWhiteSpace(empresa.Endereco))
                sb.AppendLine(empresa.Endereco);
            sb.AppendLine("----------------------------------------");
        }

        sb.AppendLine($"Número NF: {nf.Numero}");
        sb.AppendLine($"Data: {nf.DataEmissao:dd/MM/yyyy HH:mm}");
        sb.AppendLine("----------------------------------------");
        sb.AppendLine("CLIENTE:");
        sb.AppendLine($"  Nome: {nf.NomeCliente}");
        sb.AppendLine($"  Telefone: {nf.TelefoneCliente}");
        sb.AppendLine("----------------------------------------");
        sb.AppendLine("VEÍCULO:");
        sb.AppendLine($"  Modelo: {nf.MarcaVeiculo} {nf.ModeloVeiculo}");
        sb.AppendLine($"  Placa: {nf.PlacaVeiculo}");
        sb.AppendLine("----------------------------------------");
        sb.AppendLine("ITENS:");
        foreach (var item in nf.Itens)
            sb.AppendLine($"  {item.Descricao} x{item.Quantidade} = {item.ValorTotal:C2}");
        sb.AppendLine("----------------------------------------");
        sb.AppendLine($"TOTAL: {nf.ValorTotal:C2}");
        sb.AppendLine("========================================");

        TxtConteudo.Text = sb.ToString();
    }

    private async void BtnImprimir_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            var impressao = App.ServiceProvider.GetRequiredService<SgaAutoEletrica.Application.Common.Interfaces.IImpressaoService>();
            var nf = await _mediator.Send(new ObterNotaFiscalSaidaPorIdQuery { Id = _nfId });

            if (nf == null) return;

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
        catch (Exception ex)
        {
            MessageBox.Show($"Erro ao imprimir: {ex.Message}", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void BtnFechar_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }
}
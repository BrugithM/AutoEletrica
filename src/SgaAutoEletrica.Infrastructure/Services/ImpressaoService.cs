using System.Drawing.Printing;
using System.Text;
using Microsoft.EntityFrameworkCore;
using SgaAutoEletrica.Application.Common.Interfaces;
using SgaAutoEletrica.Application.Features.OrdensServico.DTOs;
using SgaAutoEletrica.Domain.Enums;
using SgaAutoEletrica.Infrastructure.Persistence.Context;

namespace SgaAutoEletrica.Infrastructure.Services;

public class ImpressaoService : IImpressaoService
{
    private readonly IConfiguracaoImpressoraRepository _configRepo;
    private readonly AppDbContext _context;

    public ImpressaoService(IConfiguracaoImpressoraRepository configRepo, AppDbContext context)
    {
        _configRepo = configRepo;
        _context = context;
    }

    public void ImprimirOS(OrdemServicoDetalheDTO os)
    {
        var config = _configRepo.ObterPorTipo(TipoImpressao.OS).GetAwaiter().GetResult();
        if (config == null)
            throw new InvalidOperationException("Nenhuma impressora configurada para OS.");

        var conteudo = GerarConteudoOS(os);
        EnviarParaImpressora(conteudo, config.NomeImpressora, config.Copias ?? 1);
    }

    public void ImprimirNotaFiscal(OrdemServicoDetalheDTO os, string numeroNota)
    {
        var config = _configRepo.ObterPorTipo(TipoImpressao.NotaFiscal).GetAwaiter().GetResult();
        if (config == null)
            throw new InvalidOperationException("Nenhuma impressora configurada para Nota Fiscal.");

        var conteudo = GerarConteudoNotaFiscal(os, numeroNota);
        EnviarParaImpressora(conteudo, config.NomeImpressora, config.Copias ?? 1);
    }

    public void ImprimirCupomFiscal(OrdemServicoDetalheDTO os)
    {
        var config = _configRepo.ObterPorTipo(TipoImpressao.CupomFiscal).GetAwaiter().GetResult();
        if (config == null)
            throw new InvalidOperationException("Nenhuma impressora configurada para Cupom Fiscal.");

        var conteudo = GerarConteudoCupom(os);
        EnviarParaImpressora(conteudo, config.NomeImpressora, config.Copias ?? 1);
    }

    private void EnviarParaImpressora(string conteudo, string nomeImpressora, int copias)
    {
        var printDocument = new PrintDocument
        {
            PrinterSettings = new PrinterSettings
            {
                PrinterName = nomeImpressora,
                Copies = (short)copias
            },
            DocumentName = "SGA Impressão"
        };

        printDocument.PrintPage += (sender, e) =>
        {
            var fonte = new System.Drawing.Font("Consolas", 10);
            var brush = System.Drawing.Brushes.Black;
            var margem = 40f;
            var yPos = margem;

            foreach (var linha in conteudo.Split('\n'))
            {
                e.Graphics.DrawString(linha, fonte, brush, margem, yPos);
                yPos += fonte.GetHeight(e.Graphics) + 2;
            }
        };

        printDocument.Print();
    }

    private string ObterCabecalhoEmpresa()
    {
        var empresa = _context.ConfiguracoesEmpresa
            .AsNoTracking()
            .FirstOrDefault();

        if (empresa == null)
            return string.Empty;

        var sb = new StringBuilder();
        sb.AppendLine(empresa.NomeEmpresa);
        sb.AppendLine($"CNPJ: {empresa.Cnpj}");
        sb.AppendLine($"Tel: {empresa.Telefone}");
        if (!string.IsNullOrWhiteSpace(empresa.Endereco))
            sb.AppendLine(empresa.Endereco);
        sb.AppendLine("----------------------------------------");

        return sb.ToString();
    }

    private string GerarConteudoOS(OrdemServicoDetalheDTO os)
    {
        var sb = new StringBuilder();
        sb.AppendLine("========================================");
        sb.AppendLine("         ORDEM DE SERVIÇO");
        sb.AppendLine("========================================");
        sb.AppendLine(ObterCabecalhoEmpresa());
        sb.AppendLine($"Número: {os.Numero}");
        sb.AppendLine($"Data: {os.DataAbertura:dd/MM/yyyy HH:mm}");
        sb.AppendLine($"Status: {os.Status}");
        sb.AppendLine("----------------------------------------");
        sb.AppendLine("CLIENTE:");
        sb.AppendLine($"  Nome: {os.NomeCliente}");
        sb.AppendLine($"  Telefone: {os.TelefoneCliente}");
        sb.AppendLine("----------------------------------------");
        sb.AppendLine("VEÍCULO:");
        sb.AppendLine($"  Modelo: {os.MarcaVeiculo} {os.ModeloVeiculo}");
        sb.AppendLine($"  Placa: {os.PlacaVeiculo}");
        sb.AppendLine($"  Ano: {os.AnoVeiculo}");
        sb.AppendLine("----------------------------------------");
        sb.AppendLine("PEÇAS:");
        foreach (var peca in os.ItensPeca)
            sb.AppendLine($"  {peca.NomePeca} x{peca.Quantidade} = {peca.ValorTotal:C2}");
        sb.AppendLine("----------------------------------------");
        sb.AppendLine("SERVIÇOS:");
        foreach (var servico in os.ItensServico)
            sb.AppendLine($"  {servico.NomeServico} = {servico.PrecoUnitario:C2}");
        sb.AppendLine("----------------------------------------");
        sb.AppendLine($"TOTAL PEÇAS: {os.ValorTotalPecas:C2}");
        sb.AppendLine($"TOTAL SERVIÇOS: {os.ValorTotalServicos:C2}");
        if (os.Desconto > 0)
            sb.AppendLine($"DESCONTO: -{os.Desconto:C2}");
        sb.AppendLine($"TOTAL GERAL: {os.ValorTotal:C2}");
        sb.AppendLine("========================================");
        if (!string.IsNullOrWhiteSpace(os.Observacao))
        {
            sb.AppendLine($"OBSERVAÇÃO: {os.Observacao}");
            sb.AppendLine("========================================");
        }
        sb.AppendLine("   Obrigado pela preferência!");
        sb.AppendLine("========================================");

        return sb.ToString();
    }

    private string GerarConteudoNotaFiscal(OrdemServicoDetalheDTO os, string numeroNota)
    {
        var sb = new StringBuilder();
        sb.AppendLine("========================================");
        sb.AppendLine("           NOTA FISCAL");
        sb.AppendLine("========================================");
        sb.AppendLine(ObterCabecalhoEmpresa());
        sb.AppendLine($"Número NF: {numeroNota}");
        sb.AppendLine($"Data: {DateTime.Now:dd/MM/yyyy HH:mm}");
        sb.AppendLine("----------------------------------------");
        sb.AppendLine("CLIENTE:");
        sb.AppendLine($"  Nome: {os.NomeCliente}");
        sb.AppendLine($"  Telefone: {os.TelefoneCliente}");
        sb.AppendLine("----------------------------------------");
        sb.AppendLine("VEÍCULO:");
        sb.AppendLine($"  Modelo: {os.MarcaVeiculo} {os.ModeloVeiculo}");
        sb.AppendLine($"  Placa: {os.PlacaVeiculo}");
        sb.AppendLine("----------------------------------------");
        sb.AppendLine("ITENS:");
        foreach (var peca in os.ItensPeca)
            sb.AppendLine($"  {peca.NomePeca} x{peca.Quantidade} = {peca.ValorTotal:C2}");
        foreach (var servico in os.ItensServico)
            sb.AppendLine($"  {servico.NomeServico} = {servico.PrecoUnitario:C2}");
        sb.AppendLine("----------------------------------------");
        sb.AppendLine($"TOTAL PEÇAS: {os.ValorTotalPecas:C2}");
        sb.AppendLine($"TOTAL SERVIÇOS: {os.ValorTotalServicos:C2}");
        if (os.Desconto > 0)
            sb.AppendLine($"DESCONTO: -{os.Desconto:C2}");
        sb.AppendLine($"TOTAL GERAL: {os.ValorTotal:C2}");
        sb.AppendLine("========================================");

        return sb.ToString();
    }

    private string GerarConteudoCupom(OrdemServicoDetalheDTO os)
    {
        var empresa = _context.ConfiguracoesEmpresa
            .AsNoTracking()
            .FirstOrDefault();

        var sb = new StringBuilder();

        if (empresa != null)
        {
            sb.AppendLine(empresa.NomeEmpresa);
            sb.AppendLine($"CNPJ: {empresa.Cnpj}");
            sb.AppendLine($"Tel: {empresa.Telefone}");
        }
        else
        {
            sb.AppendLine("        AUTO ELÉTRICA");
        }

        sb.AppendLine("========================================");
        sb.AppendLine($"OS Nº: {os.Numero}  Data: {os.DataAbertura:dd/MM/yyyy}");
        sb.AppendLine("----------------------------------------");
        sb.AppendLine($"CLIENTE: {os.NomeCliente}");
        sb.AppendLine($"VEÍCULO: {os.ModeloVeiculo} {os.PlacaVeiculo}");
        sb.AppendLine("----------------------------------------");
        sb.AppendLine("ITENS:");
        foreach (var peca in os.ItensPeca)
            sb.AppendLine($" {peca.NomePeca} x{peca.Quantidade} {peca.ValorTotal:C2}");
        foreach (var servico in os.ItensServico)
            sb.AppendLine($" {servico.NomeServico} {servico.PrecoUnitario:C2}");
        sb.AppendLine("----------------------------------------");
        sb.AppendLine($"TOTAL: {os.ValorTotal:C2}");
        sb.AppendLine("========================================");
        sb.AppendLine("       OBRIGADO E VOLTE SEMPRE!");

        return sb.ToString();
    }
}
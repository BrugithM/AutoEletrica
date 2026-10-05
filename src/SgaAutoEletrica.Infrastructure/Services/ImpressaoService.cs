using System.Drawing.Printing;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using Microsoft.EntityFrameworkCore;
using SgaAutoEletrica.Application.Common.Interfaces;
using SgaAutoEletrica.Application.Features.OrdensServico.DTOs;
using SgaAutoEletrica.Domain.Enums;
using SgaAutoEletrica.Application.Features.Pecas.DTOs;
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

    public void ImprimirCupomOrcamento(OrdemServicoDetalheDTO os)
    {
        var config = _configRepo.ObterPorTipo(TipoImpressao.Cupom).GetAwaiter().GetResult();
        if (config == null)
            throw new InvalidOperationException("Nenhuma impressora configurada para Cupom.");

        var conteudo = GerarConteudoCupomOrcamento(os);
        EnviarParaImpressora(conteudo, config.NomeImpressora, config.Copias ?? 1);
    }

    private string GerarConteudoCupomOrcamento(OrdemServicoDetalheDTO os)
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

        sb.AppendLine("========================================");
        sb.AppendLine("             ORÇAMENTO");
        sb.AppendLine("========================================");
        sb.AppendLine($"Nº: {os.Numero}   Data: {os.DataAbertura:dd/MM/yyyy}");
        sb.AppendLine("----------------------------------------");
        sb.AppendLine($"Cliente: {os.NomeCliente}");
        sb.AppendLine($"Veículo: {os.MarcaVeiculo} {os.ModeloVeiculo}");
        sb.AppendLine($"Placa: {os.PlacaVeiculo}");
        sb.AppendLine("----------------------------------------");

        if (os.ItensPeca.Any())
        {
            sb.AppendLine("PEÇAS:");
            foreach (var peca in os.ItensPeca)
            {
                sb.AppendLine($"  {peca.NomePeca} x{peca.Quantidade}");
                sb.AppendLine($"    {peca.ValorTotal:C2}");
            }
        }

        if (os.ItensServico.Any())
        {
            sb.AppendLine("SERVIÇOS:");
            foreach (var servico in os.ItensServico)
            {
                sb.AppendLine($"  {servico.NomeServico} - {servico.PrecoUnitario:C2}");
            }
        }

        sb.AppendLine("----------------------------------------");
        sb.AppendLine($"Subtotal: {os.ValorTotalPecas + os.ValorTotalServicos:C2}");
        if (os.Desconto > 0)
            sb.AppendLine($"Desconto: -{os.Desconto:C2}");
        sb.AppendLine($"TOTAL: {os.ValorTotal:C2}");
        sb.AppendLine("========================================");
        sb.AppendLine("Este documento é um orçamento");
        sb.AppendLine("e não tem valor fiscal.");
        sb.AppendLine("Válido por 30 dias.");
        sb.AppendLine("========================================");

        return sb.ToString();
    }

    public Bitmap GerarEtiquetaBitmap(PecaDTO peca, string nomeEmpresa)
    {
        // 60x40mm a 300 DPI = 708 x 472 pixels
        const int largura = 708;
        const int altura = 472;

        var bitmap = new Bitmap(largura, altura);
        bitmap.SetResolution(300, 300);

        using (var g = Graphics.FromImage(bitmap))
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;
            g.Clear(Color.White);

            var alturaCabecalho = 75f;
            g.FillRectangle(Brushes.Black, 0, 0, largura, alturaCabecalho);

            var fonteEmpresa = new Font("Arial", 14, FontStyle.Bold);

            var tamanhoEmpresa = g.MeasureString(nomeEmpresa, fonteEmpresa);
            var xEmpresa = (largura - tamanhoEmpresa.Width) / 2;
            var yEmpresa = (alturaCabecalho - tamanhoEmpresa.Height) / 2;

            g.SetClip(new RectangleF(0, 0, largura, alturaCabecalho));
            g.DrawString(nomeEmpresa, fonteEmpresa, Brushes.White, xEmpresa, yEmpresa);
            g.ResetClip();

            var fonteCodigo = new Font("Arial", 12);
            var fonteNome = new Font("Arial", 12, FontStyle.Bold);
            var fonteValor = new Font("Arial", 14, FontStyle.Bold);
            var fonteId = new Font("Arial", 12);

            var centroX = largura / 2f;
            var y = alturaCabecalho + 15f;

            var codigo = peca.CodigoPeca ?? "-";
            var textoCodigo = $"Código: {codigo}";
            var tamanhoCodigo = g.MeasureString(textoCodigo, fonteCodigo);
            g.DrawString(textoCodigo, fonteCodigo, Brushes.Black,
                centroX - tamanhoCodigo.Width / 2, y);
            y += fonteCodigo.GetHeight(g) + 8;

            var nome = peca.Nome.Length > 30 ? peca.Nome.Substring(0, 30) + "..." : peca.Nome;
            var tamanhoNome = g.MeasureString(nome, fonteNome);
            g.DrawString(nome, fonteNome, Brushes.Black,
                centroX - tamanhoNome.Width / 2, y);
            y += fonteNome.GetHeight(g) + 12;

            var textoValor = peca.ValorVenda.ToString("C2");
            var tamanhoValor = g.MeasureString(textoValor, fonteValor);
            g.DrawString(textoValor, fonteValor, Brushes.Black,
                centroX - tamanhoValor.Width / 2, y);
            y += fonteValor.GetHeight(g) + 10;

            var textoId = $"ID: {peca.IdPeca}";
            var tamanhoId = g.MeasureString(textoId, fonteId);
            g.DrawString(textoId, fonteId, Brushes.Gray,
                centroX - tamanhoId.Width / 2, y);

            fonteEmpresa.Dispose();
            fonteCodigo.Dispose();
            fonteNome.Dispose();
            fonteValor.Dispose();
            fonteId.Dispose();
        }
        return bitmap;
    }
    public void ImprimirEtiqueta(PecaDTO peca)
    {
        var config = _configRepo.ObterPorTipo(TipoImpressao.Etiqueta).GetAwaiter().GetResult();
        if (config == null)
            throw new InvalidOperationException("Nenhuma impressora configurada para Etiqueta.");

        var empresa = _context.ConfiguracoesEmpresa
            .AsNoTracking()
            .FirstOrDefault();

        var nomeEmpresa = empresa?.NomeEmpresa ?? "AUTO ELÉTRICA";

        using var bitmap = GerarEtiquetaBitmap(peca, nomeEmpresa);

        var printDocument = new PrintDocument
        {
            PrinterSettings = new PrinterSettings
            {
                PrinterName = config.NomeImpressora,
                Copies = 1
            },
            DocumentName = "Etiqueta de Peça",
            DefaultPageSettings = new PageSettings
            {
                // 60x40mm em centésimos de polegada
                PaperSize = new PaperSize("Etiqueta 60x40", 236, 157),
                Margins = new Margins(0, 0, 0, 0)
            }
        };

        printDocument.PrintPage += (sender, e) =>
        {
            var g = e.Graphics!;
            g.DrawImage(bitmap, e.MarginBounds);
        };

        printDocument.Print();
    }
}
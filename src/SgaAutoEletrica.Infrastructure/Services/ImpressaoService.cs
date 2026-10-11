using System.Drawing.Printing;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using Microsoft.EntityFrameworkCore;
using SgaAutoEletrica.Application.Common.Interfaces;
using SgaAutoEletrica.Application.Features.OrdensServico.DTOs;
using SgaAutoEletrica.Application.Features.Pecas.DTOs;
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

        var empresa = _context.ConfiguracoesEmpresa
            .AsNoTracking()
            .FirstOrDefault();

        using var bitmap = GerarOSBitmap(os, empresa);

        var printDocument = new PrintDocument
        {
            PrinterSettings = new PrinterSettings
            {
                PrinterName = config.NomeImpressora,
                Copies = (short)(config.Copias ?? 1)
            },
            DocumentName = "Ordem de Serviço",
            DefaultPageSettings = new PageSettings
            {
                PaperSize = new PaperSize("A4", 827, 1169), // A4 em centésimos de polegada
                Margins = new Margins(40, 40, 40, 40),
                Landscape = false
            }
        };

        printDocument.PrintPage += (sender, e) =>
        {
            var g = e.Graphics!;
            var area = e.MarginBounds;
            g.DrawImage(bitmap, area);
        };

        printDocument.Print();
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

        sb.AppendLine($"Orçamento Nº: {os.Numero}");
        sb.AppendLine($"Data: {os.DataAbertura:dd/MM/yyyy HH:mm}");
        sb.AppendLine("========================================");

        if (empresa != null)
        {
            sb.AppendLine(empresa.NomeEmpresa);
            sb.AppendLine($"CNPJ: {empresa.Cnpj}");
            sb.AppendLine($"Tel: {empresa.Telefone}");
            if (!string.IsNullOrWhiteSpace(empresa.Endereco))
                sb.AppendLine(empresa.Endereco);
        }

        sb.AppendLine("========================================");
        sb.AppendLine("             ORÇAMENTO");
        sb.AppendLine("========================================");
        sb.AppendLine($"Cliente: {os.NomeCliente}");
        sb.AppendLine($"Veículo: {os.MarcaVeiculo} {os.ModeloVeiculo} - {os.PlacaVeiculo}");
        if (os.Quilometragem.HasValue)
            sb.AppendLine($"Km: {os.Quilometragem.Value:N0}");
        sb.AppendLine("----------------------------------------");

        sb.AppendLine("Peças/Serviços:");

        foreach (var peca in os.ItensPeca)
        {
            var descricao = $"{peca.NomePeca} x{peca.Quantidade}";
            var valor = peca.ValorTotal.ToString("C2");
            sb.AppendLine(FormatarLinhaComValorDireita(descricao, valor, 40));
        }

        foreach (var servico in os.ItensServico)
        {
            var descricao = servico.NomeServico;
            var valor = servico.PrecoUnitario.ToString("C2");
            sb.AppendLine(FormatarLinhaComValorDireita(descricao, valor, 40));
        }

        sb.AppendLine("----------------------------------------");
        sb.AppendLine(FormatarLinhaComValorDireita("TOTAL:", os.ValorTotal.ToString("C2"), 40));
        sb.AppendLine("========================================");
        sb.AppendLine("Este documento é um orçamento");
        sb.AppendLine("e não tem valor fiscal.");
        sb.AppendLine("Válido por 7 dias.");
        sb.AppendLine("========================================");

        return sb.ToString();
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
            var fonte = new Font("Consolas", 10);
            var brush = Brushes.Black;
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

    private string FormatarLinhaComValorDireita(string descricao, string valor, int larguraTotal)
    {
        var maxDescricao = larguraTotal - valor.Length - 1;
        if (descricao.Length > maxDescricao)
            descricao = descricao.Substring(0, maxDescricao);

        var espacos = larguraTotal - descricao.Length - valor.Length;
        if (espacos < 1) espacos = 1;

        return descricao + new string(' ', espacos) + valor;
    }

    // Gera a OS em formato bitmap A4 (2480 x 3508 pixels a 300 DPI).
    private Bitmap GerarOSBitmap(
    OrdemServicoDetalheDTO os,
    Domain.Entities.ConfiguracaoEmpresa? empresa)
{
    const int larguraPagina = 2480;
    const int alturaPagina = 3508;

    var bitmap = new Bitmap(larguraPagina, alturaPagina);
    bitmap.SetResolution(300, 300);

    using (var g = Graphics.FromImage(bitmap))
    {
        g.SmoothingMode = SmoothingMode.AntiAlias;
        g.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;
        g.Clear(Color.White);

        var fonteTitulo = new Font("Arial", 15, FontStyle.Bold);
        var fonteLabel = new Font("Arial", 8, FontStyle.Bold);
        var fonteTexto = new Font("Arial", 8);
        var fonteTextoBold = new Font("Arial", 8, FontStyle.Bold);
        var fonteNumeroOS = new Font("Arial", 16, FontStyle.Bold);
        var fonteValorTotal = new Font("Arial", 10, FontStyle.Bold);
        var fonteRodape = new Font("Arial", 7, FontStyle.Italic);

        var canetaFina = new Pen(Color.Black, 1.5f);
        var canetaGrossa = new Pen(Color.Black, 2.5f);

        float margemEsq = 100;
        float margemDir = larguraPagina - 100;
        float larguraUtil = margemDir - margemEsq;

        float y = 100;

        // CABEÇALHO
        float alturaCabecalho = 320;

        float larguraColuna1 = larguraUtil * 0.35f;
        float larguraColuna2 = larguraUtil * 0.30f;
        float larguraColuna3 = larguraUtil - larguraColuna1 - larguraColuna2;

        float xCol2 = margemEsq + larguraColuna1;
        float xCol3 = xCol2 + larguraColuna2;

        // Logo + empresa
        DesenharLogoEEmpresa(
            g,
            empresa,
            margemEsq + 10,
            y + 10,
            larguraColuna1 - 20,
            alturaCabecalho - 20,
            fonteTitulo,
            fonteTexto
        );

        var centroCol2 = xCol2 + larguraColuna2 / 2;

        var textoOS = "ORDEM DE SERVIÇO";
        var tamanhoTextoOS = g.MeasureString(textoOS, fonteLabel);
        g.DrawString(textoOS, fonteLabel, Brushes.Black,
            centroCol2 - tamanhoTextoOS.Width / 2, y + 40);

        var numeroTexto = $"Nº {os.Numero:D6}";
        var tamanhoNumero = g.MeasureString(numeroTexto, fonteNumeroOS);
        g.DrawString(numeroTexto, fonteNumeroOS, Brushes.Black,
            centroCol2 - tamanhoNumero.Width / 2, y + 75);

        var dataTexto = $"Emissão: {os.DataAbertura:dd/MM/yyyy}";
        var tamanhoData = g.MeasureString(dataTexto, fonteTexto);
        g.DrawString(dataTexto, fonteTexto, Brushes.Black,
            centroCol2 - tamanhoData.Width / 2, y + 160);

        var horaTexto = $"Hora: {os.DataAbertura:HH:mm}";
        var tamanhoHora = g.MeasureString(horaTexto, fonteTexto);
        g.DrawString(horaTexto, fonteTexto, Brushes.Black,
            centroCol2 - tamanhoHora.Width / 2, y + 200);

        g.DrawString("DADOS DA EMPRESA", fonteLabel, Brushes.Black, xCol3 + 15, y + 40);

        float yEmpresa = y + 95;

        if (empresa != null)
        {
            g.DrawString(empresa.NomeEmpresa, fonteTextoBold, Brushes.Black, xCol3 + 15, yEmpresa);
            yEmpresa += 45;

            g.DrawString($"CNPJ: {empresa.Cnpj}", fonteTexto, Brushes.Black, xCol3 + 15, yEmpresa);
            yEmpresa += 45;

            g.DrawString($"Tel: {empresa.Telefone}", fonteTexto, Brushes.Black, xCol3 + 15, yEmpresa);
            yEmpresa += 45;

            if (!string.IsNullOrWhiteSpace(empresa.Endereco))
            {
                var enderecoLinhas = QuebrarTexto(empresa.Endereco, 40);
                foreach (var linha in enderecoLinhas)
                {
                    g.DrawString(linha, fonteTexto, Brushes.Black, xCol3 + 15, yEmpresa);
                    yEmpresa += 38;
                }
            }
        }

        g.DrawRectangle(canetaGrossa, margemEsq, y, larguraUtil, alturaCabecalho);
        g.DrawLine(canetaFina, xCol2, y, xCol2, y + alturaCabecalho);
        g.DrawLine(canetaFina, xCol3, y, xCol3, y + alturaCabecalho);

        y += alturaCabecalho;

        // CLIENTE E VEÍCULO
        float alturaCabecalhoBloco = 60;

        g.DrawRectangle(canetaFina, margemEsq, y, larguraUtil, alturaCabecalhoBloco);
        g.DrawString("CLIENTE E VEÍCULO", fonteLabel, Brushes.Black,
            margemEsq + 15, y + 16);

        y += alturaCabecalhoBloco;

        float alturaLinha = 100;

        float colCliente1 = larguraUtil * 0.40f;
        float colCliente2 = larguraUtil * 0.30f;
        float colCliente3 = larguraUtil - colCliente1 - colCliente2;

        float xC2 = margemEsq + colCliente1;
        float xC3 = xC2 + colCliente2;

        g.DrawString("CLIENTE", fonteLabel, Brushes.Black, margemEsq + 15, y + 18);
        g.DrawString(TruncarTexto(os.NomeCliente, 35), fonteTexto, Brushes.Black,
            margemEsq + 15, y + 55);

        g.DrawString("CPF/CNPJ", fonteLabel, Brushes.Black, xC2 + 15, y + 18);
        g.DrawString(TruncarTexto(os.CpfCliente ?? "-", 25), fonteTexto, Brushes.Black,
            xC2 + 15, y + 55);

        g.DrawString("TELEFONE", fonteLabel, Brushes.Black, xC3 + 15, y + 18);
        g.DrawString(TruncarTexto(os.TelefoneCliente ?? "-", 25), fonteTexto, Brushes.Black,
            xC3 + 15, y + 55);

        g.DrawLine(canetaFina, margemEsq, y + alturaLinha, margemDir, y + alturaLinha);
        y += alturaLinha;

        float colVeiculo1 = larguraUtil * 0.50f;
        float colVeiculo2 = larguraUtil * 0.25f;

        float xV2 = margemEsq + colVeiculo1;
        float xV3 = xV2 + colVeiculo2;

        var descricaoVeiculo = $"{os.MarcaVeiculo} {os.ModeloVeiculo} {os.AnoVeiculo}".Trim();

        g.DrawString("VEÍCULO", fonteLabel, Brushes.Black, margemEsq + 15, y + 18);
        g.DrawString(TruncarTexto(descricaoVeiculo, 45), fonteTexto, Brushes.Black,
            margemEsq + 15, y + 55);

        g.DrawString("PLACA", fonteLabel, Brushes.Black, xV2 + 15, y + 18);
        g.DrawString(TruncarTexto(os.PlacaVeiculo ?? "-", 15), fonteTexto, Brushes.Black,
            xV2 + 15, y + 55);

        g.DrawString("QUILOMETRAGEM", fonteLabel, Brushes.Black, xV3 + 15, y + 18);
        var km = os.Quilometragem.HasValue ? $"{os.Quilometragem.Value:N0} km" : "-";
        g.DrawString(km, fonteTexto, Brushes.Black, xV3 + 15, y + 55);

        y += alturaLinha;

        g.DrawRectangle(canetaFina, margemEsq, y - alturaLinha * 2 - alturaCabecalhoBloco,
            larguraUtil, alturaCabecalhoBloco + alturaLinha * 2);

        // PEÇAS E SERVIÇOS
        y += 25;

        g.DrawRectangle(canetaFina, margemEsq, y, larguraUtil, alturaCabecalhoBloco);
        g.DrawString("PEÇAS E SERVIÇOS", fonteLabel, Brushes.Black, margemEsq + 15, y + 16);

        y += alturaCabecalhoBloco;

        float colCodigo = larguraUtil * 0.12f;
        float colDescricao = larguraUtil * 0.48f;
        float colQtd = larguraUtil * 0.10f;
        float colUnit = larguraUtil * 0.15f;
        float colTotal = larguraUtil * 0.15f;

        float xCodigo = margemEsq;
        float xDescricao = xCodigo + colCodigo;
        float xQtd = xDescricao + colDescricao;
        float xUnit = xQtd + colQtd;
        float xTotal = xUnit + colUnit;

        float alturaCabecalhoGrade = 55;

        g.DrawRectangle(canetaFina, margemEsq, y, larguraUtil, alturaCabecalhoGrade);
        g.DrawString("CÓDIGO", fonteLabel, Brushes.Black, xCodigo + 10, y + 18);
        g.DrawString("DESCRIÇÃO", fonteLabel, Brushes.Black, xDescricao + 10, y + 18);
        g.DrawString("QTD.", fonteLabel, Brushes.Black, xQtd + 10, y + 18);
        g.DrawString("VL UNIT.", fonteLabel, Brushes.Black, xUnit + 10, y + 18);
        g.DrawString("TOTAL", fonteLabel, Brushes.Black, xTotal + 10, y + 18);

        y += alturaCabecalhoGrade;

        float alturaLinhaGrade = 60;
        float yInicioGrade = y;

        // Peças
        foreach (var peca in os.ItensPeca)
        {
            g.DrawString(peca.PecaId.ToString(), fonteTexto, Brushes.Black, xCodigo + 10, y + 22);
            g.DrawString(TruncarTexto(peca.NomePeca, 50), fonteTexto, Brushes.Black, xDescricao + 10, y + 22);
            g.DrawString($"{peca.Quantidade}", fonteTexto, Brushes.Black, xQtd + 10, y + 22);
            g.DrawString(peca.PrecoUnitario.ToString("C2"), fonteTexto, Brushes.Black, xUnit + 10, y + 22);
            g.DrawString(peca.ValorTotal.ToString("C2"), fonteTextoBold, Brushes.Black, xTotal + 10, y + 22);

            y += alturaLinhaGrade;
            g.DrawLine(canetaFina, margemEsq, y, margemDir, y);
        }

        // Serviços
        foreach (var servico in os.ItensServico)
        {
            g.DrawString("-", fonteTexto, Brushes.Black, xCodigo + 10, y + 22);
            g.DrawString(TruncarTexto(servico.NomeServico, 50), fonteTexto, Brushes.Black, xDescricao + 10, y + 22);
            g.DrawString("-", fonteTexto, Brushes.Black, xQtd + 10, y + 22);
            g.DrawString(servico.PrecoUnitario.ToString("C2"), fonteTexto, Brushes.Black, xUnit + 10, y + 22);
            g.DrawString(servico.PrecoUnitario.ToString("C2"), fonteTextoBold, Brushes.Black, xTotal + 10, y + 22);

            y += alturaLinhaGrade;
            g.DrawLine(canetaFina, margemEsq, y, margemDir, y);
        }

        g.DrawRectangle(canetaFina, margemEsq, yInicioGrade - alturaCabecalhoGrade,
            larguraUtil, y - (yInicioGrade - alturaCabecalhoGrade));
        g.DrawLine(canetaFina, xDescricao, yInicioGrade - alturaCabecalhoGrade, xDescricao, y);
        g.DrawLine(canetaFina, xQtd, yInicioGrade - alturaCabecalhoGrade, xQtd, y);
        g.DrawLine(canetaFina, xUnit, yInicioGrade - alturaCabecalhoGrade, xUnit, y);
        g.DrawLine(canetaFina, xTotal, yInicioGrade - alturaCabecalhoGrade, xTotal, y);

        // OBSERVAÇÕES + RESUMO
        y += 25;

        float alturaBlocoRodape = 300;

        float larguraObs = larguraUtil * 0.55f;
        float larguraResumo = larguraUtil - larguraObs;
        float xResumo = margemEsq + larguraObs;

        g.DrawRectangle(canetaFina, margemEsq, y, larguraUtil, alturaBlocoRodape);
        g.DrawLine(canetaFina, xResumo, y, xResumo, y + alturaBlocoRodape);

        g.DrawString("OBSERVAÇÕES", fonteLabel, Brushes.Black, margemEsq + 15, y + 20);

        if (!string.IsNullOrWhiteSpace(os.Observacao))
        {
            var obsLinhas = QuebrarTexto(os.Observacao, 50);
            float yObs = y + 65;

            foreach (var linha in linhasObservacao(obsLinhas, 8))
            {
                g.DrawString(linha, fonteTexto, Brushes.Black, margemEsq + 15, yObs);
                yObs += 35;
            }
        }

        // Resumo de valores
        float yResumo = y + 30;
        float xResumoLabel = xResumo + 15;
        float xResumoValor = margemDir - 15;

        g.DrawString("Subtotal de peças", fonteTexto, Brushes.Black, xResumoLabel, yResumo);
        var textoSubtotalPecas = os.ValorTotalPecas.ToString("C2");
        var tamanhoSubtotalPecas = g.MeasureString(textoSubtotalPecas, fonteTexto);
        g.DrawString(textoSubtotalPecas, fonteTexto, Brushes.Black,
            xResumoValor - tamanhoSubtotalPecas.Width, yResumo);
        yResumo += 50;

        g.DrawString("Subtotal de serviços", fonteTexto, Brushes.Black, xResumoLabel, yResumo);
        var textoSubtotalServicos = os.ValorTotalServicos.ToString("C2");
        var tamanhoSubtotalServicos = g.MeasureString(textoSubtotalServicos, fonteTexto);
        g.DrawString(textoSubtotalServicos, fonteTexto, Brushes.Black,
            xResumoValor - tamanhoSubtotalServicos.Width, yResumo);
        yResumo += 50;

        var textoDescontoLabel = $"Descontos (%)";
        g.DrawString(textoDescontoLabel, fonteTexto, Brushes.Black, xResumoLabel, yResumo);
        var textoDesconto = $"{os.DescontoPercentual:F1}%";
        var tamanhoDesconto = g.MeasureString(textoDesconto, fonteTexto);
        g.DrawString(textoDesconto, fonteTexto, Brushes.Black,
            xResumoValor - tamanhoDesconto.Width, yResumo);
        yResumo += 55;

        g.DrawLine(canetaFina, xResumo + 15, yResumo, margemDir - 15, yResumo);
        yResumo += 25;

        g.DrawString("VALOR TOTAL", fonteValorTotal, Brushes.Black, xResumoLabel, yResumo + 5);
        var textoValorTotal = os.ValorTotal.ToString("C2");
        var tamanhoValorTotal = g.MeasureString(textoValorTotal, fonteValorTotal);
        g.DrawString(textoValorTotal, fonteValorTotal, Brushes.Black,
            xResumoValor - tamanhoValorTotal.Width, yResumo + 3);

        y += alturaBlocoRodape + 30;

        var textoRodape = "ESTE ORÇAMENTO NÃO É UM DOCUMENTO FISCAL E POSSUI VALIDADE DE 7 DIAS.";
        var tamanhoRodape = g.MeasureString(textoRodape, fonteRodape);
        g.DrawString(textoRodape, fonteRodape, Brushes.Black,
            (larguraPagina - tamanhoRodape.Width) / 2, y + 20);

        fonteTitulo.Dispose();
        fonteLabel.Dispose();
        fonteTexto.Dispose();
        fonteTextoBold.Dispose();
        fonteNumeroOS.Dispose();
        fonteValorTotal.Dispose();
        fonteRodape.Dispose();
        canetaFina.Dispose();
        canetaGrossa.Dispose();
    }

    return bitmap;
}
    public Bitmap GerarEtiquetaBitmap(PecaDTO peca, string nomeEmpresa)
    {
        // 40mm x 30mm a 300 DPI = 472 x 354 pixels (formato horizontal ANTES da rotação)
        const int largura = 472;
        const int altura = 354;

        var bitmap = new Bitmap(largura, altura);
        bitmap.SetResolution(300, 300);

        using (var g = Graphics.FromImage(bitmap))
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;
            g.Clear(Color.White);

            float alturaCabecalho = 60f;
            g.FillRectangle(Brushes.Black, 0, 0, largura, (int)alturaCabecalho);

            var fonteEmpresa = new Font("Arial", 13, FontStyle.Bold);
            var tamanhoEmpresa = g.MeasureString(nomeEmpresa, fonteEmpresa);
            var xEmpresa = (largura - tamanhoEmpresa.Width) / 2;
            var yEmpresa = (alturaCabecalho - tamanhoEmpresa.Height) / 2;

            g.SetClip(new RectangleF(0, 0, largura, alturaCabecalho));
            g.DrawString(nomeEmpresa, fonteEmpresa, Brushes.White, xEmpresa, yEmpresa);
            g.ResetClip();

            var fonteId = new Font("Arial", 12, FontStyle.Bold);
            var fonteNome = new Font("Arial", 12);
            var fonteValor = new Font("Arial", 14, FontStyle.Bold);
            var fonteCodigo = new Font("Arial", 12);

            float centroX = largura / 2f;
            float y = alturaCabecalho + 15f;

            var textoId = peca.Id.ToString();
            var tamanhoId = g.MeasureString(textoId, fonteId);
            g.DrawString(textoId, fonteId, Brushes.Black, centroX - tamanhoId.Width / 2, y);
            y += fonteId.GetHeight(g) + 5;

            var nome = peca.Nome.Length > 40 ? peca.Nome.Substring(0, 40) + "..." : peca.Nome;
            var tamanhoNome = g.MeasureString(nome, fonteNome);
            g.DrawString(nome, fonteNome, Brushes.Black, centroX - tamanhoNome.Width / 2, y);
            y += fonteNome.GetHeight(g) + 10;

            var textoValor = peca.ValorVenda.ToString("C2");
            var tamanhoValor = g.MeasureString(textoValor, fonteValor);
            g.DrawString(textoValor, fonteValor, Brushes.Black, centroX - tamanhoValor.Width / 2, y);
            y += fonteValor.GetHeight(g) + 8;

            var codigo = peca.CodigoPeca ?? "-";
            var tamanhoCodigo = g.MeasureString(codigo, fonteCodigo);
            g.DrawString(codigo, fonteCodigo, Brushes.Black, centroX - tamanhoCodigo.Width / 2, y);

            fonteEmpresa.Dispose();
            fonteId.Dispose();
            fonteNome.Dispose();
            fonteValor.Dispose();
            fonteCodigo.Dispose();
        }

        return bitmap;
    }

    public void ImprimirEtiqueta(PecaDTO peca, int copias)
    {
        if (copias < 1) copias = 1;

        var config = _configRepo.ObterPorTipo(TipoImpressao.Etiqueta).GetAwaiter().GetResult();
        if (config == null)
            throw new InvalidOperationException("Nenhuma impressora configurada para Etiqueta.");

        var empresa = _context.ConfiguracoesEmpresa
            .AsNoTracking()
            .FirstOrDefault();

        var nomeEmpresa = empresa?.NomeEmpresa ?? "AUTO ELÉTRICA";

        using var bitmapOriginal = GerarEtiquetaBitmap(peca, nomeEmpresa);

        // Rotaciona 90°: horizontal (40x30) → vertical (30x40)
        bitmapOriginal.RotateFlip(RotateFlipType.Rotate90FlipNone);

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
                // 30mm largura x 40mm altura em centésimos de polegada
                PaperSize = new PaperSize("Etiqueta 30x40 Rotacionada", 118, 157),
                Margins = new Margins(0, 0, 0, 0),
                Landscape = false
            }
        };

        var copiaAtual = 0;

        printDocument.PrintPage += (sender, e) =>
        {
            var g = e.Graphics!;
            g.DrawImage(bitmapOriginal, e.MarginBounds);

            copiaAtual++;
            e.HasMorePages = copiaAtual < copias;
        };

        printDocument.Print();
    }

    private void DesenharLogoEEmpresa(Graphics g, Domain.Entities.ConfiguracaoEmpresa? empresa,
    float x, float y, float larguraMaxima, float alturaMaxima,
    Font fonteTitulo, Font fonteTexto)
{
    if (empresa == null)
        return;

    float xAtual = x;
    float espacamento = 20;
    if (!string.IsNullOrWhiteSpace(empresa.LogoPath) && File.Exists(empresa.LogoPath))
    {
        try
        {
            using var logo = Image.FromFile(empresa.LogoPath);

            float alturaLogo = alturaMaxima;

            float larguraLogo = logo.Width * (alturaLogo / logo.Height);

            var tamanhoNome = g.MeasureString(empresa.NomeEmpresa, fonteTitulo);
            float larguraNecessariaNome = tamanhoNome.Width + espacamento * 2;

            if (larguraLogo + larguraNecessariaNome > larguraMaxima)
            {
                larguraLogo = larguraMaxima - larguraNecessariaNome;
                alturaLogo = logo.Height * (larguraLogo / logo.Width);
            }

            float logoY = y + (alturaMaxima - alturaLogo) / 2;
            g.DrawImage(logo, xAtual, logoY, larguraLogo, alturaLogo);
            xAtual += larguraLogo + espacamento;
        }
        catch
        {
            // Se falhar, segue sem o logo
        }
    }

    var tamanhoTexto = g.MeasureString(empresa.NomeEmpresa, fonteTitulo);
    var textoY = y + (alturaMaxima - tamanhoTexto.Height) / 2;
    g.DrawString(empresa.NomeEmpresa, fonteTitulo, Brushes.Black, xAtual, textoY);
}

    private string TruncarTexto(string texto, int tamanhoMaximo)
    {
        if (string.IsNullOrEmpty(texto)) return "";
        return texto.Length > tamanhoMaximo ? texto.Substring(0, tamanhoMaximo - 3) + "..." : texto;
    }

    private List<string> QuebrarTexto(string texto, int tamanhoMaximo)
    {
        var linhas = new List<string>();
        if (string.IsNullOrWhiteSpace(texto)) return linhas;

        var palavras = texto.Split(' ');
        var linhaAtual = "";

        foreach (var palavra in palavras)
        {
            if ((linhaAtual + " " + palavra).Trim().Length > tamanhoMaximo)
            {
                if (!string.IsNullOrEmpty(linhaAtual))
                    linhas.Add(linhaAtual.Trim());

                linhaAtual = palavra;
            }
            else
            {
                linhaAtual = (linhaAtual + " " + palavra).Trim();
            }
        }

        if (!string.IsNullOrEmpty(linhaAtual))
            linhas.Add(linhaAtual.Trim());

        return linhas;
    }

    // Retorna apenas as primeiras N linhas de uma lista.
    private IEnumerable<string> linhasObservacao(List<string> linhas, int maxLinhas)
    {
        return linhas.Take(maxLinhas);
    }
}
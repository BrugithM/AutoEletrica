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
    // IMPRESSÃO DE OS
    public void ImprimirOS(OrdemServicoDetalheDTO os)
    {
        var config = _configRepo.ObterPorTipo(TipoImpressao.OS).GetAwaiter().GetResult();
        if (config == null)
            throw new InvalidOperationException("Nenhuma impressora configurada para OS.");
        var empresa = _context.ConfiguracoesEmpresa
            .AsNoTracking()
            .FirstOrDefault();
        var estado = new EstadoImpressaoOS(os, empresa);
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
                PaperSize = new PaperSize("A4", 827, 1169),
                Margins = new Margins(40, 40, 40, 40),
                Landscape = false
            }
        };
        printDocument.PrintPage += (sender, e) =>
        {
            var g = e.Graphics!;
            var area = e.MarginBounds;
            bool temMais = DesenharPaginaOS(g, area, estado);
            e.HasMorePages = temMais;
        };
        printDocument.Print();
    }
    private class EstadoImpressaoOS
    {
        public OrdemServicoDetalheDTO Os { get; }
        public Domain.Entities.ConfiguracaoEmpresa? Empresa { get; }
        public bool CabecalhoConcluido { get; set; } = false;
        public bool ClienteConcluido { get; set; } = false;
        public int IndiceLinhasDescricao { get; set; } = 0;
        public int IndicePecas { get; set; } = 0;
        public int IndiceServicos { get; set; } = 0;
        public int IndiceLinhasObservacao { get; set; } = 0;
        public int NumeroPagina { get; set; } = 1;
        public EstadoImpressaoOS(OrdemServicoDetalheDTO os, Domain.Entities.ConfiguracaoEmpresa? empresa)
        {
            Os = os;
            Empresa = empresa;
        }
    }
    private bool DesenharPaginaOS(Graphics g, Rectangle area, EstadoImpressaoOS estado)
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
        float margemEsq = area.Left;
        float margemDir = area.Right;
        float larguraUtil = margemDir - margemEsq;
        float y = area.Top;
        float limiteSeguranca = area.Bottom - 20;
        try
        {
            // CABEÇALHO
            if (!estado.CabecalhoConcluido)
            {
                float alturaCabecalho = 110;
                float larguraColuna1 = larguraUtil * 0.35f;
                float larguraColuna2 = larguraUtil * 0.30f;
                float larguraColuna3 = larguraUtil - larguraColuna1 - larguraColuna2;
                float xCol2 = margemEsq + larguraColuna1;
                float xCol3 = xCol2 + larguraColuna2;
                DesenharLogoEmpresa(g, estado.Empresa, margemEsq + 10, y + 5, larguraColuna1 - 20, alturaCabecalho - 10, fonteTitulo, fonteTexto);
                var centroCol2 = xCol2 + larguraColuna2 / 2;
                var textoOS = "ORDEM DE SERVIÇO";
                var tamanhoTextoOS = g.MeasureString(textoOS, fonteLabel);
                g.DrawString(textoOS, fonteLabel, Brushes.Black, centroCol2 - tamanhoTextoOS.Width / 2, y + 15);
                var numeroTexto = $"Nº {estado.Os.Numero:D6}";
                var tamanhoNumero = g.MeasureString(numeroTexto, fonteNumeroOS);
                g.DrawString(numeroTexto, fonteNumeroOS, Brushes.Black, centroCol2 - tamanhoNumero.Width / 2, y + 30);
                var dataTexto = $"Emissão: {estado.Os.DataAbertura.ToLocalTime():dd/MM/yyyy}";
                var tamanhoData = g.MeasureString(dataTexto, fonteTexto);
                g.DrawString(dataTexto, fonteTexto, Brushes.Black, centroCol2 - tamanhoData.Width / 2, y + 65);
                var horaTexto = $"Hora: {estado.Os.DataAbertura.ToLocalTime():HH:mm}";
                var tamanhoHora = g.MeasureString(horaTexto, fonteTexto);
                g.DrawString(horaTexto, fonteTexto, Brushes.Black, centroCol2 - tamanhoHora.Width / 2, y + 80);
                g.DrawString("DADOS DA EMPRESA", fonteLabel, Brushes.Black, xCol3 + 5, y + 15);
                float yEmpresa = y + 30;
                if (estado.Empresa != null)
                {
                    g.DrawString(estado.Empresa.NomeEmpresa, fonteTextoBold, Brushes.Black, xCol3 + 5, yEmpresa); yEmpresa += 15;
                    g.DrawString($"CNPJ: {estado.Empresa.Cnpj}", fonteTexto, Brushes.Black, xCol3 + 5, yEmpresa); yEmpresa += 15;
                    g.DrawString($"Tel: {estado.Empresa.Telefone}", fonteTexto, Brushes.Black, xCol3 + 5, yEmpresa); yEmpresa += 15;
                    if (!string.IsNullOrWhiteSpace(estado.Empresa.Endereco))
                    {
                        foreach (var linha in QuebrarTexto(estado.Empresa.Endereco, 35))
                        {
                            g.DrawString(linha, fonteTexto, Brushes.Black, xCol3 + 5, yEmpresa); yEmpresa += 15;
                        }
                    }
                }
                g.DrawRectangle(canetaGrossa, margemEsq, y, larguraUtil, alturaCabecalho);
                g.DrawLine(canetaFina, xCol2, y, xCol2, y + alturaCabecalho);
                g.DrawLine(canetaFina, xCol3, y, xCol3, y + alturaCabecalho);
                y += alturaCabecalho;
                estado.CabecalhoConcluido = true;
            }
            else
            {
                g.DrawString($"OS Nº {estado.Os.Numero:D6} - Continuação", fonteLabel, Brushes.Black, margemEsq, y);
                y += 20;
                g.DrawLine(canetaFina, margemEsq, y, margemDir, y);
                y += 10;
            }
            // CLIENTE E VEÍCULO
            if (!estado.ClienteConcluido)
            {
                float alturaCabecalhoBloco = 20;
                g.DrawRectangle(canetaFina, margemEsq, y, larguraUtil, alturaCabecalhoBloco);
                g.DrawString("CLIENTE E VEÍCULO", fonteLabel, Brushes.Black, margemEsq + 5, y + 5);
                y += alturaCabecalhoBloco;
                float alturaLinha = 35;
                float colCliente1 = larguraUtil * 0.40f;
                float colCliente2 = larguraUtil * 0.30f;
                float xC2 = margemEsq + colCliente1;
                float xC3 = xC2 + colCliente2;
                g.DrawString("CLIENTE", fonteLabel, Brushes.Black, margemEsq + 5, y + 5);
                g.DrawString(TruncarTexto(estado.Os.NomeCliente, 35), fonteTexto, Brushes.Black, margemEsq + 5, y + 20);
                g.DrawString("CPF/CNPJ", fonteLabel, Brushes.Black, xC2 + 5, y + 5);
                g.DrawString(TruncarTexto(estado.Os.CpfCliente ?? "-", 25), fonteTexto, Brushes.Black, xC2 + 5, y + 20);
                g.DrawString("TELEFONE", fonteLabel, Brushes.Black, xC3 + 5, y + 5);
                g.DrawString(TruncarTexto(estado.Os.TelefoneCliente ?? "-", 25), fonteTexto, Brushes.Black, xC3 + 5, y + 20);
                y += alturaLinha;
                g.DrawLine(canetaFina, margemEsq, y, margemDir, y);
                g.DrawString("VEÍCULO", fonteLabel, Brushes.Black, margemEsq + 5, y + 5);
                g.DrawString(TruncarTexto(estado.Os.ModeloVeiculo ?? "-", 45), fonteTexto, Brushes.Black, margemEsq + 5, y + 20);
                g.DrawString("PLACA", fonteLabel, Brushes.Black, xC2 + 5, y + 5);
                g.DrawString(TruncarTexto(estado.Os.PlacaVeiculo ?? "-", 15), fonteTexto, Brushes.Black, xC2 + 5, y + 20);
                g.DrawString("QUILOMETRAGEM", fonteLabel, Brushes.Black, xC3 + 5, y + 5);
                var km = estado.Os.Quilometragem.HasValue ? $"{estado.Os.Quilometragem.Value:N0} km" : "-";
                g.DrawString(km, fonteTexto, Brushes.Black, xC3 + 5, y + 20);
                y += alturaLinha;
                g.DrawRectangle(canetaFina, margemEsq, y - alturaLinha * 2 - alturaCabecalhoBloco, larguraUtil, alturaCabecalhoBloco + alturaLinha * 2);
                estado.ClienteConcluido = true;
            }
            // DESCRIÇÃO DE SERVIÇOS
            if (!string.IsNullOrWhiteSpace(estado.Os.DescricaoServicos))
            {
                var descLinhas = QuebrarTextoPorLargura(g, estado.Os.DescricaoServicos, fonteTexto, larguraUtil - 10);
                if (estado.IndiceLinhasDescricao < descLinhas.Count)
                {
                    y += 10;
                    float startY = y;
                    if (estado.IndiceLinhasDescricao == 0)
                    {
                        g.DrawString("DESCRIÇÃO DOS SERVIÇOS", fonteLabel, Brushes.Black, margemEsq + 5, y + 5);
                        y += 15;
                    }
                    while (estado.IndiceLinhasDescricao < descLinhas.Count)
                    {
                        if (y + 15 > limiteSeguranca - 100)
                        {
                            g.DrawRectangle(canetaFina, margemEsq, startY - 5, larguraUtil, y - startY + 5);
                            estado.NumeroPagina++;
                            return true;
                        }
                        g.DrawString(descLinhas[estado.IndiceLinhasDescricao], fonteTexto, Brushes.Black, margemEsq + 5, y);
                        y += 15;
                        estado.IndiceLinhasDescricao++;
                    }
                    g.DrawRectangle(canetaFina, margemEsq, startY - 5, larguraUtil, y - startY + 5);
                }
            }

            // PEÇAS E SERVIÇOS
            float colCodigo = larguraUtil * 0.12f;
            float colDescricao = larguraUtil * 0.48f;
            float colQtd = larguraUtil * 0.10f;
            float colUnit = larguraUtil * 0.15f;

            float xCodigo = margemEsq;
            float xDescricao = xCodigo + colCodigo;
            float xQtd = xDescricao + colDescricao;
            float xUnit = xQtd + colQtd;
            float xTotal = xUnit + colUnit;

            bool temPecas = estado.Os.ItensPeca.Count > 0;
            bool temServicos = estado.Os.ItensServico.Count > 0;

            if (temPecas || temServicos)
            {
                if (estado.IndicePecas == 0 && estado.IndiceServicos == 0)
                    y += 10;

                float alturaTituloGrade = 22;
                float alturaCabecalhoGrade = 22;
                float alturaLinhaGrade = 20;

                if (estado.IndicePecas == 0 && estado.IndiceServicos == 0)
                {
                    g.DrawRectangle(
                        canetaFina,
                        margemEsq,
                        y,
                        larguraUtil,
                        alturaTituloGrade);

                    g.DrawString(
                        "PEÇAS E SERVIÇOS",
                        fonteLabel,
                        Brushes.Black,
                        margemEsq + 5,
                        y + 6);

                    y += alturaTituloGrade;

                    float yInicioCabecalho = y;

                    g.DrawRectangle(
                        canetaFina,
                        margemEsq,
                        yInicioCabecalho,
                        larguraUtil,
                        alturaCabecalhoGrade);

                    float yTextoCabecalho =
                        yInicioCabecalho +
                        (alturaCabecalhoGrade - fonteLabel.GetHeight(g)) / 2f;

                    g.DrawString("CÓDIGO", fonteLabel, Brushes.Black,
                        xCodigo + 5, yTextoCabecalho);

                    g.DrawString("DESCRIÇÃO", fonteLabel, Brushes.Black,
                        xDescricao + 5, yTextoCabecalho);

                    g.DrawString("QTD.", fonteLabel, Brushes.Black,
                        xQtd + 5, yTextoCabecalho);

                    g.DrawString("VL UNIT.", fonteLabel, Brushes.Black,
                        xUnit + 5, yTextoCabecalho);

                    g.DrawString("TOTAL", fonteLabel, Brushes.Black,
                        xTotal + 5, yTextoCabecalho);

                    y += alturaCabecalhoGrade;
                }

                float yInicioGrade = y - alturaCabecalhoGrade;

                while (estado.IndicePecas < estado.Os.ItensPeca.Count)
                {
                    if (y + alturaLinhaGrade > limiteSeguranca - 120)
                    {
                        DesenharBordasGradeOS(
                            g, canetaFina, margemEsq, margemDir,
                            xDescricao, xQtd, xUnit, xTotal,
                            yInicioGrade, y);

                        estado.NumeroPagina++;
                        return true;
                    }

                    var peca = estado.Os.ItensPeca[estado.IndicePecas];

                    g.DrawString(peca.PecaId.ToString(), fonteTexto,
                        Brushes.Black, xCodigo + 5, y + 3);

                    g.DrawString(TruncarTexto(peca.NomePeca, 50), fonteTexto,
                        Brushes.Black, xDescricao + 5, y + 3);

                    g.DrawString($"{peca.Quantidade:0.###}", fonteTexto,
                        Brushes.Black, xQtd + 5, y + 3);

                    g.DrawString(peca.PrecoUnitario.ToString("C2"), fonteTexto,
                        Brushes.Black, xUnit + 5, y + 3);

                    g.DrawString(peca.ValorTotal.ToString("C2"), fonteTextoBold,
                        Brushes.Black, xTotal + 5, y + 3);

                    y += alturaLinhaGrade;
                    g.DrawLine(canetaFina, margemEsq, y, margemDir, y);

                    estado.IndicePecas++;
                }

                while (estado.IndiceServicos < estado.Os.ItensServico.Count)
                {
                    if (y + alturaLinhaGrade > limiteSeguranca - 120)
                    {
                        DesenharBordasGradeOS(
                            g, canetaFina, margemEsq, margemDir,
                            xDescricao, xQtd, xUnit, xTotal,
                            yInicioGrade, y);

                        estado.NumeroPagina++;
                        return true;
                    }

                    var servico = estado.Os.ItensServico[estado.IndiceServicos];

                    g.DrawString("-", fonteTexto, Brushes.Black,
                        xCodigo + 5, y + 3);

                    g.DrawString(TruncarTexto(servico.NomeServico, 50), fonteTexto,
                        Brushes.Black, xDescricao + 5, y + 3);

                    g.DrawString("-", fonteTexto, Brushes.Black,
                        xQtd + 5, y + 3);

                    g.DrawString(servico.PrecoUnitario.ToString("C2"), fonteTexto,
                        Brushes.Black, xUnit + 5, y + 3);

                    g.DrawString(servico.PrecoUnitario.ToString("C2"), fonteTextoBold,
                        Brushes.Black, xTotal + 5, y + 3);

                    y += alturaLinhaGrade;
                    g.DrawLine(canetaFina, margemEsq, y, margemDir, y);

                    estado.IndiceServicos++;
                }

                DesenharBordasGradeOS(
                    g, canetaFina, margemEsq, margemDir,
                    xDescricao, xQtd, xUnit, xTotal,
                    yInicioGrade, y);
            }

            // OBSERVAÇÕES + RESUMO
            if (y + 100 > limiteSeguranca)
            {
                estado.NumeroPagina++;
                return true;
            }
            y += 10;
            float startObsY = y;
            float larguraObs = larguraUtil * 0.55f;
            float xResumo = margemEsq + larguraObs;
            g.DrawString("OBSERVAÇÕES", fonteLabel, Brushes.Black, margemEsq + 5, y + 5);
            float yTextoObs = y + 20;
            if (!string.IsNullOrWhiteSpace(estado.Os.Observacao))
            {
                var obsLinhas = QuebrarTextoPorLargura(g, estado.Os.Observacao, fonteTexto, larguraObs - 10);
                while (estado.IndiceLinhasObservacao < obsLinhas.Count)
                {
                    if (yTextoObs + 15 > limiteSeguranca - 50)
                    {
                        g.DrawRectangle(canetaFina, margemEsq, startObsY, larguraObs, yTextoObs - startObsY + 10);
                        estado.NumeroPagina++;
                        return true;
                    }
                    g.DrawString(obsLinhas[estado.IndiceLinhasObservacao], fonteTexto, Brushes.Black, margemEsq + 5, yTextoObs);
                    yTextoObs += 15;
                    estado.IndiceLinhasObservacao++;
                }
            }
            float alturaObs = Math.Max(80, yTextoObs - startObsY + 10);
            g.DrawRectangle(canetaFina, margemEsq, startObsY, larguraUtil, alturaObs);
            g.DrawLine(canetaFina, xResumo, startObsY, xResumo, startObsY + alturaObs);
            float yResumo = startObsY + 10;
            float xResumoLabel = xResumo + 5;
            float xResumoValor = margemDir - 5;
            g.DrawString("Subtotal de peças", fonteTexto, Brushes.Black, xResumoLabel, yResumo);
            var t1 = estado.Os.ValorTotalPecas.ToString("C2");
            g.DrawString(t1, fonteTexto, Brushes.Black, xResumoValor - g.MeasureString(t1, fonteTexto).Width, yResumo);
            yResumo += 15;
            g.DrawString("Subtotal de serviços", fonteTexto, Brushes.Black, xResumoLabel, yResumo);
            var t2 = estado.Os.ValorTotalServicos.ToString("C2");
            g.DrawString(t2, fonteTexto, Brushes.Black, xResumoValor - g.MeasureString(t2, fonteTexto).Width, yResumo);
            yResumo += 15;
            g.DrawString("Descontos (%)", fonteTexto, Brushes.Black, xResumoLabel, yResumo);
            var t3 = $"{estado.Os.DescontoPercentual:F1}%";
            g.DrawString(t3, fonteTexto, Brushes.Black, xResumoValor - g.MeasureString(t3, fonteTexto).Width, yResumo);
            yResumo += 15;
            g.DrawLine(canetaFina, xResumo + 5, yResumo, margemDir - 5, yResumo);
            yResumo += 10;
            g.DrawString("VALOR TOTAL", fonteValorTotal, Brushes.Black, xResumoLabel, yResumo + 2);
            var t4 = estado.Os.ValorTotal.ToString("C2");
            g.DrawString(t4, fonteValorTotal, Brushes.Black, xResumoValor - g.MeasureString(t4, fonteValorTotal).Width, yResumo);
            y = startObsY + alturaObs + 15;
            var textoRodape = "ESTE ORÇAMENTO NÃO É UM DOCUMENTO FISCAL E POSSUI VALIDADE DE 7 DIAS.";
            g.DrawString(textoRodape, fonteRodape, Brushes.Black,
                area.Left + (area.Width - g.MeasureString(textoRodape, fonteRodape).Width) / 2, y);
            return false;
        }
        finally
        {
            fonteTitulo.Dispose(); fonteLabel.Dispose(); fonteTexto.Dispose();
            fonteTextoBold.Dispose(); fonteNumeroOS.Dispose(); fonteValorTotal.Dispose();
            fonteRodape.Dispose(); canetaFina.Dispose(); canetaGrossa.Dispose();
        }
    }
    // CUPOM DE ORÇAMENTO
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
        const int LARGURA = 40;
        sb.AppendLine($"Orçamento Nº: {os.Numero}");
        sb.AppendLine($"Data: {os.DataAbertura.ToLocalTime():dd/MM/yyyy HH:mm}");
        sb.AppendLine(new string('=', LARGURA));
        if (empresa != null)
        {
            sb.AppendLine(empresa.NomeEmpresa);
            sb.AppendLine($"CNPJ: {empresa.Cnpj}");
            sb.AppendLine($"Tel: {empresa.Telefone}");
            if (!string.IsNullOrWhiteSpace(empresa.Endereco))
                sb.AppendLine(empresa.Endereco);
        }
        sb.AppendLine(new string('=', LARGURA));
        sb.AppendLine("             ORÇAMENTO");
        sb.AppendLine(new string('=', LARGURA));
        sb.AppendLine($"Cliente: {os.NomeCliente}");
        sb.AppendLine($"Veículo: {os.MarcaVeiculo ?? ""} {os.ModeloVeiculo ?? ""} - {os.PlacaVeiculo ?? ""}");
        if (os.Quilometragem.HasValue)
            sb.AppendLine($"Km: {os.Quilometragem.Value:N0}");
        sb.AppendLine(new string('-', LARGURA));
        sb.AppendLine(FormatarLinhaColunas("DESCRIÇÃO", "QTD", "VALOR", LARGURA));
        sb.AppendLine(new string('-', LARGURA));
        // Peças
        foreach (var peca in os.ItensPeca)
        {
            var linhasDescricao = QuebrarTextoEmLinhas(peca.NomePeca, 20);
            var qtd = peca.Quantidade.ToString("0.###");
            var valor = peca.ValorTotal.ToString("C2");
            var primeiraLinha = linhasDescricao.FirstOrDefault() ?? "";
            sb.AppendLine(FormatarLinhaColunas(primeiraLinha, qtd, valor, LARGURA));
            foreach (var linha in linhasDescricao.Skip(1))
                sb.AppendLine(linha);
        }
        // Serviços
        foreach (var servico in os.ItensServico)
        {
            var linhasDescricao = QuebrarTextoEmLinhas(servico.NomeServico, 20);
            var valor = servico.PrecoUnitario.ToString("C2");
            var primeiraLinha = linhasDescricao.FirstOrDefault() ?? "";
            sb.AppendLine(FormatarLinhaColunas(primeiraLinha, "1", valor, LARGURA));
            foreach (var linha in linhasDescricao.Skip(1))
                sb.AppendLine(linha);
        }
        sb.AppendLine(new string('-', LARGURA));
        sb.AppendLine(FormatarLinhaColunas("TOTAL", "", os.ValorTotal.ToString("C2"), LARGURA));
        sb.AppendLine(new string('=', LARGURA));
        sb.AppendLine("Este documento é um orçamento");
        sb.AppendLine("e não tem valor fiscal.");
        sb.AppendLine("Válido por 7 dias.");
        sb.AppendLine(new string('=', LARGURA));
        return sb.ToString();
    }
    private static string FormatarLinhaColunas(string descricao, string qtd, string valor, int larguraTotal)
    {
        const int LARG_DESC = 20;
        const int LARG_QTD = 6;
        const int LARG_VALOR = 12;
        if (descricao.Length > LARG_DESC)
            descricao = descricao.Substring(0, LARG_DESC - 3) + "...";
        return descricao.PadRight(LARG_DESC)
             + " "
             + qtd.PadLeft(LARG_QTD)
             + " "
             + valor.PadLeft(LARG_VALOR);
    }
    private static List<string> QuebrarTextoEmLinhas(string texto, int larguraMaxima)
    {
        var linhas = new List<string>();
        if (string.IsNullOrWhiteSpace(texto))
        {
            linhas.Add(string.Empty);
            return linhas;
        }
        var palavras = texto.Split(' ');
        var linhaAtual = "";
        foreach (var palavra in palavras)
        {
            if ((linhaAtual + " " + palavra).Trim().Length > larguraMaxima)
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
        if (linhas.Count == 0)
            linhas.Add(string.Empty);
        return linhas;
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
            var fonte = new Font("Consolas", 8);
            var brush = Brushes.Black;
            var margem = 2f;
            var yPos = margem;
            foreach (var linha in conteudo.Split('\n'))
            {
                e.Graphics.DrawString(linha, fonte, brush, margem, yPos);
                yPos += fonte.GetHeight(e.Graphics) + 2;
            }
        };
        printDocument.Print();
    }
    // ETIQUETAS
    public Bitmap GerarEtiquetaBitmap(PecaDTO peca, string nomeEmpresa)
    {
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
            var nome = peca.Descricao.Length > 40 ? peca.Descricao.Substring(0, 40) + "..." : peca.Descricao;
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
        var empresa = _context.ConfiguracoesEmpresa.AsNoTracking().FirstOrDefault();
        var nomeEmpresa = empresa?.NomeEmpresa ?? "AUTO ELÉTRICA";
        using var bitmapOriginal = GerarEtiquetaBitmap(peca, nomeEmpresa);
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
    private void DesenharLogoEmpresa(Graphics g, Domain.Entities.ConfiguracaoEmpresa? empresa,
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
            catch { }
        }
    }
    private static string TruncarTexto(string texto, int tamanhoMaximo)
    {
        if (string.IsNullOrEmpty(texto)) return "";
        return texto.Length > tamanhoMaximo ? texto.Substring(0, tamanhoMaximo - 3) + "..." : texto;
    }
    private static List<string> QuebrarTexto(string texto, int tamanhoMaximo)
    {
        var linhas = new List<string>();
        if (string.IsNullOrWhiteSpace(texto)) return linhas;
        var palavras = texto.Split(' ');
        var linhaAtual = "";
        foreach (var palavra in palavras)
        {
            if (palavra.Length > tamanhoMaximo)
            {
                if (!string.IsNullOrEmpty(linhaAtual))
                {
                    linhas.Add(linhaAtual.Trim());
                    linhaAtual = "";
                }
                for (int i = 0; i < palavra.Length; i += tamanhoMaximo)
                {
                    int tamanhoPedaco = Math.Min(tamanhoMaximo, palavra.Length - i);
                    linhas.Add(palavra.Substring(i, tamanhoPedaco));
                }
            }
            else if ((linhaAtual + " " + palavra).Trim().Length > tamanhoMaximo)
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
    private static List<string> QuebrarTextoPorLargura(Graphics g, string texto, Font fonte, float larguraMaxima)
    {
        var linhas = new List<string>();
        if (string.IsNullOrWhiteSpace(texto) || larguraMaxima <= 0)
            return linhas;

        var paragrafos = texto.Replace("\r\n", "\n").Replace("\r", "\n").Split('\n');
        foreach (var paragrafo in paragrafos)
        {
            if (string.IsNullOrWhiteSpace(paragrafo))
            {
                linhas.Add(string.Empty);
                continue;
            }

            var palavras = paragrafo.Split((char[]?)null, StringSplitOptions.RemoveEmptyEntries);
            var linhaAtual = string.Empty;
            foreach (var palavraOriginal in palavras)
            {
                var palavra = palavraOriginal;
                var tentativa = string.IsNullOrEmpty(linhaAtual) ? palavra : linhaAtual + " " + palavra;
                if (g.MeasureString(tentativa, fonte).Width <= larguraMaxima)
                {
                    linhaAtual = tentativa;
                    continue;
                }

                if (!string.IsNullOrEmpty(linhaAtual))
                {
                    linhas.Add(linhaAtual);
                    linhaAtual = string.Empty;
                }

                while (g.MeasureString(palavra, fonte).Width > larguraMaxima && palavra.Length > 1)
                {
                    int quantidade = palavra.Length;
                    while (quantidade > 1 && g.MeasureString(palavra.Substring(0, quantidade), fonte).Width > larguraMaxima)
                        quantidade--;
                    linhas.Add(palavra.Substring(0, quantidade));
                    palavra = palavra.Substring(quantidade);
                }
                linhaAtual = palavra;
            }

            if (!string.IsNullOrEmpty(linhaAtual))
                linhas.Add(linhaAtual);
        }
        return linhas;
    }


    private static void DesenharBordasGradeOS(
        Graphics g,
        Pen caneta,
        float margemEsq,
        float margemDir,
        float xDescricao,
        float xQtd,
        float xUnit,
        float xTotal,
        float yInicio,
        float yFim)
    {
        g.DrawRectangle(
            caneta,
            margemEsq,
            yInicio,
            margemDir - margemEsq,
            yFim - yInicio);

        g.DrawLine(caneta, xDescricao, yInicio, xDescricao, yFim);
        g.DrawLine(caneta, xQtd, yInicio, xQtd, yFim);
        g.DrawLine(caneta, xUnit, yInicio, xUnit, yFim);
        g.DrawLine(caneta, xTotal, yInicio, xTotal, yFim);
    }

}

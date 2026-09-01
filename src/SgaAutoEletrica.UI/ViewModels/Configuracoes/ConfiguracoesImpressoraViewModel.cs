using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing.Printing;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using MediatR;
using SgaAutoEletrica.Application.Features.Configuracoes.Commands;
using SgaAutoEletrica.Application.Features.Configuracoes.DTOs;
using SgaAutoEletrica.Application.Features.Configuracoes.Queries;
using SgaAutoEletrica.Domain.Enums;

namespace SgaAutoEletrica.UI.ViewModels.Configuracoes;

public class ConfiguracoesImpressoraViewModel : INotifyPropertyChanged
{
    private readonly IMediator _mediator;

    public ObservableCollection<string> ImpressorasInstaladas { get; } = new();
    public Array TiposImpressao => Enum.GetValues(typeof(TipoImpressao));
    public Array TamanhosPapel => Enum.GetValues(typeof(TamanhoPapel));

    // OS
    public string ImpressoraOS { get; set; } = string.Empty;
    public TamanhoPapel TamanhoPapelOS { get; set; } = TamanhoPapel.A4;
    public int CopiasOS { get; set; } = 1;

    // Nota Fiscal
    public string ImpressoraNF { get; set; } = string.Empty;
    public TamanhoPapel TamanhoPapelNF { get; set; } = TamanhoPapel.A4;
    public int CopiasNF { get; set; } = 1;

    // Cupom Fiscal
    public string ImpressoraCupom { get; set; } = string.Empty;
    public TamanhoPapel TamanhoPapelCupom { get; set; } = TamanhoPapel.Cupom80mm;
    public int CopiasCupom { get; set; } = 1;

    // Etiqueta
    public string ImpressoraEtiqueta { get; set; } = string.Empty;
    public TamanhoPapel TamanhoPapelEtiqueta { get; set; } = TamanhoPapel.EtiquetaPadrao;
    public int CopiasEtiqueta { get; set; } = 1;

    public ICommand SalvarCommand { get; }

    public ConfiguracoesImpressoraViewModel(IMediator mediator)
    {
        _mediator = mediator;
        SalvarCommand = new RelayCommand(async _ => await SalvarAsync());
    }

    public async Task CarregarAsync()
    {
        // Lista impressoras do Windows
        ImpressorasInstaladas.Clear();
        foreach (string impressora in PrinterSettings.InstalledPrinters)
        {
            ImpressorasInstaladas.Add(impressora);
        }

        // Carrega configurações existentes
        var configs = await _mediator.Send(new ListarConfiguracoesImpressoraQuery());

        var os = configs.FirstOrDefault(c => c.Tipo == TipoImpressao.OS);
        if (os != null)
        {
            ImpressoraOS = os.NomeImpressora;
            TamanhoPapelOS = os.TamanhoPapel;
            CopiasOS = os.Copias ?? 1;
        }

        var nf = configs.FirstOrDefault(c => c.Tipo == TipoImpressao.NotaFiscal);
        if (nf != null)
        {
            ImpressoraNF = nf.NomeImpressora;
            TamanhoPapelNF = nf.TamanhoPapel;
            CopiasNF = nf.Copias ?? 1;
        }

        var cupom = configs.FirstOrDefault(c => c.Tipo == TipoImpressao.CupomFiscal);
        if (cupom != null)
        {
            ImpressoraCupom = cupom.NomeImpressora;
            TamanhoPapelCupom = cupom.TamanhoPapel;
            CopiasCupom = cupom.Copias ?? 1;
        }

        var etiqueta = configs.FirstOrDefault(c => c.Tipo == TipoImpressao.Etiqueta);
        if (etiqueta != null)
        {
            ImpressoraEtiqueta = etiqueta.NomeImpressora;
            TamanhoPapelEtiqueta = etiqueta.TamanhoPapel;
            CopiasEtiqueta = etiqueta.Copias ?? 1;
        }

        OnPropertyChanged(nameof(ImpressoraOS));
        OnPropertyChanged(nameof(TamanhoPapelOS));
        OnPropertyChanged(nameof(CopiasOS));
        OnPropertyChanged(nameof(ImpressoraNF));
        OnPropertyChanged(nameof(TamanhoPapelNF));
        OnPropertyChanged(nameof(CopiasNF));
        OnPropertyChanged(nameof(ImpressoraCupom));
        OnPropertyChanged(nameof(TamanhoPapelCupom));
        OnPropertyChanged(nameof(CopiasCupom));
        OnPropertyChanged(nameof(ImpressoraEtiqueta));
        OnPropertyChanged(nameof(TamanhoPapelEtiqueta));
        OnPropertyChanged(nameof(CopiasEtiqueta));
    }

    public async Task SalvarAsync()
    {
        // Salva OS
        await _mediator.Send(new SalvarConfiguracaoImpressoraCommand
        {
            Tipo = TipoImpressao.OS,
            NomeImpressora = ImpressoraOS,
            TamanhoPapel = TamanhoPapelOS,
            Copias = CopiasOS
        });

        // Salva Nota Fiscal
        await _mediator.Send(new SalvarConfiguracaoImpressoraCommand
        {
            Tipo = TipoImpressao.NotaFiscal,
            NomeImpressora = ImpressoraNF,
            TamanhoPapel = TamanhoPapelNF,
            Copias = CopiasNF
        });

        // Salva Cupom Fiscal
        await _mediator.Send(new SalvarConfiguracaoImpressoraCommand
        {
            Tipo = TipoImpressao.CupomFiscal,
            NomeImpressora = ImpressoraCupom,
            TamanhoPapel = TamanhoPapelCupom,
            Copias = CopiasCupom
        });

        // Salva Etiqueta
        await _mediator.Send(new SalvarConfiguracaoImpressoraCommand
        {
            Tipo = TipoImpressao.Etiqueta,
            NomeImpressora = ImpressoraEtiqueta,
            TamanhoPapel = TamanhoPapelEtiqueta,
            Copias = CopiasEtiqueta
        });

        MessageBox.Show("Configurações de impressão salvas com sucesso!", "Sucesso", MessageBoxButton.OK, MessageBoxImage.Information);
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using SgaAutoEletrica.Application.Common.Interfaces;
using SgaAutoEletrica.Application.Features.Configuracoes.Queries;
using SgaAutoEletrica.Application.Features.Pecas.DTOs;

namespace SgaAutoEletrica.UI.Views.Pecas;

public partial class ImprimirEtiquetaWindow : Window
{
    private readonly PecaDTO _peca;
    private readonly IMediator _mediator;

    public ImprimirEtiquetaWindow(PecaDTO peca)
    {
        InitializeComponent();
        _peca = peca;
        _mediator = App.ServiceProvider.GetRequiredService<IMediator>();
        Loaded += ImprimirEtiquetaWindow_Loaded;
    }

    private async void ImprimirEtiquetaWindow_Loaded(object sender, RoutedEventArgs e)
    {
        var empresa = await _mediator.Send(new ObterConfiguracaoEmpresaQuery());
        var nomeEmpresa = empresa?.NomeEmpresa ?? "AUTO ELÉTRICA";

        var impressao = App.ServiceProvider.GetRequiredService<IImpressaoService>();
        using var bitmap = impressao.GerarEtiquetaBitmap(_peca, nomeEmpresa);

        using var memory = new MemoryStream();
        bitmap.Save(memory, System.Drawing.Imaging.ImageFormat.Png);
        memory.Position = 0;

        var bitmapImage = new BitmapImage();
        bitmapImage.BeginInit();
        bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
        bitmapImage.StreamSource = memory;
        bitmapImage.EndInit();
        bitmapImage.Freeze();

        ImgEtiqueta.Source = bitmapImage;
    }

    private void BtnImprimir_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            var impressao = App.ServiceProvider.GetRequiredService<IImpressaoService>();
            impressao.ImprimirEtiqueta(_peca);
            MessageBox.Show("Etiqueta enviada para impressão.", "Sucesso",
                MessageBoxButton.OK, MessageBoxImage.Information);
            Close();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Erro ao imprimir: {ex.Message}", "Erro",
                MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void BtnFechar_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }
}
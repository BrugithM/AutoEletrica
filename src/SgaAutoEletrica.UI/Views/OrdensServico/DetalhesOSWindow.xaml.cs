using System.Windows;
using MediatR;
using SgaAutoEletrica.Application.Features.OrdensServico.Queries;

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

        var itens = new List<dynamic>();
        foreach (var peca in os.ItensPeca)
            itens.Add(new { Tipo = "Peça", Descricao = peca.NomePeca, Quantidade = peca.Quantidade.ToString(), Valor = peca.ValorTotal });
        foreach (var servico in os.ItensServico)
            itens.Add(new { Tipo = "Serviço", Descricao = servico.NomeServico, Quantidade = "1", Valor = servico.PrecoUnitario });

        GridItens.ItemsSource = itens;
    }

    private void BtnFechar_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }
}
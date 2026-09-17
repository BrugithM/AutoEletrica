using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using SgaAutoEletrica.Application.Common.Interfaces;
using SgaAutoEletrica.Application.Features.CategoriasPeca.DTOs;
using SgaAutoEletrica.Application.Features.CategoriasPeca.Queries;
using SgaAutoEletrica.Application.Features.Pecas.Commands;
using SgaAutoEletrica.Application.Features.Pecas.DTOs;
using SgaAutoEletrica.Application.Features.Pecas.Queries;

namespace SgaAutoEletrica.UI.ViewModels.Pecas;

public class ListaPecasViewModel : INotifyPropertyChanged
{
    private readonly IMediator _mediator;

    public ObservableCollection<PecaDTO> Pecas { get; } = new();
    public ObservableCollection<CategoriaPecaDTO> Categorias { get; } = new();

    public bool EhAdministrador => App.ServiceProvider
        .GetRequiredService<ISessaoUsuario>().EhAdministrador;

    private PecaDTO? _pecaSelecionada;
    public PecaDTO? PecaSelecionada
    {
        get => _pecaSelecionada;
        set
        {
            _pecaSelecionada = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(TemPecaSelecionada));
            OnPropertyChanged(nameof(TemFornecedor));
            OnPropertyChanged(nameof(FornecedorNomeExibir));
            OnPropertyChanged(nameof(FornecedorCnpjExibir));
            OnPropertyChanged(nameof(FornecedorTelefoneExibir));
            OnPropertyChanged(nameof(FornecedorContatoExibir));
        }
    }

    public bool TemPecaSelecionada => PecaSelecionada != null;
    public bool TemFornecedor => !string.IsNullOrWhiteSpace(PecaSelecionada?.FornecedorNome);

    public string FornecedorNomeExibir => PecaSelecionada?.FornecedorNome ?? "";
    public string FornecedorCnpjExibir => PecaSelecionada?.FornecedorCnpj ?? "";
    public string FornecedorTelefoneExibir => PecaSelecionada?.FornecedorTelefone ?? "";
    public string FornecedorContatoExibir => PecaSelecionada?.FornecedorContato ?? "";

    // ─── Filtros ───

    private string _termoBusca = string.Empty;
    public string TermoBusca
    {
        get => _termoBusca;
        set { _termoBusca = value; OnPropertyChanged(); }
    }

    private CategoriaPecaDTO? _categoriaFiltro;
    public CategoriaPecaDTO? CategoriaFiltro
    {
        get => _categoriaFiltro;
        set
        {
            _categoriaFiltro = value;
            OnPropertyChanged();
            _ = BuscarAsync();
        }
    }

    private bool _apenasEstoqueBaixo;
    public bool ApenasEstoqueBaixo
    {
        get => _apenasEstoqueBaixo;
        set
        {
            _apenasEstoqueBaixo = value;
            OnPropertyChanged();
            _ = BuscarAsync();
        }
    }

    // ─── Totais ───

    public int TotalItens => Pecas.Count;
    public int TotalCriticos => Pecas.Count(p => p.EstoqueBaixo);

    // ─── Comandos ───

    public ICommand BuscarCommand { get; }
    public ICommand LimparCommand { get; }
    public ICommand NovaPecaCommand { get; }
    public ICommand EditarPecaCommand { get; }
    public ICommand ExcluirPecaCommand { get; }
    public ICommand MovimentarEstoqueCommand { get; }
    public ICommand AtualizarCommand { get; }
    public ICommand AbrirFornecedorCommand { get; }

    public ListaPecasViewModel(IMediator mediator)
    {
        _mediator = mediator;

        BuscarCommand = new RelayCommand(async _ => await BuscarAsync());
        LimparCommand = new RelayCommand(async _ =>
        {
            TermoBusca = string.Empty;
            ApenasEstoqueBaixo = false;
            CategoriaFiltro = Categorias.FirstOrDefault();
            await BuscarAsync();
        });
        NovaPecaCommand = new RelayCommand(async _ => await NovaPecaAsync());
        EditarPecaCommand = new RelayCommand(async _ => await EditarPecaAsync(), _ => TemPecaSelecionada);
        ExcluirPecaCommand = new RelayCommand(async _ => await ExcluirPecaAsync(), _ => TemPecaSelecionada && EhAdministrador);
        MovimentarEstoqueCommand = new RelayCommand(async _ => await MovimentarEstoqueAsync(), _ => TemPecaSelecionada);
        AtualizarCommand = new RelayCommand(async _ => await BuscarAsync());
        AbrirFornecedorCommand = new RelayCommand(_ => AbrirFornecedor(), _ => TemFornecedor);
    }

    public async Task CarregarCategoriasAsync()
    {
        Categorias.Clear();

        // Adiciona "Todos" como primeira opção
        Categorias.Add(new CategoriaPecaDTO { Id = Guid.Empty, Nome = "Todos" });

        var cats = await _mediator.Send(new ListarCategoriasPecaQuery());
        foreach (var cat in cats)
            Categorias.Add(cat);

        // Seleciona "Todos" por padrão
        CategoriaFiltro = Categorias.FirstOrDefault();
    }

    public async Task BuscarAsync()
    {
        try
        {
            Pecas.Clear();

            var resultado = await _mediator.Send(new ListarPecasQuery
            {
                TermoBusca = TermoBusca,
                CategoriaId = (CategoriaFiltro == null || CategoriaFiltro.Id == Guid.Empty)
                    ? null
                    : CategoriaFiltro.Id,
                Ativo = true
            });

            var filtradas = ApenasEstoqueBaixo
                ? resultado.Where(p => p.EstoqueBaixo).ToList()
                : resultado;

            foreach (var peca in filtradas)
                Pecas.Add(peca);

            OnPropertyChanged(nameof(TotalItens));
            OnPropertyChanged(nameof(TotalCriticos));
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Erro: {ex.Message}", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private async Task NovaPecaAsync()
    {
        var dialog = new Views.Pecas.CadastroPecaWindow(_mediator);
        dialog.ShowDialog();
        await BuscarAsync();
    }

    private async Task EditarPecaAsync()
    {
        if (PecaSelecionada == null) return;
        var dialog = new Views.Pecas.CadastroPecaWindow(_mediator, PecaSelecionada.Id);
        dialog.ShowDialog();
        await BuscarAsync();
    }

    private async Task ExcluirPecaAsync()
    {
        if (PecaSelecionada == null) return;

        var confirmacao = MessageBox.Show(
            $"Deseja realmente excluir a peça '{PecaSelecionada.Nome}'?",
            "Confirmar exclusão",
            MessageBoxButton.YesNo,
            MessageBoxImage.Warning);

        if (confirmacao == MessageBoxResult.Yes)
        {
            await _mediator.Send(new ExcluirPecaCommand { Id = PecaSelecionada.Id });
            await BuscarAsync();
        }
    }

    private async Task MovimentarEstoqueAsync()
    {
        if (PecaSelecionada == null) return;
        var dialog = new Views.Pecas.MovimentacaoEstoqueWindow(_mediator, PecaSelecionada);
        dialog.ShowDialog();
        await BuscarAsync();
    }

    private void AbrirFornecedor()
    {
        if (PecaSelecionada?.FornecedorId == null) return;
        MessageBox.Show($"Abrir fornecedor {PecaSelecionada.FornecedorNome} — em breve", "Em breve");
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using MediatR;
using SgaAutoEletrica.Application.Features.Fornecedores.Commands;
using SgaAutoEletrica.Application.Features.Fornecedores.DTOs;
using SgaAutoEletrica.Application.Features.Fornecedores.Queries;
using SgaAutoEletrica.Application.Features.Pecas.Queries;
using SgaAutoEletrica.Application.Features.Pecas.DTOs;

namespace SgaAutoEletrica.UI.ViewModels.Fornecedores;

public class CriarNotaFiscalEntradaViewModel : INotifyPropertyChanged
{
    private readonly IMediator _mediator;

    public ObservableCollection<FornecedorDTO> Fornecedores { get; } = new();
    public ObservableCollection<PecaDTO> PecasDisponiveis { get; } = new();
    public ObservableCollection<ItemNFEntradaTemporario> ItensNF { get; } = new();

    public string Numero { get; set; } = string.Empty;
    public string Observacao { get; set; } = string.Empty;

    private FornecedorDTO? _fornecedorSelecionado;
    public FornecedorDTO? FornecedorSelecionado
    {
        get => _fornecedorSelecionado;
        set { _fornecedorSelecionado = value; OnPropertyChanged(); }
    }

    private PecaDTO? _pecaSelecionada;
    public PecaDTO? PecaSelecionada
    {
        get => _pecaSelecionada;
        set { _pecaSelecionada = value; OnPropertyChanged(); }
    }

    public int Quantidade { get; set; } = 1;
    public decimal ValorUnitario { get; set; }

    public decimal ValorTotal => ItensNF.Sum(i => i.ValorTotal);

    public CriarNotaFiscalEntradaViewModel(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task CarregarDadosAsync()
    {
        var fornecedores = await _mediator.Send(new ListarFornecedoresQuery());
        foreach (var f in fornecedores)
            Fornecedores.Add(f);

        var pecas = await _mediator.Send(new ListarPecasQuery { Ativo = true });
        foreach (var p in pecas)
            PecasDisponiveis.Add(p);
    }

    public void AdicionarItem()
    {
        if (PecaSelecionada == null || Quantidade <= 0 || ValorUnitario <= 0) return;

        ItensNF.Add(new ItemNFEntradaTemporario
        {
            PecaId = PecaSelecionada.Id,
            NomePeca = PecaSelecionada.Nome,
            Quantidade = Quantidade,
            ValorUnitario = ValorUnitario
        });

        OnPropertyChanged(nameof(ValorTotal));
    }

    public async Task<bool> SalvarAsync()
    {
        if (string.IsNullOrWhiteSpace(Numero))
        {
            MessageBox.Show("Número da NF é obrigatório.", "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning);
            return false;
        }
        if (FornecedorSelecionado == null)
        {
            MessageBox.Show("Selecione um fornecedor.", "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning);
            return false;
        }
        if (!ItensNF.Any())
        {
            MessageBox.Show("Adicione pelo menos um item.", "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning);
            return false;
        }

        var command = new CriarNotaFiscalEntradaCommand
        {
            Numero = Numero,
            FornecedorId = FornecedorSelecionado.Id,
            Observacao = Observacao
        };

        foreach (var item in ItensNF)
        {
            command.Itens.Add(new ItemNotaEntradaRequest
            {
                PecaId = item.PecaId,
                Quantidade = item.Quantidade,
                ValorUnitario = item.ValorUnitario
            });
        }

        await _mediator.Send(command);
        return true;
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}

public class ItemNFEntradaTemporario
{
    public Guid PecaId { get; set; }
    public string NomePeca { get; set; } = string.Empty;
    public int Quantidade { get; set; }
    public decimal ValorUnitario { get; set; }
    public decimal ValorTotal => Quantidade * ValorUnitario;
}
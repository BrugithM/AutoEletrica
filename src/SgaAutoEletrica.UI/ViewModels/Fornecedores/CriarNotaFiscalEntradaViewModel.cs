using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
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

    private string _numero = string.Empty;
    public string Numero
    {
        get => _numero;
        set { _numero = value; OnPropertyChanged(); }
    }

    private DateTime _dataEntrada = DateTime.Now;
    public DateTime DataEntrada
    {
        get => _dataEntrada;
        set { _dataEntrada = value; OnPropertyChanged(); }
    }

    private string _observacao = string.Empty;
    public string Observacao
    {
        get => _observacao;
        set { _observacao = value; OnPropertyChanged(); }
    }

    private FornecedorDTO? _fornecedorSelecionado;
    public FornecedorDTO? FornecedorSelecionado
    {
        get => _fornecedorSelecionado;
        set
        {
            _fornecedorSelecionado = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(CnpjFornecedor));
        }
    }

    public string CnpjFornecedor => FornecedorSelecionado?.Cnpj ?? "";

    private PecaDTO? _pecaSelecionada;
    public PecaDTO? PecaSelecionada
    {
        get => _pecaSelecionada;
        set { _pecaSelecionada = value; OnPropertyChanged(); }
    }

    private int _quantidade = 1;
    public int Quantidade
    {
        get => _quantidade;
        set { _quantidade = value; OnPropertyChanged(); }
    }

    private decimal _valorUnitario;
    public decimal ValorUnitario
    {
        get => _valorUnitario;
        set { _valorUnitario = value; OnPropertyChanged(); }
    }

    private ItemNFEntradaTemporario? _itemSelecionado;
    public ItemNFEntradaTemporario? ItemSelecionado
    {
        get => _itemSelecionado;
        set
        {
            _itemSelecionado = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(TemItemSelecionado));
        }
    }

    public bool TemItemSelecionado => ItemSelecionado != null;

    public decimal ValorTotal => ItensNF.Sum(i => i.ValorTotal);

    public ICommand AdicionarItemCommand { get; }
    public ICommand RemoverItemCommand { get; }

    public CriarNotaFiscalEntradaViewModel(IMediator mediator)
    {
        _mediator = mediator;

        AdicionarItemCommand = new RelayCommand(_ => AdicionarItem());
        RemoverItemCommand = new RelayCommand(_ => RemoverItem(), _ => TemItemSelecionado);
    }

    public async Task CarregarDadosAsync()
    {
        var fornecedores = await _mediator.Send(new ListarFornecedoresQuery { Ativo = true });
        foreach (var f in fornecedores)
            Fornecedores.Add(f);

        var pecas = await _mediator.Send(new ListarPecasQuery { Ativo = true });
        foreach (var p in pecas)
            PecasDisponiveis.Add(p);
    }

    public void AdicionarItem()
    {
        if (PecaSelecionada == null || Quantidade <= 0 || ValorUnitario <= 0)
        {
            MessageBox.Show("Selecione uma peça, informe a quantidade e o valor unitário.", 
                "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        var existente = ItensNF.FirstOrDefault(i => i.PecaId == PecaSelecionada.Id);
        if (existente != null)
        {
            existente.Quantidade += Quantidade;
        }
        else
        {
            var novo = new ItemNFEntradaTemporario
            {
                PecaId = PecaSelecionada.Id,
                NomePeca = PecaSelecionada.Nome,
                Quantidade = Quantidade,
                ValorUnitario = ValorUnitario
            };

            novo.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(ItemNFEntradaTemporario.ValorTotal))
                    OnPropertyChanged(nameof(ValorTotal));
            };

            ItensNF.Add(novo);
        }

        PecaSelecionada = null;
        Quantidade = 1;
        ValorUnitario = 0;

        OnPropertyChanged(nameof(ValorTotal));
    }

    public void RemoverItem()
    {
        if (ItemSelecionado == null) return;
        ItensNF.Remove(ItemSelecionado);
        ItemSelecionado = null;
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

        try
        {
            // Verifica duplicidade
            var jaExiste = await _mediator.Send(new VerificarNFExistenteQuery
            {
                Numero = Numero,
                FornecedorId = FornecedorSelecionado.Id
            });

            if (jaExiste)
            {
                var confirmacao = MessageBox.Show(
                    $"Já existe uma NF '{Numero}' para este fornecedor. Deseja continuar mesmo assim?",
                    "NF Duplicada",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning);

                if (confirmacao == MessageBoxResult.No)
                    return false;
            }

            var command = new CriarNotaFiscalEntradaCommand
            {
                Numero = Numero,
                FornecedorId = FornecedorSelecionado.Id,
                DataEntrada = DataEntrada,
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

            MessageBox.Show("Nota Fiscal registrada com sucesso!", "Sucesso", MessageBoxButton.OK, MessageBoxImage.Information);
            return true;
        }
        catch (Exception ex)
        {
            var inner = ex;
            while (inner.InnerException != null)
                inner = inner.InnerException;
            MessageBox.Show($"Erro: {inner.Message}", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            return false;
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}

public class ItemNFEntradaTemporario : INotifyPropertyChanged
{
    public Guid PecaId { get; set; }
    public string NomePeca { get; set; } = string.Empty;

    private int _quantidade;
    public int Quantidade
    {
        get => _quantidade;
        set
        {
            _quantidade = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(ValorTotal));
        }
    }

    private decimal _valorUnitario;
    public decimal ValorUnitario
    {
        get => _valorUnitario;
        set
        {
            _valorUnitario = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(ValorTotal));
        }
    }

    public decimal ValorTotal => Quantidade * ValorUnitario;

    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}
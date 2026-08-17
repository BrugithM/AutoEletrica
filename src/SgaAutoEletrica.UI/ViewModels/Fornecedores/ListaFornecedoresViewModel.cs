using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using MediatR;
using SgaAutoEletrica.Application.Features.Fornecedores.Commands;
using SgaAutoEletrica.Application.Features.Fornecedores.DTOs;
using SgaAutoEletrica.Application.Features.Fornecedores.Queries;

namespace SgaAutoEletrica.UI.ViewModels.Fornecedores;

public class ListaFornecedoresViewModel : INotifyPropertyChanged
{
    private readonly IMediator _mediator;

    public ObservableCollection<FornecedorDTO> Fornecedores { get; } = new();

    private FornecedorDTO? _fornecedorSelecionado;
    public FornecedorDTO? FornecedorSelecionado
    {
        get => _fornecedorSelecionado;
        set { _fornecedorSelecionado = value; OnPropertyChanged(); }
    }

    private string _termoBusca = string.Empty;
    public string TermoBusca
    {
        get => _termoBusca;
        set { _termoBusca = value; OnPropertyChanged(); }
    }

    public ICommand BuscarCommand { get; }
    public ICommand LimparCommand { get; }
    public ICommand NovoFornecedorCommand { get; }
    public ICommand EditarFornecedorCommand { get; }
    public ICommand ExcluirFornecedorCommand { get; }

    public ListaFornecedoresViewModel(IMediator mediator)
    {
        _mediator = mediator;

        BuscarCommand = new RelayCommand(async _ => await BuscarAsync());
        LimparCommand = new RelayCommand(async _ => { TermoBusca = string.Empty; await BuscarAsync(); });
        NovoFornecedorCommand = new RelayCommand(async _ => await NovoFornecedorAsync());
        EditarFornecedorCommand = new RelayCommand(async _ => await EditarFornecedorAsync(), _ => FornecedorSelecionado != null);
        ExcluirFornecedorCommand = new RelayCommand(async _ => await ExcluirFornecedorAsync(), _ => FornecedorSelecionado != null);
    }

    public async Task BuscarAsync()
    {
        Fornecedores.Clear();
        var resultado = await _mediator.Send(new ListarFornecedoresQuery { TermoBusca = TermoBusca });
        foreach (var fornecedor in resultado)
            Fornecedores.Add(fornecedor);
    }

    private async Task NovoFornecedorAsync()
    {
        var dialog = new Views.Fornecedores.CadastroFornecedorWindow(_mediator);
        dialog.ShowDialog();
        await BuscarAsync();
    }

    private async Task EditarFornecedorAsync()
    {
        if (FornecedorSelecionado == null) return;
        var dialog = new Views.Fornecedores.CadastroFornecedorWindow(_mediator, FornecedorSelecionado.Id);
        dialog.ShowDialog();
        await BuscarAsync();
    }

    private async Task ExcluirFornecedorAsync()
    {
        if (FornecedorSelecionado == null) return;

        var confirmacao = MessageBox.Show(
            $"Deseja realmente excluir o fornecedor '{FornecedorSelecionado.NomeEmpresa}'?",
            "Confirmar exclusão",
            MessageBoxButton.YesNo,
            MessageBoxImage.Warning);

        if (confirmacao == MessageBoxResult.Yes)
        {
            await _mediator.Send(new ExcluirFornecedorCommand { Id = FornecedorSelecionado.Id });
            await BuscarAsync();
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}
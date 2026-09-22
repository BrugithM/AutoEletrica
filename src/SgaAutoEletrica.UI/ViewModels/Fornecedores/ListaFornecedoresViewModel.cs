using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using SgaAutoEletrica.Application.Common.Interfaces;
using SgaAutoEletrica.Application.Features.Fornecedores.Commands;
using SgaAutoEletrica.Application.Features.Fornecedores.DTOs;
using SgaAutoEletrica.Application.Features.Fornecedores.Queries;

namespace SgaAutoEletrica.UI.ViewModels.Fornecedores;

public class ListaFornecedoresViewModel : INotifyPropertyChanged
{
    private readonly IMediator _mediator;

    public ObservableCollection<FornecedorDTO> Fornecedores { get; } = new();

    public bool EhAdministrador => App.ServiceProvider
        .GetRequiredService<ISessaoUsuario>().EhAdministrador;

    private FornecedorDTO? _fornecedorSelecionado;
    public FornecedorDTO? FornecedorSelecionado
    {
        get => _fornecedorSelecionado;
        set
        {
            _fornecedorSelecionado = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(TemFornecedorSelecionado));
            OnPropertyChanged(nameof(TextoBotaoDesativar));
        }
    }

    public bool TemFornecedorSelecionado => FornecedorSelecionado != null;

    public string TextoBotaoDesativar =>
        FornecedorSelecionado?.Ativo == false ? "▶️ Reativar" : "⏸️ Desativar";

    private string _termoBusca = string.Empty;
    public string TermoBusca
    {
        get => _termoBusca;
        set { _termoBusca = value; OnPropertyChanged(); }
    }

    private bool _mostrarInativos;
    public bool MostrarInativos
    {
        get => _mostrarInativos;
        set
        {
            _mostrarInativos = value;
            OnPropertyChanged();
            _ = BuscarAsync();
        }
    }

    public ICommand BuscarCommand { get; }
    public ICommand LimparCommand { get; }
    public ICommand NovoFornecedorCommand { get; }
    public ICommand EditarFornecedorCommand { get; }
    public ICommand DesativarFornecedorCommand { get; }
    public ICommand AtualizarCommand { get; }

    public ListaFornecedoresViewModel(IMediator mediator)
    {
        _mediator = mediator;

        BuscarCommand = new RelayCommand(async _ => await BuscarAsync());
        LimparCommand = new RelayCommand(async _ => { TermoBusca = ""; await BuscarAsync(); });
        NovoFornecedorCommand = new RelayCommand(async _ => await NovoFornecedorAsync());
        EditarFornecedorCommand = new RelayCommand(async _ => await EditarFornecedorAsync(), _ => TemFornecedorSelecionado);
        DesativarFornecedorCommand = new RelayCommand(async _ => await DesativarFornecedorAsync(), _ => TemFornecedorSelecionado && EhAdministrador);
        AtualizarCommand = new RelayCommand(async _ => await BuscarAsync());
    }

    public async Task BuscarAsync()
    {
        try
        {
            Fornecedores.Clear();
            var resultado = await _mediator.Send(new ListarFornecedoresQuery
            {
                TermoBusca = TermoBusca,
                Ativo = MostrarInativos ? null : true
            });
            foreach (var fornecedor in resultado)
                Fornecedores.Add(fornecedor);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Erro: {ex.Message}", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
        }
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

    private async Task DesativarFornecedorAsync()
    {
        if (FornecedorSelecionado == null) return;

        var acao = FornecedorSelecionado.Ativo ? "desativar" : "reativar";
        var confirmacao = MessageBox.Show(
            $"Deseja realmente {acao} o fornecedor '{FornecedorSelecionado.NomeEmpresa}'?",
            "Confirmar",
            MessageBoxButton.YesNo,
            MessageBoxImage.Question);

        if (confirmacao == MessageBoxResult.Yes)
        {
            if (FornecedorSelecionado.Ativo)
                await _mediator.Send(new DesativarFornecedorCommand { Id = FornecedorSelecionado.Id });
            else
                await _mediator.Send(new ReativarFornecedorCommand { Id = FornecedorSelecionado.Id });

            await BuscarAsync();
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}
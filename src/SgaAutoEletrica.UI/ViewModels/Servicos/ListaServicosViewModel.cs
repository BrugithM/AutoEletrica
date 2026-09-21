using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using SgaAutoEletrica.Application.Common.Interfaces;
using SgaAutoEletrica.Application.Features.Servicos.Commands;
using SgaAutoEletrica.Application.Features.Servicos.DTOs;
using SgaAutoEletrica.Application.Features.Servicos.Queries;

namespace SgaAutoEletrica.UI.ViewModels.Servicos;

public class ListaServicosViewModel : INotifyPropertyChanged
{
    private readonly IMediator _mediator;

    public ObservableCollection<ServicoDTO> Servicos { get; } = new();

    public bool EhAdministrador => App.ServiceProvider
        .GetRequiredService<ISessaoUsuario>().EhAdministrador;

    private ServicoDTO? _servicoSelecionado;
    public ServicoDTO? ServicoSelecionado
    {
        get => _servicoSelecionado;
        set
        {
            _servicoSelecionado = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(TemServicoSelecionado));
            OnPropertyChanged(nameof(TextoBotaoDesativar));
        }
    }

    public bool TemServicoSelecionado => ServicoSelecionado != null;

    public string TextoBotaoDesativar =>
        ServicoSelecionado?.Ativo == false ? "▶️ Reativar" : "⏸️ Desativar";

    private string _termoBusca = string.Empty;
    public string TermoBusca
    {
        get => _termoBusca;
        set { _termoBusca = value; OnPropertyChanged(); }
    }

    public ICommand BuscarCommand { get; }
    public ICommand LimparCommand { get; }
    public ICommand NovoServicoCommand { get; }
    public ICommand EditarServicoCommand { get; }
    public ICommand DesativarServicoCommand { get; }
    public ICommand AtualizarCommand { get; }

    public ListaServicosViewModel(IMediator mediator)
    {
        _mediator = mediator;

        BuscarCommand = new RelayCommand(async _ => await BuscarAsync());
        LimparCommand = new RelayCommand(async _ => { TermoBusca = ""; await BuscarAsync(); });
        NovoServicoCommand = new RelayCommand(async _ => await NovoServicoAsync());
        EditarServicoCommand = new RelayCommand(async _ => await EditarServicoAsync(), _ => TemServicoSelecionado);
        DesativarServicoCommand = new RelayCommand(async _ => await DesativarServicoAsync(), _ => TemServicoSelecionado && EhAdministrador);
        AtualizarCommand = new RelayCommand(async _ => await BuscarAsync());
    }

    public async Task BuscarAsync()
    {
        try
        {
            Servicos.Clear();
            var resultado = await _mediator.Send(new ListarServicosQuery { TermoBusca = TermoBusca });
            foreach (var servico in resultado)
                Servicos.Add(servico);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Erro: {ex.Message}", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private async Task NovoServicoAsync()
    {
        var dialog = new Views.Servicos.CadastroServicoWindow(_mediator);
        dialog.ShowDialog();
        await BuscarAsync();
    }

    private async Task EditarServicoAsync()
    {
        if (ServicoSelecionado == null) return;
        var dialog = new Views.Servicos.CadastroServicoWindow(_mediator, ServicoSelecionado.Id);
        dialog.ShowDialog();
        await BuscarAsync();
    }

    private async Task DesativarServicoAsync()
    {
        if (ServicoSelecionado == null) return;

        var acao = ServicoSelecionado.Ativo ? "desativar" : "reativar";
        var confirmacao = MessageBox.Show(
            $"Deseja realmente {acao} o serviço '{ServicoSelecionado.Nome}'?",
            "Confirmar",
            MessageBoxButton.YesNo,
            MessageBoxImage.Question);

        if (confirmacao == MessageBoxResult.Yes)
        {
            if (ServicoSelecionado.Ativo)
                await _mediator.Send(new DesativarServicoCommand { Id = ServicoSelecionado.Id });
            else
                await _mediator.Send(new ReativarServicoCommand { Id = ServicoSelecionado.Id });

            await BuscarAsync();
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}
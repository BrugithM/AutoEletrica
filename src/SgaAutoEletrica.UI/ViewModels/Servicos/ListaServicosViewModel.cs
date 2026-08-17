using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using MediatR;
using SgaAutoEletrica.Application.Features.Servicos.Commands;
using SgaAutoEletrica.Application.Features.Servicos.DTOs;
using SgaAutoEletrica.Application.Features.Servicos.Queries;

namespace SgaAutoEletrica.UI.ViewModels.Servicos;

public class ListaServicosViewModel : INotifyPropertyChanged
{
    private readonly IMediator _mediator;

    public ObservableCollection<ServicoDTO> Servicos { get; } = new();

    private ServicoDTO? _servicoSelecionado;
    public ServicoDTO? ServicoSelecionado
    {
        get => _servicoSelecionado;
        set { _servicoSelecionado = value; OnPropertyChanged(); }
    }

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
    public ICommand ExcluirServicoCommand { get; }

    public ListaServicosViewModel(IMediator mediator)
    {
        _mediator = mediator;

        BuscarCommand = new RelayCommand(async _ => await BuscarAsync());
        LimparCommand = new RelayCommand(async _ => { TermoBusca = string.Empty; await BuscarAsync(); });
        NovoServicoCommand = new RelayCommand(async _ => await NovoServicoAsync());
        EditarServicoCommand = new RelayCommand(async _ => await EditarServicoAsync(), _ => ServicoSelecionado != null);
        ExcluirServicoCommand = new RelayCommand(async _ => await ExcluirServicoAsync(), _ => ServicoSelecionado != null);
    }

    public async Task BuscarAsync()
    {
        Servicos.Clear();
        var resultado = await _mediator.Send(new ListarServicosQuery { TermoBusca = TermoBusca });
        foreach (var servico in resultado)
            Servicos.Add(servico);
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

    private async Task ExcluirServicoAsync()
    {
        if (ServicoSelecionado == null) return;

        var confirmacao = MessageBox.Show(
            $"Deseja realmente excluir o serviço '{ServicoSelecionado.Nome}'?",
            "Confirmar exclusão",
            MessageBoxButton.YesNo,
            MessageBoxImage.Warning);

        if (confirmacao == MessageBoxResult.Yes)
        {
            await _mediator.Send(new ExcluirServicoCommand { Id = ServicoSelecionado.Id });
            await BuscarAsync();
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}
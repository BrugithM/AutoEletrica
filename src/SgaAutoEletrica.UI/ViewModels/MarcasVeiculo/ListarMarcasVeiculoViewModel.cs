using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using SgaAutoEletrica.Application.Features.MarcasVeiculo.Commands;
using SgaAutoEletrica.Application.Features.MarcasVeiculo.DTOs;
using SgaAutoEletrica.Application.Features.MarcasVeiculo.Queries;
using SgaAutoEletrica.UI.Views.MarcasVeiculo;

namespace SgaAutoEletrica.UI.ViewModels.MarcasVeiculo;

public class ListaMarcasVeiculoViewModel : INotifyPropertyChanged
{
    private readonly IMediator _mediator;

    public ObservableCollection<MarcaVeiculoDTO> Marcas { get; } = new();

    private MarcaVeiculoDTO? _marcaSelecionada;
    public MarcaVeiculoDTO? MarcaSelecionada
    {
        get => _marcaSelecionada;
        set
        {
            _marcaSelecionada = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(TemMarcaSelecionada));
            OnPropertyChanged(nameof(TextoBotaoDesativar));
        }
    }

    public bool TemMarcaSelecionada => MarcaSelecionada != null;
    public string TextoBotaoDesativar => MarcaSelecionada?.Ativo == false ? "▶️ Reativar" : "⏸️ Desativar";

    public ICommand NovaMarcaCommand { get; }
    public ICommand EditarMarcaCommand { get; }
    public ICommand DesativarMarcaCommand { get; }
    public ICommand AtualizarCommand { get; }

    public ListaMarcasVeiculoViewModel(IMediator mediator)
    {
        _mediator = mediator;
        NovaMarcaCommand = new RelayCommand(async _ => await AbrirCadastroAsync());
        EditarMarcaCommand = new RelayCommand(async _ => await AbrirCadastroAsync(MarcaSelecionada!.Id), _ => TemMarcaSelecionada);
        DesativarMarcaCommand = new RelayCommand(async _ => await AlternarAtivoAsync(), _ => TemMarcaSelecionada);
        AtualizarCommand = new RelayCommand(async _ => await CarregarAsync());
    }

    public async Task CarregarAsync()
    {
        Marcas.Clear();
        var lista = await _mediator.Send(new ListarMarcasVeiculoQuery { Ativo = null }); // Traz todas
        foreach (var m in lista)
            Marcas.Add(m);
    }

    private async Task AbrirCadastroAsync(Guid? marcaId = null)
    {
        var dialog = new CadastroMarcaVeiculoWindow(App.ServiceProvider.GetRequiredService<IMediator>(), marcaId);
        if (dialog.ShowDialog() == true)
        {
            await CarregarAsync();
        }
    }

    private async Task AlternarAtivoAsync()
    {
        if (MarcaSelecionada == null) return;
        await _mediator.Send(new AlternarAtivoMarcaVeiculoCommand
        {
            Id = MarcaSelecionada.Id,
            Ativo = !MarcaSelecionada.Ativo
        });
        await CarregarAsync();
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}
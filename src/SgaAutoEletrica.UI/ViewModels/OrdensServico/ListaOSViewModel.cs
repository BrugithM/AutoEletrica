using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using MediatR;
using SgaAutoEletrica.Application.Features.OrdensServico.DTOs;
using SgaAutoEletrica.Application.Features.OrdensServico.Queries;
using SgaAutoEletrica.Domain.Enums;

namespace SgaAutoEletrica.UI.ViewModels.OrdensServico;

public class ListaOSViewModel : INotifyPropertyChanged
{
    private readonly IMediator _mediator;

    public ObservableCollection<OrdemServicoResumoDTO> Ordens { get; } = new();

    private OrdemServicoResumoDTO? _osSelecionada;
    public OrdemServicoResumoDTO? OsSelecionada
    {
        get => _osSelecionada;
        set { _osSelecionada = value; OnPropertyChanged(); }
    }

    private string _termoBusca = string.Empty;
    public string TermoBusca
    {
        get => _termoBusca;
        set { _termoBusca = value; OnPropertyChanged(); }
    }

    private StatusOS? _statusFiltro;
    public StatusOS? StatusFiltro
    {
        get => _statusFiltro;
        set { _statusFiltro = value; OnPropertyChanged(); }
    }

    public Array StatusLista => Enum.GetValues(typeof(StatusOS));

    public ICommand BuscarCommand { get; }
    public ICommand LimparCommand { get; }
    public ICommand NovaOSCommand { get; }
    public ICommand VerDetalhesCommand { get; }

    public ListaOSViewModel(IMediator mediator)
    {
        _mediator = mediator;

        BuscarCommand = new RelayCommand(async _ => await BuscarAsync());
        LimparCommand = new RelayCommand(async _ => 
        { 
            TermoBusca = string.Empty; 
            StatusFiltro = null;
            await BuscarAsync(); 
        });
        NovaOSCommand = new RelayCommand(async _ => await NovaOSAsync());
        VerDetalhesCommand = new RelayCommand(async _ => await VerDetalhesAsync(), _ => OsSelecionada != null);
    }

    public async Task BuscarAsync()
    {
        try
        {
            Ordens.Clear();
            var resultado = await _mediator.Send(new ListarOSQuery
            {
                TermoBusca = TermoBusca,
                Status = StatusFiltro
            });
            foreach (var os in resultado)
                Ordens.Add(os);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Erro na busca: {ex.Message}", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private async Task NovaOSAsync()
    {
        var dialog = new Views.OrdensServico.CriacaoOSWindow(_mediator);
        dialog.ShowDialog();
        await BuscarAsync();
    }

    private async Task VerDetalhesAsync()
    {
        if (OsSelecionada == null) return;
        var dialog = new Views.OrdensServico.DetalhesOSWindow(_mediator, OsSelecionada.Id);
        dialog.ShowDialog();
        await BuscarAsync();
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}
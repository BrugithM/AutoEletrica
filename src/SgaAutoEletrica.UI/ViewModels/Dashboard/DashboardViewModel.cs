using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using MediatR;
using SgaAutoEletrica.Application.Features.Dashboard.DTOs;
using SgaAutoEletrica.Application.Features.Dashboard.Queries;

namespace SgaAutoEletrica.UI.ViewModels.Dashboard;

public class DashboardViewModel : INotifyPropertyChanged
{
    private readonly IMediator _mediator;

    public List<int> Meses { get; } = Enumerable.Range(1, 12).ToList();
    public List<int> Anos { get; } = Enumerable.Range(DateTime.Now.Year - 5, 6).Reverse().ToList();

    private int _mesSelecionado = DateTime.Now.Month;
    public int MesSelecionado
    {
        get => _mesSelecionado;
        set { _mesSelecionado = value; OnPropertyChanged(); _ = CarregarAsync(); }
    }

    private int _anoSelecionado = DateTime.Now.Year;
    public int AnoSelecionado
    {
        get => _anoSelecionado;
        set { _anoSelecionado = value; OnPropertyChanged(); _ = CarregarAsync(); }
    }

    private ResumoMensalDTO? _resumoMensal;
    public ResumoMensalDTO? ResumoMensal
    {
        get => _resumoMensal;
        private set { _resumoMensal = value; OnPropertyChanged(); }
    }

    private ResumoAnualDTO? _resumoAnual;
    public ResumoAnualDTO? ResumoAnual
    {
        get => _resumoAnual;
        private set { _resumoAnual = value; OnPropertyChanged(); }
    }

    public DashboardViewModel(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task CarregarAsync()
    {
        try
        {
            ResumoMensal = await _mediator.Send(new ObterResumoMensalQuery
            {
                Mes = MesSelecionado,
                Ano = AnoSelecionado
            });

            ResumoAnual = await _mediator.Send(new ObterResumoAnualQuery
            {
                Ano = AnoSelecionado
            });
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Erro: {ex.Message}", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}
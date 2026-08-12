using System.ComponentModel;
using System.Runtime.CompilerServices;
using MediatR;
using SgaAutoEletrica.Application.Features.Dashboard.Queries;
using SgaAutoEletrica.Application.Features.Dashboard.DTOs;

namespace SgaAutoEletrica.UI.ViewModels.Dashboard;

public class DashboardViewModel : INotifyPropertyChanged
{
    private readonly IMediator _mediator;

    private ResumoDiarioDTO? _resumo;
    public ResumoDiarioDTO? Resumo
    {
        get => _resumo;
        private set {_resumo = value; OnPropertyChanged();}
    }
    public DashboardViewModel(IMediator mediator)
    {
        _mediator=mediator;
    }

    public async Task CarregarAsync()
    {
        Resumo = await _mediator.Send(new ObterResumoDiarioQuery());
    }
    
    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}
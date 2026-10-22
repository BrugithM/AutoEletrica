using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using MediatR;
using SgaAutoEletrica.Application.Features.MarcasVeiculo.Commands;
using SgaAutoEletrica.Application.Features.MarcasVeiculo.Queries;

namespace SgaAutoEletrica.UI.ViewModels.MarcasVeiculo;

public class CadastroMarcaVeiculoViewModel : INotifyPropertyChanged
{
    private readonly IMediator _mediator;
    private readonly Guid? _marcaId;

    private string _nome = string.Empty;
    public string Nome { get => _nome; set { _nome = value; OnPropertyChanged(); } }

    public string Titulo => _marcaId.HasValue ? "Editar Marca" : "Nova Marca";

    public CadastroMarcaVeiculoViewModel(IMediator mediator, Guid? marcaId = null)
    {
        _mediator = mediator;
        _marcaId = marcaId;
        
        if (_marcaId.HasValue)
        {
            CarregarDadosAsync(_marcaId.Value);
        }
    }

    private async void CarregarDadosAsync(Guid id)
    {
        var lista = await _mediator.Send(new ListarMarcasVeiculoQuery { Ativo = null });
        var marca = lista.FirstOrDefault(m => m.Id == id);
        if (marca != null)
        {
            Nome = marca.Nome;
            OnPropertyChanged(nameof(Nome));
        }
    }

    public async Task<bool> SalvarAsync()
    {
        if (string.IsNullOrWhiteSpace(Nome))
        {
            MessageBox.Show("O nome é obrigatório.", "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning);
            return false;
        }

        try
        {
            if (_marcaId.HasValue)
                await _mediator.Send(new AtualizarMarcaVeiculoCommand { Id = _marcaId.Value, Nome = Nome.Trim() });
            else
                await _mediator.Send(new CriarMarcaVeiculoCommand { Nome = Nome.Trim() });

            return true;
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Erro: {ex.Message}", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            return false;
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}
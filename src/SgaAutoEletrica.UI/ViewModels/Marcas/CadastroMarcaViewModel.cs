using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using MediatR;
using SgaAutoEletrica.Application.Features.Marcas.Commands;
using SgaAutoEletrica.Application.Features.Marcas.Queries;

namespace SgaAutoEletrica.UI.ViewModels.Marcas;

public class CadastroMarcaViewModel : INotifyPropertyChanged
{
    private readonly IMediator _mediator;
    private readonly int? _marcaId;

    private string _nome = string.Empty;
    public string Nome
    {
        get => _nome;
        set { _nome = value; OnPropertyChanged(); }
    }

    private string _titulo = "Cadastro de Marca";
    public string Titulo
    {
        get => _titulo;
        private set { _titulo = value; OnPropertyChanged(); }
    }

    public CadastroMarcaViewModel(IMediator mediator, int? marcaId = null)
    {
        _mediator = mediator;
        _marcaId = marcaId;

        if (marcaId.HasValue)
        {
            Titulo = "Editar Marca";
            CarregarDadosAsync(marcaId.Value);
        }
    }

    private async void CarregarDadosAsync(int marcaId)
    {
        var marca = await _mediator.Send(new ObterMarcaPorIdQuery { Id = marcaId });
        if (marca != null)
            Nome = marca.Nome;
    }

    public async Task<bool> SalvarAsync()
    {
        if (string.IsNullOrWhiteSpace(Nome))
        {
            MessageBox.Show("Nome é obrigatório.", "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning);
            return false;
        }

        try
        {
            if (_marcaId.HasValue)
            {
                await _mediator.Send(new AtualizarMarcaCommand
                {
                    Id = _marcaId.Value,
                    Nome = Nome
                });
            }
            else
            {
                await _mediator.Send(new CriarMarcaCommand { Nome = Nome });
            }

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

    public void Limpar()
    {
        Nome = string.Empty;
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}
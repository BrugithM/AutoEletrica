using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using MediatR;
using SgaAutoEletrica.Application.Features.Servicos.Commands;
using SgaAutoEletrica.Application.Features.Servicos.Queries;

namespace SgaAutoEletrica.UI.ViewModels.Servicos;

public class CadastroServicoViewModel : INotifyPropertyChanged
{
    private readonly IMediator _mediator;
    private readonly Guid? _servicoId;

    private string _nome = string.Empty;
    public string Nome
    {
        get => _nome;
        set { _nome = value; OnPropertyChanged(); }
    }

    private string _descricao = string.Empty;
    public string Descricao
    {
        get => _descricao;
        set { _descricao = value; OnPropertyChanged(); }
    }

    private decimal _precoPadrao;
    public decimal PrecoPadrao
    {
        get => _precoPadrao;
        set { _precoPadrao = value; OnPropertyChanged(); }
    }

    private string _titulo = "Cadastro de Serviços";
    public string Titulo
    {
        get => _titulo;
        private set { _titulo = value; OnPropertyChanged(); }
    }

    public CadastroServicoViewModel(IMediator mediator, Guid? servicoId = null)
    {
        _mediator = mediator;
        _servicoId = servicoId;

        if (servicoId.HasValue)
        {
            Titulo = "Editar Serviço";
            CarregarDadosAsync(servicoId.Value);
        }
    }

    private async void CarregarDadosAsync(Guid servicoId)
    {
        var servico = await _mediator.Send(new ObterServicoPorIdQuery { Id = servicoId });
        if (servico != null)
        {
            Nome = servico.Nome;
            Descricao = servico.Descricao ?? "";
            PrecoPadrao = servico.PrecoPadrao;
        }
    }

    public async Task<bool> SalvarAsync()
    {
        if (string.IsNullOrWhiteSpace(Nome))
        {
            MessageBox.Show("Nome é obrigatório.", "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning);
            return false;
        }
        if (PrecoPadrao <= 0)
        {
            MessageBox.Show("Preço deve ser maior que zero.", "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning);
            return false;
        }

        try
        {
            if (_servicoId.HasValue)
            {
                await _mediator.Send(new AtualizarServicoCommand
                {
                    Id = _servicoId.Value,
                    Nome = Nome,
                    Descricao = Descricao,
                    PrecoPadrao = PrecoPadrao
                });
            }
            else
            {
                await _mediator.Send(new CriarServicoCommand
                {
                    Nome = Nome,
                    Descricao = Descricao,
                    PrecoPadrao = PrecoPadrao
                });
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
        Descricao = string.Empty;
        PrecoPadrao = 0;
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}
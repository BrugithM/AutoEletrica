using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using MediatR;
using SgaAutoEletrica.Application.Features.Clientes.DTOs;
using SgaAutoEletrica.Application.Features.Clientes.Queries;
using SgaAutoEletrica.Application.Features.Veiculos.Commands;
using SgaAutoEletrica.Application.Features.Veiculos.DTOs;
using SgaAutoEletrica.Application.Features.Veiculos.Queries;

namespace SgaAutoEletrica.UI.ViewModels.Veiculos;

public class CadastroVeiculoViewModel : INotifyPropertyChanged
{
    private readonly IMediator _mediator;
    private readonly Guid? _veiculoId;
    private readonly Guid? _clienteIdPreSelecionado;
    private Guid? _clienteIdDoVeiculo;

    public string Placa { get; set; } = string.Empty;
    public string Modelo { get; set; } = string.Empty;
    public string Marca { get; set; } = string.Empty;
    public int Ano { get; set; } = DateTime.Now.Year;
    public string Versao { get; set; } = string.Empty;
    public string Motor { get; set; } = string.Empty;
    public string TipoMotor { get; set; } = string.Empty;
    public string Cor { get; set; } = string.Empty;
    private string _observacao = string.Empty;
    public string Observacao
    {
        get => _observacao;
        set
        {
            _observacao = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(ObservacaoLength));
        }
    }

    public int ObservacaoLength => Observacao.Length;

    public ObservableCollection<ClienteResumoDTO> Clientes { get; } = new();

    private ClienteResumoDTO? _clienteSelecionado;
    public ClienteResumoDTO? ClienteSelecionado
    {
        get => _clienteSelecionado;
        set { _clienteSelecionado = value; OnPropertyChanged(); }
    }

    private string _titulo = "Novo Veículo";
    public string Titulo
    {
        get => _titulo;
        private set { _titulo = value; OnPropertyChanged(); }
    }

    public CadastroVeiculoViewModel(IMediator mediator, Guid? veiculoId = null, Guid? clienteIdPreSelecionado = null)
    {
        _mediator = mediator;
        _veiculoId = veiculoId;
        _clienteIdPreSelecionado = clienteIdPreSelecionado;

        if (veiculoId.HasValue)
        {
            Titulo = "Editar Veículo";
            CarregarDadosAsync(veiculoId.Value);
        }
    }

    private async void CarregarDadosAsync(Guid veiculoId)
    {
        var veiculo = await _mediator.Send(new ObterVeiculoPorIdQuery { Id = veiculoId });
        if (veiculo != null)
        {
            Placa = veiculo.Placa;
            Modelo = veiculo.Modelo;
            Marca = veiculo.Marca;
            Ano = veiculo.Ano;
            Versao = veiculo.Versao ?? "";
            Motor = veiculo.Motor ?? "";
            TipoMotor = veiculo.TipoMotor ?? "";
            Cor = veiculo.Cor ?? "";
            Observacao = veiculo.Observacao ?? "";
            _clienteIdDoVeiculo = veiculo.ClienteId;

            await CarregarClientesAsync();
            ClienteSelecionado = Clientes.FirstOrDefault(c => c.Id == veiculo.ClienteId);

            OnPropertyChanged(nameof(Placa));
            OnPropertyChanged(nameof(Modelo));
            OnPropertyChanged(nameof(Marca));
            OnPropertyChanged(nameof(Ano));
            OnPropertyChanged(nameof(Versao));
            OnPropertyChanged(nameof(Motor));
            OnPropertyChanged(nameof(TipoMotor));
            OnPropertyChanged(nameof(Cor));
            OnPropertyChanged(nameof(Observacao));
        }
    }

    public async Task CarregarClientesAsync()
    {
        Clientes.Clear();
        var resultado = await _mediator.Send(new ListarClientesQuery());
        foreach (var cliente in resultado.Select(c => new ClienteResumoDTO
        {
            Id = c.Id,
            NomeCompleto = c.NomeCompleto,
            Telefone = c.Telefone
        }))
        {
            Clientes.Add(cliente);
        }

        var idPreSelecionar = _clienteIdDoVeiculo ?? _clienteIdPreSelecionado;
        if (idPreSelecionar.HasValue)
        {
            ClienteSelecionado = Clientes.FirstOrDefault(c => c.Id == idPreSelecionar.Value);
        }
    }

    public async Task<bool> SalvarAsync()
    {
        if (string.IsNullOrWhiteSpace(Placa))
        {
            MessageBox.Show("Placa é obrigatória.", "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning);
            return false;
        }

        if (ClienteSelecionado == null)
        {
            MessageBox.Show("Selecione um cliente.", "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning);
            return false;
        }

        if (string.IsNullOrWhiteSpace(Modelo) || string.IsNullOrWhiteSpace(Marca))
        {
            MessageBox.Show("Modelo e Marca são obrigatórios.", "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning);
            return false;
        }

        if (_veiculoId.HasValue)
        {
            await _mediator.Send(new AtualizarVeiculoCommand
            {
                Id = _veiculoId.Value,
                Placa = Placa,
                Modelo = Modelo,
                Marca = Marca,
                Ano = Ano,
                Versao = Versao,
                Motor = Motor,
                TipoMotor = TipoMotor,
                Cor = Cor,
                Observacao = Observacao
            });
        }
        else
        {
            await _mediator.Send(new CriarVeiculoCommand
            {
                Placa = Placa,
                ClienteId = ClienteSelecionado.Id,
                Modelo = Modelo,
                Marca = Marca,
                Ano = Ano,
                Versao = Versao,
                Motor = Motor,
                TipoMotor = TipoMotor,
                Cor = Cor,
                Observacao = Observacao
            });
        }

        return true;
    }

    public void Limpar()
    {
        Placa = string.Empty;
        Modelo = string.Empty;
        Marca = string.Empty;
        Ano = DateTime.Now.Year;
        Versao = string.Empty;
        Motor = string.Empty;
        TipoMotor = string.Empty;
        Cor = string.Empty;
        Observacao = string.Empty;
        ClienteSelecionado = null;

        OnPropertyChanged(nameof(Placa));
        OnPropertyChanged(nameof(Modelo));
        OnPropertyChanged(nameof(Marca));
        OnPropertyChanged(nameof(Ano));
        OnPropertyChanged(nameof(Versao));
        OnPropertyChanged(nameof(Motor));
        OnPropertyChanged(nameof(TipoMotor));
        OnPropertyChanged(nameof(Cor));
        OnPropertyChanged(nameof(Observacao));
        OnPropertyChanged(nameof(ObservacaoLength));
        OnPropertyChanged(nameof(ClienteSelecionado));
    }
    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using SgaAutoEletrica.Application.Features.Clientes.DTOs;
using SgaAutoEletrica.Application.Features.Clientes.Queries;
using SgaAutoEletrica.Application.Features.MarcasVeiculo.DTOs;
using SgaAutoEletrica.Application.Features.MarcasVeiculo.Queries;
using SgaAutoEletrica.Application.Features.Veiculos.Commands;
using SgaAutoEletrica.Application.Features.Veiculos.DTOs;
using SgaAutoEletrica.Application.Features.Veiculos.Queries;
using SgaAutoEletrica.Domain.Enums;

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
    public int Ano { get; set; } = 0;
    public string Versao { get; set; } = string.Empty;
    public string Motor { get; set; } = string.Empty;

    private TipoMotor? _tipoMotor;
    public TipoMotor? TipoMotor
    {
        get => _tipoMotor;
        set { _tipoMotor = value; OnPropertyChanged(); }
    }

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
    public Array TiposMotorDisponiveis { get; } = Enum.GetValues(typeof(SgaAutoEletrica.Domain.Enums.TipoMotor));

    public ObservableCollection<MarcaVeiculoDTO> MarcasVeiculo { get; } = new();

    private MarcaVeiculoDTO? _marcaVeiculoSelecionada;
    public MarcaVeiculoDTO? MarcaVeiculoSelecionada
    {
        get => _marcaVeiculoSelecionada;
        set { _marcaVeiculoSelecionada = value; OnPropertyChanged(); }
    }

    public ICommand NovaMarcaVeiculoCommand { get; }

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

        NovaMarcaVeiculoCommand = new RelayCommand(async _ => await NovaMarcaVeiculoAsync());

        if (veiculoId.HasValue)
        {
            Titulo = "Editar Veículo";
            CarregarDadosAsync(veiculoId.Value);
        }
    }

    private async void CarregarDadosAsync(Guid veiculoId)
    {
        await CarregarMarcasVeiculoAsync();

        var veiculo = await _mediator.Send(new ObterVeiculoPorIdQuery { Id = veiculoId });
        if (veiculo != null)
        {
            Placa = veiculo.Placa;
            Modelo = veiculo.Modelo;
            Marca = veiculo.Marca;
            Ano = veiculo.Ano;
            Versao = veiculo.Versao ?? "";
            Motor = veiculo.Motor ?? "";
            TipoMotor = veiculo.TipoMotor;
            Cor = veiculo.Cor ?? "";
            Observacao = veiculo.Observacao ?? "";
            _clienteIdDoVeiculo = veiculo.ClienteId;

            MarcaVeiculoSelecionada = MarcasVeiculo.FirstOrDefault(m => m.Id == veiculo.MarcaVeiculoId);

            if (MarcaVeiculoSelecionada == null && !string.IsNullOrWhiteSpace(veiculo.Marca))
            {
                MarcaVeiculoSelecionada = MarcasVeiculo.FirstOrDefault(m => 
                    string.Equals(m.Nome, veiculo.Marca, StringComparison.OrdinalIgnoreCase));
            }

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

    public async Task CarregarMarcasVeiculoAsync()
    {
        MarcasVeiculo.Clear();
        var marcas = await _mediator.Send(new ListarMarcasVeiculoQuery { Ativo = true });
        foreach (var m in marcas)
            MarcasVeiculo.Add(m);
    }

    public async Task CarregarClientesAsync()
    {
        Clientes.Clear();
        var resultado = await _mediator.Send(new ListarClientesQuery());
        foreach (var cliente in resultado.Itens.Select(c => new ClienteResumoDTO
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

    private async Task NovaMarcaVeiculoAsync()
    {
        var dialog = new Views.MarcasVeiculo.CadastroMarcaVeiculoWindow(App.ServiceProvider.GetRequiredService<IMediator>());
        if (dialog.ShowDialog() == true)
        {
            var idsAntes = MarcasVeiculo.Select(m => m.Id).ToHashSet();
            await CarregarMarcasVeiculoAsync();

            var nova = MarcasVeiculo.FirstOrDefault(m => !idsAntes.Contains(m.Id));
            if (nova != null)
                MarcaVeiculoSelecionada = nova;
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

        if (string.IsNullOrWhiteSpace(Modelo))
        {
            MessageBox.Show("Modelo é obrigatório.", "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning);
            return false;
        }

        if (MarcaVeiculoSelecionada == null)
        {
            MessageBox.Show("Selecione uma marca válida.", "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning);
            return false;
        }

        try
        {
            if (_veiculoId.HasValue)
            {
                await _mediator.Send(new AtualizarVeiculoCommand
                {
                    Id = _veiculoId.Value,
                    ClienteId = ClienteSelecionado.Id,
                    Placa = Placa,
                    Modelo = Modelo,
                    Marca = MarcaVeiculoSelecionada.Nome, 
                    MarcaVeiculoId = MarcaVeiculoSelecionada.Id, 
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
                    Marca = MarcaVeiculoSelecionada.Nome,
                    MarcaVeiculoId = MarcaVeiculoSelecionada.Id, 
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
        catch (Exception ex)
        {
            var inner = ex;
            while (inner.InnerException != null)
                inner = inner.InnerException;
            MessageBox.Show($"Erro ao salvar veículo: {inner.Message}", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            return false;
        }
    }

    public void Limpar()
    {
        Placa = string.Empty;
        Modelo = string.Empty;
        Marca = string.Empty;
        MarcaVeiculoSelecionada = null;
        Ano = 0;
        Versao = string.Empty;
        Motor = string.Empty;
        TipoMotor = null;
        Cor = string.Empty;
        Observacao = string.Empty;
        ClienteSelecionado = null;

        OnPropertyChanged(nameof(Placa));
        OnPropertyChanged(nameof(Modelo));
        OnPropertyChanged(nameof(Marca));
        OnPropertyChanged(nameof(MarcaVeiculoSelecionada));
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
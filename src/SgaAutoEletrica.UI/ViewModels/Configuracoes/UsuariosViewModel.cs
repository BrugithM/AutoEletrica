using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using SgaAutoEletrica.Application.Common.Interfaces;
using SgaAutoEletrica.Application.Features.Usuarios.Commands;
using SgaAutoEletrica.Application.Features.Usuarios.DTOs;
using SgaAutoEletrica.Application.Features.Usuarios.Queries;
using SgaAutoEletrica.Domain.Enums;

namespace SgaAutoEletrica.UI.ViewModels.Configuracoes;

public class UsuariosViewModel : INotifyPropertyChanged
{
    private readonly IMediator _mediator;

    public ObservableCollection<UsuarioDTO> Usuarios { get; } = new();

    private UsuarioDTO? _usuarioSelecionado;
    public UsuarioDTO? UsuarioSelecionado
    {
        get => _usuarioSelecionado;
        set { _usuarioSelecionado = value; OnPropertyChanged(); OnPropertyChanged(nameof(TemUsuarioSelecionado)); }
    }

    public bool TemUsuarioSelecionado => UsuarioSelecionado != null;

    public ICommand NovoUsuarioCommand { get; }
    public ICommand ResetarSenhaCommand { get; }
    public ICommand DesativarCommand { get; }
    public ICommand AtualizarCommand { get; }

    public UsuariosViewModel(IMediator mediator)
    {
        _mediator = mediator;

        NovoUsuarioCommand = new RelayCommand(_ => NovoUsuario());
        ResetarSenhaCommand = new RelayCommand(_ => ResetarSenha(), _ => TemUsuarioSelecionado);
        DesativarCommand = new RelayCommand(async _ => await DesativarAsync(), _ => TemUsuarioSelecionado);
        AtualizarCommand = new RelayCommand(async _ => await CarregarAsync());
    }

    public async Task CarregarAsync()
    {
        Usuarios.Clear();
        var resultado = await _mediator.Send(new ListarUsuariosQuery());
        foreach (var u in resultado)
        {
            if(MostrarInativos || u.Ativo)
            Usuarios.Add(u);
        }
    }

    private void NovoUsuario()
    {
        var dialog = new Views.Configuracoes.CadastroUsuarioWindow(
            App.ServiceProvider.GetRequiredService<IMediator>());
        if (dialog.ShowDialog() == true)
        {
            _ = CarregarAsync();
        }
    }

    private void ResetarSenha()
    {
        if (UsuarioSelecionado == null) return;

        var dialog = new Views.Configuracoes.ResetarSenhaWindow();
        if (dialog.ShowDialog() == true)
        {
            try
            {
                _mediator.Send(new ResetarSenhaUsuarioCommand
                {
                    UsuarioId = UsuarioSelecionado.Id,
                    NovaSenha = dialog.NovaSenha
                }).GetAwaiter().GetResult();

                MessageBox.Show("Senha resetada com sucesso!", "Sucesso",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                var inner = ex;
                while (inner.InnerException != null)
                    inner = inner.InnerException;
                MessageBox.Show($"Erro: {inner.Message}", "Erro",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }

    private async Task DesativarAsync()
    {
        if (UsuarioSelecionado == null) return;

        var confirmacao = MessageBox.Show(
            $"Deseja realmente desativar o usuário '{UsuarioSelecionado.Nome}'?",
            "Confirmar", MessageBoxButton.YesNo, MessageBoxImage.Warning);

        if (confirmacao == MessageBoxResult.Yes)
        {
            try
            {
                await _mediator.Send(new DesativarUsuarioCommand { UsuarioId = UsuarioSelecionado.Id });
                await CarregarAsync();
            }
            catch (Exception ex)
            {
                var inner = ex;
                while (inner.InnerException != null)
                    inner = inner.InnerException;
                MessageBox.Show($"Erro: {inner.Message}", "Erro",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }

    private bool _mostrarInativos;
    public bool MostrarInativos
    {
        get => _mostrarInativos;
        set { _mostrarInativos = value; OnPropertyChanged(); _ = CarregarAsync(); }
    }
    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}
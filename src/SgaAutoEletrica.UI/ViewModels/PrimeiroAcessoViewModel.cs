using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using MediatR;
using SgaAutoEletrica.Application.Features.Usuarios.Commands;
using SgaAutoEletrica.Domain.Entities;

namespace SgaAutoEletrica.UI.ViewModels;

public class PrimeiroAcessoViewModel : INotifyPropertyChanged
{
    private readonly IMediator _mediator;

    public string Nome { get; set; } = string.Empty;
    public string Senha { get; set; } = string.Empty;
    public string ConfirmarSenha { get; set; } = string.Empty;

    public Usuario? UsuarioCriado { get; private set; }

    public PrimeiroAcessoViewModel(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task<bool> CriarAsync()
    {
        if (string.IsNullOrWhiteSpace(Nome))
        {
            MessageBox.Show("Informe o nome de usuário.", "Aviso",
                MessageBoxButton.OK, MessageBoxImage.Warning);
            return false;
        }
        if (string.IsNullOrWhiteSpace(Senha) || Senha.Length < 6)
        {
            MessageBox.Show("A senha deve ter pelo menos 6 caracteres.", "Aviso",
                MessageBoxButton.OK, MessageBoxImage.Warning);
            return false;
        }
        if (Senha != ConfirmarSenha)
        {
            MessageBox.Show("As senhas não coincidem.", "Aviso",
                MessageBoxButton.OK, MessageBoxImage.Warning);
            return false;
        }

        try
        {
            UsuarioCriado = await _mediator.Send(new CriarPrimeiroAdminCommand
            {
                Nome = Nome,
                Senha = Senha
            });

            return true;
        }
        catch (Exception ex)
        {
            var inner = ex;
            while (inner.InnerException != null)
                inner = inner.InnerException;
            MessageBox.Show($"Erro: {inner.Message}", "Erro",
                MessageBoxButton.OK, MessageBoxImage.Error);
            return false;
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}
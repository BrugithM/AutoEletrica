using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using SgaAutoEletrica.Application.Common.Interfaces;
using SgaAutoEletrica.Domain.Entities;

namespace SgaAutoEletrica.UI.ViewModels;

public class LoginViewModel : INotifyPropertyChanged
{
    private readonly IAuthService _authService;

    public string NomeUsuario { get; set; } = string.Empty;
    public string Senha { get; set; } = string.Empty;

    public ICommand LoginCommand { get; }

    public Usuario? UsuarioAutenticado { get; private set; }

    public LoginViewModel(IAuthService authService)
    {
        _authService = authService;
        LoginCommand = new RelayCommand(async _ => await LoginAsync());
    }

    public async Task LoginAsync()
    {
        if (string.IsNullOrWhiteSpace(NomeUsuario) || string.IsNullOrWhiteSpace(Senha))
        {
            MessageBox.Show("Informe usuário e senha.", "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        var usuario = await _authService.Autenticar(NomeUsuario, Senha);
        if (usuario == null)
        {
            MessageBox.Show("Usuário ou senha inválidos.", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }

        UsuarioAutenticado = usuario;
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}
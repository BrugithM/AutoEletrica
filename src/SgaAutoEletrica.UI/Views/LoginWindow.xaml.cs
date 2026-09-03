using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using SgaAutoEletrica.Application.Common.Interfaces;
using SgaAutoEletrica.UI.ViewModels;

namespace SgaAutoEletrica.UI.Views;

public partial class LoginWindow : Window
{
    private readonly LoginViewModel _viewModel;

    public LoginWindow(LoginViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        DataContext = viewModel;
    }

    private async void BtnLogin_Click(object sender, RoutedEventArgs e)
    {
        _viewModel.Senha = TxtSenha.Password;
        await _viewModel.LoginAsync();

        if (_viewModel.UsuarioAutenticado != null)
        {
            var sessao = App.ServiceProvider.GetRequiredService<ISessaoUsuario>();
            sessao.DefinirUsuario(_viewModel.UsuarioAutenticado);
            
            var mainWindow = App.ServiceProvider.GetRequiredService<MainWindow>();
            System.Windows.Application.Current.MainWindow = mainWindow;
            mainWindow.Show();
            
            Close();
        }
    }
}
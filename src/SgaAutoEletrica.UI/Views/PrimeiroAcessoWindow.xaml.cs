using System.Windows;
using MediatR;
using SgaAutoEletrica.Application.Common.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using SgaAutoEletrica.UI.ViewModels;

namespace SgaAutoEletrica.UI.Views;

public partial class PrimeiroAcessoWindow : Window
{
    private readonly PrimeiroAcessoViewModel _viewModel;

    public PrimeiroAcessoWindow(IMediator mediator)
    {
        InitializeComponent();
        _viewModel = new PrimeiroAcessoViewModel(mediator);
    }

    private async void BtnCriar_Click(object sender, RoutedEventArgs e)
    {
        _viewModel.Nome = TxtNome.Text;
        _viewModel.Senha = TxtSenha.Senha;
        _viewModel.ConfirmarSenha = TxtConfirmarSenha.Senha;

        var sucesso = await _viewModel.CriarAsync();
        if (sucesso && _viewModel.UsuarioCriado != null)
        {
            var sessao = App.ServiceProvider.GetRequiredService<ISessaoUsuario>();
            sessao.DefinirUsuario(_viewModel.UsuarioCriado);

            var mainWindow = App.ServiceProvider.GetRequiredService<MainWindow>();
            System.Windows.Application.Current.MainWindow = mainWindow;
            mainWindow.Show();
            Close();
        }
    }
}
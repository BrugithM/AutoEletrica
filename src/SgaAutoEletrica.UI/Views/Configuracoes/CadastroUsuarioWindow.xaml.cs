using System.Windows;
using MediatR;
using SgaAutoEletrica.Application.Features.Usuarios.Commands;
using SgaAutoEletrica.Domain.Enums;

namespace SgaAutoEletrica.UI.Views.Configuracoes;

public partial class CadastroUsuarioWindow : Window
{
    private readonly IMediator _mediator;

    public CadastroUsuarioWindow(IMediator mediator)
    {
        InitializeComponent();
        _mediator = mediator;
    }

    private async void BtnSalvar_Click(object sender, RoutedEventArgs e)
    {
        var nivel = CmbNivel.SelectedIndex == 0
            ? NivelUsuario.Administrador
            : NivelUsuario.Operador;

        try
        {
            await _mediator.Send(new CriarUsuarioCommand
            {
                Nome = TxtNome.Text,
                Senha = TxtSenha.Senha,
                Nivel = nivel
            });

            MessageBox.Show("Usuário criado com sucesso!", "Sucesso",
                MessageBoxButton.OK, MessageBoxImage.Information);
            DialogResult = true;
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

    private void BtnCancelar_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
    }
}
using System.Windows;
using System.Windows.Controls;
using SgaAutoEletrica.UI.ViewModels.Configuracoes;

namespace SgaAutoEletrica.UI.Views.Configuracoes;

public partial class MeuPerfilView : UserControl
{
    private readonly MeuPerfilViewModel _viewModel;

    public MeuPerfilView(MeuPerfilViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        DataContext = viewModel;
    }

    private void BtnAlterarSenha_Click(object sender, RoutedEventArgs e)
    {
        _viewModel.SenhaAtual = TxtSenhaAtual.Senha;
        _viewModel.NovaSenha = TxtNovaSenha.Senha;
        _viewModel.ConfirmarNovaSenha = TxtConfirmarNovaSenha.Senha;
        _viewModel.AlterarSenhaCommand.Execute(null);

        TxtSenhaAtual.Senha = string.Empty;
        TxtNovaSenha.Senha = string.Empty;
        TxtConfirmarNovaSenha.Senha = string.Empty;
    }
}
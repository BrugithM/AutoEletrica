using System.Windows;

namespace SgaAutoEletrica.UI.Views.Configuracoes;

public partial class ResetarSenhaWindow : Window
{
    public string NovaSenha { get; private set; } = string.Empty;

    public ResetarSenhaWindow()
    {
        InitializeComponent();
    }

    private void BtnResetar_Click(object sender, RoutedEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(TxtNovaSenha.Senha) || TxtNovaSenha.Senha.Length < 6)
        {
            MessageBox.Show("Senha deve ter pelo menos 6 caracteres.", "Aviso",
                MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        if (TxtNovaSenha.Senha != TxtConfirmar.Senha)
        {
            MessageBox.Show("As senhas não coincidem.", "Aviso",
                MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        NovaSenha = TxtNovaSenha.Senha;
        DialogResult = true;
    }

    private void BtnCancelar_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
    }
}
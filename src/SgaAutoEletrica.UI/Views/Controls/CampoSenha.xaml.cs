using System.Windows;
using System.Windows.Controls;

namespace SgaAutoEletrica.UI.Views.Controls;

public partial class CampoSenha : UserControl
{
    public CampoSenha()
    {
        InitializeComponent();
    }

    public static readonly DependencyProperty AlturaProperty =
        DependencyProperty.Register(nameof(Altura), typeof(double), typeof(CampoSenha),
            new PropertyMetadata(32.0));

    public double Altura
    {
        get => (double)GetValue(AlturaProperty);
        set => SetValue(AlturaProperty, value);
    }

    public static readonly DependencyProperty SenhaProperty =
        DependencyProperty.Register(nameof(Senha), typeof(string), typeof(CampoSenha),
            new FrameworkPropertyMetadata(string.Empty, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

    public string Senha
    {
        get => (string)GetValue(SenhaProperty);
        set => SetValue(SenhaProperty, value);
    }

    public static readonly DependencyProperty SenhaVisivelProperty =
        DependencyProperty.Register(nameof(SenhaVisivel), typeof(bool), typeof(CampoSenha),
            new PropertyMetadata(false));

    public bool SenhaVisivel
    {
        get => (bool)GetValue(SenhaVisivelProperty);
        set => SetValue(SenhaVisivelProperty, value);
    }

    private void PwdSenha_PasswordChanged(object sender, RoutedEventArgs e)
    {
        if (!SenhaVisivel)
            Senha = PwdSenha.Password;
    }

    private void TxtSenha_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (SenhaVisivel)
            Senha = TxtSenha.Text;
    }

    private void BtnOlho_Click(object sender, RoutedEventArgs e)
    {
        if (SenhaVisivel)
        {
            PwdSenha.Password = TxtSenha.Text;
        }
        else
        {
            TxtSenha.Text = PwdSenha.Password;
        }

        SenhaVisivel = !SenhaVisivel;
    }
}
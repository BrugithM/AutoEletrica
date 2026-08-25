using System.Windows;

namespace SgaAutoEletrica.UI.Views.OrdensServico;

public partial class GerarNFWindow : Window
{
    public string NumeroNota { get; private set; } = string.Empty;

    public GerarNFWindow()
    {
        InitializeComponent();
    }

    private void BtnGerar_Click(object sender, RoutedEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(TxtNumeroNF.Text))
        {
            MessageBox.Show("Informe o número da nota fiscal.", "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        NumeroNota = TxtNumeroNF.Text;
        DialogResult = true;
    }

    private void BtnCancelar_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
    }
}
using System.Windows;

namespace SgaAutoEletrica.UI.Views.OrdensServico;

public partial class CancelarOSWindow : Window
{
    public string Motivo { get; private set; } = string.Empty;

    public CancelarOSWindow()
    {
        InitializeComponent();
    }

    private void BtnConfirmar_Click(object sender, RoutedEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(TxtMotivo.Text))
        {
            MessageBox.Show("Informe o motivo do cancelamento.", "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        Motivo = TxtMotivo.Text;
        DialogResult = true;
    }

    private void BtnVoltar_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
    }
}
using System.Windows;
using System.Windows.Controls;
using SgaAutoEletrica.Domain.ValueObjects;
using SgaAutoEletrica.UI.ViewModels.Configuracoes;

namespace SgaAutoEletrica.UI.Views.Configuracoes;

public partial class ConfiguracaoEmpresaView : UserControl
{
    private readonly ConfiguracaoEmpresaViewModel _viewModel;

    public ConfiguracaoEmpresaView(ConfiguracaoEmpresaViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        DataContext = viewModel;
        Loaded += ConfiguracaoEmpresaView_Loaded;
    }

    private async void ConfiguracaoEmpresaView_Loaded(object sender, System.Windows.RoutedEventArgs e)
    {
        await _viewModel.CarregarAsync();
    }

    private void TxtCnpj_LostFocus(object sender, RoutedEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(_viewModel.Cnpj)) return;

        try
        {
            var cnpj = new SgaAutoEletrica.Domain.ValueObjects.Cnpj(_viewModel.Cnpj);
            _viewModel.Cnpj = cnpj.Formatado();
            TxtCnpj.GetBindingExpression(System.Windows.Controls.TextBox.TextProperty)?.UpdateTarget();
        }
        catch { }
    }

    private void TxtTelefone_LostFocus(object sender, RoutedEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(_viewModel.Telefone)) return;

        try
        {
            var tel = Telefone.Criar(_viewModel.Telefone);
            _viewModel.Telefone = tel?.Formatado() ?? string.Empty;
            TxtTelefone.GetBindingExpression(TextBox.TextProperty)?.UpdateTarget();
        }
        catch { }
    }
}
using System.Windows;
using System.Windows.Controls;
using SgaAutoEletrica.UI.ViewModels.Configuracoes;

namespace SgaAutoEletrica.UI.Views.Configuracoes;

public partial class BackupView : UserControl
{
    private readonly BackupViewModel _viewModel;

    public BackupView(BackupViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        DataContext = viewModel;
        Loaded += BackupView_Loaded;
    }

    private async void BackupView_Loaded(object sender, RoutedEventArgs e)
    {
        await _viewModel.AtualizarListaAsync();
    }
}
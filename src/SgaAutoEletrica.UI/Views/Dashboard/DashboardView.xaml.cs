using System.Windows;
using System.Windows.Controls;
using SgaAutoEletrica.UI.ViewModels.Dashboard;

namespace SgaAutoEletrica.UI.Views.Dashboard;

public partial class DashboardView : UserControl
{
    private readonly DashboardViewModel _viewModel;

    public DashboardView(DashboardViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        DataContext = viewModel;
        Loaded += DashboardView_Loaded;
    }

    private async void DashboardView_Loaded(object sender, RoutedEventArgs e)
    {
        await _viewModel.CarregarAsync();
    }

    private async void BtnAtualizar_Click(object sender, RoutedEventArgs e)
    {
        await _viewModel.CarregarAsync();
    }
}
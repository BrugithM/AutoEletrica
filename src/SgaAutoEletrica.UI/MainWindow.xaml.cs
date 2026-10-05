using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Microsoft.Extensions.DependencyInjection;
using MediatR;
using SgaAutoEletrica.Application.Features.Configuracoes.Queries;
using SgaAutoEletrica.Application.Common.Interfaces;

namespace SgaAutoEletrica.UI;

public partial class MainWindow : Window
{
    private readonly IMediator _mediator;

    public MainWindow(IMediator mediator)
    {
        InitializeComponent();
        _mediator = mediator;
        Loaded += MainWindow_Loaded;
    }

    private void MainWindow_Loaded(object sender, RoutedEventArgs e)
    {
        CarregarLogo();
        CarregarUsuarioLogado();
    }

    private void BtnInicio_Click(object sender, RoutedEventArgs e)
    {
        CarregarLogo();
    }
    private void CarregarLogo()
    {
        try
        {
            var config = _mediator.Send(new ObterConfiguracaoEmpresaQuery()).GetAwaiter().GetResult();

            if (config == null || string.IsNullOrWhiteSpace(config.LogoPath) || !File.Exists(config.LogoPath))
            {
                ContentArea.Content = null;
            }
            else
            {
                var bitmap = new BitmapImage();
                using (var stream = new FileStream(config.LogoPath, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    bitmap.BeginInit();
                    bitmap.CacheOption = BitmapCacheOption.OnLoad;
                    bitmap.StreamSource = stream;
                    bitmap.EndInit();
                }
                bitmap.Freeze();

                var image = new Image
                {
                    Source = bitmap,
                    Stretch = Stretch.Uniform,
                    Margin = new Thickness(20)
                };

                ContentArea.Content = image;
            }

            var dashboardView = App.ServiceProvider.GetRequiredService<Views.Dashboard.DashboardView>();
            DashboardContent.Content = dashboardView;
            DashboardExpander.Visibility = Visibility.Visible;
        }
        catch
        {
            ContentArea.Content = null;
        }
    }
    public void CarregarUsuarioLogado()
    {
        var sessao = App.ServiceProvider.GetRequiredService<ISessaoUsuario>();
        if (sessao.UsuarioAtual != null)
        {
            TxtUsuario.Text = $"Usuario: {sessao.UsuarioAtual.Nome} ({sessao.UsuarioAtual.Nivel})";
        }
    }

    private void EsconderDashboard()
    {
        DashboardExpander.Visibility = Visibility.Collapsed;
        DashboardContent.Content = null;
    }
    // Navegação

    private void NavegarPara<T>(T view) where T : Control
    {
        ContentArea.Content = view;
    }

    // Cadastro 
    private void BtnCadastroCliente_Click(object sender, RoutedEventArgs e)
    {
        var dialog = new SgaAutoEletrica.UI.Views.Clientes.CadastroClienteWindow(_mediator);
        dialog.ShowDialog();
    }
    private void BtnCadastroVeiculos_Click(object sender, RoutedEventArgs e)
    {
        var dialog = new SgaAutoEletrica.UI.Views.Veiculos.CadastroVeiculoWindow(_mediator);
        dialog.ShowDialog();
    }
    private void BtnCadastroFornecedores_Click(object sender, RoutedEventArgs e)
    {
        var dialog = new SgaAutoEletrica.UI.Views.Fornecedores.CadastroFornecedorWindow(_mediator);
        dialog.ShowDialog();
    }
    private void BtnCadastroPecas_Click(object sender, RoutedEventArgs e)
    {
        var dialog = new SgaAutoEletrica.UI.Views.Pecas.CadastroPecaWindow(_mediator);
        dialog.ShowDialog();
    }
    private void BtnCadastroCategorias_Click(object sender, RoutedEventArgs e)
    {
        if (!TemPermissaoAdmin()) return;
        var dialog = new SgaAutoEletrica.UI.Views.CategoriasPeca.CadastroCategoriaWindow(_mediator);
        dialog.ShowDialog();
    }

    // Gerenciamento

    private void BtnClientes_Click(object sender, RoutedEventArgs e)
    {
        EsconderDashboard();
        var view = App.ServiceProvider.GetRequiredService<SgaAutoEletrica.UI.Views.Clientes.ListaClientesView>();
        NavegarPara(view);
    }

    private void BtnVeiculos_Click(object sender, RoutedEventArgs e)
    {
        EsconderDashboard();
        var view = App.ServiceProvider.GetRequiredService<SgaAutoEletrica.UI.Views.Veiculos.ListaVeiculosView>();
        NavegarPara(view);
    }

    private void BtnFornecedores_Click(object sender, RoutedEventArgs e)
    {
        EsconderDashboard();
        var view = App.ServiceProvider.GetRequiredService<SgaAutoEletrica.UI.Views.Fornecedores.ListaFornecedoresView>();
        NavegarPara(view);
    }

    private void BtnCategorias_Click(object sender, RoutedEventArgs e)
    {
        if (!TemPermissaoAdmin()) return;
        EsconderDashboard();
        var dialog = new Views.CategoriasPeca.GerenciarCategoriasWindow(_mediator);
        dialog.ShowDialog();
    }

    // Estoque

    private void BtnPecas_Click(object sender, RoutedEventArgs e)
    {
        EsconderDashboard();
        var view = App.ServiceProvider.GetRequiredService<SgaAutoEletrica.UI.Views.Pecas.ListaPecasView>();
        NavegarPara(view);
    }

    private void BtnMovEstoque_Click(object sender, RoutedEventArgs e)
    {
        EsconderDashboard();
        MessageBox.Show("Tela de Movimentação de Estoque em breve.", "Em breve", MessageBoxButton.OK, MessageBoxImage.Information);
    }
    private void BtnNotaEntradaLista_Click(object sender, RoutedEventArgs e)
    {
        EsconderDashboard();
        var view = App.ServiceProvider.GetRequiredService<SgaAutoEletrica.UI.Views.Buscas.BuscarNotasEntradaView>();
        ContentArea.Content = view;
    }

    private void BtnNotaEntrada_Click(object sender, RoutedEventArgs e)
    {
        EsconderDashboard();
        var dialog = new SgaAutoEletrica.UI.Views.Fornecedores.CriarNotaFiscalEntradaWindow(
            App.ServiceProvider.GetRequiredService<MediatR.IMediator>());
        dialog.ShowDialog();
    }

    // Ordem de Serviço

    private void BtnCriarOS_Click(object sender, RoutedEventArgs e)
    {
        EsconderDashboard();
        var dialog = new SgaAutoEletrica.UI.Views.OrdensServico.CriacaoOSWindow(_mediator);
        dialog.ShowDialog();
    }

    private void BtnOS_Click(object sender, RoutedEventArgs e)
    {
        EsconderDashboard();
        var view = App.ServiceProvider.GetRequiredService<SgaAutoEletrica.UI.Views.OrdensServico.ListaOSView>();
        NavegarPara(view);
    }
    private void BtnServicos_Click(object sender, RoutedEventArgs e)
    {
        EsconderDashboard();
        var view = App.ServiceProvider.GetRequiredService<SgaAutoEletrica.UI.Views.Servicos.ListaServicosView>();
        NavegarPara(view);
    }
    // Configurações

    private void BtnConfigEmpresa_Click(object sender, RoutedEventArgs e)
    {
        if (!TemPermissaoAdmin()) return;

        EsconderDashboard();
        var view = App.ServiceProvider.GetRequiredService<SgaAutoEletrica.UI.Views.Configuracoes.ConfiguracaoEmpresaView>();
        NavegarPara(view);
    }

    private void BtnConfigImpressoras_Click(object sender, RoutedEventArgs e)
    {
        EsconderDashboard();
        var view = App.ServiceProvider.GetRequiredService<SgaAutoEletrica.UI.Views.Configuracoes.ConfiguracoesImpressoraView>();
        NavegarPara(view);
    }

    private void BtnBackup_Click(object sender, RoutedEventArgs e)
    {
        EsconderDashboard();
        var view = App.ServiceProvider.GetRequiredService<SgaAutoEletrica.UI.Views.Configuracoes.BackupView>();
        NavegarPara(view);
    }

    // Buscas

    private void BtnBuscarCliente_Click(object sender, RoutedEventArgs e)
    {
        EsconderDashboard();
        var view = App.ServiceProvider.GetRequiredService<SgaAutoEletrica.UI.Views.Clientes.ListaClientesView>();
        NavegarPara(view);
    }

    private void BtnBuscarVeiculo_Click(object sender, RoutedEventArgs e)
    {
        EsconderDashboard();
        var view = App.ServiceProvider.GetRequiredService<SgaAutoEletrica.UI.Views.Veiculos.ListaVeiculosView>();
        NavegarPara(view);
    }

    private void BtnBuscarPlaca_Click(object sender, RoutedEventArgs e)
    {
        EsconderDashboard();
        var input = Microsoft.VisualBasic.Interaction.InputBox("Digite a placa:", "Buscar Placa", "");
        if (!string.IsNullOrWhiteSpace(input))
        {
            var view = App.ServiceProvider.GetRequiredService<SgaAutoEletrica.UI.Views.Veiculos.ListaVeiculosView>();
            if (view.DataContext is SgaAutoEletrica.UI.ViewModels.Veiculos.ListaVeiculosViewModel vm)
            {
                vm.TermoBusca = input;
                _ = vm.BuscarAsync();
            }
            ContentArea.Content = view;
        }
    }

    private bool TemPermissaoAdmin()
    {
        var sessao = App.ServiceProvider.GetRequiredService<SgaAutoEletrica.Application.Common.Interfaces.ISessaoUsuario>();
        if (!sessao.EhAdministrador)
        {
            MessageBox.Show("Esta ação é restrita.",
                "Permissão negada", MessageBoxButton.OK, MessageBoxImage.Warning);
            return false;
        }
        return true;
    }
}
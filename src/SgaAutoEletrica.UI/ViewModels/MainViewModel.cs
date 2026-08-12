using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace SgaAutoEletrica.UI.ViewModels;

public class MainViewModel : INotifyPropertyChanged
{
    private object _currentView = null!;
    public object CurrentView
    {
        get => _currentView;
        set { _currentView = value; OnPropertyChanged(); }
    }

    public ICommand NavegarClientesCommand { get; }
    public ICommand NavegarVeiculosCommand { get; }
    public ICommand NavegarPecasCommand { get; }
    public ICommand NavegarServicosCommand { get; }
    public ICommand NavegarFornecedoresCommand { get; }
    public ICommand NavegarOrdensServicoCommand { get; }
    public ICommand NavegarDashboardCommand { get; }

    public MainViewModel()
    {
        NavegarClientesCommand = new RelayCommand(_ => Navegar("Clientes"));
        NavegarVeiculosCommand = new RelayCommand(_ => Navegar("Veiculos"));
        NavegarPecasCommand = new RelayCommand(_ => Navegar("Pecas"));
        NavegarServicosCommand = new RelayCommand(_ => Navegar("Servicos"));
        NavegarFornecedoresCommand = new RelayCommand(_ => Navegar("Fornecedores"));
        NavegarOrdensServicoCommand = new RelayCommand(_ => Navegar("OrdensServico"));
        NavegarDashboardCommand = new RelayCommand(_ => Navegar("Dashboard"));

        // Tela inicial
        Navegar("Dashboard");
    }

    private void Navegar(string tela)
    {
        CurrentView = tela switch
        {
            "Clientes" => "Tela de Clientes",
            "Veiculos" => "Tela de Veículos",
            "Pecas" => "Tela de Peças",
            "Servicos" => "Tela de Serviços",
            "Fornecedores" => "Tela de Fornecedores",
            "OrdensServico" => "Tela de Ordens de Serviço",
            "Dashboard" => "Dashboard",
            _ => "Dashboard"
        };
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}

public class RelayCommand : ICommand
{
    private readonly Action<object?> _execute;
    private readonly Func<object?, bool>? _canExecute;

    public RelayCommand(Action<object?> execute, Func<object?, bool>? canExecute = null)
    {
        _execute = execute;
        _canExecute = canExecute;
    }

    public bool CanExecute(object? parameter) => _canExecute?.Invoke(parameter) ?? true;
    public void Execute(object? parameter) => _execute(parameter);

    public event EventHandler? CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }
}
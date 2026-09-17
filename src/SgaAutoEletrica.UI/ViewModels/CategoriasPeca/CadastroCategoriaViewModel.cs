using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using MediatR;
using SgaAutoEletrica.Application.Features.CategoriasPeca.Commands;

namespace SgaAutoEletrica.UI.ViewModels.CategoriasPeca;

public class CadastroCategoriaViewModel : INotifyPropertyChanged
{
    private readonly IMediator _mediator;

    private string _nome = string.Empty;
    public string Nome
    {
        get => _nome;
        set { _nome = value; OnPropertyChanged(); }
    }

    private string _descricao = string.Empty;
    public string Descricao
    {
        get => _descricao;
        set { _descricao = value; OnPropertyChanged(); }
    }

    public string Titulo => "Cadastro de Categoria";

    public CadastroCategoriaViewModel(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task<bool> SalvarAsync()
    {
        if (string.IsNullOrWhiteSpace(Nome))
        {
            MessageBox.Show("Nome é obrigatório.", "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning);
            return false;
        }

        try
        {
            await _mediator.Send(new CriarCategoriaPecaCommand
            {
                Nome = Nome,
                Descricao = Descricao
            });
            return true;
        }
        catch (Exception ex)
        {
            var inner = ex;
            while (inner.InnerException != null)
                inner = inner.InnerException;
            MessageBox.Show($"Erro: {inner.Message}", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            return false;
        }
    }

    public void Limpar()
    {
        Nome = string.Empty;
        Descricao = string.Empty;
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}
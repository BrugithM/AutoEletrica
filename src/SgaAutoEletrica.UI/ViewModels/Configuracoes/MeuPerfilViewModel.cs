using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using MediatR;
using SgaAutoEletrica.Application.Common.Interfaces;
using SgaAutoEletrica.Application.Features.Usuarios.Commands;
using Microsoft.Extensions.DependencyInjection;

namespace SgaAutoEletrica.UI.ViewModels.Configuracoes;

public class MeuPerfilViewModel : INotifyPropertyChanged
{
    private readonly IMediator _mediator;
    private readonly ISessaoUsuario _sessao;

    public string Nome { get; set; } = string.Empty;
    public string SenhaAtual { get; set; } = string.Empty;
    public string NovaSenha { get; set; } = string.Empty;
    public string ConfirmarNovaSenha { get; set; } = string.Empty;
    public string Nivel { get; set; } = string.Empty;

    public ICommand SalvarNomeCommand { get; }
    public ICommand AlterarSenhaCommand { get; }

    public MeuPerfilViewModel(IMediator mediator, ISessaoUsuario sessao)
    {
        _mediator = mediator;
        _sessao = sessao;
        SalvarNomeCommand = new RelayCommand(async _ => await SalvarNomeAsync());
        AlterarSenhaCommand = new RelayCommand(async _ => await AlterarSenhaAsync());
        Carregar();
    }

    private void Carregar()
    {
        if (_sessao.UsuarioAtual != null)
        {
            Nome = _sessao.UsuarioAtual.Nome;
            Nivel = _sessao.UsuarioAtual.Nivel.ToString();
            OnPropertyChanged(nameof(Nome));
            OnPropertyChanged(nameof(Nivel));
        }
    }

    private async Task SalvarNomeAsync()
    {
        try
        {
            await _mediator.Send(new AlterarMeuNomeCommand { NovoNome = Nome });
            MessageBox.Show("Nome atualizado com sucesso!", "Sucesso",
                MessageBoxButton.OK, MessageBoxImage.Information);
        }
        catch (Exception ex)
        {
            var inner = ex;
            while (inner.InnerException != null)
                inner = inner.InnerException;
            MessageBox.Show($"Erro: {inner.Message}", "Erro",
                MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private async Task AlterarSenhaAsync()
    {
        if (string.IsNullOrWhiteSpace(SenhaAtual) ||
            string.IsNullOrWhiteSpace(NovaSenha) ||
            NovaSenha.Length < 6)
        {
            MessageBox.Show("Preencha todos os campos. Nova senha deve ter no mínimo 6 caracteres.",
                "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        if (NovaSenha != ConfirmarNovaSenha)
        {
            MessageBox.Show("Nova senha e confirmação não coincidem.", "Aviso",
                MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        try
        {
            await _mediator.Send(new AlterarMinhaSenhaCommand
            {
                SenhaAtual = SenhaAtual,
                NovaSenha = NovaSenha
            });

            MessageBox.Show("Senha alterada com sucesso!", "Sucesso",
                MessageBoxButton.OK, MessageBoxImage.Information);

            SenhaAtual = string.Empty;
            NovaSenha = string.Empty;
            ConfirmarNovaSenha = string.Empty;
        }
        catch (Exception ex)
        {
            var inner = ex;
            while (inner.InnerException != null)
                inner = inner.InnerException;
            MessageBox.Show($"Erro: {inner.Message}", "Erro",
                MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}
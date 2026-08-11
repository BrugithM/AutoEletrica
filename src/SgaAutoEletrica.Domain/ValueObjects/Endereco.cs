namespace SgaAutoEletrica.Domain.ValueObjects;

public class Endereco
{
    public string Logradouro { get; }
    public string? Numero { get; }
    public string? Complemento { get; }
    public string Bairro { get; }
    public string Cidade { get; }
    public string Estado { get; }
    public string Cep { get; }   

    public Endereco(
        string logradouro,
        string bairro,
        string cidade,
        string estado,
        string cep,
        string? numero = null,
        string? complemento = null)
    {
        if (string.IsNullOrWhiteSpace(logradouro))
            throw new ArgumentException("Logradouro é obrigatório.", nameof(logradouro));
        if (string.IsNullOrWhiteSpace(bairro))
            throw new ArgumentException("Bairro é obrigatório.", nameof(bairro));
        if (string.IsNullOrWhiteSpace(cidade))
            throw new ArgumentException("Cidade é obrigatória.", nameof(cidade));
        if (string.IsNullOrWhiteSpace(estado))
            throw new ArgumentException("Estado é obrigatório.", nameof(estado));
        if (string.IsNullOrWhiteSpace(cep))
            throw new ArgumentException("CEP é obrigatório.", nameof(cep));

        Logradouro = logradouro;
        Numero = numero;
        Bairro = bairro;
        Cidade = cidade;
        Estado = estado;
        Cep = RemoverFormatacaoCep(cep);
        Complemento = complemento;
    }

    /// <summary>
    /// Retorna o endereço formatado em uma única linha.
    /// Ex: "Rua das Flores, 123 - Centro, São Paulo/SP - 01234-567"
    /// </summary>
    public string Completo()
    {
        var numeroStr = string.IsNullOrWhiteSpace(Numero) ? "s/n" : Numero;
        var complementoStr = string.IsNullOrWhiteSpace(Complemento) ? "" : $" {Complemento}";
        return $"{Logradouro}, {numeroStr}{complementoStr} - {Bairro}, {Cidade}/{Estado} - {FormatarCep()}";
    }

public string FormatarCep()
    {
        if (Cep.Length == 8)
            return Convert.ToUInt64(Cep).ToString(@"00000\-000");
        return Cep;
    }

    private static string RemoverFormatacaoCep(string cep)
    {
        return new string(cep.Where(char.IsDigit).ToArray());
    }

    public override bool Equals(object? obj)
    {
        if (obj is not Endereco outro)
            return false;

        return Logradouro == outro.Logradouro
            && Numero == outro.Numero
            && Bairro == outro.Bairro
            && Cidade == outro.Cidade
            && Estado == outro.Estado
            && Cep == outro.Cep;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Logradouro, Numero, Bairro, Cidade, Estado, Cep);
    }

    public override string ToString() => Completo();

}
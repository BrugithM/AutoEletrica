namespace SgaAutoEletrica.Domain.ValueObjects;

public class Telefone
{
    public string Valor {get; }
    public Telefone(string valor)
    {
        var apenasNumeros = RemoverFormatacao(valor);

        if (!EhValido(apenasNumeros))
            throw new ArgumentException($"Telefone inválido: {valor}");

        Valor = apenasNumeros;
    }

    public string Formatado()
    {
        if (Valor.Length == 11) // Celular com 9 dígitos
            return Convert.ToUInt64(Valor).ToString(@"(00) 00000-0000");
        else // Fixo
            return Convert.ToUInt64(Valor).ToString(@"(00) 0000-0000");
    }

    public string SemFormatacao() => Valor;

    public static bool EhValido(string telefone)
    {
        telefone = new string(telefone.Where(char.IsDigit).ToArray());

        if (telefone.Length < 10 || telefone.Length > 11)
            return false;

        // Não pode ser todos iguais
        if (telefone.All(c => c == telefone[0]))
            return false;

        return true;
    }

    private static string RemoverFormatacao(string valor)
    {
        return new string(valor.Where(char.IsDigit).ToArray());
    }

    public override bool Equals(object? obj)
    {
        if (obj is not Telefone outro)
            return false;
        return Valor == outro.Valor;
    }

    public override int GetHashCode() => Valor.GetHashCode();

    public override string ToString() => Formatado();

}
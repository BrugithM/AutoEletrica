namespace SgaAutoEletrica.Domain.ValueObjects;

public class Cnpj
{
    public string Valor {get;}

     public Cnpj(string valor)
    {
        var apenasNumeros = RemoverFormatacao(valor);

        if (!EhValido(apenasNumeros))
            throw new ArgumentException("CNPJ inválido");

        Valor = apenasNumeros;
    }

    public string Formatado()
    {
        return Convert.ToUInt64(Valor).ToString(@"00\.000\.000\/0000\-00");
    }

    public string SemFormatacao() => Valor;

    public static bool EhValido(string cnpj)
    {
        cnpj = new string(cnpj.Where(char.IsDigit).ToArray());

        if (cnpj.Length != 14)
            return false;

        if (cnpj.All(c => c == cnpj[0]))
            return false;

        int[] multiplicador1 = { 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };
        int[] multiplicador2 = { 6, 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };

        var digitos = cnpj.Select(c => int.Parse(c.ToString())).ToArray();

        // Primeiro dígito
        int soma = 0;
        for (int i = 0; i < 12; i++)
            soma += digitos[i] * multiplicador1[i];

        int resto = soma % 11;
        int digito1 = resto < 2 ? 0 : 11 - resto;

        if (digitos[12] != digito1)
            return false;

        // Segundo dígito
        soma = 0;
        for (int i = 0; i < 13; i++)
            soma += digitos[i] * multiplicador2[i];

        resto = soma % 11;
        int digito2 = resto < 2 ? 0 : 11 - resto;

        return digitos[13] == digito2;
    }

    private static string RemoverFormatacao(string valor)
    {
        return new string(valor.Where(char.IsDigit).ToArray());
    }

    public override bool Equals(object? obj)
    {
        if (obj is not Cnpj outro)
            return false;
        return Valor == outro.Valor;
    }

    public override int GetHashCode() => Valor.GetHashCode();

    public override string ToString() => Formatado();

}
namespace SgaAutoEletrica.Domain.ValueObjects;

public class Cpf
{
    //cpf sem formatação
    public string Valor {get;}

    public Cpf(string valor)
    {
        var apenasNumeros = RemoverFormatacao(valor);

        if(!EhValido(apenasNumeros))
        throw new ArgumentException("CPF inválido");

        Valor = apenasNumeros;
    }

    public string Formatado()
    {
        return Convert.ToUInt64(Valor).ToString(@"000\.000\.000\-00");
    }
    public string SemFormatacao() => Valor;

    public static bool EhValido(string cpf)
    {
        //remove tudo que não é número
        cpf = new string(cpf.Where(char.IsDigit).ToArray());

        if(cpf.Length != 11)
        return false;

        // rejeita cpf com todos os digitos iguais (governo não emite)
        if (cpf.All(c=>c==cpf[0]))
        return false;

        // Calcula os dois dígitos verificadores
        int[] multiplicador1 = { 10, 9, 8, 7, 6, 5, 4, 3, 2 };
        int[] multiplicador2 = { 11, 10, 9, 8, 7, 6, 5, 4, 3, 2 };
    
        var cpfNumeros = cpf.Select(c => int.Parse(c.ToString())).ToArray();

        //primeiro digito
        int soma = 0;
        for(int i =0;i<9;i++)
        soma += cpfNumeros[i] * multiplicador1[i];

        int resto = soma % 11;
        int digito1 = resto < 2 ? 0 : 11 - resto;

        if(cpfNumeros[9] != digito1)
        return false;

        //segundo digito
        soma = 0;
        for(int i = 0; i<10; i++)
        soma += cpfNumeros[i] * multiplicador2[i];

        resto = soma % 11;
        int digito2 = resto < 2 ? 0 : 11 - resto;

        return cpfNumeros[10] == digito2;
    }

    private static string RemoverFormatacao(string valor)
    {
        return new string(valor.Where(char.IsDigit).ToArray());
    }

     public override bool Equals(object? obj)
    {
        if (obj is not Cpf outro)
            return false;

        return Valor == outro.Valor;
    }

    public override int GetHashCode() => Valor.GetHashCode();

    public override string ToString() => Formatado();
}
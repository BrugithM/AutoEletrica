using SgaAutoEletrica.Domain.ValueObjects;

var cpfs = new[] 
{
    "529.982.247-25",
    "170.477.383-04"
};

foreach (var cpf in cpfs)
{
    Console.Write($"Testando: {cpf} → ");
    try
    {
        var c = new Cpf(cpf);
        Console.WriteLine($"OK! Valor: {c.Valor}, Formatado: {c.Formatado()}");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"FALHOU: {ex.Message}");
    }
}
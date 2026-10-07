namespace ApiClinica.Validators;

public static class CpfValidator
{
    public static bool EhValido(string? cpf)
    {
        if (string.IsNullOrWhiteSpace(cpf)) return false;

        // Remove pontos e traço, sobram só os dígitos
        var d = new string(cpf.Where(char.IsDigit).ToArray());

        if (d.Length != 11) return false;

        // CPFs com todos os dígitos iguais são inválidos (111.111.111-11, etc.)
        if (d.Distinct().Count() == 1) return false;

        int Digito(int qtd)
        {
            int soma = 0, peso = qtd + 1;
            for (int i = 0; i < qtd; i++) soma += (d[i] - '0') * peso--;
            int resto = soma % 11;
            return resto < 2 ? 0 : 11 - resto;
        }

        return Digito(9) == d[9] - '0' && Digito(10) == d[10] - '0';
    }
}
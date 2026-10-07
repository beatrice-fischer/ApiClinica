using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace ApiClinica.Validators;

public static class ContatoValidator
{
    // Aceita celular (11 dígitos) ou fixo (10) com DDD de 2 dígitos, com ou sem
    // parênteses, espaço e hífen. Ex.: (47)988888888, (47) 98888-8888, 47988888888
    private static readonly Regex TelefoneRegex =
        new(@"^\(?\d{2}\)?\s?\d{4,5}-?\d{4}$", RegexOptions.Compiled);

    public static string? ValidarEmail(string? email)
    {
        if (string.IsNullOrWhiteSpace(email) || !new EmailAddressAttribute().IsValid(email))
            return "E-mail inválido.";

        return null;
    }

    public static string? ValidarTelefone(string? telefone)
    {
        if (string.IsNullOrWhiteSpace(telefone) || !TelefoneRegex.IsMatch(telefone.Trim()))
            return "Telefone inválido. Use DDD com 2 dígitos e número com 8 ou 9 dígitos, ex.: (47) 98888-8888.";

        return null;
    }
}
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ApiClinica.Data;
using ApiClinica.Mappers;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace ApiClinica.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ValidadorEmailTelefoneController : ControllerBase
{
    private static string? ValidarEmail(string? email)
    {
        // Rejeita e-mail vazio, só com espaços ou em formato inválido
        if (string.IsNullOrWhiteSpace(email) || !new EmailAddressAttribute().IsValid(email))
        {
            return "E-mail inválido.";
        }
 
        return null;
    }

    private static string? ValidarTelefone(string? telefone)
    {
        // Rejeita telefone vazio, só com espaços ou em formato inválido
        var TelefoneRegex = new Regex(@"^\(?\d{2}\)?\s?\d{4,5}-?\d{4}$", RegexOptions.Compiled);
        if (string.IsNullOrWhiteSpace(telefone) || !TelefoneRegex.IsMatch(telefone.Trim()))
        {
            return "Telefone inválido. Use DDD com 2 dígitos e número com 8 ou 9 dígitos, ex.: (47) 98888-8888.";
        }
        // Aceita celular (11 dígitos) ou fixo (10 dígitos) com DDD de 2 dígitos,
        // com ou sem parênteses, espaço e hífen. Ex.: (47)988888888, (47) 98888-8888, 47988888888
 
        return null;
    }
}
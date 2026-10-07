using Microsoft.AspNetCore.Mvc;
using ApiClinica.Models;
using ApiClinica.Data;
using Microsoft.EntityFrameworkCore;

namespace ApiClinica.Controllers;

[ApiController]
[Route("api/[controller]")]

public static class CpfValidator{
    public static bool EhValido(string? cpf)
    {
        if (string.IsNullOrWhiteSpace(cpf)) return false;
        
        // Remove caracters não numéricos (pontos e traço)
        var d = new string (cpf.Where(char.IsDigit).To Array());

        // Verifica se tem 11 digitos
        if (d.Length != 11) return false;

        // Verifica se todos os digitos são iguais (CPFs inválidos conhecidos) 
        if (d.Distinct().Count() == 1) return false; // 111.111.111 - 11 é um CPF inválido
        
        int Digito(int qtd)
        {
            // Cálculo dos dígitos do verificador
            int soma = 0, peso = qtd + 1;
            for (int i = 0; i < qtd; i ++) soma += (d[i] - '0') * peso--;
            int resto = soma % 11;
            return resto < 2 ? 0 : 11 - resto;
        }

        return Digito(9) == d[9] - '0' && Digito [10] - '0'
    }
}
namespace ApiClinica.Services;

public enum TipoErro
{
    Nenhum,
    NaoEncontrado,
    Invalido,
    Conflito
}

// Para operações que NÃO devolvem dado (DELETE)

public class Resultado
{
    public bool Sucesso { get; init;}
    public  string? Erro { get; init;}
    public TipoErro Tipo { get; init;}


public static Resultado Ok() =>
    new() { Sucesso = true, Tipo = TipoErro.Nenhum };

public static Resultado NaoEncontrado()=>
    new() {Sucesso = false, Tipo = TipoErro.NaoEncontrado};

public static Resultado Invalido(string erro) =>
    new() {Sucesso = false, Erro = erro, Tipo = TipoErro.Invalido};

public static Resultado Conflito(string erro) =>
    new() {Sucesso = false, Erro = erro, Tipo = TipoErro.Conflito};
}

// Para operações que devolvem dado (GET, POST, PATCH)

public class Resultado<T>
{
    public bool Sucesso {get; init;}
    public T? Valor {get; init;}
    public string? Erro { get; init;}
    public TipoErro Tipo { get; init; }

    public static Resultado<T> Ok(T valor) =>
        new() { Sucesso = true, Valor = valor, Tipo = TipoErro.Nenhum };
    public static Resultado<T> NaoEncontrado() =>
        new() { Sucesso = false, Tipo = TipoErro.NaoEncontrado};
    public static Resultado<T> Invalido(string erro) =>
        new() {Sucesso = false, Erro = erro, Tipo = TipoErro.Invalido};
    public static Resultado<T> Conflito(string erro) =>
        new() {Sucesso = false, Erro = erro, Tipo = TipoErro.Conflito};
}
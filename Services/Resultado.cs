namespace ApiClinica.Services;

public enum TipoErro
{
    Nenhum,
    NaoEncontrado,
    Invalido,
    Conflito,
    NaoAutorizado
}

// Para operações que NÃO devolvem dado (DELETE)

public class Resultado
{
    public bool Sucesso { get; init;}
    public  string? Erro { get; init; }
    public string? Data { get; init; }
    public TipoErro Tipo { get; init;}


public static Resultado Ok(string? mensagem = "", Resultado? result = null) =>
    new() { Sucesso = true, Tipo = TipoErro.Nenhum };

    
public static Resultado LoginToken(string? token = "") =>
    new() { Sucesso = true, Tipo = TipoErro.Nenhum, Data = token};

public static Resultado NaoEncontrado()=>
    new() {Sucesso = false, Tipo = TipoErro.NaoEncontrado};

public static Resultado Invalido(string erro) =>
    new() {Sucesso = false, Erro = erro, Tipo = TipoErro.Invalido};

public static Resultado Conflito(string erro) =>
    new() {Sucesso = false, Erro = erro, Tipo = TipoErro.Conflito};
public static Resultado NaoAutorizado(string erro) =>
    new() { Sucesso = false, Erro = erro, Tipo = TipoErro.NaoAutorizado};
}

// Para operações que devolvem dado (GET, POST, PATCH)

public class Resultado<T>
{
    public bool Sucesso {get; init;}
    public T? Valor {get; init;}
    public string? Erro { get; init;}
    public TipoErro Tipo { get; init; }

    public static Resultado<T> Ok(T valor) =>
        new() { Sucesso = true, Valor = valor, Tipo = TipoErro.Nenhum};
    public static Resultado<T> NaoEncontrado() =>
        new() { Sucesso = false, Tipo = TipoErro.NaoEncontrado};
    public static Resultado<T> Invalido(string erro) =>
        new() {Sucesso = false, Erro = erro, Tipo = TipoErro.Invalido};
    public static Resultado<T> Conflito(string erro) =>
        new() {Sucesso = false, Erro = erro, Tipo = TipoErro.Conflito};
    public static Resultado<T> NaoAutorizado(string erro) =>
        new() { Sucesso = false, Erro = erro, Tipo = TipoErro.NaoAutorizado};
}
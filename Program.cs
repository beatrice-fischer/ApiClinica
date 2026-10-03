//ALUNAS: Beatrice Fischer, Gabriele Maria Freiberger, Lucas de Carvalho Ziele, Raul Schmitz, Gustavo Hreczuck

using ApiClinica.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// O banco fica na RAIZ DO PROJETO, versionado no repositório, para que todos
// usem a mesma base e um "Clean" não apague os dados.
//
// A pasta do projeto é localizada subindo a partir da pasta do executável até
// encontrar o arquivo .csproj. Isso não depende do diretório de trabalho, que
// muda conforme a aplicação seja iniciada pelo Visual Studio, por "dotnet run"
// ou pelo executável — e era essa variação que fazia cada máquina abrir um
// arquivo de banco diferente.
var pasta = new DirectoryInfo(AppContext.BaseDirectory);
while (pasta is not null && !pasta.EnumerateFiles("*.csproj").Any())
    pasta = pasta.Parent;

var dbPath = Path.Combine(pasta?.FullName ?? AppContext.BaseDirectory, "clinica.db");

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlite($"Data Source={dbPath}"));

var app = builder.Build();

// Cria o banco e aplica as migrations pendentes na inicialização, para que um
// clone novo suba funcionando sem ninguém precisar rodar "dotnet ef database
// update" manualmente.
using (var scope = app.Services.CreateScope())
{
    var contexto = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    contexto.Database.Migrate();
}

app.Logger.LogInformation("Banco de dados em uso: {Caminho}", dbPath);

app.UseHttpsRedirection();
app.MapControllers();

app.Run();

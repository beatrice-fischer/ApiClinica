//EQUIPE: Beatrice Fischer, Gabriele Maria Freiberger, Lucas de Carvalho Ziele, Raul Schmitz, Gustavo Hreczuck

using System.Text;
using ApiClinica.Data;
using ApiClinica.Interfaces;
using ApiClinica.Mappers;
using ApiClinica.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

var pasta = new DirectoryInfo(AppContext.BaseDirectory);
while (pasta is not null && !pasta.EnumerateFiles("*.csproj").Any())
    pasta = pasta.Parent;

var dbPath = Path.Combine(pasta?.FullName ?? AppContext.BaseDirectory, "clinica.db");

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlite($"Data Source={dbPath}"));

builder.Services.AddScoped<IPacienteMapper, PacienteMapper>();
builder.Services.AddScoped<IMedicoMapper, MedicoMapper>();
builder.Services.AddScoped<IConsultaMapper, ConsultaMapper>();

builder.Services.AddScoped<IPacienteService, PacienteService>();

builder.Services.AddScoped<IMedicoService, MedicoService>();
//builder.Services.AddScoped<IConsultaService, ConsultaService>();

// Busca a chave JWT definida no appsettings.json
// Essa chave será usada para assinar e validar os tokens
var jwtKey = builder.Configuration["Jwt:Key"];

builder
    .Services
    // Define que o sistema de autenticação da aplicação será JWT Bearer
    // "Bearer" significa que o token será enviado no header:
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    // Configura os parâmetros de validação do JWT
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            // Valida se quem gerou o token está correto
            ValidateIssuer = true,

            // Valida se o token foi criado para o público correto
            ValidateAudience = true,

            // Verifica se o token ainda está válido
            ValidateLifetime = true,

            // Verifica se a assinatura do token é válida
            ValidateIssuerSigningKey = true,

            // Valor esperado para o Issuer
            // Deve ser igual ao usado na geração do token
            ValidIssuer = builder.Configuration["Jwt:Issuer"],

            // Valor esperado para o Audience
            // Deve ser igual ao usado na geração do token
            ValidAudience = builder.Configuration["Jwt:Audience"],

            // Define a chave secreta usada para validar a assinatura
            // A mesma chave usada ao criar o token
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey!)),
        };
    });

builder.Services.AddAuthorization();

var app = builder.Build();

app.UseHttpsRedirection();
app.UseAuthentication(); // IMPORTANTE
app.UseAuthorization();

using (var scope = app.Services.CreateScope())
{
    var contexto = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    contexto.Database.Migrate();
}

// app.Logger.LogInformation("Banco de dados em uso: {Caminho}", dbPath);

// app.UseHttpsRedirection();
app.MapControllers();

app.Run();

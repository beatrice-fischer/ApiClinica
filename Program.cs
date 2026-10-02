//ALUNAS: Beatrice Fischer, Gabriele Maria Freiberger, Lucas de Carvalho Ziele, Raul Schmitz, Gustavo Hreczuck

using ApiClinica.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlite("Data Source=clinica.db"));

var app = builder.Build();

app.UseHttpsRedirection();
app.MapControllers();

app.Run();
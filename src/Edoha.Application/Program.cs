using Edoha.Infraestructure.Repositories;
using System.Data;
using Microsoft.Data.SqlClient;
using Edoha.Domain.Services;
using Edoha.Domain.Interfaces.Repositories;
using Edoha.Domain.Interfaces.Services;
using Edoha.Infraestructure.Data.Context;
using Microsoft.EntityFrameworkCore;


var builder = WebApplication.CreateBuilder(args);


// Conexão com o Banco de Dados
var connectionString = builder.Configuration.GetConnectionString("Default");
Console.WriteLine(connectionString);
builder.Services.AddSingleton<IDbConnection>(sp => new SqlConnection(connectionString));

// Adiciona configuração do drive psql
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("Default"))
); 


// Registro de Repositórios
builder.Services.AddScoped<ILotteryRepository, LotteryRepository>();
builder.Services.AddScoped<ITicketRepository, TicketRepository>();
builder.Services.AddScoped<ITicketbookRepository, TicketbookRepository>();
builder.Services.AddScoped<IStatusTicketbookRepository, StatusTicketbookRepository>();

// Registro de serviços
builder.Services.AddScoped<ILotteryService, LotteryService>();
builder.Services.AddScoped<ITicketService, TicketService>();
builder.Services.AddScoped<ITicketbookService, TicketbookService>();


builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

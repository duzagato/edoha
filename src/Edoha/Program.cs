using Edoha.Infrastructure;
using Edoha.Infraestructure.Repositories;
using Edoha.Infrastructure.Data.Context;
using System.Data;
using Microsoft.Data.SqlClient;
using Edoha.Domain.Interfaces;
using Edoha.Application.Services;
using Edoha.Domain.Services;


var builder = WebApplication.CreateBuilder(args);


// Conex�o com o Banco de Dados
var connectionString = builder.Configuration.GetConnectionString("Default");
Console.WriteLine(connectionString);
builder.Services.AddSingleton<IDbConnection>(sp => new SqlConnection(connectionString));
 
// Registro de Reposit�rios
builder.Services.AddScoped<ILotteryRepository, LotteryRepository>();

// Registro de Servi�os
builder.Services.AddScoped<ILotteryService, LotteryService>();

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

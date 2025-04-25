using Edoha.Infraestructure;
using Edoha.Infraestructure.Repositories;
using Edoha.Infraestructure.Util;
using Edoha.Infrastructure.Data.Context;
using System.Data;
using Microsoft.Data.SqlClient;
using Edoha.Domain.Services;
using Edoha.Domain.Interfaces.Repositories;
using Edoha.Domain.Interfaces.Services;
using Edoha.Domain.Interfaces.Util;


var builder = WebApplication.CreateBuilder(args);


// Conexão com o Banco de Dados
var connectionString = builder.Configuration.GetConnectionString("Default");
Console.WriteLine(connectionString);
builder.Services.AddSingleton<IDbConnection>(sp => new SqlConnection(connectionString));
 
// Registro de Repositórios
builder.Services.AddScoped<ILotteryRepository, LotteryRepository>();
builder.Services.AddScoped<ITicketRepository, TicketRepository>();
builder.Services.AddScoped<ITicketbookRepository, TicketbookRepository>();
builder.Services.AddScoped<IStatusTicketbookRepository, StatusTicketbookRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserTypeRepository, UserTypeRepository>();

builder.Services.AddScoped<ICrypto, Crypto>();

// Registro de serviços
builder.Services.AddScoped<ILotteryService, LotteryService>();
builder.Services.AddScoped<ITicketService, TicketService>();
builder.Services.AddScoped<ITicketbookService, TicketbookService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IUserTypeService, UserTypeService>();


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

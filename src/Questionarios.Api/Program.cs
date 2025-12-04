using Microsoft.EntityFrameworkCore;
using Questionarios.Application.Abstractions;
using Questionarios.Application.Services;
using Questionarios.Domain.Abstractions;
using Questionarios.Infrastructure;
using Questionarios.Infrastructure.Repositories;
using Swashbuckle.AspNetCore.SwaggerGen;
using Swashbuckle.AspNetCore.SwaggerUI;
using Swashbuckle.AspNetCore.Swagger;
using Microsoft.OpenApi.Models;


var builder = WebApplication.CreateBuilder(args);

var services = builder.Services;
var config = builder.Configuration;

// DbContext
services.AddDbContext<QuestionariosDbContext>(opt =>
    opt.UseSqlServer(config.GetConnectionString("DefaultConnection")));

// Domain infra
services.AddScoped<ISurveyRepository, SurveyRepository>();
services.AddScoped<IResponseRepository, ResponseRepository>();
services.AddScoped<IResultsRepository, ResultsRepository>();

services.AddScoped<IDateTimeProvider, SystemDateTimeProvider>();

// App layer services
services.AddScoped<ISurveyService, SurveyService>();
services.AddScoped<IQuestionService, QuestionService>();
services.AddScoped<IResponseService, ResponseService>();
services.AddScoped<IResultsService, ResultsService>();

// Infra auxiliares (stub)
services.AddSingleton<ICacheService, InMemoryCacheService>();
services.AddSingleton<IQueueClient, ConsoleQueueClient>();

services.AddControllers();
services.AddEndpointsApiExplorer();
services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapControllers();

app.Run();

public partial class Program { }

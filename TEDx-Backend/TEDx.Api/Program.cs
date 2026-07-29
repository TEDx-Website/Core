using Microsoft.AspNetCore.Mvc;
using TEDx.Api.Common.Respones;
using TEDx.Api.Extensions;
using TEDx.Api.Middleware;
using TEDx.Application;

var builder = WebApplication.CreateBuilder(args);

builder.Host.AddTedxSerilog();

builder.Services.AddApplicationServices();

builder.Services.AddControllers();

// Register custom API behavior for validation error responses
builder.Services.AddCustomApiBehavior();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseMiddleware<GlobalExceptionMiddleware>();
app.UseMiddleware<CorrelationIdMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();

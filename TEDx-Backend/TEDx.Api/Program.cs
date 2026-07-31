using Microsoft.AspNetCore.Mvc;
using TEDx.Api.Common.Respones;
using TEDx.Api.Extensions;
using TEDx.Api.Middleware;
using TEDx.Application;
using TEDx.Infrastructure.Persistence.Seeding;

var builder = WebApplication.CreateBuilder(args);

builder.Host.AddTedxSerilog();

builder.Services.AddApplicationServices();

builder.Services.AddInfrastructureServices(builder.Configuration);

builder.Services.AddControllers();


// Register custom API behavior for validation error responses
builder.Services.AddCustomApiBehavior();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var seeder = scope.ServiceProvider.GetRequiredService<AdminSeeder>();
    await seeder.SeedAsync();
}

app.UseMiddleware<GlobalExceptionMiddleware>();
app.UseMiddleware<CorrelationIdMiddleware>();

//if (app.Environment.IsDevelopment())
//{
    app.UseSwagger();
    app.UseSwaggerUI();
//}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();

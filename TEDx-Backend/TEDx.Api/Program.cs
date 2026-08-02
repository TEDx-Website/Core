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

// Registers the "DefaultCorsPolicy" that app.UseCors below asks for.
builder.Services.AddApiBehavior(builder.Configuration);

// JWT bearer authentication. Lives in the Api layer, not Infrastructure: which scheme
// guards the HTTP surface is a hosting concern (SD §6.2).
builder.Services.AddTedxAuthentication();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddTedxSwagger();
// Register the antiforgery service with a custom header name
//builder.Services.AddAntiforgery(options =>
//{
//    options.HeaderName = "X-CSRF-TOKEN";
//});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var seeder = scope.ServiceProvider.GetRequiredService<AdminSeeder>();
    await seeder.SeedAsync();
}

app.UseMiddleware<GlobalExceptionMiddleware>();
app.UseMiddleware<CorrelationIdMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRouting();

app.UseCors("DefaultCorsPolicy");

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.MapDevDiagnostics();

app.Run();

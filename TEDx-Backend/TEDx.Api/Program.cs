using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;
using TEDx.Api.Common.Responses;
using TEDx.Api.Extensions;
using TEDx.Api.Middleware;
using TEDx.Api.RateLimiting;
using TEDx.Application;
using TEDx.Infrastructure.Persistence.Seeding;

var builder = WebApplication.CreateBuilder(args);

builder.Host.AddTedxSerilog();

builder.Services.AddApplicationServices();

builder.Services.AddInfrastructureServices(builder.Configuration);

builder.Services.AddControllers()
    .AddJsonOptions(options =>
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

// Register custom API behavior for validation error responses
builder.Services.AddCustomApiBehavior();

// Registers the "DefaultCorsPolicy" that app.UseCors below asks for.
builder.Services.AddApiBehavior(builder.Configuration);

// JWT bearer authentication. Lives in the Api layer, not Infrastructure: which scheme
// guards the HTTP surface is a hosting concern (SD §6.2).
builder.Services.AddTedxAuthentication();

// Rate limiting for the auth endpoint group
builder.Services.AddTedxRateLimiting();

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

// Rewrites RemoteIpAddress from X-Forwarded-For when a trusted proxy is configured. Must run
// before anything reads the client IP — the rate limiter partitions on it.
app.UseTedxForwardedHeaders();

app.UseMiddleware<GlobalExceptionMiddleware>();
app.UseMiddleware<CorrelationIdMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseStaticFiles();
    app.UseSwagger();
    app.UseSwaggerUI(options => options.ConfigureTedxSwaggerUI());
}

app.UseRouting();

app.UseCors("DefaultCorsPolicy");

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

// Run after CORS so the frontend can read 429 responses normally. Also place it
// after authentication so authenticated rate-limit policies can identify callers
// by user instead of sharing an IP-based bucket.
app.UseRateLimiter();

app.MapControllers();
app.MapDevDiagnostics();

app.Run();

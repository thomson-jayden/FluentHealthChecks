using FluentHealthChecks.DependencyInjection;
using FluentHealthChecks.Example.Module;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddExampleModule(); // example module includes health checks

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseFluentHealthChecks();
app.UseHttpsRedirection();

app.Run();
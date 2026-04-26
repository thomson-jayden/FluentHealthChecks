using FluentHealthChecks.DependencyInjection;
using FluentHealthChecks.Example.ModuleA;
using FluentHealthChecks.Example.ModuleB;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddExampleModuleA()
    .AddExampleModuleB();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseFluentHealthChecks();
app.UseHttpsRedirection();

app.Run();
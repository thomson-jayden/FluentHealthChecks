using FluentHealthChecks.Example.Module;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddExampleModule(); // example module includes health checks

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseExampleModule(); // example module includes setup for health check endpoints
app.UseHttpsRedirection();

app.Run();

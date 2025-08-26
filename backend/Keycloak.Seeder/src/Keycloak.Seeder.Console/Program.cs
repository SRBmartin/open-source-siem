
using Keycloak.Seeder.Application.Contracts;
using Keycloak.Seeder.Console.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = Host.CreateApplicationBuilder(args);

builder.Configuration
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddEnvironmentVariables();

builder.Services.AddConsoleServices(builder.Configuration);

var host = builder.Build();

var worker = host.Services.GetRequiredService<ISeederWorker>();
await worker.RunAsync();
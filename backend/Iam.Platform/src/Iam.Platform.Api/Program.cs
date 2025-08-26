using Iam.Platform.Api.DependencyInjection;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog();

builder.Services.AddApiServices(builder.Configuration);

var app = builder.Build();

app.UseSerilogRequestLogging();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();

app.Run();
using MailService.Api.DependncyInjection;
using MailService.Api.Middleware;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApiServices(builder.Configuration);

var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRouting();
app.UseMiddleware<AuthMiddleware>();

app.MapControllers();

app.Run();
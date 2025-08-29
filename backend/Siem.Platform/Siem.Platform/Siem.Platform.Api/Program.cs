using Serilog;
using Siem.Platform.Api.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog();

const string CorsPolicy = "AllowAll";
builder.Services.AddCors(options =>
{
    options.AddPolicy(CorsPolicy, policy =>
        policy
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader()
    );
});

builder.Services.AddPlatformApiServices(builder.Configuration);

var app = builder.Build();

app.UseSerilogRequestLogging();
app.UseSwagger();
app.UseSwaggerUI();

app.UseRouting();
app.UseCors(CorsPolicy);

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
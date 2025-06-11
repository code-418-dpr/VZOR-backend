using Hangfire;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.AspNetCore.Server.Kestrel.Https;
using VZOR.Accounts.Domain;
using VZOR.Accounts.Infrastructure.Seeding;
using VZOR.Framework.Middlewares;
using VZOR.Images.Infrastructure;
using VZOR.Web;
using VZOR.Web.Extensions;

//TODO: Minio не должен работать на локалхосте в докере!

DotNetEnv.Env.Load();

var builder = WebApplication.CreateBuilder(args);
builder.WebHost.ConfigureKestrel(options =>
{
    options.ConfigureHttpsDefaults(o =>
    {
        //o.ClientCertificateValidation += ValidateClientCertificate;
        o.AllowAnyClientCertificate();
        o.ClientCertificateMode = ClientCertificateMode.RequireCertificate;
        o.CheckCertificateRevocation = false;
    });
});
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddCors();

builder.Services.AddModules(builder.Configuration);

builder.Services.AddLogger(builder.Configuration);
builder.Services.AddSwagger();

var app = builder.Build();

var seeder = app.Services.GetRequiredService<Seeding>();
await seeder.SeedBucket();

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.OAuth2RedirectUrl("http://localhost:5238/api/Account/yandex-callback");
});

app.UseCors(config =>
{
    config
        .WithOrigins("http://localhost:3000")
        .AllowAnyMethod()
        .AllowAnyHeader()
        .AllowCredentials();
});

app.UseHangfireDashboard();

app.UseExceptionMiddleware();

app.UseAuthentication();
app.UseScopeDataMiddleware();
app.UseAuthorization();

app.UseHangfireServer();

app.MapControllers();

await app.InitAndRunAsync();

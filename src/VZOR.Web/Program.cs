using Hangfire;
using VZOR.Accounts.Infrastructure.Seeding;
using VZOR.Framework.Middlewares;
using VZOR.Web;
using VZOR.Web.Extensions;


DotNetEnv.Env.Load();

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddGrpc();

builder.Services.AddModules(builder.Configuration);

builder.Services.AddLogger(builder.Configuration);
builder.Services.AddSwagger();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();
app.UseHangfireDashboard();

app.UseExceptionMiddleware();

app.UseAuthentication();
app.UseScopeDataMiddleware();
app.UseAuthorization();

app.UseHangfireServer();

app.MapControllers();

await app.InitAndRunAsync();

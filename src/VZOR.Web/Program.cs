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

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

var accountsSeeder = app.Services.GetRequiredService<AccountsSeeder>();

await accountsSeeder.SeedAsync();

app.UseExceptionMiddleware();

app.UseAuthentication();
app.UseScopeDataMiddleware();
app.UseAuthorization();

app.MapControllers();

await app.InitAndRunAsync();

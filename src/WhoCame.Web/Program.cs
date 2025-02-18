using WhoCame.Framework.Middlewares;
using WhoCame.Web.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddLogger(builder.Configuration);
builder.Services.AddSwagger();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseExceptionMiddleware();

app.MapControllers();

app.Run();

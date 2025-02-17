using WhoCame.Web.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddLogger(builder.Configuration);
builder.Services.AddSwagger();

var app = builder.Build();

app.UseHttpsRedirection();

app.Run();

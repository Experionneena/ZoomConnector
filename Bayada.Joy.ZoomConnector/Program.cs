using Bayada.Joy.ZoomConnector.ConfigOptions;
using Bayada.Joy.ZoomConnector.Extensions;

var builder = WebApplication.CreateBuilder(args);
var zoomConfig = builder.Configuration.GetSection("ZoomSettings").Get<ZoomSettings>();
builder.Services.RegisterDependencies( option => { option.EndPoints = zoomConfig.EndPoints; option.ClientId = zoomConfig.ClientId; option.ClientSecret = zoomConfig.ClientSecret; option.AccountId = zoomConfig.AccountId; });

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.UseSession();

app.Run();

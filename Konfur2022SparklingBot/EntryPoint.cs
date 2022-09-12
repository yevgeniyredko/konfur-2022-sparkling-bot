using Konfur2022SparklingBot;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<SparklingBotSettings>();

var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.Run();
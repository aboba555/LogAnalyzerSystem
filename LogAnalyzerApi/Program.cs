using System.Text.Json.Serialization;
using System.Threading.Channels;
using LogAnalyzerBusiness;
using LogAnalyzerBusiness.Services.Gpt;
using LogAnalyzerBusiness.Services.ResultStore;
using LogAnalyzerBusiness.Worker;

var builder = WebApplication.CreateBuilder(args);

var configuration = builder.Configuration;
// our channel
builder.Services.AddSingleton(Channel.CreateBounded<(Guid, string)>(100));

// business logic services
builder.Services.AddBusinessLogic();
builder.Services.AddHttpClient<IGptService, GptService>(client =>
{
    client.BaseAddress = new Uri("https://api.openai.com");
    var apiKey = configuration["Gpt:ApiKey"];
    client.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");
});

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

// builder.Services.AddEndpointsApiExplorer();
// builder.Services.AddSwaggerGen();

var app = builder.Build();

// app.UseSwagger();
// app.UseSwaggerUI();

app.UseHttpsRedirection();

app.MapControllers();

app.Run();
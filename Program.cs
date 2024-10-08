// var builder = WebApplication.CreateBuilder(args);
// var app = builder.Build();
//
// app.MapGet("/", () => "Hello World!");
//
// app.Run();


using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using SemanticKernel.Plugins;

#pragma warning disable SKEXP0001

var builder = Kernel.CreateBuilder();

builder.AddOpenAIChatCompletion(
    modelId: "gpt-4o-mini",
    apiKey:
    "XXXXX");

builder
    .Services
    .AddLogging(services => services.AddConsole().SetMinimumLevel(LogLevel.Information));


var kernel = builder.Build();
var chatCompletionService = kernel.GetRequiredService<IChatCompletionService>();

//kernel.Plugins.AddFromType<LightsPlugin>();
kernel.Plugins.AddFromType<SalariesPlugin>();

var openAiExecutionSettings = new OpenAIPromptExecutionSettings()
{
    FunctionChoiceBehavior = FunctionChoiceBehavior.Auto()
};

var history = new ChatHistory();

string? userInput = null;

do
{
    Console.Write("User > ");
    userInput = Console.ReadLine();

    if (string.IsNullOrWhiteSpace("userInput"))
    {
        continue;
    }

    history.AddUserMessage(userInput!);

    var result = await chatCompletionService.GetChatMessageContentAsync(
        history,
        executionSettings: openAiExecutionSettings,
        kernel: kernel);

    Console.WriteLine("Assistant > " + result.Content);
    
    history.AddAssistantMessage(result.Content ?? string.Empty);
} while (userInput is not null);
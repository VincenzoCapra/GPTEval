using Microsoft.Extensions.Configuration;
using OpenAI.Managers;
using OpenAI.ObjectModels.RequestModels;
using OpenAI.ObjectModels;
using OpenAI;
using System;
using System.Reflection;
using System.Collections.Generic;
using System.Threading.Tasks;

class Program
{
    static async Task Main()
    {
        var builder = new ConfigurationBuilder()
            .AddUserSecrets(Assembly.GetExecutingAssembly())
            .AddEnvironmentVariables();
        var configurationRoot = builder.Build();

        var key = configurationRoot.GetSection("OpenAIKey").Get<string>() ?? string.Empty;

        var openAiService = new OpenAIService(new OpenAiOptions()
        {
            ApiKey = key
        });

        await StartConversation(openAiService);
    }

    static async Task StartConversation(OpenAIService openAiService)
    {
        int recent = 0;

        List<string> prompts = new List<string> { "Name of the ai's character: ", "How old is the ai: ", "Description of the ai's character: ", " How does the ai speak: " };
        List<string> promptResponses = new();
        List<string> chatLogs = new List<string> { " Don't read this, its a placeholder. " };

        Console.WriteLine("############################################");
        Console.WriteLine("# Welcome to the simple character creator! #");
        Console.WriteLine("############################################");
        Console.WriteLine("");

        string answer = "";

        for (int i = 0; i < prompts.Count; i++)
        {
            Console.WriteLine(prompts[i]);
            Console.Write("> ");
            answer = Console.ReadLine() ?? String.Empty;
            promptResponses.Add(answer);
            Console.WriteLine("");
        }

        Console.WriteLine("");
        Console.WriteLine(string.Join(", ", promptResponses));

        Console.WriteLine("");
        Console.WriteLine("What year is it currently: ");
        Console.Write("> ");

        string rah = Console.ReadLine() ?? String.Empty;

        Console.WriteLine("");
        Console.WriteLine(string.Join("Year currently: ", rah));

        while (true)
        {
            var completionResult = openAiService.ChatCompletion.CreateCompletionAsStream(new ChatCompletionCreateRequest
            {
                Messages = new List<ChatMessage>
                {
                    new(StaticValues.ChatMessageRoles.System, " You will be acting like a character based on the traits that I give you. "),

                    new(StaticValues.ChatMessageRoles.System, $" Your name is: {promptResponses[0]}"),
                    new(StaticValues.ChatMessageRoles.System, $" You are this age: {promptResponses[1]}"),
                    new(StaticValues.ChatMessageRoles.System, $" This is the description of who you are: {promptResponses[1]}"),
                    new(StaticValues.ChatMessageRoles.System, $" This is how you should speak: {promptResponses[1]}"),

                    new(StaticValues.ChatMessageRoles.System, " Try not to say [ How does my character speak: ] or anything similar to that from the character prompts. "),

                    new(StaticValues.ChatMessageRoles.System, $" This is the year currently: {rah}"),

                    new(StaticValues.ChatMessageRoles.System, " When you write a new sentence, make sure to start it off with how you will speak instead of [ How does my character speak: ] "),

                    new(StaticValues.ChatMessageRoles.System, $" This is our chatLogs so far {chatLogs}"),
                    new(StaticValues.ChatMessageRoles.System, $" This is my most recent message {chatLogs[recent]}"),
                },
                Model = Models.Gpt_3_5_Turbo_16k,
                MaxTokens = 500
            });

            await foreach (var completion in completionResult)
            {
                if (completion.Successful)
                {
                    Console.Write(completion.Choices.First().Message.Content);
                }
                else
                {
                    if (completion.Error == null)
                    {
                        throw new Exception("Unknown Error");
                    }

                    Console.WriteLine($"{completion.Error.Code}: {completion.Error.Message}");
                }
            }

            Console.WriteLine("");
            Console.Write("> ");
            string exitCommand = Console.ReadLine()?.ToLower();
            if (exitCommand == "exit")
            {
                break;
            }
            else
            {
                if (chatLogs.Count > 10)
                {
                    chatLogs.RemoveAt(0);
                }

                chatLogs.Add(exitCommand);
                
                if (recent < 10);
                    recent++;
            }

            Console.WriteLine("");
            Console.WriteLine($"Most recent message: {chatLogs[recent]}");
            Console.WriteLine($"Whole chatLog: {chatLogs}");
            Console.WriteLine("");
        }
    }
}

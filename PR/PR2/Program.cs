using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using OpenAI.Chat; // библиотека OpenAI .NET SDK

class Audience
{
    public string Name { get; }
    public Audience(string name) => Name = name;

    public bool React()
    {
        return new Random().NextDouble() < 0.7;
    }

    public void ProcessMessage(IEnumerable<string> messages)
    {
        foreach (var msg in messages)
        {
            Thread.SpinWait(20000);
        }
    }
}

class Program
{
    static decimal P0 = 0.2m;
    static decimal P1 = 0.8m;
    static decimal currentP = P0;
    static object lockObj = new object();
    static Random rnd = new Random();

    static async Task Main()
    {
        string H = "H — событие, зависящее от реакции аудитории";

        var allAudience = new List<Audience>
        {
            new Audience("Студенты"),
            new Audience("Преподаватели"),
            new Audience("Исследователи"),
            new Audience("Журналисты"),
            new Audience("Широкая публика")
        };

        var filteredAudience = new List<Audience>();
        foreach (var a in allAudience)
        {
            if (a.Name.Length > 9)
            {
                filteredAudience.Add(a);
                Console.WriteLine($"✔ {a.Name} вошли в целевую группу");
            }
            else
            {
                Console.WriteLine($"✖ {a.Name} исключены из группы");
            }
        }

        var activeAudience = new List<Audience>();
        foreach (var a in filteredAudience)
        {
            if (a.React())
            {
                activeAudience.Add(a);
                Console.WriteLine($"  ↳ {a.Name} восприимчивы");
            }
            else
            {
                Console.WriteLine($"  ↳ {a.Name} игнорируют сообщения");
            }
        }

        List<string> dataset = await GenerateInitialDataset();

        TimeSpan timeout = TimeSpan.FromSeconds(3);

        Console.WriteLine($"\nСобытие: {H}");
        Console.WriteLine($"Начальная вероятность: {P0}, Целевая: {P1}");

        bool success = false;
        while (!success)
        {
            Console.WriteLine($"\n→ Отправка сообщений [{string.Join("; ", dataset)}]");

            var tasks = new List<Task>();
            foreach (var a in activeAudience)
            {
                tasks.Add(Task.Run(() => a.ProcessMessage(dataset)));
            }

            if (Task.WhenAll(tasks).Wait(timeout))
            {
                lock (lockObj)
                {
                    currentP += 0.2m;
                    if (currentP > P1) currentP = P1;
                }
                Console.WriteLine($"Текущая вероятность: {currentP:F2}");
            }
            else
            {
                string aiMsg = await GenerateAIMessage("Журналисты"); // пример для конкретной аудитории
                dataset.Add(aiMsg);
                Console.WriteLine($"⚠ Время истекло, AI сгенерировал новое сообщение: \"{aiMsg}\"");
            }

            success = currentP >= P1;
        }

        Console.WriteLine("\n✅ Событие H достигло целевой вероятности!");
    }

    // Генерация начального набора сообщений AI
    static async Task<List<string>> GenerateInitialDataset()
    {
        var dataset = new List<string>();
        dataset.Add(await GenerateAIMessage("Студенты"));
        dataset.Add(await GenerateAIMessage("Преподаватели"));
        dataset.Add(await GenerateAIMessage("Журналисты"));
        return dataset;
    }

    // Реальный вызов AI
    static async Task<string> GenerateAIMessage(string audience)
    {
        var client = new ChatClient(
            model: "gpt-4.1-mini",  // можно заменить на доступную модель
            apiKey: Environment.GetEnvironmentVariable("OPENAI_API_KEY")
        );

        var response = await client.CompleteChatAsync(new List<ChatMessage>
        {
            new ChatMessage(ChatRole.System, "Ты пишешь краткие агитационные сообщения."),
            new ChatMessage(ChatRole.User, $"Сгенерируй сообщение для аудитории: {audience}. Сообщение должно быть коротким и убедительным.")
        });

        return response.FirstChoice.Message.Content[0].Text.Trim();
    }
}

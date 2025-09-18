using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

class Audience
{
    public string Name { get; }
    public Audience(string name) => Name = name;

    // Имитация реакции аудитории
    public bool React()
    {
        return new Random().NextDouble() < 0.7;
    }

    public void ProcessMessage(IEnumerable<string> messages)
    {
        foreach (var msg in messages)
        {
            Thread.SpinWait(20000); // имитация восприятия информации
        }
    }
}

class Program
{
    static decimal P0 = 0.2m;  // начальная вероятность
    static decimal P1 = 0.8m;  // целевая вероятность
    static decimal currentP = P0;
    static object lockObj = new object();
    static Random rnd = new Random();

    static void Main()
    {
        string H = "H — событие, зависящее от реакции аудитории";

        // 1. Задаём аудиторию
        var allAudience = new List<Audience>
        {
            new Audience("Студенты"),
            new Audience("Преподаватели"),
            new Audience("Исследователи"),
            new Audience("Журналисты"),
            new Audience("Широкая публика")
        };

        // 2. Фильтрация аудитории по условию (имя длиннее 9 символов)
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

        // 3. Проверка восприимчивости
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

        // 4. Первичный набор сообщений от AI
        List<string> dataset = GenerateInitialDataset();

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
                string aiMsg = GenerateAIMessage();
                dataset.Add(aiMsg);
                Console.WriteLine($"⚠ Время истекло, AI сгенерировал новое сообщение: \"{aiMsg}\"");
            }

            success = currentP >= P1;
        }

        Console.WriteLine("\n✅ Событие H достигло целевой вероятности!");
    }

    // Генерация начального набора сообщений AI
    static List<string> GenerateInitialDataset()
    {
        return new List<string>
        {
            GenerateAIMessage(),
            GenerateAIMessage(),
            GenerateAIMessage()
        };
    }

    // Генерация одного сообщения AI
    static string GenerateAIMessage()
    {
        string[] templates =
        {
            "AI-анализ показал важность новых данных",
            "AI рекомендует обратить внимание на тенденцию",
            "AI сгенерировал прогноз роста интереса",
            "AI сообщает о подтверждении гипотезы",
            "AI усилил аргументы для аудитории"
        };
        return templates[rnd.Next(templates.Length)];
    }
}

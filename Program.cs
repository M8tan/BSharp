using System.Diagnostics;
using System.Dynamic;
using System.Text.Json;

using var cts = new CancellationTokenSource();
Stopwatch stopwatch = Stopwatch.StartNew();
string json = File.ReadAllText("config.json");
AppSettings settings = JsonSerializer.Deserialize<AppSettings>(json) ?? new AppSettings();
float delayInSeconds = (float)settings.delayInMilliseconds / 1000;
int delayInMinutes = (settings.delayInMilliseconds / 1000) / 60;
delayInSeconds -= delayInMinutes * 60;
string timeText = (delayInMinutes > 0) ? $"{delayInMinutes} minutes and {delayInSeconds} seconds" : $"{delayInSeconds} seconds";
Console.WriteLine("===== BSharp =====");
Console.WriteLine($"The app will make a sound every " + timeText);
int iritationCounter = 0;

Task inputTask = Task.Run(() =>
{
    while (!cts.IsCancellationRequested)
    {
        if(Console.ReadKey(true).Key == ConsoleKey.E)
        {
            cts.Cancel();
            break;
        }
    }
});

try
{
    while (!cts.Token.IsCancellationRequested)
    {
        iritationCounter++;
        await Task.Delay(settings.delayInMilliseconds, cts.Token); 
        if(iritationCounter > 0)
        {
            Console.WriteLine(iritationCounter);
            Console.Beep();
        }   
    }
} catch (OperationCanceledException)
{
    stopwatch.Stop();
    TimeSpan elapsed = stopwatch.Elapsed;
    Console.WriteLine($"Well done!\n{iritationCounter-1} iterations completed in {(int)elapsed.TotalHours} hours, {elapsed.Minutes} minutes and {elapsed.Seconds} seconds!");
    Thread.Sleep(5000);    
}

public class AppSettings
{
    public int delayInMilliseconds { get; set; } = 100;
}

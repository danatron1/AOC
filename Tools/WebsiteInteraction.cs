using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;


public static class WebsiteInteraction
{
    public const string website = "https://adventofcode.com";
    //Find your session cookie by going to adventofcode.com, pressing F12, going to the application tab, and copying the value of "session"
    static string? sessionCookieCached;
    public static string cookie
    {
        get
        {
            //cookie filepath stored separately - don't want my login cookie ending up on github!
            sessionCookieCached ??= File.ReadAllText(@"C:\Users\Danatron1\source\repos\AOCSessionCookie.txt");
            return sessionCookieCached;
        }
    }
    public static async Task<string[]> DownloadAOCInput(int year, int day, bool example = false)
    {
        string pathFull = $@"{Utility.folderPath}\Inputs\{year}";
        Directory.CreateDirectory(pathFull);
        pathFull += $@"\{Day.NameFor(year, day)}_{(example ? "Example" : "Input")}.txt";
        if (example)
        {
            if (!File.Exists(pathFull))
            {
                Console.WriteLine($"First time using example inputs for {Day.NameFor(year, day)}.\nCreated file at {pathFull}\nPaste example inputs here, leaving a blank line when done:");
                List<string> examples = new();
                while (true)
                {
                    string? testInput = Console.ReadLine();
                    if (string.IsNullOrWhiteSpace(testInput)) break;
                    examples.Add(testInput);
                }
                File.WriteAllLines(pathFull, examples.ToArray());
                return examples.ToArray();
            }
            return File.ReadAllLines(pathFull);
        }
        if (!File.Exists(pathFull))
        {
            Console.WriteLine($"Input file not downloaded for {Day.NameFor(year, day)}, fetching.");
            string result = await GetContent($"/{year}/day/{day}/input");
            await File.WriteAllTextAsync(pathFull, result);
        }
        return File.ReadAllLines(pathFull);
    }
    internal static async Task<string> SubmitAOCAnswer(string answer, int year, int day, int part)
    {
        return await GetContent($"/{year}/day/{day}/answer",
            new("level", part.ToString()),
            new("answer", answer));
    }
    internal static async Task<string> GetContent(string sitePath, params KeyValuePair<string, string>[] content)
    {
        Console.WriteLine($"Connecting to {website}{sitePath}...");
        FormUrlEncodedContent contentEncoded = new FormUrlEncodedContent(content);
        Uri uri = new Uri(website); //Uniform resource identifier
        CookieContainer cookies = new CookieContainer();
        cookies.Add(uri, new Cookie("session", cookie)); //login info
        using var handler = new HttpClientHandler() { CookieContainer = cookies };
        using var client = new HttpClient(handler) { BaseAddress = uri };
        using var response = await client.PostAsync(sitePath, contentEncoded);
        return response.Content.ReadAsStringAsync().Result;
    }
}

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;


public static class WebsiteInteraction
{
    public const string website = "https://adventofcode.com";
    public const string myRepo = "https://github.com/danatron1/AOC";

    public static string userAgentString = $".NET/{Environment.Version} (+via {myRepo} by {myEmail})";

    static string? myEmailCached;
    public static string myEmail
    {
        get
        {
            myEmailCached ??= File.ReadAllText(@"C:\Users\Danatron1\source\repos\AOCAgentEmail.txt");
            return myEmailCached;
        }
    }
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
    public static async Task<string[]> DownloadAOCInput<T>(DayBase<T> d)
    {
        string pathFull = $@"{Utility.folderPath}\Inputs\{d.Year}";
        Directory.CreateDirectory(pathFull);
        pathFull += $@"\{d.Name}_{(d.useExampleInput ? "Example" : "Input")}.txt";
        if (d.useExampleInput)
        {
            if (!File.Exists(pathFull))
            {
                Console.WriteLine($"First time using example inputs for {d}.\nCreated file at {pathFull}");
                return Utility.GetUserInputs(pathFull);
            }
            return File.ReadAllLines(pathFull);
        }
        if (!File.Exists(pathFull))
        {
            Console.WriteLine($"Input file not downloaded for {d}, fetching.");
            string result = await GetContent($"/{d.Year}/day/{d.Day}/input");
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
        client.DefaultRequestHeaders.UserAgent.ParseAdd(userAgentString);
        using var response = await client.PostAsync(sitePath, contentEncoded);
        return response.Content.ReadAsStringAsync().Result;
    }
}

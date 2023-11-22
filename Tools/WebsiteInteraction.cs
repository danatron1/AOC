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
    public const string sessionInfoFilepath = @$"{Utility.folderPath}\sessionInfo.txt";
    //Find your session cookie by going to adventofcode.com, pressing F12, going to the application tab, and copying the value of "session"
    public static string Cookie => SessionInfo["cookie"];
    public static string Email => SessionInfo["email"];
    public static string Repo => SessionInfo.TryGetValue("repo", out string? r) ? r : "unpublished code";

    static Dictionary<string, string>? sessionInfoCached;
    public static Dictionary<string, string> SessionInfo
    {
        get
        {
            sessionInfoCached ??= GetSessionInfo();
            return sessionInfoCached;
        }
    }
    public static string UserAgent = $".NET/{Environment.Version} (+via {Repo} by {Email})";
    static Dictionary<string, string> GetSessionInfo()
    {
        string[] MANDATORY = { "email", "cookie" };
        Dictionary<string, string> info = new();
        string[] lines = File.ReadAllLines(sessionInfoFilepath);
        foreach (string line in lines)
        {
            if (string.IsNullOrEmpty(line) || line[0] == '#') continue;
            int index = line.IndexOf(':');
            info.Add(line[..index].ToLower(), line[(index+1)..].Trim());
        }
        foreach (string line in MANDATORY)
        {
            if (!info.ContainsKey(line)) throw new Exception($"Missing mandatory field {line} in session info file: {sessionInfoFilepath}");
        }
        return info;
    }
    public static async Task<string[]> DownloadAOCInput<T>(DayBase<T> d)
    {
        string pathFull = $@"{Utility.folderPath}\Inputs\{d.Year}";
        string result = await GetContent($"/{d.Year}/day/{d.Day}/input");
        await File.WriteAllTextAsync(pathFull, result);
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
        cookies.Add(uri, new Cookie("session", Cookie)); //login info
        using var handler = new HttpClientHandler() { CookieContainer = cookies };
        using var client = new HttpClient(handler) { BaseAddress = uri };
        client.DefaultRequestHeaders.UserAgent.ParseAdd(UserAgent);
        using var response = await client.PostAsync(sitePath, contentEncoded);
        return response.Content.ReadAsStringAsync().Result;
    }
}

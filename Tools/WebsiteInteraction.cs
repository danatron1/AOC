using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace AOC.Tools
{
    public static class WebsiteInteraction
    {
        public const string website = "https://adventofcode.com";
        //Find your session cookie by going to adventofcode.com, pressing F12, going to the application tab, and copying the value of "session"
        static string? sessionCookieCached;
        public static string cookie
        {
            get
            {
                sessionCookieCached ??= File.ReadAllText(@"C:\Users\Danatron1\source\repos\AOCSessionCookie.txt");
                return sessionCookieCached;
            }
        }
        public static async Task<string[]> DownloadAOCInput(int day, int year, bool example = false)
        {
            string pathFull = $@"{Utility.folderPath}\Inputs\{year}";
            Directory.CreateDirectory(pathFull);
            pathFull += $@"\{Utility.NameFor(day, year)}_{(example ? "Example" : "Input")}.txt";
            if (example)
            {
                if (!File.Exists(pathFull))
                {
                    Console.WriteLine($"First time using example inputs for {Utility.NameFor(day, year)}.\nCreated file at {pathFull}\nPaste example inputs here, leaving a blank line when done:");
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
                using var stream = await GetContent($"/{year}/day/{day}/answer").Result.ReadAsStreamAsync();
                await stream.CopyToAsync(pathFull);
                Uri uri = new Uri(website);
                CookieContainer cookies = new CookieContainer();
                cookies.Add(uri, new Cookie("session", cookie));
                using var file = new FileStream(pathFull, FileMode.Create, FileAccess.Write, FileShare.None);
                using var handler = new HttpClientHandler() { CookieContainer = cookies };
                using var client = new HttpClient(handler) { BaseAddress = uri };
                using var response = await client.GetAsync($"/{year}/day/{day}/input");
                using var stream = await response.Content.ReadAsStreamAsync();
                await stream.CopyToAsync(file);
            }
            return File.ReadAllLines(pathFull);
        }
        internal static async Task<string> SubmitAOCAnswer(string answer, int day, int year, int part)
        {
            return await GetContent($"/{year}/day/{day}/answer", 
                new("level", part.ToString()),
                new("answer", answer)).Result.ReadAsStringAsync();
        }
        internal static async Task<HttpContent> GetContent(string sitePath, params KeyValuePair<string, string>[] content)
        {
            FormUrlEncodedContent contentEncoded = new FormUrlEncodedContent(content);
            using var response = await GetClient().PostAsync(sitePath, contentEncoded);
            return response.Content;
        }
        internal static HttpClient GetClient()
        {
            Uri uri = new Uri(website); //Uniform resource identifier
            CookieContainer cookies = new CookieContainer();
            cookies.Add(uri, new Cookie("session", cookie)); //login info
            using var handler = new HttpClientHandler() { CookieContainer = cookies };
            return new HttpClient(handler) { BaseAddress = uri };
        }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

public static class Utility
{
    //Find your session cookie by pressing F12, going to the application tab, and copying the value of "session"
    const string cookie = "53616c7465645f5f587fc9b6476d3b82a520ed5e35b7b2d96cffd48a390c5b9a2bb29d891e8645c5ccfb51ff13c56203";

    public const string folderPath = @"F:\Documents\AdventOfCode";
    public const string website = "https://adventofcode.com";
    public static async Task<string[]> DownloadAOCInput(int day, int year, bool example = false)
    {
        string pathFull = $@"{folderPath}\{year}";
        Directory.CreateDirectory(pathFull);
        if (example)
        {
            pathFull += $@"\E{day}.txt";
            if (!File.Exists(pathFull))
            {
                File.WriteAllText(pathFull, "paste example here");
            }
            return File.ReadAllLines(pathFull);
        }
        else pathFull += $@"\D{day}.txt";
        if (!File.Exists(pathFull))
        {
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
    public static void Copy(object item)
    {
        if (item == null) return;
        Cmd($"echo | set /p={item}| clip");
    }
    public static string Cmd(string command)
    {
        command = command.Replace("\"", "\\\"");
        return Run("cmd.exe", $"/c \"{command}\"");
    }
    public static string Run(string filename, string arguments = "")
    {
        Process process = new Process()
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = filename,
                Arguments = arguments,
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = false
            }
        };
        process.Start();
        string result = process.StandardOutput.ReadToEnd();
        process.WaitForExit();
        return result;
    }
    public static int FrequencyOf(this string whole, char section) => whole.FrequencyOf(section.ToString());
    public static int FrequencyOf(this string whole, string section)
    {
        return (whole.Length - whole.Replace(section, "").Length) / section.Length;
    }
    public static T[] Split<T>(this string line, params char[]? seperators)
    {
        if (seperators is null || seperators.Length == 0) seperators = new char[] { ',' };
        string[] normalSplit = line.Split(seperators, StringSplitOptions.TrimEntries);
        return normalSplit.ConvertTo<T>();

    }
    public static T[] ConvertTo<T>(this object[] array, bool forceMatchingLength = false)
    {
        List<T> converted = new List<T>();
        TypeConverter converter = TypeDescriptor.GetConverter(typeof(T));
        for (int i = 0; i < array.Length; i++)
        {
            object? ob = converter.ConvertFrom(array[i]);
            if (ob is T t) converted.Add(t);
            else if (forceMatchingLength) converted.Add(default);
        }
        return converted.ToArray();
    }
    public static Array ConvertTo<T>(this Array array, bool forceMatchingLength = false)
    {
        int[] limits = new int[array.Rank];
        int[] lower = new int[array.Rank];
        for (int l = 0; l < limits.Length; l++)
        {
            limits[l] = array.GetLength(l);
            lower[l] = array.GetLowerBound(l);
        }
        TypeConverter converter = TypeDescriptor.GetConverter(typeof(T));
        Array converted = Array.CreateInstance(typeof(T), limits, lower);
        int i = 0;
        int[] indices = new int[limits.Length];
        foreach (object item in array)
        {
            int size = i++;
            for (int pos = indices.Length - 1; pos >= 0; pos--)
            {
                indices[pos] = size % limits[pos];
                size /= limits[pos];
            }
            object? ob = converter.ConvertFrom(item);
            if (ob is T t) converted.SetValue(t, indices);
            else if (forceMatchingLength) converted.SetValue(default, indices);
        }
        return converted;
    }
}

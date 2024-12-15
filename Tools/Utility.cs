using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

public static class Utility
{
    public const string folderPath = @"C:\Users\Danatron1\source\repos\AOC";
    public static void Print(this object o) => Console.WriteLine(o);
    public static void Copy(this object item)
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

    //Old GetParse method - kept just in case.
    public static Func<string, T> GetParse<T>()
    {
        var methodInfo = typeof(T).GetMethod("Parse", BindingFlags.Public | BindingFlags.Static, new[] { typeof(string) });
        if (methodInfo == null || methodInfo.ReturnType != typeof(T))
        {
            throw new Exception(Canned.Exceptions.TypeHasNoParseMethod);
        }
        return (Func<string, T>)methodInfo.CreateDelegate(Expression.GetFuncType(typeof(string), typeof(T)));
    }   /**/
    public static void RefSwap<T>(ref T a, ref T b)
    {
        (a, b) = (b, a);
    }
    public static void RepeatAction(this Action action, int count)
    {
        for (int i = 0; i < count; i++) action();
    }
    public static T RepeatAction<T>(this T t, Func<T, T> func, int amount)
    {
        for (int i = 0; i < amount; i++) t = func(t);
        return t;
    }
    public static void Wait(long seconds)
    {
        Console.Write($"Waiting {seconds} seconds");
        while (seconds > 0)
        {
            Thread.Sleep(1000);
            Console.Write('.');
            seconds--;
            if (seconds % 10 == 0 && seconds > 0) Console.Write($"\n{seconds}");
        }
        Console.WriteLine($"\nWait complete");
    }
    public static string[] GetMultipleInputs(string? saveToFilePath = null)
    {
        Console.WriteLine(Canned.Messages.GetUserInput);
        List<string> inputs = new();
        while (true)
        {
            string? testInput = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(testInput)) break;
            inputs.Add(testInput);
        }
        if (saveToFilePath != null) File.WriteAllLines(saveToFilePath, inputs.ToArray());
        return inputs.ToArray();
    }
    public static bool UserInputConfirm()
    {
        return UserInputCharacter("yes", "no") == 'y';
    }
    public static char? UserInputCharacter(params string[] options)
    {
        string optionList = string.Join(" / ", options.Select(s => $"*{s[0]}*{s[1..]}")); //emphasise first letter
        Emphasis($"OPTIONS: ( {optionList} )", ConsoleColor.Cyan);
        string? response = Console.ReadLine()?.ToLower();
        return response?[0] ?? null;
    }
    public static string AsTime(this TimeSpan t)
    { 
        //I just want it to display more precise millisecond times as your time gets lower
        if (t.TotalSeconds < 1) return $"{t:s'.'FFFFFF}s";
        if (t.TotalSeconds < 10) return $"{t:s'.'FFFF}s";
        if (t.TotalMinutes < 1) return $"{t:s'.'FF}s";
        if (t.TotalHours < 1) return $"{t:m':'ss}";
        return $"{t:h':'mm':'ss}";
    }
    public static IEnumerable<long> CountForever(long startingAt = 0, long stepsOf = 1)
    {
        yield return startingAt;
        while (true) yield return startingAt += stepsOf;
    }
    /// <summary>
    /// Prints a message to have text surrounded with *asterisks* emphasised
    /// </summary>
    /// <param name="message"></param>
    public static void Emphasis(string? message, ConsoleColor emphasisColour = ConsoleColor.White)
    {
        if (message is null) return;
        bool bold = message[0] == '*';
        foreach (string line in message.Split('*'))
        {
            Console.ForegroundColor = bold ? emphasisColour : ConsoleColor.Gray;
            Console.Write(line);
            bold = !bold;
        }
        Console.ForegroundColor = ConsoleColor.Gray;
        Console.WriteLine(); 
    }
}

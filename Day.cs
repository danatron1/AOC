using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using System.Windows;
public abstract class Day
{
    public abstract void Solve();
    public static void Solve(int year, int day)
    {
        Type? type = Type.GetType($"AOC.D{day}_{year}");
        if (type == null) return;
        object? obj = Activator.CreateInstance(type);
        MethodInfo? methodInfo = type.GetMethod("Solve");
        if (obj == null || methodInfo == null) return;
        methodInfo.Invoke(obj, null);
        Console.ReadLine();
    }
    public abstract void PartA();
    public abstract void PartB();
    public virtual void Copy(object item)
    {
        Console.WriteLine(item);
        Utility.Copy(item);
    }
    public virtual string[] GetInputForDay(int day = -1, int year = -1, bool example = false)
    {
        if (day == -1)
        {
            string[] caller = NameOfCallingClass("Day").Split('_');
            while (!char.IsDigit(caller[0][0])) caller[0] = caller[0][1..];
            day = int.Parse(caller[0]);
            year = int.Parse(caller[1]);
        }
        if (year == -1) year = DateTime.Now.Year;
        return Utility.DownloadAOCInput(day, year, example).Result;
    }
    public virtual T[] GetInputForDay<T>(int day = -1, int year = -1, bool example = false) => GetInputForDay(day, year).ConvertTo<T>();
    public virtual string[,] GetInputForDay2D(string split = "", int day = -1, int year = -1)
    {
        string[] lines = GetInputForDay();
        int max = lines.Max(l => l.Length);
        string[,] result = new string[max, lines.Length];
        string[] cells;
        for (int y = 0; y < lines.Length; y++)
        {
            if (split == "")
            {
                cells = new string[lines[y].Length];
                for (int i = 0; i < lines[y].Length; i++) cells[i] = lines[y][i].ToString();
            }
            else cells = lines[y].Split(split);
            for (int x = 0; x < max; x++)
            {
                if (x < cells.Length) result[x, y] = cells[x];
                else result[x, y] = "";
            }
        }
        return result;
    }
    public virtual T[,] GetInputForDay2D<T>(string split = "", int day = -1, int year = -1)
    {
        return (T[,])GetInputForDay2D(split, day, year).ConvertTo<T>();
    }
    public static string NameOfCallingClass(params string[] ignore)
    {
        string fullName;
        Type declaringType;
        int skipFrames = 2;
        do
        {
            MethodBase method = new StackFrame(skipFrames, false).GetMethod();
            declaringType = method.DeclaringType;
            if (declaringType == null)
            {
                return method.Name;
            }
            skipFrames++;
            fullName = declaringType.FullName;
        }
        while (declaringType.Module.Name.Equals("mscorlib.dll", StringComparison.OrdinalIgnoreCase) || ignore.Contains(fullName));
        return fullName;
    }
}

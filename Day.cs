using Newtonsoft.Json.Linq;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Windows;
public abstract class Day
{
    
    public virtual void Solve()
    {
        try
        {
            PartA();
        }
        catch (NotImplementedException)
        {
            Console.WriteLine($"Part A not yet solved.");
        }
        try 
        { 
            PartB(); 
        }
        catch (NotImplementedException)
        {
            Console.WriteLine($"Part B not yet solved.");
        }
    }
    public static void Solve(int year, int day)
    {
        Type? type = Type.GetType($"AOC.D{day}_{year}");
        type ??= Type.GetType($"AOC.D{day.ToString().PadLeft(2, '0')}_{year}");
        if (type == null)
        {
            Create(year, day);
            return;
        }
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
    private int thisDay = -1;
    private int thisYear = -1;
    private void CacheThisYearAndDay()
    {
        if (thisDay > 0 && thisYear > 0) return;
        string[] caller = NameOfCallingClass("Day").Split('_');
        while (!char.IsDigit(caller[0][0])) caller[0] = caller[0][1..];
        thisDay = int.Parse(caller[0]);
        thisYear = int.Parse(caller[1]);
    }
    private int GetDay()
    {
        CacheThisYearAndDay();
        return thisDay;
    }
    private int GetYear()
    {
        CacheThisYearAndDay();
        return thisYear;
    }
    public virtual string GetInputForDayPath(int day = -1, int year = -1, bool example = false)
    {
        if (day == -1) day = GetDay();
        if (year == -1) year = GetYear();
        _ = Utility.DownloadAOCInput(day, year, example).Result;
        return Utility.InputFilepath(day, year, example);
    }
    public virtual JToken GetInputForDayJson(int day = -1, int year = -1, bool example = false)
    {
        return JToken.Parse(string.Join("", GetInputForDay(day, year, example)));
    }
    public virtual string[] GetInputForDay(int day = -1, int year = -1, bool example = false)
    {
        if (day == -1) day = GetDay();
        if (year == -1) year = GetYear();
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

    internal static void Create(int year, int day)
    {
        string repo = @$"C:\Users\Danatron1\source\repos\AOC\{year}";
        string path = $"{repo}\\{Utility.NameFor(day, year)}.cs";
        if (!Directory.Exists(repo))
        {
            Directory.CreateDirectory(repo);
            Console.WriteLine($"Created new folder for year {year}");
        }
        if (File.Exists(path)) return;
        string file = File.ReadAllText(@"C:\Users\Danatron1\source\repos\AOC\DayTemplate.txt");
        file = file.Replace("DAYNUMBER", day.ToString().PadLeft(2, '0'));
        file = file.Replace("YEARNUMBER", year.ToString());
        File.WriteAllText(path, file);
        Console.WriteLine($"Created day solution {Utility.NameFor(day, year)}");
    }
    internal static void CreateYear(int year)
    {
        for (int i = 1; i <= 25; i++)
        {
            Create(year, i);
        }
    }
    internal virtual string Submit(object answer, int day = -1, int year = -1, int part = -1)
    {
        if (day == -1) day = GetDay();
        if (year == -1) year = GetYear();
        if (part == -1) part = GetPart();
        string result = Submit(answer.ToString() ?? string.Empty, day, year, part);
        Console.WriteLine($"result: {result}\n");
        return result;
    }
    private const int secondsBetweenSubmissions = 60;
    private string RangeString(long? minimum, long? maximum)
    {
        string rangeMessage = "";
        if (minimum.HasValue && maximum.HasValue) rangeMessage = $"between {minimum} and {maximum}";
        else if (minimum.HasValue) rangeMessage = $"above {minimum}";
        else if (maximum.HasValue) rangeMessage = $"below {maximum}";
        return rangeMessage;
    }
    internal virtual string Submit(string answer, int day, int year, int part)
    {
        if (string.IsNullOrWhiteSpace(answer)) return "Invalid answer - cannot be null";
        #region Load log
        string logPath = @$"C:\Users\Danatron1\source\repos\AOC\Logs\{year}";
        string path = $"{logPath}\\{Utility.NameFor(day, year)}_LOG.txt";
        if (!Directory.Exists(logPath))
        {
            Directory.CreateDirectory(logPath);
            Console.WriteLine($"Created logging folder for year {year}");
        }
        string[] logs = new string[0];
        if (File.Exists(path)) logs = File.ReadAllLines(path);
        #endregion
        //logging format:
        //unixTimestamp,submission,result,part
        #region Check log
        long? minimum = null, maximum = null;
        long newLimit;
        string[]? lastLog = null;
        foreach (string log in logs)
        {
            lastLog = log.Split(',');
            if (lastLog[3] != part.ToString()) continue;
            //don't submit if you've already completed the puzzle
            if (lastLog[2] == "Correct")
            {
                Console.WriteLine($"You've previously solved this puzzle! The correct answer was {lastLog[1]}.");
                if (lastLog[1] == answer) Console.WriteLine("Well done on getting it right again :)");
                else Console.WriteLine($"Unfortunately... this means {answer} is not correct");
                return "Already solved";
            }
            //don't submit if you've already submitted this answer
            if (lastLog[1] == answer && lastLog[2] != "Undetermined")
            {
                Console.WriteLine($"You previously submitted an answer of {answer}, and got the response {lastLog[2]}.");
                return "Duplicate answer";
            }
            //calculate range
            if (lastLog[2] == "Incorrect low" && long.TryParse(lastLog[1], out newLimit) && (!minimum.HasValue || newLimit > minimum))
            {
                minimum = newLimit;
            }
            else if (lastLog[2] == "Incorrect high" && long.TryParse(lastLog[1], out newLimit) && (!maximum.HasValue || newLimit < maximum))
            {
                maximum = newLimit;
            }
            
        }
        //don't submit answers that are out of range
        if (minimum.HasValue && long.Parse(answer) < minimum)
        {
            Console.WriteLine($"Didn't submit {answer} because we know it's {RangeString(minimum, maximum)}");
            return "Too low";
        }
        if (maximum.HasValue && long.Parse(answer) > maximum)
        {
            Console.WriteLine($"Didn't submit {answer} because we know it's {RangeString(minimum, maximum)}");
            return "Too high";
        }
        //limit submissions to 1 every 1 minutes;
        long timeSinceLastSubmission = long.MaxValue;
        if (lastLog != null && lastLog[3] == part.ToString())
        {
            long lastSubmission = long.Parse(lastLog[0]);
            timeSinceLastSubmission = DateTimeOffset.Now.ToUnixTimeSeconds() - lastSubmission;
        }
        if (timeSinceLastSubmission < secondsBetweenSubmissions)
        {
            long timeToNextSubmission = secondsBetweenSubmissions - timeSinceLastSubmission;
            Console.WriteLine($"It's been less than {secondsBetweenSubmissions} seconds since your last submission.");
            Console.Write($"Waiting {timeToNextSubmission} seconds");
            for (int i = 0; i < timeToNextSubmission; i++)
            {
                Thread.Sleep(1000);
                Console.Write('.');
            }
            Console.WriteLine($"\nWait complete");
        }
        long now = DateTimeOffset.Now.ToUnixTimeSeconds();
        #endregion
        string result = Utility.SubmitAOCAnswer(answer, day, year, part).Result;
        #region Feedback messages
        if (result.Contains("That's the right answer!"))
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("CORRECT!");
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.Write($"{answer} is the correct answer! You've earned 1 ");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write("gold star");
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine("!");
            Console.Write($"{year} day {day}, part {part} is now complete. ");
            if (part == 1) Console.WriteLine("Time to move onto part 2!");
            else Console.WriteLine("Time for the next day!");
            result = "Correct";
        }
        else if (result.Contains("That's not the right answer"))
        {
            Console.WriteLine("That is not the right answer;");
            string lowhigh = "";
            if (result.Contains("answer is too low"))
            {
                lowhigh = "low";
                minimum = long.Parse(answer);
            }
            else if (result.Contains("answer is too high"))
            {
                lowhigh = "high";
                maximum = long.Parse(answer);
            }

            if (lowhigh.Length == 0 && (minimum.HasValue || maximum.HasValue))
            {
                Console.WriteLine("You're no longer being told whether your answer is too high or low.");
                Console.WriteLine($"We know the answer is {RangeString(minimum, maximum)}");
                result = "Incorrect";
            }
            else if (lowhigh.Length > 0)
            {
                Console.Write($"Your answer of {answer} is too ");
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write(lowhigh);
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.WriteLine($". Please wait {secondsBetweenSubmissions} seconds before trying again.");
                Console.WriteLine($"We now know the answer is {RangeString(minimum, maximum)}.");
                result = $"Incorrect {lowhigh}";
            }
            else
            {
                Console.WriteLine($"Please wait {secondsBetweenSubmissions} seconds before trying again.");
                result = "Incorrect";
            }
        }
        else if (result.Contains("You don't seem to be solving the right level"))
        {
            Console.WriteLine("You don't seem to be solving the right level.  Did you already complete it?");
            Console.WriteLine($"It appears you've already solved {year} day {day}, part {part}.");
            result = "Already solved";
        }
        else if (result.Contains("You gave an answer too recently"))
        {
            Console.WriteLine("Answer was submitted, but another was submitted too recently.");
            if (lastLog != null) 
            {
                long lastSubmission = long.Parse(lastLog[0]);
                timeSinceLastSubmission = DateTimeOffset.Now.ToUnixTimeSeconds() - lastSubmission;
                Console.WriteLine($"The log says the last submission was {timeSinceLastSubmission} seconds ago.");
            }
            Console.WriteLine("Advent of Code says: " + Regex.Match(result, "You have \\d+s left to wait."));
            result = "Undetermined";
        }
        else
        {
            string errorPath = $"{logPath}\\D{day.ToString().PadLeft(2, '0')}_ERROR_{now}.txt";
            Console.WriteLine($"Couldn't tell if {answer} was right or wrong. Error log created at:\n{errorPath}");
            File.WriteAllText(errorPath, result);
            result = "Undetermined";
        }
        #endregion
        File.AppendAllText(path, $"{now},{answer},{result},{part}\n");
        return result;
    }
    
    internal int GetPart()
    {
        StackFrame[] frames = new StackTrace().GetFrames();
        foreach (StackFrame frame in frames)
        {
            if (frame.GetMethod().Name == "PartA") return 1;
            if (frame.GetMethod().Name == "PartB") return 2;
        }
        throw new Exception("Could not determine calling part");
        return -1;
    }
}

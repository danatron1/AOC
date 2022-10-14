using Newtonsoft.Json.Linq;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Windows;

public abstract class DayBase<InputType>
{
    public int Day { get; private set; }
    public int Year { get; private set; }
    public string Name { get; private set; }
    protected bool useExampleInput = false;
    public long? Minimum => GetPart() == 1 ? MinimumA : MinimumB;
    public long? Maximum => GetPart() == 1 ? MaximumA : MaximumB;
    private long? MinimumA, MinimumB, MaximumA, MaximumB;
    public override string ToString() => Name;
    public DayBase()
    {
        string[] caller = NameOfCallingClass("Day").Split('_', '.');
        Year = int.Parse(caller[^1]);
        Day = int.Parse(caller[^2][1..]);
        Name = NameFor(Year, Day);
        CacheAnswerLimits(); //min and max
        Directory.CreateDirectory($@"{Utility.folderPath}\Inputs\{Year}");
    }
    private string[]? _inputRaw;
    public string[] InputRaw
    {
        get
        {
            _inputRaw ??= GetInputForDay();
            return _inputRaw;
        }
    }
    private InputType[]? _input;
    public InputType[] Input
    {
        get
        {
            _input ??= GetInputForDay<InputType>();
            return _input;
        }
    }
    private InputType[,]? _input2D;
    public InputType[,] Input2D
    {
        get
        {
            _input2D ??= GetInputForDay2D<InputType>();
            return _input2D;
        }
    }
    public InputType InputLine => Input[0];
    private string _inputPath => $@"{Utility.folderPath}\Inputs\{Year}\{Name}_{(useExampleInput ? "Example" : "Input")}.txt";
    public string InputPath
    {
        get
        {
            if (!File.Exists(_inputPath)) GetInputForDay();
            return _inputPath;
        }
    }
    private void CacheAnswerLimits()
    {
        string path = @$"{Utility.folderPath}\Logs\{Year}\{Name}_LOG.txt";
        string[] lastLog, logs = new string[0];
        if (File.Exists(path)) logs = File.ReadAllLines(path);
        foreach (string log in logs)
        {
            lastLog = log.Split(',');
            if (!long.TryParse(lastLog[1], out long givenAnswer)) continue;
            bool partA = lastLog[3] == "1";
            if (lastLog[2] == "Correct")
            {
                if (partA)
                {
                    MinimumA = givenAnswer;
                    MaximumA = givenAnswer;
                }
                else
                {
                    MinimumB = givenAnswer;
                    MaximumB = givenAnswer;
                }
                continue;

            }
            if (lastLog[2] == "Incorrect low")
            {
                if (partA && (!MinimumA.HasValue || givenAnswer > MinimumA)) MinimumA = givenAnswer;
                else if (!partA && (!MinimumB.HasValue || givenAnswer > MinimumB)) MinimumB = givenAnswer;
            }
            else if (lastLog[2] == "Incorrect high")
            {
                if (partA && (!MaximumA.HasValue || givenAnswer < MaximumA)) MaximumA = givenAnswer;
                else if (!partA && (!MaximumB.HasValue || givenAnswer < MaximumB)) MaximumB = givenAnswer;
            }
        }
    }
    public virtual void Solve()
    {
        try
        {
            PartASetup();
            PartA();
        }
        catch (NotImplementedException)
        {
            Console.WriteLine($"Part A not yet solved.");
        }
        _input = null;
        useExampleInput = false;
        try
        {
            PartBSetup();
            PartB();
        }
        catch (NotImplementedException)
        {
            Console.WriteLine($"Part B not yet solved.");
        }
    }
    public virtual void PartASetup() { }
    public virtual void PartBSetup() { }
    public static void Solve(int year, int day, bool trackPerformance = false)
    {
        Type? type = Type.GetType($"AOC.Y{year}.{NameFor(year, day)}");
        type ??= Type.GetType($"AOC.{NameFor(year, day)}");
        type ??= Type.GetType($"AOC.D{day}_{year}");
        if (type == null)
        {
            Create(year, day);
            return;
        }
        object? obj = Activator.CreateInstance(type);
        MethodInfo? methodInfo = type.GetMethod("Solve");
        if (obj == null || methodInfo == null) return;
        if (trackPerformance)
        {
            Stopwatch sw = Stopwatch.StartNew();
            methodInfo.Invoke(obj, null);
            sw.Stop();
            Console.WriteLine($"Completed both parts in {sw.ElapsedMilliseconds} milliseconds\n");
        }
        else methodInfo.Invoke(obj, null);
    }
    public abstract void PartA();
    public abstract void PartB();
    public virtual void Copy(object item)
    {
        Console.WriteLine($"{item} (copied to clipboard)");
        Utility.Copy(item);
    }
    public static string[] GetInputForAnyDay(int year, int day, bool example)
    {
        return WebsiteInteraction.DownloadAOCInput(year, day, example).Result;
    }
    public virtual string[] GetInputForDay() => GetInputForAnyDay(Year, Day, useExampleInput);
    public virtual T[] GetInputForDay<T>() => GetInputForDay().ConvertTo<T>();
    public virtual string[,] GetInputForDay2D(string split = "")
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
    public virtual T[,] GetInputForDay2D<T>(string split = "") => GetInputForDay2D(split).ConvertTo<T>();
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
            fullName = declaringType.FullName.Split('`')[0];
        }
        while (declaringType.Module.Name.Equals("mscorlib.dll", StringComparison.OrdinalIgnoreCase) || ignore.Contains(fullName));
        return fullName;
    }
    public static string NameFor(int year, int day)
    {
        if (year < 2000 || day > 31) throw new ArgumentException($"Invalid; year {year}, day {day}");
        return $"D{day.ToString().PadLeft(2, '0')}_{year}";
    }
    internal static void Create(int year, int day)
    {
        string repo = @$"C:\Users\Danatron1\source\repos\AOC\{year}";
        string path = $"{repo}\\{NameFor(year, day)}.cs";
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
        Console.WriteLine($"Created day solution {NameFor(year, day)}");
    }
    internal static void CreateYear(int year)
    {
        for (int i = 1; i <= 25; i++)
        {
            Create(year, i);
        }
    }
    internal virtual string Submit(object answer)
    {
        int part = GetPart();
        string result = Submit(answer.ToString(), this, part);
        Console.WriteLine($"(Puzzle {Name}, Part{(part==1?"A":"B")}) result: {result}\n");
        return result;
    }
    private const int secondsBetweenSubmissions = 60;
    internal static string Submit<T>(string? answer, DayBase<T> day, int part)
    {
        if (string.IsNullOrWhiteSpace(answer)) return "Cannot answer null";
        Console.Write($"You answered; ");
        Console.ForegroundColor = ConsoleColor.White;
        Console.WriteLine(answer);
        Console.ForegroundColor = ConsoleColor.Gray;
        if (answer == "0" || answer == "-1") //double check common fail values
        {
            Console.WriteLine("Are you sure you want to submit this answer? (y/n)");
            if ((Console.ReadLine()?.ToLower()[0]) != 'y') return "Declined to submit";
        }
        long? minimum = null, maximum = null;
        #region Load log
        string logPath = @$"{Utility.folderPath}\Logs\{day.Year}";
        string path = $"{logPath}\\{day}_LOG.txt";
        if (!Directory.Exists(logPath))
        {
            Directory.CreateDirectory(logPath);
            Console.WriteLine($"Created logging folder for year {day.Year}");
        }
        string[] logs = new string[0];
        if (File.Exists(path)) logs = File.ReadAllLines(path);
        #endregion
        //logging format:
        //unixTimestamp,submission,result,part
        #region Check log
        long newLimit;
        string[]? lastLog = null;
        string? duplicate = null;
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
            if (lastLog[1] == answer && lastLog[2] != "Undetermined" && !lastLog[2].StartsWith("Too soon"))
            {
                duplicate = lastLog[2];
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
        if (!string.IsNullOrWhiteSpace(duplicate))
        {
            Console.WriteLine($"You previously submitted an answer of {answer}, and got the response;\n{duplicate}.\t");
            Console.WriteLine($"The answer is {day.RangeString()}.");
            return "Duplicate answer";
        }
        //don't submit answers that are out of range
        if (minimum.HasValue && long.Parse(answer) < minimum)
        {
            Console.WriteLine($"Didn't submit {answer} because we know it's {day.RangeString()}");
            return "Too low";
        }
        if (maximum.HasValue && long.Parse(answer) > maximum)
        {
            Console.WriteLine($"Didn't submit {answer} because we know it's {day.RangeString()}");
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
            Utility.Wait(timeToNextSubmission);
        }

        long now = DateTimeOffset.Now.ToUnixTimeSeconds();
        #endregion
        return ActuallySubmitAnswer();
        string ActuallySubmitAnswer()
        {
            string result = WebsiteInteraction.SubmitAOCAnswer(answer, day.Year, day.Day, part).Result;
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
                Console.Write($"{day.Year} day {day.Day}, part {part} is now complete. ");
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
                    Console.WriteLine($"We know the answer is {day.RangeString()}");
                    result = "Incorrect";
                }
                else if (lowhigh.Length > 0)
                {
                    Console.Write($"Your answer of {answer} is too ");
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.Write(lowhigh);
                    Console.ForegroundColor = ConsoleColor.Gray;
                    Console.WriteLine($". Please wait {secondsBetweenSubmissions} seconds before trying again.");
                    Console.WriteLine($"We now know the answer is {day.RangeString()}.");
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
                Console.WriteLine($"It appears you've already solved {day.Year} day {day.Day}, part {part}. (although logs are empty)");
                result = "Already solved";
            }
            else if (result.Contains("You gave an answer too recently"))
            {
                Console.WriteLine("Answer was submitted, but another was submitted too recently.");
                if (lastLog != null)
                {
                    long lastSubmission = long.Parse(lastLog[0]);
                    timeSinceLastSubmission = DateTimeOffset.Now.ToUnixTimeSeconds() - lastSubmission;
                    Console.WriteLine($"The log says the last submission was {timeSinceLastSubmission} seconds ago (at {lastSubmission}).");
                }
                result = Regex.Match(result, "You have \\d+s left to wait").Value;
                int toWait = int.Parse(result.Split(" ")[2].Trim('s'));
                Console.WriteLine($"Advent of Code says: {result}.");
                Utility.Wait(toWait);
                return ActuallySubmitAnswer();
            }
            else
            {
                string errorPath = $"{logPath}\\D{day}_ERROR_{now}.txt";
                Console.WriteLine($"Couldn't tell if {answer} was right or wrong. Error log created at:\n{errorPath}");
                File.WriteAllText(errorPath, result);
                result = "Undetermined";
            }
            #endregion
            File.AppendAllText(path, $"{now},{answer},{result},{part}\n");
            return result;
        }
    }
    public string RangeString()
    {
        string rangeMessage = "unknown";
        if (Minimum.HasValue && Maximum.HasValue)
        {
            if (Minimum.Value == Maximum.Value) rangeMessage = $"exactly {Minimum}";
            else rangeMessage = $"between {Minimum} and {Maximum}";
        }
        else if (Minimum.HasValue) rangeMessage = $"above {Minimum}";
        else if (Maximum.HasValue) rangeMessage = $"below {Maximum}";
        return rangeMessage;
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
public abstract class Day : DayBase<string>
{

}
public abstract class Day<T> : DayBase<T>
{
}

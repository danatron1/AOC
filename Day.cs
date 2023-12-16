using Newtonsoft.Json.Linq;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.Intrinsics.Arm;
using System.Text.RegularExpressions;
using System.Windows;

public interface IDayBase
{
    void Solve(bool trackPerformance);
}
public abstract class DayBase<InputType> : IDayBase
{
    public static int[] EventDays = new int[]
    { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25 };
    public int Day { get; init; }
    public int Year { get; init; }
    public string Name { get; init; }
    public string NameSpace => $"AOC.Y{Year}";
    public bool useExampleInput = false;
    public long? Minimum => GetPart() == 1 ? MinimumA : MinimumB;
    public long? Maximum => GetPart() == 1 ? MaximumA : MaximumB;
    private long? MinimumA, MinimumB, MaximumA, MaximumB;
    public bool TrackingPerformance;
    private TimeSpan wastedTime = new(0);
    public override string ToString() => Name;
    private static TimeSpan? __est_utc_offset;
    public static TimeSpan EST_UTC_OFFSET
    {
        get
        {
            if (__est_utc_offset is null)
            {
                try
                { //try to get timezone the proper way
                    __est_utc_offset = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time").BaseUtcOffset;
                }
                catch
                { //if that fails, assume it to be -5 (not exactly going to change often)
                    __est_utc_offset = TimeSpan.FromHours(-5);
                }
            }
            return __est_utc_offset.Value;
        }
    }
    public DayBase()
    {
        string[] caller = NameOfCallingClass("Day").Split('_', '.');
        Year = int.Parse(caller[^1]);
        Day = int.Parse(caller[^2][1..]);
        Name = NameFor(Year, Day);
        CacheAnswerLimits(); //min and max
        Directory.CreateDirectory($@"{Utility.folderPath}\Inputs\{Year}");
        Setup();
    }
    public DateTime PuzzleReleaseDate => PuzzleReleaseDateUTC(Year, Day);
    public bool PuzzleReleased => DateTime.UtcNow >= PuzzleReleaseDate;
    public TimeSpan PuzzleReleaseTimeRemaining => PuzzleReleaseDate - DateTime.UtcNow;
    public static DateTime EST_Date => DateTime.UtcNow + EST_UTC_OFFSET;
    public static bool EventRunning(out int newestYear, out int newestDay)
    {
        if (EST_Date.Month != 12)
        {
            newestYear = EST_Date.Year - 1;
            newestDay = EventDays.Last();
            return false;
        }
        newestYear = EST_Date.Year;
        if (EventDays.Contains(EST_Date.Day))
        {
            newestDay = EST_Date.Day;
            return true;
        }
        newestDay = EventDays.Last();
        return false;
    }
    public static TimeSpan NextPuzzleReleaseTimeRemaining => NextPuzzleReleaseDate() - DateTime.UtcNow;
    public static DateTime NextPuzzleReleaseDate()
    {
        if (EventRunning(out int year, out int day) && day < EventDays.Length) return PuzzleReleaseDateUTC(year, day + 1);
        if (EST_Date.Month < 12) year = EST_Date.Year;
        else year = EST_Date.Year + 1;
        return PuzzleReleaseDateUTC(year, 1);
    }
    public static DateTime PuzzleReleaseDateUTC(int year, int day)
    {
        return new DateTime(year, 12, day, 0, 0, 0, DateTimeKind.Utc) - EST_UTC_OFFSET;
    }
    public virtual void Setup() { } 
    public virtual void ClearCaches()
    {
        _inputRaw= null;
        _input = null;
        _inputBlocks = null;
        _input2D = null;
        _input2DJagged = null;
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
    private InputType[][]? _inputBlocks;
    //InputBlocks are defined by inputs that are clusters of data separated by empty lines. For example, 2022 day 1.
    public InputType[][] InputBlocks
    {
        get
        {
            _inputBlocks ??= GetInputForDayBlocks<InputType>();
            return _inputBlocks;
        }
    }
    private InputType[,]? _input2D;
    private InputType[][]? _input2DJagged;
    public InputType[,] Input2D
    {
        get
        {
            _input2D ??= GetInputForDay2D<InputType>();
            return _input2D;
        }
    }
    public InputType[][] Input2DJagged
    {
        get
        {
            _input2DJagged ??= GetInputForDay2D<InputType>().ToJagged();
            return _input2DJagged;
        }
    }
    private string[]? _extraInput;
    private string _extraInputPath => $@"{Utility.folderPath}\Inputs\{Year}\{Name}_Extra.txt";
    public string[] ExtraInput
    {
        get
        {
            if (_extraInput == null)
            {
                if (File.Exists(_extraInputPath)) _extraInput = File.ReadAllLines(_extraInputPath);
                else _extraInput = Utility.GetUserInputs(_extraInputPath);
            }
            return _extraInput;
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
    public virtual void Solve() => Solve(false); //need for legacy compatibility.
    public virtual void Solve(bool trackPerformance)
    {
        TrackingPerformance = trackPerformance;
        TimeSpan part1Time, wastedTimeTotal = TimeSpan.Zero;
        Stopwatch sw = Stopwatch.StartNew();
        try
        {
            PrintSolveBeginMessage("A");
            PartASetup();
            PartA();
        }
        catch (NotImplementedException)
        {
            Console.WriteLine($"Part A not yet solved.");
        }
        part1Time = sw.Elapsed - wastedTime;
        wastedTimeTotal += wastedTime;
        wastedTime = TimeSpan.Zero;
        ClearCaches();
        sw.Restart();
        _input = null;
        useExampleInput = false;
        try
        {
            PrintSolveBeginMessage("B");
            PartBSetup();
            PartB();
        }
        catch (NotImplementedException)
        {
            Console.WriteLine($"Part B not yet solved.");
        }
        sw.Stop();
        if (TrackingPerformance)
        {
            Console.WriteLine($"Part A completed in {part1Time.AsTime()}");
            Console.WriteLine($"Part B completed in {(sw.Elapsed - wastedTime).AsTime()}");
            Console.WriteLine($"Time spent submitting/fetching inputs: {wastedTimeTotal.AsTime()}");
        }
        void PrintSolveBeginMessage(string part)
        {
            Console.Write($"\nNow solving {this} (");
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write($"Part {part}");
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine(")");

        }
    }
    public virtual void PartASetup() { }
    public virtual void PartBSetup() { }
    public static void SolveMostRecent(bool trackPerformance = false)
    {
        Console.WriteLine($"Today's date is {EST_Date:dd/MM/yyyy} (EST)");
        if (EST_Date.Day != DateTime.Today.Day)
        {
            Console.WriteLine($"(EST is AOC's timezone. While it's {DateTime.Today.DayOfWeek} for you, it's {EST_Date:t} on {EST_Date.DayOfWeek} on the US east coast)");
        }
        bool running = EventRunning(out int newestYear, out int newestDay);
        if (running) Console.WriteLine($"Advent of Code is running! Solving most recent day: {NameFor(newestYear, newestDay)}");
        else if (NextPuzzleReleaseTimeRemaining < TimeSpan.FromMinutes(20))
        {
            Console.WriteLine($"Next puzzle releases in under 20 minutes. Waiting...");
            Thread.Sleep(NextPuzzleReleaseTimeRemaining);
            newestDay++;
        }
        else Console.WriteLine($"Most recent puzzle is day {newestDay} of {newestYear}");
        Solve(newestYear, newestDay, trackPerformance);
    }
    public static void Solve(int year, int day, bool trackPerformance = false)
    {
        Stopwatch sw = Stopwatch.StartNew();
        
        Type? type = Type.GetType($"AOC.Y{year}.{NameFor(year, day)}");
        type ??= Type.GetType($"AOC.{NameFor(year, day)}"); //for legacy compatibility
        type ??= Type.GetType($"AOC.D{day}_{year}"); //for legacy compatibility
        if (type is null)
        {
            Console.WriteLine($"{NameFor(year, day)} doesn't appear to exist. Creating...");
            Create(year, day);
            Console.WriteLine($"Cannot continue with code execution as code must be recompiled to include the newly created file.\nRun the program again.");
            return;
        }
        IDayBase? obj = Activator.CreateInstance(type) as IDayBase;
        if (obj is null)
        {
            Console.WriteLine($"Failed to create instance of {type}");
            return;
        }
        obj.Solve(trackPerformance);

        //this solution also works:
        //dynamic obj = Activator.CreateInstance(type);
        //obj.Solve(trackPerformance);

        //get generic type:
        //Type implementingType = type.BaseType.GetGenericArguments().Length == 0 ? typeof(string) : type.BaseType.GetGenericArguments()[0];
        sw.Stop();
        if (trackPerformance) Console.WriteLine($"Completed everything in {sw.Elapsed.AsTime()}\n");
    }
    public abstract void PartA();
    public abstract void PartB();
    public virtual void Copy(object item)
    {
        Console.WriteLine($"{item} (copied to clipboard)");
        Utility.Copy(item);
    }
    public static string[] GetInputForAnyDay<T>(DayBase<T> d)
    {
        Stopwatch sw = Stopwatch.StartNew();
        string pathFull = $@"{Utility.folderPath}\Inputs\{d.Year}";
        string[] input;
        Directory.CreateDirectory(pathFull);
        pathFull += $@"\{d.Name}_{(d.useExampleInput ? "Example" : "Input")}.txt";
        //if should use example
        if (d.useExampleInput)
        {
            if (!File.Exists(pathFull))
            {
                Console.WriteLine($"First time using example inputs for {d}.\nCreated file at {pathFull}");
                input = Utility.GetUserInputs(pathFull);
            }
            else input = File.ReadAllLines(pathFull);
        }
        //using real input
        else if (!File.Exists(pathFull))
        {
            Console.WriteLine($"Input file not downloaded for {d}, fetching.");

            //check if you're requesting a puzzle input download prior to its release
            DateTime release = new DateTime(d.Year, 12, d.Day, 0, 0, 0, DateTimeKind.Utc) + EST_UTC_OFFSET;
            if (release > DateTime.UtcNow)
            {
                Console.WriteLine($"The requested puzzle ({d}) is expected to release in the future.");
                Console.WriteLine($"{"Current UTC time:",-18}{DateTime.UtcNow}");
                Console.WriteLine($"{"Expected release:",-18}{release}");
                Console.WriteLine("Are you sure you want to attempt to download the puzzle input? (y/n)");
                string? response = Console.ReadLine()?.ToLower();
                if (string.IsNullOrWhiteSpace(response) || response[0] != 'y') return new string[] { "Puzzle not yet available!" };
            }

            input = WebsiteInteraction.DownloadAOCInput(d).Result;
        }
        else input = File.ReadAllLines(pathFull);
        sw.Stop();
        if (d.TrackingPerformance) Console.WriteLine($"Retrieved input for {d} in {sw.Elapsed.AsTime()}");
        d.wastedTime += sw.Elapsed;
        return input;
    }
    public virtual string[] GetInputForDay() => GetInputForAnyDay(this);
    public virtual T[] GetInputForDay<T>() => InputRaw.ConvertTo<T>().ToArray();
    public virtual T[][] GetInputForDayBlocks<T>(string splitLine = "")
    {
        List<T[]> results = new List<T[]>();
        List<T> currentBlock = new List<T>();
        string[] lines = InputRaw;
        foreach (string line in lines)
        {
            if (line == splitLine)
            {
                if (currentBlock.Count == 0) continue;
                results.Add(currentBlock.ToArray());
                currentBlock.Clear();
            }
            else currentBlock.Add(line.ConvertTo<T>());
        }
        if (currentBlock.Count > 0) results.Add(currentBlock.ToArray());
        return results.ToArray();
    }
    public virtual string[,] GetInputForDay2D(string split = "")
    {
        string[] lines = InputRaw;
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
        foreach (int day in EventDays) Create(year, day);
    }
    private static int StarsOnDay(int year, int day)
    {
        int stars = 0;
        string logPath = @$"{Utility.folderPath}\Logs\{year}";
        string filePath = @$"{logPath}\{NameFor(year, day)}_LOG.txt";
        if (Directory.Exists(logPath) && File.Exists(filePath))
        {
            string file = File.ReadAllText(filePath);
            if (file.Contains(",Correct,1")) stars++;
            if (file.Contains(",Correct,2")) stars++;
        }
        return stars;
    }
    private static int StarsOnYear(int year)
    {
        return EventDays.Select(d => StarsOnDay(year, d)).Sum();
    }
    internal static void OverallCompletion()
    {
        int possible, total = 0, totalPossible = 0;
        bool running = EventRunning(out int newestYear, out int newestDay);
        for (int year = 2015; year <= newestYear; year++)
        {
            if (year == newestYear) possible = newestDay * 2;
            else possible = EventDays.Length * 2;
            Console.Write($"[{year}] ");
            int count = StarsOnYear(year);
            total += count;
            totalPossible += possible;
            PrintStarCompletionRatio(count, possible);
        }
        Console.Write("\nTotal stars: ");
        PrintStarCompletionRatio(total, totalPossible);
    }
    private static void PrintStarCompletionRatio(int count, int possible)
    {
        if (count == possible) Console.ForegroundColor = ConsoleColor.Yellow;
        else if (count > 0) Console.ForegroundColor = ConsoleColor.White;
        Console.WriteLine($"{count}/{possible}*");
        Console.ForegroundColor = ConsoleColor.Gray;
    }
    internal static void Completion(params int[] years)
    {
        EventRunning(out int newestYear, out int newestDay);
        foreach (int year in years)
        {
            if (year < 2015 || year > newestYear) continue;
            Console.WriteLine($"\n====={year}=====");
            int total = 0;
            int days = year == newestYear ? newestDay : EventDays.Length;
            int possible = days * 2;
            for (int day = 1; day <= days; day++)
            {
                Console.Write($"{day,2} ");
                int stars = StarsOnDay(year, day);
                total += stars;
                Console.ForegroundColor = stars == 2 ? ConsoleColor.Yellow : ConsoleColor.White;
                Console.WriteLine("*".Repeat(stars));
                Console.ForegroundColor = ConsoleColor.Gray;
            }
            Console.Write($"\nTotal stars for {year}: ");
            PrintStarCompletionRatio(total, possible);
        }
    }
    internal virtual string Submit(object answer)
    {
        if (useExampleInput)
        {
            Console.WriteLine($"Your answer ({answer}) was not submitted as you're using the example input.");
            return "Example input";
        }
        Stopwatch sw = Stopwatch.StartNew();
        if (answer is null)
        {
            Console.WriteLine("Cannot submit null");
            return "Cannot submit null";
        }
        int part = GetPart();
        string result = Submit(answer.ToString(), this, part);
        Console.Write($"(Puzzle {Name}, Part{(part==1?"A":"B")}) result: ");
        Console.ForegroundColor = ConsoleColor.White;
        Console.Write(result);
        Console.ForegroundColor = ConsoleColor.Gray;
        if (!TrackingPerformance) Console.WriteLine("\n");
        else Console.WriteLine($"\nTime taken to submit answer: {sw.Elapsed.AsTime()}\n");
        wastedTime += sw.Elapsed;
        return result;
    }
    private const int secondsBetweenSubmissions = 61;
    internal static string Submit<T>(string? answer, DayBase<T> day, int part)
    {
        if (string.IsNullOrWhiteSpace(answer)) return "Cannot answer null";
        Console.Write($"You answered; ");
        Console.ForegroundColor = ConsoleColor.White;
        Console.WriteLine(answer);
        Console.ForegroundColor = ConsoleColor.Gray;

        DateTime release = new DateTime(day.Year, 12, day.Day, 0, 0, 0, DateTimeKind.Utc) - EST_UTC_OFFSET;
        if (release > DateTime.UtcNow)
        {
            Console.WriteLine($"The requested puzzle ({day}) is expected to release in the future.");
            Console.WriteLine($"{"Current UTC time:",-18}{DateTime.UtcNow}");
            Console.WriteLine($"{"Expected release:",-18}{release}");
            Console.WriteLine("Are you sure you want to submit this answer? (y/n)");
            string? response = Console.ReadLine()?.ToLower();
            if (string.IsNullOrWhiteSpace(response) || response[0] != 'y') return "Declined to submit";
        }
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
            if (lastLog[2] == "Incorrect low" && long.TryParse(lastLog[1], out newLimit))
            {
                day.ImproveMinMaxGuesses(newLimit, null, part);
            }
            else if (lastLog[2] == "Incorrect high" && long.TryParse(lastLog[1], out newLimit))
            {
                day.ImproveMinMaxGuesses(null, newLimit, part);
            }
        }
        if (!string.IsNullOrWhiteSpace(duplicate))
        {
            Console.WriteLine($"You previously submitted an answer of {answer}, and got the response;\n{duplicate}.\t");
            Console.WriteLine($"The answer is {day.RangeString()}.");
            return "Duplicate answer";
        }
        //don't submit answers that are out of range
        if (day.Minimum.HasValue && long.Parse(answer) < day.Minimum)
        {
            Console.WriteLine($"Didn't submit {answer} because we know it's {day.RangeString()}");
            return "Too low";
        }
        if (day.Maximum.HasValue && long.Parse(answer) > day.Maximum)
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
        if (answer == "-1" || answer == "0" || answer == "1") //double check common fail values
        {
            Console.WriteLine($"{answer} is likely an error or sentinel value.");
            Console.WriteLine("Are you sure you want to submit this answer? (y/n)");
            string? response = Console.ReadLine()?.ToLower();
            if (string.IsNullOrWhiteSpace(response) || response[0] != 'y') return "Declined to submit";
        }
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
                    day.ImproveMinMaxGuesses(long.Parse(answer), null, part);
                }
                else if (result.Contains("answer is too high"))
                {
                    lowhigh = "high";
                    day.ImproveMinMaxGuesses(null, long.Parse(answer), part);
                }

                if (lowhigh.Length == 0 && (day.Minimum.HasValue || day.Maximum.HasValue))
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
                string errorPath = $"{logPath}\\{day}_ERROR_{now}.txt";
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
    void ImproveMinMaxGuesses(long? newMin, long? newMax, int part)
    {
        if (part == 1)
        {
            if (!MinimumA.HasValue || (newMin.HasValue && newMin > MinimumA)) MinimumA = newMin;
            if (!MaximumA.HasValue || (newMax.HasValue && newMax < MaximumA)) MaximumA = newMax;
        }
        else if (part == 2)
        {
            if (!MinimumB.HasValue || (newMin.HasValue && newMin > MinimumB)) MinimumB = newMin;
            if (!MaximumB.HasValue || (newMax.HasValue && newMax < MaximumB)) MaximumB = newMax;
        }
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
    }
}
public abstract class Day : DayBase<string>
{

}
public abstract class Day<T> : DayBase<T>
{
}

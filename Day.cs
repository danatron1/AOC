using System.Diagnostics;
using System.Reflection;
using System.Text.RegularExpressions;
using AOC;

public interface IDay
{
    void Solve(bool trackPerformance);
    int Day { get; }
    int Year { get; }
    string NameSpace { get; }
    string Name { get; }
    bool useExampleInput { get; }
    bool OnPart2 { get; }
    long? Minimum { get; }
    long? Maximum { get; }
    bool TrackingPerformance { get; }
    void AddWastedTime(TimeSpan time);
    void ImproveMinMaxGuesses(long? newMin, long? newMax, int part);
    IEnumerable<Log> GetLogData();
}
public abstract class DayBase<InputType> : IDay
{
    #region Properties, Fields, and Variables 
    public static int[] EventDays = new int[]
    { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25 };
    public int Day { get; init; }
    public int Year { get; init; }
    public string NameSpace => $"AOC.Y{Year}";
    public string Name { get; init; }
    public override string ToString() => Name;

    public bool useExampleInput { get; set; }
    public bool OnPart2 { get; private set; }
    public long? Minimum => GetPart() == 1 ? MinimumA : MinimumB;
    public long? Maximum => GetPart() == 1 ? MaximumA : MaximumB;
    private long? MinimumA, MinimumB, MaximumA, MaximumB;
    public bool TrackingPerformance { get; set; }
    public TimeSpan WastedTime = TimeSpan.Zero;
    #endregion

    public DayBase()
    {
        string[] caller = NameOfCallingClass("Day").Split('_', '.');
        useExampleInput = false;
        Year = int.Parse(caller[^1]);
        Day = int.Parse(caller[^2][1..]);
        Name = NameFor(Year, Day);
        CacheAnswerLimits(); //min and max
        Directory.CreateDirectory($@"{Utility.folderPath}\Inputs\{Year}");
        Setup();
    }
    
    #region Puzzle release timezone stuff

    private static TimeSpan? __est_utc_offset;
    public static TimeSpan EST_UTC_OFFSET
    {
        get
        {
            if (__est_utc_offset is null)
            {
                try
                { //try to get timezone the proper way
                    __est_utc_offset = TimeZoneInfo.FindSystemTimeZoneById(Canned.EST).BaseUtcOffset;
                }
                catch
                { //if that fails, assume it to be -5 (not exactly going to change often)
                    __est_utc_offset = TimeSpan.FromHours(-5);
                }
            }
            return __est_utc_offset.Value;
        }
    }

    DateTime PuzzleReleaseDate => PuzzleReleaseDateUTC(Year, Day);
    bool PuzzleReleased => DateTime.UtcNow >= PuzzleReleaseDate;
    TimeSpan PuzzleReleaseTimeRemaining => PuzzleReleaseDate - DateTime.UtcNow;
    static DateTime EST_Date => DateTime.UtcNow + EST_UTC_OFFSET;
    static bool EventRunning(out int newestYear, out int newestDay)
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
    static TimeSpan NextPuzzleReleaseTimeRemaining => NextPuzzleReleaseDate() - DateTime.UtcNow;
    static DateTime NextPuzzleReleaseDate()
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
    #endregion
    internal virtual void Setup() { } 
    internal virtual void ClearCaches()
    {
        _inputRaw= null;
        _input = null;
        _inputBlocks = null;
        _input2D = null;
        _input2DJagged = null;
    }

    #region Readable puzzle input data

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
                else _extraInput = Utility.GetMultipleInputs(_extraInputPath);
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
    #endregion

    #region Log data
    public IEnumerable<Log> GetLogData() => Log.GetAllLogsForDay(this);
    private void CacheAnswerLimits()
    {
        foreach (Log log in GetLogData())
        {
            if (log.AnswerLong is null) continue;
            if (log.Response == Canned.LogResponse.Correct)
            {
                if (log.Part1)
                {
                    MinimumA = log.AnswerLong;
                    MaximumA = log.AnswerLong;
                }
                else
                {
                    MinimumB = log.AnswerLong;
                    MaximumB = log.AnswerLong;
                }
                continue;
            }
            if (log.Response == Canned.LogResponse.IncorrectLow)
            {
                if (log.Part1 && (!MinimumA.HasValue || log.AnswerLong > MinimumA)) MinimumA = log.AnswerLong;
                else if (log.Part2 && (!MinimumB.HasValue || log.AnswerLong > MinimumB)) MinimumB = log.AnswerLong;
            }
            else if (log.Response == Canned.LogResponse.IncorrectHigh)
            {
                if (log.Part1 && (!MaximumA.HasValue || log.AnswerLong < MaximumA)) MaximumA = log.AnswerLong;
                else if (log.Part2 && (!MaximumB.HasValue || log.AnswerLong < MaximumB)) MaximumB = log.AnswerLong;
            }
        }
    }
    #endregion

    public void AddWastedTime(TimeSpan time) => WastedTime += time;
    public virtual void Solve() => Solve(false); //need for legacy compatibility.
    public virtual void Solve(bool trackPerformance)
    {
        TrackingPerformance = trackPerformance;
        TimeSpan part1Time, part2Time, wastedTimeTotal = TimeSpan.Zero;
        Stopwatch sw = Stopwatch.StartNew();
        part1Time = SolveAndTrackPart(1);
        part2Time = SolveAndTrackPart(2);
        sw.Stop();
        if (TrackingPerformance)
        {
            Canned.PrintParagraph.PerformanceStatistics(part1Time, part2Time, wastedTimeTotal);
        }
        TimeSpan SolveAndTrackPart(int part)
        {
            useExampleInput = false;
            OnPart2 = part == 2;
            Canned.PrintParagraph.NowSolving(this);
            try
            {
                if (OnPart2)
                {
                    PartTwoSetup();
                    PartTwo();
                }
                else
                {
                    PartOneSetup();
                    PartOne();
                }
            }
            catch (NotImplementedException)
            {
                Console.WriteLine(Canned.Messages.NotSolvedYet(OnPart2));
            }
            TimeSpan time = sw.Elapsed - WastedTime;
            wastedTimeTotal += WastedTime;
            WastedTime = TimeSpan.Zero;
            ClearCaches();
            sw.Restart();
            return time;
        }
    }
    public virtual void PartOneSetup() { }
    public virtual void PartTwoSetup() { }
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
            if (++newestDay > EventDays.Last()) newestDay = EventDays.First();
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
            Create(year, day);
            Canned.PrintParagraph.DayDoesntExistSoCreate(NameFor(year, day));
            return;
        }
        if (Activator.CreateInstance(type) is not IDay dayObject)
        {
            Console.WriteLine($"Failed to create instance of {type}");
            return;
        }
        dayObject.Solve(trackPerformance);
        sw.Stop();
        if (trackPerformance) Console.WriteLine($"Completed everything in {sw.Elapsed.AsTime()}\n");
    }
    public abstract void PartOne();
    public abstract void PartTwo();
    public virtual void Copy(object item)
    {
        Console.WriteLine($"{item} (copied to clipboard)");
        Utility.Copy(item);
    }
    public static string[] GetInputForAnyDay(IDay d)
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
                input = Utility.GetMultipleInputs(pathFull);
                Console.WriteLine($"Example input captured!");
            }
            else input = File.ReadAllLines(pathFull);
        }
        //using real input
        else if (!File.Exists(pathFull))
        {
            Console.WriteLine(Canned.Messages.DownloadInputFile(d));

            //check if you're requesting a puzzle input download prior to its release
            DateTime release = new DateTime(d.Year, 12, d.Day, 0, 0, 0, DateTimeKind.Utc) + EST_UTC_OFFSET;
            if (release > DateTime.UtcNow)
            {
                Canned.PrintParagraph.ConfirmDownloadFutureInput(d);
                if (!Utility.UserInputConfirm()) return new string[] { "Puzzle not yet available!" };
            }

            input = WebsiteInteraction.DownloadAOCInput(d).Result;
        }
        else input = File.ReadAllLines(pathFull);
        sw.Stop();
        if (d.TrackingPerformance) Console.WriteLine($"Retrieved input for {d} in {sw.Elapsed.AsTime()}");
        d.AddWastedTime(sw.Elapsed);
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
            if (file.Contains($",{Canned.LogResponse.Correct},1")) stars++;
            if (file.Contains($",{Canned.LogResponse.Correct},2")) stars++;
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
        EventRunning(out int newestYear, out int newestDay);
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
    internal virtual string Submit(object answer) //object
    {
        Stopwatch sw = Stopwatch.StartNew();
        if (useExampleInput) return Canned.SubmitResponse.ExampleInputMessage(answer);
        if (answer is null) return Canned.SubmitResponse.NullMessage();
        int part = GetPart();
        string result = Submit(answer.ToString(), this, part);
        Utility.Emphasis($"(Puzzle {Name}, Part {part}) result: *{result}*\n");
        if (TrackingPerformance) Console.WriteLine($"Time taken to submit answer: {sw.Elapsed.AsTime()}\n");
        WastedTime += sw.Elapsed;
        return result;
    }
    internal static string Submit(string? answer, IDay day, int part)
    {
        if (string.IsNullOrWhiteSpace(answer)) return Canned.SubmitResponse.NullMessage();
        Utility.Emphasis($"You answered; *{answer}*");
        if (PuzzleReleaseDateUTC(day.Year, day.Day) > DateTime.UtcNow)
        {
            Canned.PrintParagraph.ConfirmSubmitFutureAnswer(day);
            if (!Utility.UserInputConfirm()) return Canned.SubmitResponse.Declined;
        }
        IEnumerable<Log> logs = day.GetLogData();
        string part1Correct = string.Empty;
        string? duplicate = null;
        long latestSubmissionTime = 0;
        foreach (Log log in logs)
        {
            //skip log entries that are for a different part
            if (log.Part != part)
            {
                if (log.Response == Canned.LogResponse.Correct && log.Part1) part1Correct = log.Answer;
                continue;
            }
            latestSubmissionTime = Math.Max(log.Time, latestSubmissionTime);
            //don't submit if you've already completed the puzzle
            if (log.Response == Canned.LogResponse.Correct) return Canned.SubmitResponse.AlreadySolvedCorrectMessage(log, answer);
            //Update log if you've already solved it but don't have logs of it;
            if (log.Response == Canned.SubmitResponse.AlreadySolved)
            {
                Canned.SubmitResponse.AlreadySolvedCreateLog(day, answer);
                if (Utility.UserInputConfirm()) log.Overwrite(day, answer: answer, response: Canned.LogResponse.Correct);
                else return Canned.SubmitResponse.AlreadySolved;
            }
            //don't submit if you've already submitted this answer
            if (log.Answer == answer && log.Response != Canned.LogResponse.Undetermined) duplicate = log.Answer;
            //calculate range
            if      (log.Response == Canned.LogResponse.IncorrectLow)  day.ImproveMinMaxGuesses(log.AnswerLong, null, part);
            else if (log.Response == Canned.LogResponse.IncorrectHigh) day.ImproveMinMaxGuesses(null, log.AnswerLong, part);
        }
        if (!string.IsNullOrWhiteSpace(duplicate)) return Canned.SubmitResponse.DuplicateAnswerMessage(day, answer, duplicate);
        //don't submit answers that are out of range
        if (day.Minimum.HasValue && long.Parse(answer) < day.Minimum) return Canned.SubmitResponse.OutOfRangeMessage(day, answer, false);
        if (day.Maximum.HasValue && long.Parse(answer) > day.Maximum) return Canned.SubmitResponse.OutOfRangeMessage(day, answer, true);
        //limit submissions to 1 every 1 minutes;
        if (DateTimeOffset.Now.ToUnixTimeSeconds() - latestSubmissionTime < WebsiteInteraction.secondsBetweenSubmissions)
        {
            Console.WriteLine($"It's been less than {WebsiteInteraction.secondsBetweenSubmissions} seconds since your last submission.");
            Utility.Wait(WebsiteInteraction.secondsBetweenSubmissions - latestSubmissionTime);
        }
        latestSubmissionTime = DateTimeOffset.Now.ToUnixTimeSeconds();
        if (answer is "-1" or "0" or "1") //double check common fail values (dummy values)
        {
            Canned.SubmitResponse.CommonlyWrongValuesMessage(answer);
            if (!Utility.UserInputConfirm()) return Canned.SubmitResponse.Declined;
        }
        if (answer == part1Correct) //double check common fail values (part 2 same as part 1)
        {
            Canned.SubmitResponse.PartTwoSameAsPartOneMessage(answer);
            if (!Utility.UserInputConfirm()) return Canned.SubmitResponse.Declined;
        }
        return ActuallySubmitAnswer();
        string ActuallySubmitAnswer()
        {
            string result = WebsiteInteraction.SubmitAOCAnswer(answer, day.Year, day.Day, part).Result;
            switch (result)
            {
                case string s when s.Contains(Canned.WebsiteResponse.Correct):
                    result = Canned.SubmitResponse.CorrectMessage(day, answer, part);
                    break;
                case string s when s.Contains(Canned.WebsiteResponse.Incorrect):
                    result = Canned.SubmitResponse.IncorrectMessage(day, answer, part, result);
                    break;
                case string s when s.Contains(Canned.WebsiteResponse.AlreadySolved):
                    Canned.SubmitResponse.AlreadySolvedCreateLog(day, answer);
                    if (Utility.UserInputConfirm()) result = Canned.LogResponse.Correct;
                    else return Canned.SubmitResponse.AlreadySolved;
                    break;
                case string s when s.Contains(Canned.WebsiteResponse.TooSoon):
                    Utility.Wait(Canned.SubmitResponse.TooSoonMessage(result));
                    return ActuallySubmitAnswer();
                default:
                    string errorPath = Log.ErrorPath(day, latestSubmissionTime);
                    Console.WriteLine($"Couldn't tell if {answer} was right or wrong. Error log created at:\n{errorPath}");
                    File.WriteAllText(errorPath, result);
                    result = Canned.LogResponse.Undetermined;
                    break;
            };
            Log.NewTextfileEntry(day, latestSubmissionTime, answer, result, part);
            return result;
        }
    }
    public void ImproveMinMaxGuesses(long? newMin, long? newMax, int part)
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
            if (frame.GetMethod().Name == Canned.PART_ONE) return 1;
            if (frame.GetMethod().Name == Canned.PART_TWO) return 2;
        }
        throw new Exception(Canned.Exceptions.CouldntDeterminePart);
    }
}
public abstract class Day : DayBase<string>
{

}
public abstract class Day<T> : DayBase<T>
{
}

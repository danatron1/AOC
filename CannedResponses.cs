//For canned (string) responses, text, etc.

using AOC;
using System.Text.RegularExpressions;

public static class Canned
{
    public const string EST = "Eastern Standard Time";
    public const string LOG = "LOG";
    public const string INPUT = "Input";
    public const string EXAMPLE = "Example";
    public const string PART_ONE = "PartOne";
    public const string PART_TWO = "PartTwo";
    public const string ERROR = "ERROR";
    public static class Messages
    {
        public const string GetUserInput = "Please input below. Input blank line when you're done";
        public static string NotSolvedYet(bool OnPart2) => $"Part {(OnPart2 ? "2" : "1")} not yet solved.";
        public static string RangeString(IDay day)
        {
            string rangeMessage = "unknown";
            if (day.Minimum.HasValue && day.Maximum.HasValue)
            {
                if (day.Minimum.Value == day.Maximum.Value) rangeMessage = $"exactly {day.Minimum}";
                else rangeMessage = $"between {day.Minimum} and {day.Maximum}";
            }
            else if (day.Minimum.HasValue) rangeMessage = $"above {day.Minimum}";
            else if (day.Maximum.HasValue) rangeMessage = $"below {day.Maximum}";
            return rangeMessage;
        }
        public static string DownloadInputFile(IDay day)
        {
            return $"Input file not downloaded for {day}, fetching.";
        }
    }
    public static class Exceptions
    {
        public const string TypeHasNoParseMethod = "Type does not have a parse method.";
        public const string CouldntDeterminePart = "Could not determine calling part";
        public static string MissingFieldInSessionFile(string line, string sessionInfoFilepath)
        {
            return $"Missing mandatory field {line} in session info file: {sessionInfoFilepath}";
        }
    }
    public static class WebsiteResponse
    {
        public const string PuzzleInputsDifferByUser = "Puzzle inputs differ by user.  Please log in to get your puzzle input.";
        public const string Correct = "That's the right answer!";
        public const string Incorrect = "That's not the right answer";
        public const string TooLow = "answer is too low";
        public const string TooHigh = "answer is too high";
        public const string TooSoon = "You gave an answer too recently";
        public const string AlreadySolved = "You don't seem to be solving the right level";
        public const string TimeLeftToWaitRegex = "You have \\d+s left to wait";
    }
    public static class PrintParagraph
    {
        public static void CannotGetPuzzleInput()
        {
            Utility.Emphasis("You are *not logged in* - cannot get puzzle input.", ConsoleColor.Red);
            Console.WriteLine("To get log in token, go to the application tab (chrome) or the storage tab (firefox)");
            Console.WriteLine("paste your token in \"sessionInfo.txt\" and try again.");
        }
        public static void NowSolving(IDay day)
        {
            Utility.Emphasis($"\nNow solving {day} (*Part {(day.OnPart2 ? "2" : "1")}*)");
        }
        internal static void PuzzleExpectedInFuture(IDay day)
        {
            Console.WriteLine($"The requested puzzle ({day}) is expected to release in the future.");
            Console.WriteLine($"{"Current UTC time:",-18}{DateTime.UtcNow}");
            Console.WriteLine($"{"Expected release:",-18}{Day.PuzzleReleaseDateUTC(day.Year, day.Day)}");
        }
        internal static void ConfirmDownloadFutureInput(IDay day)
        {
            PuzzleExpectedInFuture(day);
            Console.WriteLine("Are you sure you want to attempt to download the puzzle input?");
        }

        internal static void ConfirmSubmitFutureAnswer(IDay day)
        {
            PuzzleExpectedInFuture(day);
            Console.WriteLine("Are you sure you want to attempt to download the puzzle input?");
        }

        internal static void DayDoesntExistSoCreate(string name)
        {
            Console.WriteLine($"Creating {name} as it doesn't appear to exist.");
            Console.WriteLine($"Cannot continue with code execution as code must be recompiled to include the newly created file.\nRun the program again.");
        }

        internal static void PerformanceStatistics(TimeSpan part1Time, TimeSpan part2Time, TimeSpan wastedTimeTotal)
        {
            Utility.Emphasis($"Part *A* completed in *{part1Time.AsTime()}*");
            Utility.Emphasis($"Part *B* completed in *{part2Time.AsTime()}*");
            Console.WriteLine($"Time spent submitting/fetching inputs: {wastedTimeTotal.AsTime()}");
        }
    }
    public static class LogResponse
    {
        //Logged responses
        public const string Correct = "Correct";
        public const string Incorrect = "Incorrect";
        public const string IncorrectLow = "Incorrect low";
        public const string IncorrectHigh = "Incorrect high";
        public const string Undetermined = "Undetermined";
    }
    public static class SubmitResponse
    {
        //unlogged. For logged responses, look in LogResponse
        public const string Declined = "Declined to submit";
        public const string ExampleInput = "Example input";
        public const string AlreadySolved = "Already solved";
        public const string Null = "Cannot submit null";
        public const string DuplicateAnswer = "Duplicate answer";
        public const string TooHigh = "Too high";
        public const string TooLow = "Too low";

        public static string ExampleInputMessage(object answer)
        {
            Console.WriteLine($"Your answer ({answer}) was not submitted as you're using the example input.");
            return ExampleInput;
        }

        internal static string AlreadySolvedCorrectMessage(Log log, string answer)
        {
            Console.WriteLine($"You've previously solved this puzzle! The correct answer was {log.Answer}.");
            if (log.Answer == answer) Console.WriteLine("Well done on getting it right again :D");
            else Console.WriteLine($"Unfortunately... this means {answer} is not correct");
            return AlreadySolved;
        }

        internal static string NullMessage()
        {
            Console.WriteLine("Cannot submit null");
            return Null;
        }

        internal static void AlreadySolvedCreateLog(IDay day, string answer)
        {
            Console.WriteLine($"It appears you've already solved this day, however a log is missing.");
            Utility.Emphasis($"Check if your answer is correct at: *{WebsiteInteraction.AddressForDay(day)}*");
            Utility.Emphasis($"Would you like to log {day} as completed with *{answer}* as the right answer?");
        }

        internal static string DuplicateAnswerMessage(IDay day, string answer, string otherAnswer)
        {
            Console.WriteLine($"You previously submitted an answer of {answer}, and got the response;\n{otherAnswer}.\t");
            Console.WriteLine($"The answer is {Messages.RangeString(day)}.");
            return DuplicateAnswer;
        }

        internal static string OutOfRangeMessage(IDay day, string answer, bool tooHigh)
        {
            Console.WriteLine($"Didn't submit {answer} because we know it's {Messages.RangeString(day)}");
            return tooHigh ? TooHigh : TooLow;
        }

        internal static void CommonlyWrongValuesMessage(string answer)
        {
            Console.WriteLine($"{answer} is likely an error or sentinel value.");
            Console.WriteLine("Are you sure you want to submit this answer?");
        }

        internal static void PartTwoSameAsPartOneMessage(string answer)
        {
            Console.WriteLine($"{answer} is the same as your answer for part 1.");
            Console.WriteLine("Are you sure you want to submit this answer?");
        }

        internal static string CorrectMessage(IDay day, string answer, int part)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("CORRECT!");
            Utility.Emphasis($"{answer} is the correct answer! You've earned 1 *gold star*!", ConsoleColor.Yellow);
            Console.Write($"{day.Year} day {day.Day}, part {part} is now complete. ");
            if (part == 1) Console.WriteLine("Time to move onto part 2!");
            else Console.WriteLine("Time for the next day!");
            return LogResponse.Correct;
        }

        internal static string IncorrectMessage(IDay day, string answer, int part, string result)
        {
            Console.WriteLine("That is not the right answer;");
            bool? tooHigh = null;
            if (result.Contains(WebsiteResponse.TooLow))
            {
                tooHigh = false;
                day.ImproveMinMaxGuesses(long.Parse(answer), null, part);
            }
            else if (result.Contains(WebsiteResponse.TooHigh))
            {
                tooHigh = true;
                day.ImproveMinMaxGuesses(null, long.Parse(answer), part);
            }

            if (tooHigh is null && (day.Minimum.HasValue || day.Maximum.HasValue))
            {
                Console.WriteLine("You're no longer being told whether your answer is too high or low.");
                Console.WriteLine($"We know the answer is {Messages.RangeString(day)}");
                return LogResponse.Incorrect;
            }
            if (tooHigh is not null)
            {
                Utility.Emphasis($"Your answer of {answer} is too *{(tooHigh.Value ? "high" : "low")}*. " +
                    $"Please wait {WebsiteInteraction.secondsBetweenSubmissions} seconds before trying again.");
                Console.WriteLine($"We now know the answer is {Messages.RangeString(day)}.");
                return tooHigh.Value ? LogResponse.IncorrectHigh : LogResponse.IncorrectLow;
            }
            Console.WriteLine($"Please wait {WebsiteInteraction.secondsBetweenSubmissions} seconds before trying again.");
            return LogResponse.Incorrect;
        }

        internal static int TooSoonMessage(string result)
        {
            Console.WriteLine("Answer was submitted, but another was submitted too recently.");
            result = Regex.Match(result, WebsiteResponse.TimeLeftToWaitRegex).Value;
            Console.WriteLine($"Advent of Code says: {result}.");
            return result.ExtractNumber<int>();
        }
    }
}

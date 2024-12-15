using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace AOC
{
    public struct Log
    {
        public int LogEntryNumber;

        public long Time;
        public string Answer;
        public long? AnswerLong;
        public string Response;
        public int Part;
        public readonly bool Part1 => Part == 1;
        public readonly bool Part2 => Part == 2;
        public Log(string logLine, int number) : this()
        {
            LogEntryNumber = number;
            string[] log = logLine.Split(',');
            Time = long.TryParse(log[0], out long l) ? l : 0;
            Answer = log[1];
            Response = log[2];
            Part = int.TryParse(log[3], out int i) ? i : 0;
            AnswerLong = long.TryParse(Answer, out l) ? l : 0;
        }

        internal readonly void Overwrite(IDay day, long? time = null, string? answer = null, string? response = null, int? part = null)
        {
            time ??= Time;
            answer ??= Answer;
            response ??= Response;
            part ??= Part;
            string path = FilePathDay(day);
            string[] existingLogs = File.ReadAllLines(path);
            existingLogs[LogEntryNumber] = LogText(time.Value, answer, response, part.Value);
            File.WriteAllLines(path, existingLogs);
        }
        public static string FolderPathYear(int year) => @$"{Utility.folderPath}\Logs\{year}";
        public static string FilePathDay(IDay day) => @$"{FolderPathYear(day.Year)}\{day.Name}_{Canned.LOG}.txt";
        public static string ErrorPath(IDay day, long time) => @$"{FolderPathYear(day.Year)}\{day.Name}_{Canned.ERROR}_{time}.txt";
        public static string LogText(long time, string answer, string response, int part) => $"{time},{answer},{response},{part}";
        public static void NewTextfileEntry(IDay day, long time, string answer, string response, int part) => File.AppendAllText(FilePathDay(day), $"{LogText(time, answer, response, part)}\n");
        public static IEnumerable<Log> GetAllLogsForDay(IDay day)
        {
            if (!Directory.Exists(FolderPathYear(day.Year)))
            {
                Directory.CreateDirectory(FolderPathYear(day.Year));
                Console.WriteLine($"Created logging folder for year {day.Year}");
            }
            if (!File.Exists(FilePathDay(day))) yield break;
            string[] logs = File.ReadAllLines(FilePathDay(day));
            for (int i = 0; i < logs.Length; i++)
            {
                yield return new Log(logs[i], i);
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace AOC.Y2022
{
    internal class D07_2022 : Day
    {
        class File
        {
            public static List<File> AllFiles = new();
            public string name;
            public Folder parent;
            public virtual int Size { get; set; }
            public File(string name, int size, Folder parent)
            {
                this.name = name;
                Size = size;   
                this.parent = parent;
                AllFiles.Add(this);
            }
            internal virtual void PrintContents(int indent = 0)
            {
                Console.WriteLine($"{new string(' ',indent)}- {ToString()}");
            }
            public override string ToString() => $"{name} (file, size={Size})";
        }
        class Folder : File
        {
            public static List<Folder> AllFolders = new();
            public List<File> contents;
            public override int Size => contents.Sum(x => x.Size);
            public Folder(string name, Folder parent) : base(name, 0, parent) 
            {
                contents = new List<File>();
                AllFolders.Add(this);
            }
            internal void AddContent(string[] content)
            {
                if (contents.Any(f => f.name == content[1])) return; //already added, skip
                if (content[0] == "dir") contents.Add(new Folder(content[1], this));
                else contents.Add(new File(content[1], int.Parse(content[0]), this));
                //Console.WriteLine($"Found new file {content[1]} ({content[0]}) to folder {name}");
            }
            internal override void PrintContents(int indent = 0)
            {
                Console.WriteLine($"{new string(' ', indent)}- {ToString()}");
                foreach (File file in contents) file.PrintContents(indent + 1);
            }
            public override string ToString() => $"{name} (dir, size={Size})";
        }
        void MapDrive(Folder drive)
        {
            Folder current = null;
            foreach (string line in Input)
            {
                string[] split = line.Split(' ');
                if (split[0] == "$") //command
                {
                    if (split[1] == "ls") continue;
                    if (split[2] == drive.name) current = drive;
                    else if (split[2] == "..") current = current.parent;
                    else current = (Folder)current.contents.FirstOrDefault(f => f is Folder && f.name == split[2]);
                    //Console.WriteLine($"Current directory changed to {current}");
                }
                else current.AddContent(split);
            }
        }
        public override void PartA()
        {
            Folder drive = new("/", null);
            MapDrive(drive);
            Submit(Folder.AllFolders.Where(x => x.Size <= 100_000).Sum(x => x.Size));
        }
        public override void PartB()
        {
            Folder drive = new("/", null);
            MapDrive(drive);
            int amountNeededToFree = 30_000_000 - (70_000_000 - drive.Size);
            Submit(Folder.AllFolders.Where(x => x.Size >= amountNeededToFree).MinBy(x => x.Size).Size);
        }
    }
}

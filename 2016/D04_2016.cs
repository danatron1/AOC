
namespace AOC.Y2016;

internal class D04_2016 : Day
{
    struct Room
    {
        private string? _decryptedName;
        public string Letters;
        public int SectorId;
        public string Checksum;
        public Room(string line)
        {
            string[] split = line.Split('-');
            Letters = string.Join("", split[..^1]);
            split = split[^1].Split('[');
            SectorId = int.Parse(split[0]);
            Checksum = split[1].Trim(']');
        }
        public override string ToString() => $"{Letters}-{SectorId}[{Checksum}]";
        internal string DecryptedName()
        {
            if (!string.IsNullOrWhiteSpace(_decryptedName)) return _decryptedName;
            int shift = SectorId % 26;
            string decrypted = "";
            foreach (char c in Letters)
            {
                int letter = c + shift;
                if (letter > 'z') letter -= 26;
                decrypted += (char)letter;
            }
            _decryptedName = decrypted;
            return decrypted;
        }
    }
    public override void PartOne()
    {
        Room[] rooms = Input.Select(x => new Room(x)).ToArray();
        Submit(rooms.Where(RealRoom).Sum(x => x.SectorId));

    }
    public override void PartTwo()
    {
        Room[] rooms = Input.Select(x => new Room(x)).ToArray();
        Submit(rooms.First(x => x.DecryptedName().StartsWith("northpole")).SectorId);
    }
    bool RealRoom(Room room)
    {
        Tally<char> tally = room.Letters.ToCharArray().ToTally();
        char[] charOrder = tally.ToKeyValuePairs().OrderByDescending(x => x.Value).ThenBy(x => x.Key).Select(x => x.Key).ToArray();
        for (int i = 0; i < room.Checksum.Length; i++)
        {
            if (room.Checksum[i] != charOrder[i]) return false;
        }
        return true;
    }
}

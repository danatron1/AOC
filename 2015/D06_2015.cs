namespace AOC
{
    internal class D06_2015 : Day
    {
        public override void PartOne()
        {
            List<Light> lights = Light.CreateGrid(1000);
            Instruction[] instructions = GetInstructions();
            foreach (Instruction instruction in instructions)
            {
                IEnumerable<Light> square = lights.Where(x => x.InArea(instruction.coords));
                foreach (Light light in square)
                {
                    light.Execute(instruction.set);
                }
            }
            Submit(lights.Count(x => x.on));
        }
        public override void PartTwo()
        {
            List<NewLight> lights = NewLight.CreateGrid(1000);
            Instruction[] instructions = GetInstructions();
            foreach (Instruction instruction in instructions)
            {
                IEnumerable<NewLight> square = lights.Where(x => x.InArea(instruction.coords));
                foreach (NewLight light in square)
                {
                    light.Execute(instruction.set);
                }
            }
            Submit(lights.Sum(x => x.brightness));
        }
        Instruction[] GetInstructions() => Array.ConvertAll(GetInputForDay(), Instruction.Parse);
        private class Light
        {
            public bool on;
            public int x, y;

            public Light(int x, int y)
            {
                this.x = x;
                this.y = y;
                on = false;
            }
            internal static List<Light> CreateGrid(int width, int height = -1)
            {
                List<Light> lights = new();
                if (height == -1) height = width;
                for (int x = 0; x < width; x++)
                {
                    for (int y = 0; y < height; y++)
                    {
                        lights.Add(new Light(x, y));
                    }
                }
                return lights;
            }
            public bool InArea(params int[] coords)
            {
                return coords[0] <= x && coords[2] >= x && coords[1] <= y && coords[3] >= y;
            }
            internal void Execute(bool? mode)
            {
                on = mode.HasValue ? mode.Value : !on;
            }
        }
        private struct Instruction
        {
            public bool? set;
            public int[] coords;

            internal static Instruction Parse(string input)
            {
                Instruction i;
                if (input.StartsWith("turn on")) i.set = true;
                else if (input.StartsWith("turn off")) i.set = false;
                else i.set = null;
                while (!char.IsDigit(input[0])) input = input[1..];
                i.coords = input.Replace(" through ", ",").Split(',').Select(int.Parse).ToArray();
                return i;
            }
        }
        private class NewLight
        {
            public int x, y, brightness;

            public NewLight(int x, int y)
            {
                this.x = x;
                this.y = y;
                brightness = 0;
            }
            internal static List<NewLight> CreateGrid(int width, int height = -1)
            {
                List<NewLight> lights = new();
                if (height == -1) height = width;
                for (int x = 0; x < width; x++)
                {
                    for (int y = 0; y < height; y++)
                    {
                        lights.Add(new NewLight(x, y));
                    }
                }
                return lights;
            }
            public bool InArea(params int[] coords)
            {
                return coords[0] <= x && coords[2] >= x && coords[1] <= y && coords[3] >= y;
            }
            internal void Execute(bool? mode)
            {
                if (mode.HasValue)
                {
                    if (!mode.Value && brightness == 0) return;
                    brightness += mode.Value ? 1 : -1;
                }
                else brightness += 2;
            }
        }
    }
}

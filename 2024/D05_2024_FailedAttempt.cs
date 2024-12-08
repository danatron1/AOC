using System.Data;

namespace AOC.Y2024;

//"Hello, I'm an elf, please can you sort my list using non-transitive rules?"
//   - Statements dreamed up by the utterly deranged.

internal class D05_2024_FailedAttempt : Day
{
    public override void PartA()
    {
        Page.OrderRules = InputBlocks[0];
        Submit(InputBlocks[1].Select(Pages).Where(IsCorrectOrder).Select(Middle).Sum());
    }
    public override void PartB()
    {
        Page.OrderRules = InputBlocks[0];
        Submit(InputBlocks[1].Select(Pages).Where(IsIncorrectOrder).Select(CorrectOrder).Select(Middle).Sum());
    }
    static IEnumerable<Page> Pages(string input) => input.ExtractNumbers<int>().Select(Page.GetPage);
    static bool IsIncorrectOrder(IEnumerable<Page> pages) => !IsCorrectOrder(pages);
    static bool IsCorrectOrder(IEnumerable<Page> pages) => CorrectOrder(pages.ToHashSet()).SequenceEqual(pages);
    static IEnumerable<Page> CorrectOrder(IEnumerable<Page> pages) => CorrectOrder(pages.ToHashSet());
    static IEnumerable<Page> CorrectOrder(HashSet<Page> pages) => Page.Order.Where(pages.Contains);
    static int Middle(IEnumerable<Page> pages)
    {
        int[] values = pages.Select(x => x.value).ToArray();
        if (values.Length % 2 == 1) return values[(values.Length - 1) / 2]; //middle of odd length list
        return values[values.Length / 2]; //middle of even length list ([0, 1, 2, 3] => 2)
    }


    class Page
    {
        public static HashSet<Page> Pages = new();
        
        private static Page[]? _order;
        public static Page[] Order
        {
            get
            {
                _order ??= GetOrder();
                return _order;
            }
        }

        private static string[]? _orderRules;
        public static string[] OrderRules
        {
            get => _orderRules ?? Array.Empty<string>();
            set 
            { 
                if (value == _orderRules) return;
                Reset();
                _orderRules = value; 
            }
        }
        public static void Reset()
        {
            Pages.Clear();
            _order = null;
            _orderRules = null;
        }
        private static Page[] GetOrder()
        {
            foreach (string rule in OrderRules)
            {
                int[] nums = rule.ExtractNumbers<int>();
                GetPage(nums[0]).AddSubPage(GetPage(nums[1]));

                #region Debugging text
                continue;

                Console.WriteLine($"Step for rule {rule}:");
                for (int i = 0; i <= Pages.MaxBy(x => x.depth)?.depth; i++)
                {
                    IEnumerable<Page> pagesAtDepth = Pages.Where(x => x.depth == i);
                    Console.WriteLine($"Depth {i}: {string.Join(", ", pagesAtDepth)}");
                }
                Console.ReadLine();
                #endregion
            }
            return Pages.OrderBy(x => x.depth).ToArray();
        }
        public static Page GetPage(int num)
        {
            return Pages.FirstOrDefault(x => x.value == num) ?? new Page(num);
        }


        public int depth = 0;
        public int value;
        private readonly HashSet<Page> pagesAfter = new();
        public Page(int num)
        {
            value = num;
            Pages.Add(this);
        }
        public void AddSubPage(Page page)
        {
            pagesAfter.Add(page);
            page.UpdateDepth(depth + 1);
        }
        public void UpdateDepth(int newDepth)
        {
            if (newDepth <= depth) return;
            depth = newDepth;
            foreach (Page page in pagesAfter)
            {
                page.UpdateDepth(depth + 1);
            }
        }
        public override string ToString() => value.ToString();
    }
}

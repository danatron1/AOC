# AOC

This is a personal helper tool designed by danatron1 (me) to assist with [Advent of Code](https://adventofcode.com) puzzles. 
I've participated annually in AOC since 2019, although some older puzzle solutions aren't in this repo - this is because prior to creating this project, I began by solving puzzles as individual projects. Solutions for puzzles prior to 2019 are me going back to previous years and solving them for fun :]

## Notes on automation: 

This follows all the [subreddit automation guidelines](https://www.reddit.com/r/adventofcode/wiki/faqs/automation/).

All automation features are in the `WebsiteInteraction` class. 

All requests are made with the `User-Agent` string: ".NET/[version] (+via github.com/danatron1/AOC by [my email])". 

Input is only extracted once, and cached for subsequent uses. 
Instances of corruption are so rare that I manually delete the locally saved input, triggering a re-download. 
Inputs are not included in this repo (see .gitignore) as per [input sharing guidelines](https://www.reddit.com/r/adventofcode/wiki/faqs/copyright/inputs/).

Submissions are also automated, with each submission being logged. A submission request to the website is only made if:
* The answer is NOT null, empty string, white space, 1, 0, or -1.
* The answer is not one you've submitted before.
* The guess is within the range of possible answers (e.g. if a previous guess of 5000 received the response "too low", all guesses below 5000 are blocked)
* It's been more than 60 seconds since your last guess. (If it's been less, the program will wait, then submit)
* The puzzle isn't already solved.
* The puzzle release date isn't in the future (e.g. cannot submit for 2023-12-01 prior to that date EST)

Assuming all of those checks pass, a submission is made.

## Usage

Each day's solution is stored in its own class, formatted as D(day)_(year), e.g. D01_2015.cs

These files can be created automatically;
* `Day.Create(2022, 1);` will create the file D01_2022.cs
* `Day.CreateYear(2022);` will create 25 files, one for each day of the event.

To run the contents of these files, call `Day.Solve(2015, 1);` or `Day.SolveToday();`
* `Day.SolveToday();` will run today's solution, for if you're completing puzzles on release day. This method only works December 1st to December 25th.
* This executes the methods `PartA()` and `PartB()`. It always executes *both*, and in that order. Consider this incentive to write performant code :)
* You can also use the methods `Setup()`, `PartASetup()`, and `PartBSetup()` if desired, although you have no requirement to implement those. 
This can be useful to clear data you may not wish to carry between part A and B. `Setup()` is run on initialisation, the others are run before their respective parts.
Remember, it's the same class running both, so anything outside of the scope of `PartA()` will still exist when `PartB()` runs.
* You may also pass in a boolean value to track performance, e.g. `Day.Solve(2022, 1, true);` - this will tell you how long your code takes to execute. Default false.
* If this is run for a class that doesn't exist, one will be created first. Code will need recompiling if this happens.

You can copy anything you like to the clipboard using `Copy(object);` 
This also prints it. I mainly use this to substitute a `Submit` for if I want to view the answer before submitting it.

Answers can be automatically submitted to the AOC website using `Submit(object);`. 
Assuming it passes the checks detailed above, this will be submitted to the website, and the response (correct or not) will be displayed to you. 

### Accessing input

You can access the input using the parameter `Input` provided by the inherited class. This will be an array of the chosen input type - by default, `string`.

You can change the type of the input by adding a <Type Argument> to the inherited day class, 
e.g. changing `internal class D10_2022 : Day` to `internal class D10_2022 : Day<int>`. 
This will convert each row of the input to an int. `Input` will now be of type `int[]`. 
This assumes that the rows are able to be converted; an error will be thrown otherwise. 
If your puzzle was ([theoretically](https://adventofcode.com/2018/day/1)) to simply sum the values, then changing `Day` to `Day<int>` would allow you to simply use `Input.Sum()` to get the answer.

You can always access the original input with `InputRaw`, which will always be a `string[]`.

Single-line inputs, such as day 6 for [2021](https://adventofcode.com/2021/day/6) or [2022](https://adventofcode.com/2021/day/6), 
can be accessed easily with `InputLine` - this is identical to using `Input[0]`, but in my opinion looks nicer. 
Same type argument can be used to change this type.

Inputs with multiple sections separated by blank lines, such as [2021 day 4](https://adventofcode.com/2021/day/4) or [2022 day 1](https://adventofcode.com/2022/day/1), 
can be easily accessed using `InputBlocks`, which is a `T[][]` - essentially an array of `Input` arrays.

Inputs with a 2D array of values, such as [2021 day 9](https://adventofcode.com/2021/day/9) or [2022 day 8](https://adventofcode.com/2022/day/8), 
can be accessed using `Input2D`, which returns a `string[,]` of the input, also converted according to the Day type. (e.g. `int[,]`)
You can also alternatively use `Input2DJagged`, which is the exact same data, except as a `T[][]` instead of a `T[,]`. 
If you need to specify a split character (e.g. for a comma separated 2D array), you can use the method `GetInputForDay2D(",")` - cache the result. 

If a puzzle needs supplementary information you want to read from a file, use `ExtraInput`, which you can populate with any information you like. 
This is always a `string[]`

Finally, the file path of your cached input can be found using `InputPath`.

### Using example inputs

you can toggle whether or not a puzzle is using an example input by adding `useExampleInput = true;` to your part A or B solution. 
Setting it to true for part A will not cause part B to also use an example input, they must be set individually. 

Example inputs are useful for debugging and testing against the examples given on the AOC website. 
If you attempt to use one without one cached, it will prompt you for it; paste the example input and hit enter twice. 
If you wish to manually overwrite it to test a different input then the text file is found in the same location as your puzzle input, at `InputPath`. 

## Example

A puzzle that provides a list of numbers as an input, and asks you to add together all the even numbers, could be solved like this. 
This will automatically download the puzzle input, convert it to integers, and submit your result to the site. 

```cs
internal class D31_2999 : Day<int>
{
    public override void PartA()
    {
        Submit(Input.Where(n => n % 2 == 0).Sum());
    }
    public override void PartB()
    {
        //Imagine that this is a slightly harder version of the first part.
    }
}
```

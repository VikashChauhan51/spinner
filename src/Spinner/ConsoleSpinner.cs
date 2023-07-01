using System.Diagnostics;

namespace VSpinner;
public class ConsoleSpinner
{
    private const double DefaultDelay = 0.25;
    private Spinner spinner;
    private double? delay;
    private bool active;
    private ConsoleColor color;

    private static readonly IReadOnlyCollection<Spinner> supportedSpinners = new List<Spinner>()
    {
        new Spinner(SpinnerTypes.Dots, new string[]{ ":", "¨","."}),
        new Spinner(SpinnerTypes.Dots2, new string[]{ ".  ", ".. ", "...", ".. ", "   "}),
        new Spinner(SpinnerTypes.DotsScrolling, new string[]{ ":", "¨","."}),
        new Spinner(SpinnerTypes.DotsSimple, new string[]{ ":", ".",":",".",":",".",":"}),
        new Spinner(SpinnerTypes.DotsRolling, new string[]{ ".    ", " .   ", "  .  ", "   . ", "    .", "   . ", "  .  ", " .   ", " . "}),
        new Spinner(SpinnerTypes.DotsShifter, new string[]{ ":.:.:.:.:.:.:.:.:.:.:.:.:.:.:.:.:.:.:.:.:.:.:", ".:.:.:.:.:.:.:.:.:.:.:.:.:.:.:.:.:.:.:.:.:.:."}),
        new Spinner(SpinnerTypes.Classic, new string[]{ "-", "\\", "|","/"}),
        new Spinner(SpinnerTypes.Classic2, new string[]{ "--- | ---", "--- / ---", "--- - ---" ,"--- \\ ---"}),
        new Spinner(SpinnerTypes.Ping, new string[]{ "||.      ||", "|| .     ||", "||  .    ||", "||   .   ||", "||    .  ||", "||     . ||", "||      .||", "||     . ||", "||    .  ||", "||   .   ||", "||  .    ||", "|| .     |"}),
        new Spinner(SpinnerTypes.Balloon, new string[]{ "", ".","o","O","@","*"}),
        new Spinner(SpinnerTypes.Stars, new string[]{ "*", "ˣ", "ᵒ"}),
        new Spinner(SpinnerTypes.Stars2, new string[]{ "+", "x","*"}),
        new Spinner(SpinnerTypes.Flag, new string[]{"{|=__________________}","{_|=_________________}","{__|=________________}","{___|=_______________}","{____|=______________}","{_____|=_____________}","{______|=____________}","{_______|=___________}","{________|=__________}","{_________|=_________}","{__________|=________}","{___________|=_______}","{___________|=_______}","{____________|=______}","{_____________|=_____}","{______________|=____}","{_______________|=___}","{________________|=__}","{_________________|=_}","{__________________=|}","{_________________=|_}","{________________=|__}","{_______________=|___}","{______________=|____}","{_____________=|_____}","{____________=|______}","{___________=|_______}","{__________=|________}","{_________=|_________}","{________=|__________}","{_______=|___________}","{______=|____________}","{_____=|_____________}","{____=|______________}","{___=|_______________}","{__=|________________}","{_=|_________________}","{=|__________________}"}),
        new Spinner(SpinnerTypes.Bar, new string[]{ "[---   =   ---]","[---  ===  ---]","[--- ===== ---]","[---  ===  ---]","[---   =   ---]"}),
        new Spinner(SpinnerTypes.PercentageFive, new string[]{"[--- :: 5 :: ---]","[--- :: 10 :: ---]","[--- :: 15 :: ---]","[--- :: 20 :: ---]","[--- :: 25 :: ---]","[--- :: 30 :: ---]","[--- :: 35 :: ---]","[--- :: 40 :: ---]","[--- :: 45 :: ---]","[--- :: 50 :: ---]","[--- :: 55 :: ---]","[--- :: 60 :: ---]","[--- :: 65 :: ---]","[--- :: 70 :: ---]","[--- :: 75 :: ---]","[--- :: 80 :: ---]","[--- :: 85 :: ---]","[--- :: 90 :: ---]","[--- :: 95 :: ---]","[--- :: 100 :: ---]"}),
        new Spinner(SpinnerTypes.RugCrawler, new string[]{ "|:::::::::", ":|::::::::", "::|:::::::", ":::|::::::", "::::|:::::", ":::::|::::", "::::::|:::", ":::::::|::", "::::::::|:", ":::::::::|",":|::::::::", "::|:::::::", ":::|::::::", "::::|:::::", ":::::|::::", "::::::|:::", ":::::::|::", "::::::::|:" }),
        new Spinner(SpinnerTypes.HappyBeast, new string[]{ ":", "¨","."}),
        new Spinner(SpinnerTypes.Eyes, new string[]{ "###","¤ ¤","###","###","¤:¤", "###","###","¤-¤", "###"}),
        new Spinner(SpinnerTypes.Ooh, new string[]{ "ₒ", "o", "O","ᵒ"}),
        new Spinner(SpinnerTypes.Brackets, new string[]{ "/", "\\", "(", ")", "{", "}"}),
        new Spinner(SpinnerTypes.Pulse, new string[]{ "-    "," -   ","  -  ","   -" ,"    -"}),
        new Spinner(SpinnerTypes.HashFest, new string[]{ "#####", "_####", "#_###","##_##","###_#","####_"}),
        new Spinner(SpinnerTypes.Flippie, new string[]{ "_","`", "'", "´", "`", "^","´"}),
        new Spinner(SpinnerTypes.Loading, new string[]{ "L______", "_O_____", "__A____", "___D___", "____I__", "_____N_", "______G", "LOADING", "_______", "LOADING","_______"}),
        new Spinner(SpinnerTypes.Unspecified, new string[]{ "¤¤¤", "_¤¤", "¤_¤", "¤¤_", "¤¤¤","¤_¤","_¤¤"}),
        new Spinner(SpinnerTypes.Money, new string[]{ "$", "£", "€"}),
        new Spinner(SpinnerTypes.Ball, new string[]{ "o    ", " o   ", "  o  ", "   o ", "    o", "   o ", "  o  ", " o   ", " o "}),
        new Spinner(SpinnerTypes.Plus, new string[]{ "+", "x"}),
        new Spinner(SpinnerTypes.Crocodiles, new string[]{ "v", "<", "^",">"}),
        new Spinner(SpinnerTypes.Circle, new string[]{"/", "-", "\\","|"}),
        new Spinner(SpinnerTypes.WorkingDots, new string[]{ "Working.  ", "Working.. ", "Working...", "Working   "}),
        new Spinner(SpinnerTypes.WorkingCircle, new string[]{ "Working /", "Working -", "Working \\", "Working |"}),
        new Spinner(SpinnerTypes.LoadingDots, new string[]{"Loading.  ", "Loading.. ", "Loading...","Loading   "}),
        new Spinner(SpinnerTypes.LoadingCircle, new string[]{ "Loading /", "Loading -", "Loading \\","Loading |"}),
    };
    private readonly Stopwatch stopwatch = new();
    private readonly object objectLock = new();
    private static readonly Lazy<ConsoleSpinner> DefaultSpinner = new Lazy<ConsoleSpinner>(() => new ConsoleSpinner());

    public static ConsoleSpinner Default
    {
        get { return DefaultSpinner.Value; }
    }
    public ConsoleSpinner(SpinnerTypes spinnerType, ConsoleColor color, double? delay)
    {
        this.delay = delay;
        this.color = color;
        this.spinner = supportedSpinners.FirstOrDefault(x => x.Name == spinnerType) ?? throw new KeyNotFoundException(nameof(spinnerType));

    }

    public ConsoleSpinner()
    {
        this.delay = DefaultDelay;
        this.color = ConsoleColor.White;
        this.spinner = supportedSpinners.FirstOrDefault(x => x.Name == SpinnerTypes.Classic) ?? throw new KeyNotFoundException(nameof(SpinnerTypes.Classic));
    }

    public ConsoleSpinner(SpinnerTypes spinnerType)
    {
        this.delay = DefaultDelay;
        this.color = ConsoleColor.White;
        this.spinner = supportedSpinners.FirstOrDefault(x => x.Name == spinnerType) ?? throw new KeyNotFoundException(nameof(spinnerType));
    }

    public ConsoleSpinner(SpinnerTypes spinnerType, ConsoleColor color) : this(spinnerType, color, DefaultDelay)
    {
        this.delay = DefaultDelay;
        this.color = color;
        this.spinner = supportedSpinners.FirstOrDefault(x => x.Name == spinnerType) ?? throw new KeyNotFoundException(nameof(spinnerType));
    }
    public ConsoleSpinner SetDelaySeconds(double delay)
    {
        this.delay = delay;
        return this;
    }

    public ConsoleSpinner SetColor(ConsoleColor color)
    {
        this.color = color;
        return this;
    }

    public ConsoleSpinner SetSpinner(SpinnerTypes spinnerType)
    {
        this.spinner = supportedSpinners.FirstOrDefault(x => x.Name == spinnerType) ?? throw new KeyNotFoundException(nameof(spinnerType));
        return this;
    }


    public async Task Start(int timeout)
    {
        if (active)
            return;

        bool cursorVisibility = OperatingSystem.IsWindows() ? Console.CursorVisible : true;

        try
        {

            active = true;
            int counter = -1;

            if (timeout > 0) stopwatch.Start();

            if (OperatingSystem.IsWindows())
                Console.CursorVisible = false;

            Console.ForegroundColor = color;
            while (active)
            {
                PrintSpinners(ref counter);
                if (timeout > 0 && stopwatch.Elapsed.Seconds >= timeout) Stop();
                await Task.Delay(TimeSpan.FromSeconds(delay ?? 0));
            }


        }
        finally
        {

            Reset(cursorVisibility);
        }
    }


    public async Task Start(Task task)
    {
        if (active || task is null)
            return;

        bool cursorVisibility = OperatingSystem.IsWindows() ? Console.CursorVisible : true;

        try
        {
            int counter = -1;

            if (OperatingSystem.IsWindows())
                Console.CursorVisible = false;

            Console.ForegroundColor = color;
            while (!task.IsCompleted)
            {
                PrintSpinners(ref counter);
                await Task.Delay(TimeSpan.FromSeconds(delay ?? 0));
            }


        }
        finally
        {

            Reset(cursorVisibility);
        }
    }

    public async Task Start(ValueTask task)
    {
        if (active)
            return;

        bool cursorVisibility = OperatingSystem.IsWindows() ? Console.CursorVisible : true;

        try
        {
            int counter = -1;

            if (OperatingSystem.IsWindows())
                Console.CursorVisible = false;

            Console.ForegroundColor = color;
            while (!task.IsCompleted)
            {
                PrintSpinners(ref counter);
                await Task.Delay(TimeSpan.FromSeconds(delay ?? 0));
            }


        }
        finally
        {
            Reset(cursorVisibility);
        }
    }



    private void PrintSpinners(ref int counter)
    {
        counter++;
        if (counter >= spinner.Sequence.Length)
            counter = 0;

        lock (objectLock)
        {
            Console.Write(spinner.Sequence[counter]);
            Console.SetCursorPosition(Math.Max(0, Console.CursorLeft - spinner.Sequence[counter].Length), Console.CursorTop);
        }

    }

    private void Reset(bool cursorVisibility)
    {
        if (OperatingSystem.IsWindows())
            Console.CursorVisible = cursorVisibility;

        Stop();

    }
    private void Stop()
    {
        this.active = false;
        Console.ResetColor();
        stopwatch.Reset();
    }
}

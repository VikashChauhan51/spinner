using System.Diagnostics;

namespace Spinner;
public class ConsoleSpinner
{

    private const int DefaultTime = 5;
    private const double DefaultDelay = 0.25;
    private Spinner spinner;
    private int? time;
    private double? delay;
    private bool active = false;
    private ConsoleColor color;
    private readonly List<Spinner> spinners = new List<Spinner>();
    private readonly Stopwatch stopwatch = new Lazy<Stopwatch>(() => new Stopwatch()).Value;
    private readonly object objectLock = new();
    private static readonly Lazy<ConsoleSpinner> DefaultSpinner = new Lazy<ConsoleSpinner>(
       () => new ConsoleSpinner());

    public static ConsoleSpinner Default
    {
        get { return DefaultSpinner.Value; }
    }

    public ConsoleSpinner(SpinnerTypes spinnerType, ConsoleColor color, int? time, double? delay)
    {
        this.time = time;
        this.delay = delay;
        this.color = color;
        LoadSpinners();
        this.spinner = spinners.First(x => x.Name == spinnerType);

    }

    public ConsoleSpinner() : this(SpinnerTypes.Circle, ConsoleColor.White, DefaultTime, DefaultDelay)
    {

    }

    public ConsoleSpinner(SpinnerTypes spinnerType) : this(spinnerType, ConsoleColor.White, DefaultTime, DefaultDelay)
    {

    }

    public ConsoleSpinner(SpinnerTypes spinnerType, ConsoleColor color) : this(spinnerType, color, DefaultTime, DefaultDelay)
    {

    }
    public ConsoleSpinner SetDelaySeconds(double delay)
    {
        this.delay = delay;
        return this;
    }

    public ConsoleSpinner SetTimeoutSeconds(int time)
    {
        this.time = time;
        return this;
    }

    public ConsoleSpinner SetColor(ConsoleColor color)
    {
        this.color = color;
        return this;
    }

    public ConsoleSpinner SetSpinner(SpinnerTypes spinnerType)
    {
        this.spinner = spinners.First(x => x.Name == spinnerType);
        return this;
    }


    public async Task Start()
    {
        if (active)
            return;

        bool cursorVisibility = OperatingSystem.IsWindows() ? Console.CursorVisible : true;

        try
        {

            active = true;
            int counter = -1;

            if (time > 0) stopwatch.Start();

            if (OperatingSystem.IsWindows())
                Console.CursorVisible = false;

            Console.ForegroundColor = color;
            while (active)
            {
                PrintSpinners(ref counter);
                if (time > 0 && stopwatch.Elapsed.Seconds >= time) Stop();
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
    private void LoadSpinners()
    {
        var spinnersText = Resource.spinners;
        var lines = spinnersText.Split(new[] { Environment.NewLine }, StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);

        foreach (var line in lines.Where(l => !string.IsNullOrWhiteSpace(l)))
        {
            var splitByComma = line.Split(',');
            spinners.Add(new Spinner((SpinnerTypes)Enum.Parse(typeof(SpinnerTypes), splitByComma[0], true), splitByComma.ToList().Skip(1).ToArray()));
        }

    }
}

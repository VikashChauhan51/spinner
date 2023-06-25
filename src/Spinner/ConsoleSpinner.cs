using System.Diagnostics;
using System.Resources;
using System.Threading;
using System.Xml.Linq;

namespace Spinner;
public class ConsoleSpinner
{

    private const int DefaultTime = 5;
    private const double DefaultDelay = 0.25;
    private Spinner spinner;
    private int? time;
    private double? delay;
    private Task? task;
    private ValueTask? valueTask;
    private bool active = false;
    private ConsoleColor color;
    private readonly List<Spinner> spinners = new List<Spinner>();
    private readonly Stopwatch stopwatch = new Lazy<Stopwatch>(() => new Stopwatch()).Value;
    private readonly static object objectLock = new();
    private static readonly Lazy<ConsoleSpinner> DefaultSpinner = new Lazy<ConsoleSpinner>(
       () => new ConsoleSpinner());

    public static ConsoleSpinner Default
    {
        get { return DefaultSpinner.Value; }
    }

    public ConsoleSpinner(SpinnerTypes spinnerType, ConsoleColor color, int? time, double? delay, Task? task, ValueTask? valueTask)
    {
        this.time = time;
        this.delay = delay;
        this.task = task;
        this.valueTask = valueTask;
        this.color = color;
        LoadSpinners();
        this.spinner = spinners.First(x => x.Name == spinnerType);

    }

    public ConsoleSpinner() : this(SpinnerTypes.Dots2, ConsoleColor.White, DefaultTime, DefaultDelay, null, null)
    {

    }

    public ConsoleSpinner(SpinnerTypes spinnerType) : this(spinnerType, ConsoleColor.White, null, DefaultDelay, null, null)
    {

    }

    public ConsoleSpinner(SpinnerTypes spinnerType, ConsoleColor color) : this(spinnerType, color, null, DefaultDelay, null, null)
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

    public ConsoleSpinner SetDoWork(Task task)
    {
        this.task = task;
        return this;
    }

    public ConsoleSpinner SetDoWork(ValueTask task)
    {
        this.valueTask = task;
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
        bool cursorVisibility = OperatingSystem.IsWindows() ? Console.CursorVisible : true;

        try
        {
            active = true;
            int counter = -1;

            if (time > 0) stopwatch.Start();

            if (OperatingSystem.IsWindows())
                Console.CursorVisible = false;

            Console.ForegroundColor = color;
            while (!TaskCompleted() ?? active)
            {
                PrintSpinners(ref counter);
                if (task == null && valueTask == null && time > 0 && stopwatch.Elapsed.Seconds >= time) Stop();
                await Task.Delay(TimeSpan.FromSeconds(delay ?? 0));
            }


        }
        finally
        {

            if (OperatingSystem.IsWindows())
                Console.CursorVisible = cursorVisibility;

            Console.ResetColor();
        }
    }

    private bool? TaskCompleted()
    {
        var taskCompleted = task?.IsCompleted;
        var valueTaskCompleted = valueTask?.IsCompleted;

        if (taskCompleted != null && valueTaskCompleted != null)
        {
            return (bool)taskCompleted && (bool)taskCompleted;
        }

        return taskCompleted ?? valueTaskCompleted;
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
    private void Stop()
    {
        this.active = false;
        this.time = null;
        this.delay = null;
        this.task = null;
        this.valueTask = null;
        this.color = ConsoleColor.White;
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

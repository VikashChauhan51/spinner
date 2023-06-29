// See https://aka.ms/new-console-template for more information
using Spinner;

await ConsoleSpinner.Default.Start(5);

await Demo();

static void DoWork()
{
    for (int i = 0; i < 5; i++)
    {
        Thread.Sleep(100);
    }
}
static async ValueTask DoIt()
{
    await Task.Run(() => DoWork()).ConfigureAwait(false);
}

static async Task DoTask()
{
    for (int i = 0; i < 10; i++)
    {
        await Task.Delay(100);

    }
}

static async Task Spin(SpinnerTypes type, ConsoleColor color, int time)
{
    System.Console.WriteLine(type.ToString());
    await new ConsoleSpinner(type).SetColor(color).SetDelaySeconds(1).Start(time);
    System.Console.Clear();
    System.Console.WriteLine();
}
static async Task Demo()
{
    var task1 = DoIt();
    var task2 = DoTask();
    await new ConsoleSpinner(SpinnerTypes.Flippie, ConsoleColor.Cyan)
        .Start(task2);

    Console.Clear();
    var task3 = DoTask();
    await new ConsoleSpinner(SpinnerTypes.Loading, ConsoleColor.Yellow)
        .Start(task3);

    Console.Clear();

    await Spin(SpinnerTypes.Classic, ConsoleColor.Yellow, 10);
    await Spin(SpinnerTypes.Dots2, ConsoleColor.Yellow, 10);
    await Spin(SpinnerTypes.Plus, ConsoleColor.Cyan, 5);
    await Spin(SpinnerTypes.Circle, ConsoleColor.Magenta, 5);
    await Spin(SpinnerTypes.Money, ConsoleColor.DarkGray, 5);

    await Spin(SpinnerTypes.LoadingCircle, ConsoleColor.DarkGray, 15);
    await Spin(SpinnerTypes.LoadingDots, ConsoleColor.DarkGray, 15);
}

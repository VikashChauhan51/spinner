// See https://aka.ms/new-console-template for more information
using Spinner;

await ConsoleSpinner.Default.Start();

await Demo();

static void DoWork()
{
    for (int i = 0; i < 25; i++)
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
    for (int i = 0; i < 30; i++)
    {
        await Task.Delay(100);

    }
}

static async Task Spin(SpinnerTypes type, int time)
{
    System.Console.WriteLine(type.ToString());
    await new ConsoleSpinner(type).SetColor(ConsoleColor.Magenta).SetTimeoutSeconds(time).SetDelaySeconds(1).Start();
    System.Console.Clear();
    System.Console.WriteLine();
}
static async Task Demo()
{
    await new ConsoleSpinner(SpinnerTypes.Classic, ConsoleColor.Cyan)
        .SetDoWork(DoIt())
        .SetDoWork(DoTask())
        .Start();

    await new ConsoleSpinner(SpinnerTypes.Classic, ConsoleColor.Yellow)
        .SetDoWork(DoTask())
        .Start();

    Console.Clear();
    Console.WriteLine();

    await Spin(SpinnerTypes.Dots2, 4);
    await Spin(SpinnerTypes.Plus, 4);
    await Spin(SpinnerTypes.Circle, 10);
    await Spin(SpinnerTypes.Money, 10);
}

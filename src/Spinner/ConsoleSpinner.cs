using System.Diagnostics;

namespace VSpinner;
public class ConsoleSpinner
{
    private const double DefaultDelay = 0.25;
    private Spinner spinner;
    private double? delay;
    private bool active;
    private ConsoleColor color;
    private static List<Spinner> spinners = new List<Spinner>();
    private readonly Stopwatch stopwatch = new Lazy<Stopwatch>(() => new Stopwatch()).Value;
    private readonly object objectLock = new();
    private static readonly ConsoleSpinner DefaultSpinner = new ConsoleSpinner();

    public static ConsoleSpinner Default
    {
        get { return DefaultSpinner; }
    }

    static ConsoleSpinner()
    {
        LoadSpinners();
    }
    public ConsoleSpinner(SpinnerTypes spinnerType, ConsoleColor color, double? delay)
    {
        this.delay = delay;
        this.color = color;
        
        this.spinner = spinners.First(x => x.Name == spinnerType);

    }

    public ConsoleSpinner() : this(SpinnerTypes.Circle, ConsoleColor.White, DefaultDelay)
    {

    }

    public ConsoleSpinner(SpinnerTypes spinnerType) : this(spinnerType, ConsoleColor.White, DefaultDelay)
    {

    }

    public ConsoleSpinner(SpinnerTypes spinnerType, ConsoleColor color) : this(spinnerType, color, DefaultDelay)
    {

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
        this.spinner = spinners.First(x => x.Name == spinnerType);
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
    private static void LoadSpinners()
    {
        var spinnersText = @"Dots,:,¨,.
Dots2,.   ,.. ,...,   
DotsScrolling,.  ,.. ,...,.. ,.  
DotsSimple,:,.,:,.,:,.,:
DotsRolling,.     , .    ,  .   ,   .  ,    . ,     .,    . ,   .  ,  .   , .    
DotsShifter,:.:.:.:.:.:.:.:.:.:.:.:.:.:.:.:.:.:.:.:.:.:.:,.:.:.:.:.:.:.:.:.:.:.:.:.:.:.:.:.:.:.:.:.:.:.
Classic,-,\,|,/
Classic2,--- | ---,--- / ---,--- - ---,--- \ ---
Ping,||.      ||,|| .     ||,||  .    ||,||   .   ||,||    .  ||,||     . ||,||      .||,||     . ||,||    .  ||,||   .   ||,||  .    ||,|| .     ||,||.      ||,
Balloon, ,.,o,O,@,*, 
Stars,*,ˣ,ᵒ
Stars2,+,x,*
Bar,[---   =   ---],[---  ===  ---],[--- ===== ---],[---  ===  ---],[---   =   ---]
Flag,{|=__________________},{_|=_________________},{__|=________________},{___|=_______________},{____|=______________},{_____|=_____________},{______|=____________},{_______|=___________},{________|=__________},{_________|=_________},{__________|=________},{___________|=_______},{___________|=_______},{____________|=______},{_____________|=_____},{______________|=____},{_______________|=___},{________________|=__},{_________________|=_},{__________________=|},{_________________=|_},{________________=|__},{_______________=|___},{______________=|____},{_____________=|_____},{____________=|______},{___________=|_______},{__________=|________},{_________=|_________},{________=|__________},{_______=|___________},{______=|____________},{_____=|_____________},{____=|______________},{___=|_______________},{__=|________________},{_=|_________________},{=|__________________}
PercentageFive,[--- :: 0  :: ---],[--- :: 5  :: ---],[--- :: 10 :: ---],[--- :: 15 :: ---],[--- :: 20 :: ---],[--- :: 25 :: ---],[--- :: 30 :: ---],[--- :: 35 :: ---],[--- :: 40 :: ---],[--- :: 45 :: ---],[--- :: 50 :: ---],[--- :: 55 :: ---],[--- :: 60 :: ---],[--- :: 65 :: ---],[--- :: 70 :: ---],[--- :: 75 :: ---],[--- :: 80 :: ---],[--- :: 85 :: ---],[--- :: 90 :: ---],[--- :: 95 :: ---],[--- :: 100 :: ---],
RugCrawler,|:::::::::,:|::::::::,::|:::::::,:::|::::::,::::|:::::,:::::|::::,::::::|:::,:::::::|::,::::::::|:,:::::::::|,::::::::|:,:::::::|::,::::::|:::,:::::|::::,::::|:::::,:::|::::::,::|:::::::,:|::::::::,
Eyes,### ¤ ¤ ###,### ¤:¤ ###,### ¤-¤ ###,
HappyBeast,!_-_-_-_-_-_-_-_-_-_-_-_-_-_!,!:=>-_-_-_-_-_-_-_-_-_-_-_-_!,!_:=>_-_-_-_-_-_-_-_-_-_-_-_!,!_-:=>-_-_-_-_-_-_-_-_-_-_-_!,!_-_:=>_-_-_-_-_-_-_-_-_-_-_!,!_-_-:=>-_-_-_-_-_-_-_-_-_-_!,!_-_-_:=>_-_-_-_-_-_-_-_-_-_!,!_-_-_-:=>-_-_-_-_-_-_-_-_-_!,!_-_-_-_:=>_-_-_-_-_-_-_-_-_!,!_-_-_-_-:=>-_-_-_-_-_-_-_-_!,!_-_-_-_-_:=>_-_-_-_-_-_-_-_!,!_-_-_-_-_-:=>-_-_-_-_-_-_-_!,!_-_-_-_-_-_:=>_-_-_-_-_-_-_!,!_-_-_-_-_-_-:=>-_-_-_-_-_-_!,!_-_-_-_-_-_-_:=>_-_-_-_-_-_!,!_-_-_-_-_-_-_-:=>-_-_-_-_-_!,!_-_-_-_-_-_-_-_:=>_-_-_-_-_!,!_-_-_-_-_-_-_-_-:=>-_-_-_-_!,!_-_-_-_-_-_-_-_-_:=>_-_-_-_!,!_-_-_-_-_-_-_-_-_-:=>-_-_-_!,!_-_-_-_-_-_-_-_-_-_:=>_-_-_!,!_-_-_-_-_-_-_-_-_-_-:=>-_-_!,!_-_-_-_-_-_-_-_-_-_-_:=>_-_!,!_-_-_-_-_-_-_-_-_-_-_-:=>-_!,!_-_-_-_-_-_-_-_-_-_-_-_:=>_!,!_-_-_-_-_-_-_-_-_-_-_-_-:=>!,!_-_-_-_-_-_-_-_-_-_-_-_-<=:!,!_-_-_-_-_-_-_-_-_-_-_-_<=:_!,!_-_-_-_-_-_-_-_-_-_-_-<=:-_!,!_-_-_-_-_-_-_-_-_-_-_<=:_-_!,!_-_-_-_-_-_-_-_-_-_-<=:-_-_!,!_-_-_-_-_-_-_-_-_-_<=:_-_-_!,!_-_-_-_-_-_-_-_-_-<=:-_-_-_!,!_-_-_-_-_-_-_-_-_<=:_-_-_-_!,!_-_-_-_-_-_-_-_-<=:-_-_-_-_!,!_-_-_-_-_-_-_-_<=:_-_-_-_-_!,!_-_-_-_-_-_-_-<=:-_-_-_-_-_!,!_-_-_-_-_-_-_<=:_-_-_-_-_-_!,!_-_-_-_-_-_-<=:-_-_-_-_-_-_!,!_-_-_-_-_-_<=:_-_-_-_-_-_-_!,!_-_-_-_-_-<=:-_-_-_-_-_-_-_!,!_-_-_-_-_<=:_-_-_-_-_-_-_-_!,!_-_-_-_-<=:-_-_-_-_-_-_-_-_!,!_-_-_-_<=:_-_-_-_-_-_-_-_-_!,!_-_-_-<=:-_-_-_-_-_-_-_-_-_!,!_-_-_<=:_-_-_-_-_-_-_-_-_-_!,!_-_-<=:-_-_-_-_-_-_-_-_-_-_!,!_-_<=:_-_-_-_-_-_-_-_-_-_-_!,!_-<=:-_-_-_-_-_-_-_-_-_-_-_!,!_<=:_-_-_-_-_-_-_-_-_-_-_-_!,!<=:-_-_-_-_-_-_-_-_-_-_-_-_!,
Ooh,ₒ,o,O,ᵒ
Brackets,/,\,(,),{,}
HashFest,#####,_####,#_###,##_##,###_#,####_
Pulse,-    , -   ,  -  ,   - ,    -
Matrix,|: :: :: :: ::,:| :: :: :: ::,:: |: :: :: ::,:: :| :: :: ::,:: :: |: :: ::,:: :: :| :: ::,:: :: :: |: ::,:: :: :: :| ::,:: :: :: :: |:,:: :: :: :: :|,:: :: :: :: |:,:: :: :: :| ::,:: :: :: |: ::,:: :: :| :: ::,:: :: |: :: ::,:: :| :: :: ::,:: |: :: :: ::,:| :: :: :: ::,
Flippie,_,`,',´,`,^,´
Loading,L______,_O_____,__A____,___D___,____I__,_____N_,______G,LOADING,_______,LOADING,_______
Unspecified,¤¤¤,_¤¤,¤_¤,¤¤_,¤¤¤,¤_¤,_¤¤
Money,$,€,£
Ball,o    , o   ,  o  ,   o ,    o,   o ,  o  , o   , o   
Plus,+,x
Crocodiles,v,<,^,>
Circle,/,-,\,|
WorkingDots,Working.  ,Working.. ,Working...,Working   ,             
WorkingCircle,Working /,Working -,Working \,Working |
LoadingDots,Loading.  ,Loading.. ,Loading...,Loading   ,             
LoadingCircle,Loading /,Loading -,Loading \,Loading |
";

        var lines = spinnersText.Split(new[] { Environment.NewLine }, StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);

        foreach (var line in lines.Where(l => !string.IsNullOrWhiteSpace(l)))
        {
            var splitByComma = line.Split(',');
            spinners.Add(new Spinner((SpinnerTypes)Enum.Parse(typeof(SpinnerTypes), splitByComma[0], true), splitByComma.ToList().Skip(1).ToArray()));
        }

    }
}

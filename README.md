# Spinner
Console spinners with simple and minimal api

## Quick Start Example

```C#
using Spinner;
// show spinner for 5 seconds
await ConsoleSpinner.Default.Start(5);

```
## Task Example

```C#
using Spinner;

static async Task DoTask()
{
    for (int i = 0; i < 10; i++)
    {
        await Task.Delay(100);

    }
}

var task1 = DoIt();

// show spinner till task is not completed
await ConsoleSpinner.Default.Start(task1);

```

## Value Task Example

```C#
using Spinner;

static async ValueTask DoTask()
{
    for (int i = 0; i < 10; i++)
    {
        await Task.Delay(100);

    }
}

var task1 = DoIt();

// show spinner till valuetask is not completed
await ConsoleSpinner.Default.Start(task1);

```

## Set Custom Color & Delay Example

```C#
 await new ConsoleSpinner(SpinnerTypes.Loading)
        .SetColor(ConsoleColor.Yellow)
        .SetDelaySeconds(1)
        .Start(5);
```
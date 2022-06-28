using Injectors;
using Injectors.Loggers;

Console.WriteLine("Start");
new Injector<StopWatchInjector,ConsoleLogger>().Build(@"..\..\..\..\Dummy\bin\debug\net6.0\Dummy.dll");
Console.WriteLine("End");
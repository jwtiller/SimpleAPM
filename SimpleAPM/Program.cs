using Injectors;
Console.WriteLine("Start");
new ConsoleStopWatchInjector().Run(@"..\..\..\..\Dummy\bin\debug\net6.0\Dummy.dll");
Console.WriteLine("End");
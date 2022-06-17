using Dummy;

public static class Program
{
    private static readonly SomeService _someService = new();
    public static void Main(string[] args)
    {
        Console.WriteLine("Start");
        _someService.Execute();
        Console.WriteLine("End");
    }
} 
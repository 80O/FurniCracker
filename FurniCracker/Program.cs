using System;

namespace FurniCracker
{
    class Program
    {
        static void Main(string[] args)
        {
            var currentDomain = AppDomain.CurrentDomain;
            currentDomain.UnhandledException += new UnhandledExceptionEventHandler(ExceptionHandler);

            Cracker.Initialize();
        }

        static void ExceptionHandler(object sender, UnhandledExceptionEventArgs args)
        {
            var e = (Exception)args.ExceptionObject;
            Logging.WriteLine("Application Error Occured: " + e.ToString(), ConsoleColor.Red);
            Console.ReadKey(true);
        }
    }
}

using System;

namespace RouteUpdateConsoleApplication
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Route Update Process");
            Console.WriteLine("List of Processes :- ");
            Console.WriteLine("1. Route Import");
            Console.WriteLine("2. PIB Import");
            Console.WriteLine("3. Route Generation");
            Console.WriteLine("4. Route Recreation");
            Console.WriteLine("5. Run PreAutoUpdate ");
            Console.WriteLine("Please enter the Process Number :- ");
            int input = Convert.ToInt32(Console.ReadLine());
            if (input >= 1 && input <= 5)
            {
                Helper.RouteUpdate.RunRouteUpdate(input);
            }
            else
                Console.WriteLine("Process Id is not Valid, please restart the program and enter vaild process Id");
        }
    }
}

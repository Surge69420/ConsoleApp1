using System;
using Microsoft.Extensions.DependencyInjection;


namespace ConsoleApp1
{   
    class Program
    {  


        static void ListOrInsert(TimedBackgroundService service)
        {
            service.Line();
            Console.WriteLine($"1) Insert Customer {Environment.NewLine}2) List Customers" );
            int ReadLine;
            try
            {
                ReadLine = Convert.ToInt32(Console.ReadKey(true).KeyChar.ToString());

                switch (ReadLine)
                {
                    case 1:
                        service.Line();
                        Console.WriteLine("Qual o nome?");
                        string name = Console.ReadLine();
                        Console.WriteLine("Qual a morada?");
                        string address = Console.ReadLine();
                        Console.WriteLine("Qual o contribuinte?");
                        int taxpayerId = Convert.ToInt32( Console.ReadLine());
                        Console.WriteLine("Qual o código postal?");
                        int pcode = Convert.ToInt32(Console.ReadLine());
                        service.AddToDatabase(name, address, taxpayerId, pcode);
                        break;
                    case 2:
                        service.queryDatabase();
                        break ;
                    default:
                        Console.WriteLine(Environment.NewLine + "incorrect");
                        break;
                }
                ListOrInsert(service);
            }
            catch (Exception e) {
                service.Line();
                Console.WriteLine($"{Environment.NewLine}Enter a Number {Environment.NewLine}"+e.Message);
                ListOrInsert(service);
            }
        } 


        static void Main(string[] args)
        {
            var services = new ServiceCollection();
            services.AddLogging();
            services.AddTransient<TimedBackgroundService>();
            var serviceProvider = services.BuildServiceProvider();
            var backgroundService = serviceProvider.GetRequiredService<TimedBackgroundService>();
            ListOrInsert(backgroundService);
        }

    }

}

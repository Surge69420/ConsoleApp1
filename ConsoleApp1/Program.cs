using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.SqlServer;
using Microsoft.EntityFrameworkCore.InMemory;
using Microsoft.Extensions.Options;
using ConsoleApp1;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Net;


namespace ConsoleApp1
{   
    class Program
    {  


        static void ListOrInsert(TimedBackgroundService service)
        {
            
            Console.WriteLine($"1) Insert Customer {Environment.NewLine}2) List Customers" );
            int ReadLine;
            try
            {
                ReadLine = Convert.ToInt32(Console.ReadKey(true).KeyChar.ToString());
                switch (ReadLine)
                {
                    case 1:
                        Console.WriteLine(Environment.NewLine + "Qual o nome?");
                        string name = Console.ReadLine();
                        Console.WriteLine(Environment.NewLine + "Qual a morada?");
                        string address = Console.ReadLine();
                        Console.WriteLine(Environment.NewLine + "Qual o contribuinte?");
                        int taxpayerId = Convert.ToInt32( Console.ReadLine());
                        Console.WriteLine(Environment.NewLine + "Qual o código postal?");
                        int pcode = Convert.ToInt32(Console.ReadLine());
                        service.AddToDatabase(name, address, taxpayerId, pcode);
                        break;
                    case 2:
                        service.queryDatabase();
                        break ;
                    default:
                        Console.WriteLine(Environment.NewLine + "incoorect");
                        break;
                }
                ListOrInsert(service);
            }
            catch (Exception e) { 
                Console.WriteLine ($"{Environment.NewLine}Enter a Number {Environment.NewLine}"+e.Message);
                ListOrInsert(service);
            }
        } 


        static async Task Main(string[] args)
        {
            var services = new ServiceCollection();

            services.AddLogging();

            services.AddTransient<TimedBackgroundService>();
 
            var serviceProvider = services.BuildServiceProvider();

            var backgroundService = serviceProvider.GetRequiredService<TimedBackgroundService>();

            backgroundService.Log();

            ListOrInsert(backgroundService);
        }

    }
    public class MemoryContext : DbContext
    {
        public DbSet<TaxPayer> TaxPayers { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server=DESKTOP-7R8OIVU\\SQLEXPRESS01;Database=TaxPayers;Trusted_Connection=True;TrustServerCertificate=True;");

        }
    }

    public class TaxPayer
    {
        public string name { get; set; }
        public string address { get; set; }
        public int taxPayerId { get; set; }
        public int postalCode { get; set; }
    }

}

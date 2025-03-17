using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.IO;
using System.Text.Json;
using System.Text;
using System.Data.SqlClient;
namespace ConsoleApp1
{
    public class serverConnectionJSON
    {
        public string ServerName { get; set; }
        public string ServerUser { get; set; }
        public string DataBaseName { get; set; }
        public string Variables { get; set; }
    }
    public class MemoryContext : DbContext
    {
        public void createJSON(string json)
        {
            using (FileStream fs = File.Create(json))
            {
                byte[] info = new UTF8Encoding(true).GetBytes("{\r\n    \"ServerName\": \"DESKTOP-7R8OIVU\",\r\n    \"ServerUser\": \"SQLEXPRESS01\",\r\n    \"DataBaseName\": \"TaxPayers\",\r\n    \"Variables\": \"Trusted_Connection=True;TrustServerCertificate=True;\"\r\n}\r\n");
                fs.Write(info, 0, info.Length);
            }
        }
        public string ReadFile(string json)
        {
            if (!File.Exists(json))
            {
                createJSON(json);
            }
            using (StreamReader sr = File.OpenText(json))
            {
                return sr.ReadToEnd();
            }
        }
        public serverConnectionJSON GetJSON(string json)
        {
            var jsonString = ReadFile(json);
            var js = new serverConnectionJSON();
            try
            {
                js = JsonSerializer.Deserialize<serverConnectionJSON>(jsonString);
            }
            catch (JsonException e)
            {
                Console.WriteLine($"------------------------------------------");
                Console.WriteLine("Failed reading json" + e.Message);
                Console.WriteLine($"{Environment.NewLine}1 | Rewrite JSON to Defauls ");
                Console.WriteLine("2 | Exit and fix the JSON yourself");
                int ReadLine = Convert.ToInt32(Console.ReadKey(true).KeyChar.ToString());
                switch(ReadLine)
                {
                    case 1:
                        Console.WriteLine("Rewriting JSON");
                        createJSON(json);
                        jsonString = ReadFile(json);
                        js = JsonSerializer.Deserialize<serverConnectionJSON>(jsonString);
                        break;
                    case 2:
                        Environment.Exit(-1);
                        break;
                    default:
                        js = GetJSON(json);
                        break;
                }

            }
            return js;
        }
        public DbSet<TaxPayer> TaxPayers { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            serverConnectionJSON serverSettings = GetJSON("serverSettings.json");
                optionsBuilder.UseSqlServer($"Server={serverSettings.ServerName}\\{serverSettings.ServerUser};Database={serverSettings.DataBaseName};{serverSettings.Variables}");
        

        }
    }

    public class TaxPayer
    {
        public string name { get; set; }
        public string address { get; set; }
        public int taxPayerId { get; set; }
        public int postalCode { get; set; }
    }
    public class TimedBackgroundService : BackgroundService
    {

        private readonly ILogger<TimedBackgroundService> _logger;
       public  void Line()
        {
            Console.WriteLine($"------------------------------------------");
        }
        public TimedBackgroundService(ILogger<TimedBackgroundService> logger)
        {
            _logger = logger;
        }
        public void UpdateTable(int taxPayerId)
        {
            using (var context = new MemoryContext())
            {
                var payer = context.TaxPayers.First(a => a.taxPayerId == taxPayerId);
          
                Line();
                Console.WriteLine($"1 | Alterar nome{Environment.NewLine}2 | Alterar morda{Environment.NewLine}3 | Alterar codigo Postal{Environment.NewLine}4 | Apagar");
                int ReadLine = Convert.ToInt32(Console.ReadKey(true).KeyChar.ToString());
                switch (ReadLine)
                {
                    case 1:
                        Console.WriteLine(Environment.NewLine + "Qual o nome?");
                        payer.name = Console.ReadLine();
                        break;
                    case 2:
                        Console.WriteLine(Environment.NewLine + "Qual a morada?");
                        payer.address = Console.ReadLine();
                        break;
                    case 3:
                        Console.WriteLine(Environment.NewLine + "Qual o código postal?");
                        payer.postalCode = Convert.ToInt32(Console.ReadLine());
                        break;
                    case 4:
                        context.Remove(payer);
                        Console.WriteLine(Environment.NewLine + "Excluído");
                        break;
                    default:
                        Console.WriteLine("Incorreto");
                        break;
                }
                context.SaveChanges();
            }
        }
        public void queryDatabase()
        {

            using (var context = new MemoryContext())
            {
                var payers = context.TaxPayers.ToArray();

                for (int i = 0; i < payers.Length; i++)
                {
                    var payer = payers[i];
                    Line();
                    Console.WriteLine($"{i + 1} | {payer.name} | {payer.taxPayerId}");
                }
                var EditVar = Convert.ToInt32(Console.ReadKey(true).KeyChar.ToString()) - 1;
                if (EditVar <= payers.Length)
                {
                    UpdateTable(payers[EditVar].taxPayerId);
                }
            }
        }
        public void AddToDatabase(string name, string address, int taxpayer, int postalcode)
        {

            using (var context = new MemoryContext())
            {

                if (!context.TaxPayers.Any(o => o.taxPayerId == taxpayer))
                {
                    var taxp = new TaxPayer
                    {
                        name = name,
                        address = address,
                        taxPayerId = taxpayer,
                        postalCode = postalcode
                    };
                    context.TaxPayers.Add(taxp);
               
                context.SaveChanges();
                 }
                else {  
                    Console.WriteLine($"User with the taxPayerId Already exists");
                }
            }
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Timed Background Service running.");

            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Timed Background Service is working.");
                await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
            }

            _logger.LogInformation("Timed Background Service is stopping.");
        }
    }
}
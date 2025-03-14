using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
namespace ConsoleApp1
{
    public class TimedBackgroundService : BackgroundService
    {
        private readonly ILogger<TimedBackgroundService> _logger;

        public TimedBackgroundService(ILogger<TimedBackgroundService> logger)
        {
            _logger = logger;
        }
        public void Log()
        {
            Console.WriteLine("adada");
        }
        public void LogPayerData(TaxPayer payer)
        {
            Console.WriteLine($"{Environment.NewLine}Nome) {payer.name}{Environment.NewLine}address) {payer.address}{Environment.NewLine}taxPayerId) {payer.taxPayerId}{Environment.NewLine}codigo postal) {payer.postalCode}");
        }
        public void UpdateTable(int taxPayerId)
        {
            using (var context = new MemoryContext())
            {
                var payer = context.TaxPayers.First(a => a.taxPayerId == taxPayerId);
                LogPayerData(payer);
                Console.WriteLine($"{Environment.NewLine}1) Alterar nome{Environment.NewLine}2) Alterar morda{Environment.NewLine}3) Alterar codigo Postal");
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
                    default:
                        Console.WriteLine("wrong");
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
            Console.WriteLine($"User with the taxPayerId Already exists");
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
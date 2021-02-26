using System;
using System.IO;
using Newtonsoft.Json;

namespace LoanCalc
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            if (args.Length < 2)
            {
                Console.WriteLine("Usage: loancalc <loan amount> <duration years> [optional config file]. E.g. loancalc 100000 10");
                return;
            }

            if (!float.TryParse(args[0], out var loanAmount))
            {
                Console.WriteLine("Fist argument loan amount must be float value");
                return;
            }

            if (!int.TryParse(args[1], out var durationYears))
            {
                Console.WriteLine("Second argument duration years must be integer value");
                return;
            }

            var configFileName = args.Length > 2 ? args[2] : "config.json";

            try
            {
                var options = JsonConvert.DeserializeObject<LoanOptions>(File.ReadAllText(configFileName));
                var calculator = new LoanCalculator(options);
                var result = calculator.Calculate(loanAmount, durationYears * 12);

                Console.WriteLine($"Monthly payment: {result.MonthlyPayment:F2}");
                Console.WriteLine($"Total interest: {result.TotalInterest:F2}");
                Console.WriteLine($"Administration fee: {result.AdminstrationFee:F2}");
                Console.WriteLine($"Annual percentage rate (APR): {result.AnnualPercentageRate:F2} %");
            }
            catch (ArgumentException e)
            {
                Console.WriteLine(e.Message);
            }
            catch (FileNotFoundException e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}
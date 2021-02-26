using System;
using Extreme.Mathematics;
using Extreme.Mathematics.EquationSolvers;

namespace LoanCalc
{
    public class LoanCalculator
    {
        private readonly LoanOptions _options;

        public LoanCalculator(LoanOptions options)
        {
            _options = options;
        }

        public LoanResult Calculate(double loanAmount, int durationMonths)
        {
            if (loanAmount <= 0)
            {
                throw new ArgumentException("Must be positive", nameof(loanAmount));
            }

            if (durationMonths <= 0)
            {
                throw new ArgumentException("Must be positive", nameof(durationMonths));
            }

            var interestRate = CalculateMonthlyInterestRate();
            var factor = CalculateLoanFactor(interestRate, durationMonths);

            var administrationFee = CalculateAdministrationFee(loanAmount);
            var monthlyPayment = CalculatePaymentAmount(loanAmount, factor);

            return new LoanResult
            {
                MonthlyPayment = (float)monthlyPayment,
                AdminstrationFee = (float)administrationFee,
                TotalInterest = (float)CalculateTotalInterest(loanAmount, factor, durationMonths),
                AnnualPercentageRate = (float)CalculateAnnualPercentageRate(loanAmount, monthlyPayment, administrationFee, durationMonths)
            };
        }

        private double CalculateTotalInterest(double loanAmount, double factor, int durationMonths)
        {
            return Math.Round(loanAmount / factor * durationMonths - loanAmount, 2);
        }

        private double CalculateAdministrationFee(double loanAmount)
        {
            return Math.Min(Math.Round(loanAmount * _options.AdministrationFeeRate / 100, 2), _options.AdministrationFeeMax);
        }

        private double CalculateMonthlyInterestRate()
        {
            return _options.Interest / 100 / 12;
        }

        private double CalculatePaymentAmount(double loanAmount, double factor)
        {
            return Math.Round(loanAmount / factor, 2);
        }

        private double CalculateLoanFactor(double r, int n)
        {
            return (Math.Pow(1 + r, n) - 1) / (r * Math.Pow(1 + r, n));
        }

        private double CalculateAnnualPercentageRate(double loanAmount, double monthlyPayment, double administrationFee, int durationMonths)
        {
            loanAmount -= administrationFee;

            Func<double, double> f = r => loanAmount * Math.Pow(r, durationMonths + 1) - (loanAmount + monthlyPayment) * Math.Pow(r, durationMonths) + monthlyPayment;
            Func<double, double> df = r => loanAmount * (durationMonths + 1) * Math.Pow(r, durationMonths) - (loanAmount + monthlyPayment) * durationMonths * Math.Pow(r, durationMonths - 1);

            var solver = new NewtonRaphsonSolver()
            {
                TargetFunction = f,
                DerivativeOfTargetFunction = df,
                InitialGuess = _options.Interest,
            };

            var result = solver.Solve();

            return solver.Status == AlgorithmStatus.Converged ? Math.Round(12 * (result - 1) * 100, 2) : _options.Interest;
        }
    }
}
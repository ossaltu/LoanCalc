using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LoanCalc.Test
{
    [TestClass]
    public class LoanCalculatorTest
    {
        [TestMethod]
        public void TestExampleCase()
        {
            var calculator = new LoanCalculator(new LoanOptions { Interest = 5, AdministrationFeeRate = 1, AdministrationFeeMax = 10000 });
            var result = calculator.Calculate(500000, 120);

            result.AdminstrationFee.Should().Be(5000);
            result.MonthlyPayment.Should().Be(5303.28f);
            result.TotalInterest.Should().Be(136393.09f);
            result.AnnualPercentageRate.Should().Be(5.22f);
        }
    }
}

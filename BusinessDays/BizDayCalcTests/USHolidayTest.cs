using System;
using System.Collections.Generic;
using BizDayCalc;
using Xunit;
using Xunit.Abstractions;

namespace BizDayCalcTests
{
    public class USHolidayTest
    {
        private readonly ITestOutputHelper _output;
        public static IEnumerable<object[]> Holidays{
            get {
                yield return new object[] {new DateTime(2016, 1, 1)};
                yield return new object[] {new DateTime(2016, 7, 4)};
                yield return new object[] {new DateTime(2016, 12, 24)};
                yield return new object[] {new DateTime(2016, 12, 25)};
            }
        }

        private Calculator calculator;

        public USHolidayTest(ITestOutputHelper output)
        {
            calculator = new Calculator();
            calculator.AddRule(new HolidayRule());
            _output = output;
            _output.WriteLine("In USHolidayTest constructor");
        }

        [Theory]
        [MemberData(nameof(Holidays))]
        public void TestHolidays(DateTime date)
        {
            Assert.False(calculator.IsBusinessDay(date));
            _output.WriteLine($"In TestHolidays {date:yyyy-MM-dd}"); 
        }

        [Theory]
        [InlineData("2016-02-28")]
        [InlineData("2016-01-02")]
        public void TestNonHolidays(string date)
        {
            Assert.True(calculator.IsBusinessDay(DateTime.Parse(date)));
            _output.WriteLine($"In TestNonHolidays {date}"); 
        }
    }
}
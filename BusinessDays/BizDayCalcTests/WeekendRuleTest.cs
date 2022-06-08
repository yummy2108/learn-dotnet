using System;
using Xunit;
using BizDayCalc;

namespace BizDayCalcTests
{
    public class WeekendRuleTest
    {
        [Fact]
        public void TestCheckIsBusinessDay()
        {
            var rule = new WeekendRule();
            Assert.True(rule.CheckIsBusinessDay(new DateTime(2016, 6, 27)));
            Assert.False(rule.CheckIsBusinessDay(new DateTime(2016, 6, 26)));
        }

        [Theory]
        [InlineData("2016-06-27")]
        [InlineData("2016-03-01")]
        [InlineData("2017-09-20")]
        // [InlineData("2017-09-17")]
        public void IsBusinessDay(string date)
        {
            var rule = new WeekendRule();
            Assert.True(rule.CheckIsBusinessDay(DateTime.Parse(date)));
        }

        [Theory]
        [InlineData("2016-06-26")]
        [InlineData("2016-11-12")]
        public void IsNotBusinessDay(string date){
            var rule = new WeekendRule();
            Assert.False(rule.CheckIsBusinessDay(DateTime.Parse(date)));
        }
    }
}

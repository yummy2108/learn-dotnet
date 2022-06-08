using System;
using Xunit;
using BizDayCalc;
using System.Collections.Generic;

namespace BizDayCalcTests
{
    public class WeekendRuleTest
    {
        public static IEnumerable<object[]> Days{
            get {
                yield return new object[] {true, new DateTime(2016, 6, 27)};
                yield return new object[] {true, new DateTime(2016, 3, 1)};
                yield return new object[] {false, new DateTime(2016, 6, 26)};
                yield return new object[] {false, new DateTime(2016, 11, 12)};
            }
        }

        [Theory]
        [MemberData(nameof(Days))]
        public void TestCheckIsBusinessDay(bool expected, DateTime date)
        {
            var rule = new WeekendRule();
            // Assert.True(rule.CheckIsBusinessDay(new DateTime(2016, 6, 27)));
            // Assert.False(rule.CheckIsBusinessDay(new DateTime(2016, 6, 26)));
            Assert.Equal(expected, rule.CheckIsBusinessDay(date));
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

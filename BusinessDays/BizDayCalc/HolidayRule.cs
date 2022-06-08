using System;

namespace BizDayCalc
{
    public class HolidayRule : IRule
    {
        public static readonly int[,] USHolidays = {
            { 1, 1},
            { 7, 4},
            { 12, 24},
            { 12, 25}
        };

        public bool CheckIsBusinessDay(DateTime date)
        {
            for(int day = 0; day <= USHolidays.GetUpperBound(0); day++)
            {
                if(date.Month == USHolidays[day, 0] && date.Day == USHolidays[day, 1])
                    return false;
            }
            return true;
        }
    }
}
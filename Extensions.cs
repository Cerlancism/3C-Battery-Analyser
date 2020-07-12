using System;
using System.Collections.Generic;
using System.Text;

namespace _3C_Battery_Analyser
{
    public static class Extensions
    {
        public static string FormattedString(this DateTime date)
        {
            return $"Date: {date.ToShortDateString(), 10} {date.ToLongTimeString(), 11}";
        }
    }
}

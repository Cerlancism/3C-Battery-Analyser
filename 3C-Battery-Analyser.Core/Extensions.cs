using System;
using System.Collections.Generic;
using System.Text;

namespace _3C_Battery_Analyser.Core
{
    static class Extensions
    {
        public static string FormattedString(this DateTime date)
        {
            return $"{date.ToShortDateString(), -10} {date.ToLongTimeString(), 11}";
        }

        public static string FormattedString(this bool theBool)
        {
            return $"{(theBool ? "Yes" : "No"), 3}";
        }
    }
}

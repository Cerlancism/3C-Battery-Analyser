using System;
using System.Collections.Generic;
using System.Text;

namespace _3C_Battery_Analyser.Core
{
    static class Extensions
    {
        public static string FormattedString(this DateTime value)
        {
            return $"{value:dd/MM/yyyy HH:mm:ss}";
        }

        public static string FormattedString(this TimeSpan value)
        {
            return $"{value.Hours:D2}:{value.Minutes:D2}";
        }

        public static string FormattedString(this bool value)
        {
            return $"{(value ? "Yes" : "No"),3}";
        }
    }
}

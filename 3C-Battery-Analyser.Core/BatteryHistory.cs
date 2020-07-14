using System;

namespace _3C_Battery_Analyser.Core
{
    public class BatteryHistory
    {
        public DateTime Date { get; private set; }
        public double Percent { get; private set; }
        public int Flow_mA { get; private set; }
        public double Temperature_C { get; private set; }
        public double PercentHour { get; private set; }
        public int Voltage_mV { get; private set; }
        public bool Charging { get; private set; }
        public bool Screen { get; private set; }
        public bool Restarted { get; private set; }

        public override string ToString()
        {
            return
                $"{Date.FormattedString()} " +
                $"{Percent,4:P0} " +
                $"{Flow_mA,5} mA {PercentHour,6:N2} %/h " +
                $"{Temperature_C,4:N1}°C " +
                $"Charging: {Charging.FormattedString()} " +
                $"Screen: {Screen.FormattedString()} " +
                $"Restarted: {Restarted.FormattedString()}";
        }


        // 12 Jul 2020 19:28:52: 100%,1mA,0%/h,31.2°C,4307mV,ac,off,restart,1594553332370
        public static BatteryHistory Parse(string raw)
        {
            var firstParseSplit = raw.Split(": ");

            if (firstParseSplit.Length != 2)
            {
                throw new Exception("First parse split does not yield 2 parts.");
            }

            var dataParse = firstParseSplit[1];
            var dataParts = dataParse.Split(",");

            return new BatteryHistory 
            {
                Date= DateTime.UnixEpoch.AddMilliseconds(double.Parse(dataParts[8])).ToLocalTime(),
                Percent= double.Parse(dataParts[0].Replace("%", "")) / 100.0,
                Flow_mA = int.Parse(dataParts[1].Replace("mA", "")),
                PercentHour = double.Parse(dataParts[2].Replace("%/h", "")) / 100.0,
                Temperature_C = double.Parse(dataParts[3].Replace("°C", "")),
                Charging = dataParts[5] == "ac",
                Screen = dataParts[6] == "on",
                Restarted = dataParts[7] == "restart"
            };
        }
    }
}

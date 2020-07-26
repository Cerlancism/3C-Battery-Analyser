using System;

namespace _3C_Battery_Analyser.Core
{
    public readonly struct BatteryHistory
    {
        public DateTime Date { get; }
        public double Percent { get; }
        public int Flow_mA { get; }
        public double Temperature_C { get; }
        public double PercentHour { get; }
        public int Voltage_mV { get; }
        public bool Charging { get; }
        public bool Screen { get; }
        public bool Restarted { get; }

        public BatteryHistory(
            DateTime date,
            double percent,
            int flow_mA,
            double temperature_C,
            double percentHour,
            int voltage_mV,
            bool charging,
            bool screen,
            bool restarted
        )
        {
            Date = date;
            Percent = percent;
            Flow_mA = flow_mA;
            Temperature_C = temperature_C;
            PercentHour = percentHour;
            Voltage_mV = voltage_mV;
            Charging = charging;
            Screen = screen;
            Restarted = restarted;
        }

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

        public static bool TryParse(string raw, out BatteryHistory result)
        {
            try
            {
                result = Parse(raw);
                return true;
            }
            catch (Exception)
            {
                result = default;
                return false;
            }
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
            (
                date: DateTime.UnixEpoch.AddMilliseconds(double.Parse(dataParts[8])).ToLocalTime(),
                percent: double.Parse(dataParts[0].Replace("%", "")) / 100.0,
                flow_mA: int.Parse(dataParts[1].Replace("mA", "")),
                voltage_mV: int.Parse(dataParts[4].Replace("mV", "")),
                percentHour: double.Parse(dataParts[2].Replace("%/h", "")) / 100.0,
                temperature_C: double.Parse(dataParts[3].Replace("°C", "")),
                charging: dataParts[5] == "ac",
                screen: dataParts[6] == "on",
                restarted: dataParts[7] == "restart"
            );
        }
    }
}

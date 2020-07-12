﻿using System;

namespace _3C_Battery_Analyser.Core
{
    public class BatteryHistory
    {
        public DateTime Date { get; }
        public double Percent { get; }
        public int Flow_mA { get; }
        public double Temperature { get; }

        public BatteryHistory(DateTime date, double percent, int flow_mA, double temperature)
        {
            Date = date;
            Percent = percent;
            Flow_mA = flow_mA;
            Temperature = temperature;
        }

        public override string ToString()
        {
            return 
                $"{Date.FormattedString()} " +
                $"{$"Percent: {Percent, 4:P0}"} " +
                $"{$"Flow: {Flow_mA, 5} mA"} " +
                $"{$"{Temperature, 4:N1} °C"}";
        }

        // 12 Jul 2020 19:28:52: 100%,1mA,0%/h,31.2°C,4307mV,ac,off,restart,1594553332370
        public static BatteryHistory Parse(string raw)
        {
            var firstParseSplit = raw.Split(": ");

            if (firstParseSplit.Length != 2)
            {
                throw new Exception("First Parse split does not yield 2 parts");
            }

            var dataParse = firstParseSplit[1];
            var dataParts = dataParse.Split(",");

            return new BatteryHistory(
                date: DateTime.UnixEpoch.AddMilliseconds(double.Parse(dataParts[8])).ToLocalTime(),
                percent: double.Parse(dataParts[0].Replace("%", "")) / 100.0,
                flow_mA: int.Parse(dataParts[1].Replace("mA", "")),
                temperature: double.Parse(dataParts[3].Replace("°C", ""))
            );
        }
    }
}
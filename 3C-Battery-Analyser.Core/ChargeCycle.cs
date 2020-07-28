using System;
using System.Collections.Generic;
using System.Linq;

namespace _3C_Battery_Analyser.Core
{
    public class ChargeCycle
    {
        public const double MINIMUM_CHARGE = 1.0 / 3;
        public const int MINIMUM_FLOW_MA = 50;

        public BatteryHistory Start { get; }
        public BatteryHistory End { get; }
        public double PercentCharged { get; }
        public TimeSpan OverCharge { get;  }
        public double Capacity_mAH { get; }

        private readonly Lazy<TimeSpan> lazyDuration;
        public TimeSpan Duration => lazyDuration.Value;
        
        private readonly Lazy<double> lazyRate_percentHour;
        public double Rate_PercentHour => lazyRate_percentHour.Value;

        public ChargeCycle(BatteryHistory start, BatteryHistory end, double percentCharged, TimeSpan overCharge, double chargeCapacity)
        {
            Start = start;
            End = end;
            PercentCharged = percentCharged;
            OverCharge = overCharge;
            Capacity_mAH = chargeCapacity;
            lazyDuration = new Lazy<TimeSpan>(() => End.Date - Start.Date);
            lazyRate_percentHour = new Lazy<double>(() => PercentCharged / lazyDuration.Value.TotalHours);
        }

        public override string ToString()
        {
            return 
                $"{Start.Date.FormattedString()} - {End.Date.FormattedString()} " +
                $"{Start.Percent,3:P0} - {End.Percent,-4:P0} {$"({PercentCharged:P0})", -6} " +
                $"{Duration.FormattedString()} " +
                $"{Rate_PercentHour:P2}/Hour " +
                $"{OverCharge.FormattedString()} " +
                $"{Capacity_mAH:N4} mAH";
        }

        public string ToCSVString()
        {
            return $"{End.Date.FormattedString(true)}, {(int)Math.Round(Capacity_mAH)}";
        }

        public static IEnumerable<ChargeCycle> EnumerateChargeCycles(IEnumerable<BatteryHistory> history_set)
        {
            var iter = history_set.GetEnumerator();

            while (iter.MoveNext())
            {
                if (!iter.Current.Charging)
                {
                    continue;
                }

                IEnumerable<BatteryHistory> enumerateSessionHistory()
                {
                    do
                    {
                        yield return iter.Current;

                        if (!iter.MoveNext())
                        {
                            break;
                        }

                    } while (iter.Current.Charging);
                }

                var session = enumerateSessionHistory().ToArray();

                if (session.Length < 2)
                {
                    continue;
                }

                var start = session.Last();
                var charged = session.First().Percent - start.Percent;

                if (charged < MINIMUM_CHARGE)
                {
                    continue;
                }

                var overCharge = session.TakeWhile((x, i) =>
                {
                    var next = session.ElementAtOrDefault(i + 1);
                    return x.Percent == 1 && next.Percent == 1 && next.Flow_mA < MINIMUM_FLOW_MA;
                });
                var overChargeCount = overCharge.Count();
                var overChargeDuration = overChargeCount > 0 ? overCharge.First().Date - overCharge.Last().Date : default;
                var end = session.ElementAt(overChargeCount);
                var estimatedCharge_mAH = EstimateCharge_mAH(session.Skip(overChargeCount)) / charged;

                yield return new ChargeCycle(start, end, charged, overChargeDuration, estimatedCharge_mAH);
            }
        }

        private static double EstimateCharge_mAH(IEnumerable<BatteryHistory> chargeSession)
        {
            var output = 0.0;
            var current = chargeSession.First();
            foreach (var next in chargeSession.Skip(1))
            {
                if (current.Flow_mA <= 0 || next.Flow_mA <= 0)
                {
                    current = next;
                    continue;
                }

                var ma = current.Flow_mA;
                var duration = current.Date - next.Date;

                output += ma * duration.TotalHours;

                current = next;
            }

            return output;
        }
    }
}

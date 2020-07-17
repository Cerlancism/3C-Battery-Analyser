using System;
using System.Collections.Generic;
using System.Linq;

namespace _3C_Battery_Analyser.Core
{
    public class ChargeCycle
    {
        private readonly BatteryHistory[] historySet;
        private readonly Lazy<double> lazyPercentCharged;
        private readonly Lazy<BatteryHistory> lazyStart;
        private readonly Lazy<BatteryHistory> lazyEnd;

        public BatteryHistory Start => lazyStart.Value;
        public BatteryHistory End => lazyEnd.Value;
        public double PercentCharged => lazyPercentCharged.Value;

        public ChargeCycle(BatteryHistory[] history_set)
        {
            historySet = history_set;

            lazyStart = new Lazy<BatteryHistory>(() => historySet.Last());
            lazyEnd = new Lazy<BatteryHistory>(() => historySet.First());
            
            lazyPercentCharged = new Lazy<double>(() =>
            {
                return End.Percent - Start.Percent;
            });
        }

        public override string ToString()
        {
            return $"{Start.Date.FormattedString()} - {End.Date.FormattedString()} {PercentCharged,6:P1} ";
        }

        public static IEnumerable<ChargeCycle> GetChargeCycles(IEnumerable<BatteryHistory> history_set)
        {
            var iter = history_set.GetEnumerator();

            while (iter.MoveNext())
            {
                if (!iter.Current.Charging)
                {
                    continue;
                }
                // WIP
                IEnumerable<BatteryHistory> getHistorySet()
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

                yield return new ChargeCycle(getHistorySet().ToArray());
            }
        }

    }
}

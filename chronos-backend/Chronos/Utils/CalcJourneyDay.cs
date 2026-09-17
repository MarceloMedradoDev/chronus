using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chronos.Utils
{
    public static class CalculatorJourneyDay
    {
        public static TimeSpan CalculateJourney(List<string> day)
        {
            if (day == null || day.Count < 2) return TimeSpan.Zero;

            TimeSpan totalDoDia = TimeSpan.Zero;

            TimeSpan entrada1 = TimeSpan.Parse(day[0]);
            TimeSpan saida1 = TimeSpan.Parse(day[1]);

            totalDoDia += (saida1 - entrada1);

            if (day.Count >= 4)
            {
                TimeSpan entrada2 = TimeSpan.Parse(day[2]);
                TimeSpan saida2 = TimeSpan.Parse(day[3]);

                totalDoDia += (saida2 - entrada2);
            }

            return totalDoDia;
        }
    }
}

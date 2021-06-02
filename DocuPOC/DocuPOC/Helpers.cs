using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocuPOC
{
    public static class Helpers
    {
        public static int YearsDifference(this DateTime date)
        {
            return Convert.ToInt32(Math.Floor(date.DaysDifference() / 365.25));
        }

        public static int YearsDifference(this DateTimeOffset date)
        {
            return date.DateTime.YearsDifference();
        }

        public static int DaysDifference(this DateTime date)
        {
            return Convert.ToInt32(Math.Floor((DateTime.Now - date).TotalDays));
        }

        public static int DaysDifference(this DateTimeOffset date)
        {
            return date.DateTime.DaysDifference();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMS.Extensions
{
    /// <summary>
    /// Extension methods for <see cref="DayOfWeekExtensions"/>.
    /// </summary>
    public static class DayOfWeekExtensions
    {
        /// <summary>
        /// Check if given <see cref="DayOfWeek"/> value is weekend.
        /// </summary>
        public static bool IsWeekend(this DayOfWeek dayOfWeek)
        {
            return dayOfWeek.IsIn(DayOfWeek.Saturday, DayOfWeek.Sunday);
        }

        /// <summary>
        /// Check if given <see cref="DayOfWeek"/> value is weekday.
        /// </summary>
        public static bool IsWeekday(this DayOfWeek dayOfWeek)
        {
            return dayOfWeek.IsIn(DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Wednesday, DayOfWeek.Thursday, DayOfWeek.Friday);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LCK_WPFclient
{
    public static class Globals
    {

        public static string DateTime_to_DayNumber(DateTime date)
        {
            int year = date.Year;
            int day = date.DayOfYear;
            return year + "_" + day;
        }

        public static DateTime DayNumber_to_DateTime(string day_number)
        {
            string[] pieces = day_number.Split('_');
            int days = int.Parse(pieces[1]) - 1; // dateTime days are zero based
            int year = int.Parse(pieces[0]);
            DateTime theDate = new DateTime(year, 1, 1).AddDays(days);
            return theDate;
        }

    }
}

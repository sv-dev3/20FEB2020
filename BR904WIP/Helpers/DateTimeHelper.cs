using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BR904WIP.Helpers
{
    public class DateTimeHelper
    {
        static public DateTime ConvertToUTCDateTime(string originTimestamp)
        {
            try
            {
                var timestamp = DateTime.Parse(originTimestamp).ToUniversalTime();
                return timestamp;
            }
            catch (Exception exception)
            {
                return DateTime.UtcNow;
            }
        }

        static public TimeSpan ConvertToTimeSpan(string originTimestamp)
        {
            try
            {
                var timestamp = TimeSpan.Parse(originTimestamp);
                return timestamp;
            }
            catch (Exception exception)
            {
                return new TimeSpan(0, 0, 0);
            }
        }

        static public string GetDateTimeString(DateTime time)
        {
            return time.ToString("yyyy-MM-ddTHH\\:mm\\:ss.ffffffZ");
        }

        static public string GetFormattedTimestamp(string originTimestamp, string format)
        {
            try
            {
                var timestamp = DateTime.Parse(originTimestamp).ToString(format);
                return timestamp;
            }
            catch (Exception)
            {
                return "";
            }
        }
    }
}

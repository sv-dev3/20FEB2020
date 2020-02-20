using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BR904WIP.Helpers
{
    /// <summary>
    /// This Class Used for seraching
    /// </summary>
    public class SerchCriteria
    {
        public string[] Process_status { get; set; }
        public DateTime? Process_time { get; set; }
        public DateTime? Create_datetime { get; set; }
        public string Customer_id { get; set; }
        public string detail_level { get; set; }
    }
}

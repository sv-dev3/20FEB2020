using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BR904WIP.Models.Email
{
    public class Email
    {
        public int? process_attempts { get; set; }
        public string process_status { get; set; }
        public string source_system_type_id { get; set; }
        public string source_sys_type_name { get; set; }
        public string submission_id { get; set; }
        public string submission_type { get; set; }
        public string submitter_email { get; set; }
        public DateTime? create_datetime { get; set; }
        public string create_user_id { get; set; }
        public DateTime? edit_datetime { get; set; }
        public string edit_user_id { get; set; }
    }
}

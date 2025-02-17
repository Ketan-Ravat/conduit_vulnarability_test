using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Jarvis.db.Models
{
    public class TrackMobileSyncOffline
    {
        [Key]
        public Guid trackmobilesyncoffline_id { get; set; }
        public Guid device_uuid { get; set; }
        public string device_code { get; set; }
        public Guid user_id { get; set; }
        public DateTime sync_time { get; set; }
        public int status { get; set; } //1 - completed , 2 - open , 3 - inprogress , 4 - failed
        public string lambda_logs { get; set; }
        public string s3_file_name { get; set; }
    }
}

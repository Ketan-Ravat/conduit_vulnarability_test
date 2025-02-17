﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Jarvis.db.Models {
    public class ServiceDealers {
        [Key]
        public Guid service_dealer_id { get; set; }
        public string name { get; set; }
        public string email { get; set; }
        public string phone { get; set; }
        public string address { get; set; }
        [ForeignKey("StatusMaster")]
        public int status { get; set; }
        public Nullable<DateTime> created_at { get; set; }

        public bool is_archive { get; set; }

        public string created_by { get; set; }

        public Nullable<DateTime> modified_at { get; set; }

        public string modified_by { get; set; }

        public virtual StatusMaster StatusMaster { get; set; }
    }
}

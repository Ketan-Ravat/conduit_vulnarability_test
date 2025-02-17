using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.db.Models
{
    public class CommentJsonObject
    {
        public string comment { get; set; }

        public Guid created_by { get; set; }

        public DateTime created_at { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Jarvis.db.Models
{
    public class PreferLanguageMaster
    {
        [Key]
        public int prefer_lang_id { get; set; }
        public string key_name { get; set; }
        public string spanish_name { get; set; }
    }
}
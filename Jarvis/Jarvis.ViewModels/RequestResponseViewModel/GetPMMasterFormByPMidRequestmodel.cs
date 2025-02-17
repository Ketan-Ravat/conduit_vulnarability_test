using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels.RequestResponseViewModel
{
    public class GetPMMasterFormByPMidRequestmodel
    {
        public Guid pmitemmasterform_id { get; set; }
        public string form_json { get; set; }
        public Guid pm_id { get; set; }

        public int success { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Jarvis.ViewModels.RequestResponseViewModel
{
    public class AssignPMToAsset
    {
        [Required]
        public Guid asset_id { get; set; }
        [Required]
        public Guid pm_plan_id { get; set; }
    }
}

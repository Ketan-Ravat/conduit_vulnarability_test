using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Jarvis.ViewModels.RequestResponseViewModel {
    public class ResolveMaintenanceRequestModel {
        [Required]
        public Guid mr_id { get; set; }
        [Required]
        public string resolve_reason { get; set; }
    }
}

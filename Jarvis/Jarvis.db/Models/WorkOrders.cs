﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Jarvis.db.Models
{
    public class WorkOrders
    {
        [Key]
        public Guid wo_id { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long wo_number { get; set; } // this will be autogenerated.
        public string manual_wo_number { get; set; } // this user will enter .
        public string title { get; set; }   

        public string description { get; set; }

        [ForeignKey("Asset")]
        public Guid? asset_id { get; set; }

        [ForeignKey("PriorityStatusMaster")]
        public int? priority { get; set; }

        public DateTime due_at { get; set; }
        public DateTime start_date { get; set; }
        public int? wo_due_overdue_flag { get; set; }
        public string wo_due_time_duration { get; set; }

        [ForeignKey("WOTypeStatusMaster")]
        public int wo_type { get; set; }

        [ForeignKey("StatusMaster")]
        public int status { get; set; }

        public Nullable<DateTime> completed_date { get; set; }

        [ForeignKey("Sites")]
        public Guid site_id { get; set; }

        [ForeignKey("ServiceDealers")]
        public Guid? service_dealer_id { get; set; }

        [ForeignKey("TechnicianUser")]
        public Guid? technician_user_id { get; set; }  //deprecated -- New flow for Multiple Technicians has introduced

        [ForeignKey("ClientCompany")]
        public Guid? client_company_id { get; set; }
        public bool is_archive { get; set; }

        public Nullable<DateTime> created_at { get; set; }

        public string created_by { get; set; }

        public Nullable<DateTime> modified_at { get; set; }

        public string modified_by { get; set; }
        public string ir_wo_pdf_report { get; set; }
        public int? ir_wo_pdf_report_status { get; set; }
        public DateTime? ir_wo_export_pdf_at { get; set; }
        public int? complete_wo_thread_status { get; set; } // Inprogress = 1, Completed = 2,  Failed = 3
        public int? bulk_data_import_status { get; set; } // Completed = 1, Inprogress = 2,  Failed = 3
        public string bulk_data_import_failed_logs { get; set; } // identification array if bulk import is failed to insert in db 
        public string po_number { get; set; }
        public int? ir_visual_camera_type { get; set; } // FLIR = 1 , FLUKE = 2
        public int? ir_visual_image_type { get; set; }  // IR = 1, Visual = 2, IR + Visual = 3

        [ForeignKey("QuoteStatusMaster")]
        public int? quote_status { get; set; } // accepted , rejected, Defered

        [ForeignKey("ResponsibleParty")]
        public Guid? responsible_party_id { get; set; }
        public bool is_reminder_required { get; set; } //if true then send email reminder
        public string reminders_frequency_json { get; set; } //json -> reminders to send notification via calendar
        public string calendarId { get; set; } // we are storing event_id in this field
        public string wo_vendor_notes_json { get; set; }// json to store notes vendor wise

        public virtual StatusMaster QuoteStatusMaster { get; set; }
        public virtual ResponsibleParty ResponsibleParty { get; set; }
        public virtual ICollection<MaintenanceRequests> MaintenanceRequests { get; set; }
        public virtual Sites Sites { get; set; }
        public virtual Asset Asset { get; set; }
        public virtual StatusMaster StatusMaster { get; set; }
        public virtual StatusMaster PriorityStatusMaster { get; set; }
        public virtual StatusMaster WOTypeStatusMaster { get; set; }
        public virtual ServiceDealers ServiceDealers { get; set; }
        public virtual User TechnicianUser { get; set; }
        public virtual ClientCompany ClientCompany { get; set; }
        public virtual ICollection<AssetFormIO> AssetFormIO { get; set; }
        public virtual ICollection<WorkOrderTasks> WorkOrderTasks { get; set; }
        public virtual ICollection<WorkOrderAttachments> WorkOrderAttachments { get; set; }
        public virtual ICollection<WOInspectionsTemplateFormIOAssignment> WOInspectionsTemplateFormIOAssignment { get; set; }
        public virtual ICollection<WOcategorytoTaskMapping> WOcategorytoTaskMapping { get; set; }
        public virtual ICollection<WOOnboardingAssets> WOOnboardingAssets { get; set; }
        public virtual ICollection<IRScanWOImageFileMapping> IRScanWOImageFileMapping { get; set; }
        public virtual ICollection<WOLineIssue> WOLineIssue { get; set; }
        public virtual ICollection<AssetPMs> AssetPMs { get; set; }
        public virtual ICollection<AssetIssue> AssetIssue { get; set; }

        public virtual ICollection<TempFormIOBuildings> TempFormIOBuildings { get; set; }
        public virtual ICollection<TempFormIOFloors> TempFormIOFloors { get; set; }
        public virtual ICollection<TempFormIORooms> TempFormIORooms { get; set; }
        public virtual ICollection<TempFormIOSections> TempFormIOSections { get; set; }

        public virtual ICollection<WorkOrderTechnicianMapping> WorkOrderTechnicianMapping { get; set; }
        public virtual ICollection<WorkOrderBackOfficeUserMapping> WorkOrderBackOfficeUserMapping { get; set; }
        public virtual ICollection<TempAsset> TempAsset { get; set; }
        public virtual ICollection<TimeMaterials> TimeMaterials { get; set; }
        public virtual ICollection<WorkordersVendorContactsMapping> WorkordersVendorContactsMapping { get; set; }

    }
}

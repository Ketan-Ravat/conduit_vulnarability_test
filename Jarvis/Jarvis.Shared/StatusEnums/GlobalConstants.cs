using DocumentFormat.OpenXml.Presentation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.Shared.StatusEnums
{
    
    public static class GlobalConstants
    {
        public const string Admin = "SuperAdmin";
        public const string Operator = "Operator";
        public const string Manager = "Manager";
        public const string MS = "Maintenance staff";
        public const string Executive = "Executive";
        public const string CompanyAdmin = "Company Admin";
        public const string Technician = "Technician";
        public const string FirstDueDateReminder = "PM - 1st Notification";
        public const string FirstMeterHoursDueReminder = "Upcoming PM - 1st Notification";
        public const string SecondDueDateReminder = "Upcoming PM - 2nd Notification";
        public const string SecondMeterHoursDueReminder = "Upcoming PM - 2nd Notification";
        public const string OnDueDateReminder = "Due PM - Final Notification";
        public const string OnMeterHoursDueReminder = "Due PM - Final Notification";
        public const int DateDiffForDueStatus = 1;
        public const string BackOffice_Role_Id = "a988c901-d5ed-4564-a969-ef0ac64725c6";
        public const string Technician_Role_id = "b22217c2-a932-498c-8ab2-11fc2628104b";
        public const string CompanyAdmin_Role_id = "972e1960-947b-42b0-a8ea-5e23a68e9632";
        public const string Executive_Role_id = "082e1960-947b-42b0-a8ea-5e23a68e9632";
        public const string SuperAdmin_Role_id = "ff52a40a-b130-4388-bb1c-4237f8dae72e";
        public const string session_management_multi_browser_feature_id = "24eb9b86-2be8-4373-9b5e-28bc12daf902";
        public const string egalvanic_ai = "4043256a-c3ed-4c8f-93fc-db15e1fc21a9";
        public const string estimator_feature = "819f22e2-0016-4cb2-bd47-d16d66b9e3ff";
        public const string allowed_to_update_formio_feature_id = "352ec1ed-d19f-4588-9939-ab8b9f600687";
        public const string hide_egalvanic_users_feature_id = "96686ce8-0173-4d9e-924b-e7d5b015b349";
        public const string is_mfa_enabled_feature_id = "d0ea808d-0d63-408d-9ac6-f13ca9c8ef12";
        public const string hide_email_for_user_feature_id = "f54b8e4e-9581-425e-bde0-20637684f357";
        public const string is_reactflow_required = "93ec3e68-e441-4eb7-9959-837e2775c094";
        public const string is_retool_bulk_operation_required_id = "3bcb82e5-b5a0-4ed3-bbf5-50f17bd7ca60";
        public const string is_required_maintenance_command_center = "71949b13-d8bc-494a-867a-9acfc5b92b71";

        public static string NECViolationDropdown(int index_id)
        {
            Dictionary<int, string> NECViolationDropdown = new Dictionary<int, string>();
            NECViolationDropdown.Add(1, "Circuit - Breaker is restricted from freely operating - (NEC 240.8)");
            NECViolationDropdown.Add(2, "Circuit - Exceeds panel limit - (NEC 408.36)");
            NECViolationDropdown.Add(3, "Component - Visible Corrosion");
            NECViolationDropdown.Add(4, "Conduit - Improperly fastened or secured - (NEC 300.11(A))");
            NECViolationDropdown.Add(5, "Enclosure - Missing dead front, door, cover, etc. - (NEC 110.12(A))");
            NECViolationDropdown.Add(6, "Enclosure - Must be free of foreign materials - (NEC 110.12(B) / NFPA 70B 13.3.2)");
            NECViolationDropdown.Add(7, "Enclosure - Unused opening must be sealed - (NEC 110.12 (A) / 312.5(A))");
            NECViolationDropdown.Add(8, "Fuse - Parallel fuses must match - (NEC 240.8)");
            NECViolationDropdown.Add(9, "General - Lack of component integrity");
            NECViolationDropdown.Add(10, "General - Not installed in a proper worklike manner");
            NECViolationDropdown.Add(11, "Grounding - Need earth connection - (NEC 250.4(A))");
            NECViolationDropdown.Add(12, "Marking/Labels - Missing or Insufficient information - (NEC 110.21(B) or 408.4)");
            NECViolationDropdown.Add(13, "Mechanics - Damaged/broken parts");
            NECViolationDropdown.Add(14, "Missing arc flash and shock hazard warning labels - (NEC 110.16(A))");
            NECViolationDropdown.Add(15, "Plug Fuse - Exposed energized parts - (NEC 240.50(D))");
            NECViolationDropdown.Add(16, "Temperature - Inadequate ventilation/cooling for component");
            NECViolationDropdown.Add(17, "Terminals - Connection made without damaging wire - (NEC 110.14(A))");
            NECViolationDropdown.Add(18, "Wire - 1 wire per terminal - (NEC 110.14(A))");
            NECViolationDropdown.Add(19, "Wire - Improper Neutral Conductor - (NEC 200.4(A))");
            NECViolationDropdown.Add(20, "Wire - Not Protected from damage - (NEC 300.4)");
            NECViolationDropdown.Add(21, "Wire - Size wrong for load - (NEC 210.19(A))");
            NECViolationDropdown.Add(22, "Wire - Wire bundle should have listed bushing");
            NECViolationDropdown.Add(23, "Wire - Wire burned or damaged");
            NECViolationDropdown.Add(24, "NEC.250.97 Bonding and Grounding");
            return NECViolationDropdown[index_id];
        }

        public static string NFPA70BViolation(int indexId)
        {
            Dictionary<int, string> NFPA70BViolation = new Dictionary<int, string>();
            NFPA70BViolation.Add(1, "Chapter 11.3.2 Power and Distribution transformer Cleaning");
            NFPA70BViolation.Add(2, "Chapter 11.3.1 Visual Inspections");
            NFPA70BViolation.Add(3, "Chapter 12.3.2 Substations and Switchgear Cleaning");
            NFPA70BViolation.Add(4, "Chapter 12.3.1 Visual Inspections");
            NFPA70BViolation.Add(5, "Chapter 13.3.2 Panelboards and Switchboards Cleaning");
            NFPA70BViolation.Add(6, "Chapter 13.3.1 Visual Inspections");
            NFPA70BViolation.Add(7, "Chapter 15.3.2 Circuit Breakers Low- and Medium Voltage");
            NFPA70BViolation.Add(8, "Chapter 15.3.1 Visual Inspections");
            NFPA70BViolation.Add(9, "Chapter 25.3.2 UPS Cleaning");
            NFPA70BViolation.Add(10, "Chapter 25.3.1 Visual Inspections");
            NFPA70BViolation.Add(11, "Chapter 28.3.2 Motor Control Equipment Cleaning");
            NFPA70BViolation.Add(12, "Chapter 28.3.1 Visual Inspections");
            return NFPA70BViolation[indexId];
        }
        public static string OSHAViolationDropdown(int index_id)
        {
            Dictionary<int, string> NECViolationDropdown = new Dictionary<int, string>();
            NECViolationDropdown.Add(1, "Clearance - Insufficient Access");
            NECViolationDropdown.Add(2, "Enclosure - Broken locking mechanism");
            NECViolationDropdown.Add(3, "Enclosure - Damaged");
            NECViolationDropdown.Add(4, "Enclosure - Should be waterproof");
            NECViolationDropdown.Add(5, "Equipment - Free of Hazards");
            NECViolationDropdown.Add(6, "Grounding - Must be permanent & continuous");
            NECViolationDropdown.Add(7, "Lighting - Inadequate around equipment");
            NECViolationDropdown.Add(8, "Marking/Labels - Inadequate or missing information on equipment");
            NECViolationDropdown.Add(9, "Mounting - Should be secure");
            NECViolationDropdown.Add(10, "Noise - Excessive");
            NECViolationDropdown.Add(11, "Wire - Exposed");
            return NECViolationDropdown[index_id];
        }
        public static string ImageRectQueryQuestion(string class_type)
        {
            Dictionary <string, string> ImageRectQueryQuestion = new Dictionary<string, string> ();
            ImageRectQueryQuestion.Add("ARRESTERS", "Who is the manufacturer? | What is the model number? | What is the catalog number? | What is the ampere rating? | What is the voltage rating?");
            ImageRectQueryQuestion.Add("BATTERIES", "Who is the manufacturer? | What is the model number? | What is the catalog number? | What is the ampere rating? | What is the voltage rating?");
            ImageRectQueryQuestion.Add("BUS", "Who is the manufacturer? | What is the model number? | What is the catalog number? | What is the ampere rating? | What is the voltage rating?");
            ImageRectQueryQuestion.Add("CAPACITORS", "Who is the manufacturer? | What is the model number? | What is the catalog number? | What is the ampere rating? | What is the voltage rating?");
            ImageRectQueryQuestion.Add("CIRCUIT BREAKERS", "Who is the manufacturer? | What is the frame ampere rating? | What is the model number? | What is the catalog number? | What is the frame fault withstand / AiC rating? | What is the trip unit model / catalog #? | What is the trip unit ampere rating? | What is the voltage rating?");
            ImageRectQueryQuestion.Add("ELECTRICAL PANELS", "Who is the manufacturer? | What is the model number? | What is the catalog number? | What is the serial number? | What is the voltage rating? | What is the configuration? | What is the ampere rating? ");
            ImageRectQueryQuestion.Add("FUSES", "Who is the fuse manufacturer? | What is the fuse size? | What is the model the fuse? | What is the catalog the fuse?");
            ImageRectQueryQuestion.Add("GENERATORS", "Who is the manufacturer? | What is the model number? | What is the catalog number? | What is the serial number? | What is the voltage rating? | What is the generator size (kW or kVA/PF) | What is the RPM? | What is the X’d or Reactance? | What is the generator configuration?");
            ImageRectQueryQuestion.Add("GROUND FAULT SYSTEMS", "Who is the manufacturer? | What is the model number? | What is the catalog number? | What is the ratio?");
            ImageRectQueryQuestion.Add("INSTRUMENT TRANSFORMERS", "Who is the manufacturer? | What is the model number? | What is the catalog number? | What is the ampere rating? | What is the voltage rating? | What is the ratio?");
            ImageRectQueryQuestion.Add("METERS", "What is the meter number?");
            ImageRectQueryQuestion.Add("MISCELLANEOUS", "Who is the manufacturer? | What is the model number? | What is the catalog number? | What is the ampere rating? | What is the voltage rating?");
            ImageRectQueryQuestion.Add("MOTORS", "Who is the manufacturer? | What is the model number? | What is the catalog number? | What is the ampere rating? | What is the horsepower / FLA? | What is the classification of the motor? | What is the motor class?");
            ImageRectQueryQuestion.Add("RELAYS", "Who is the manufacturer? | What is the model number? | What is the catalog number? | What is the serial number? | What is the C.T. ratio? | What is the relay classification?");
            ImageRectQueryQuestion.Add("SWITCHES", "Who is the manufacturer? | What is the model number? | What is the catalog number? | What is the voltage rating? | What is the ampere rating? | What is the fault withstand rating / AiC? | If there is a fuse , who is the fuse manufacturer? | If there is a fuse , what is the fuse size? | If there is a fuse , what is the model of the fuse? | If there is a fuse , what is the catalog of the fuse?");
            ImageRectQueryQuestion.Add("TRANSFER SWITCHES", "Who is the manufacturer? | What is the model number? | What is the catalog number? | What is the voltage rating? | What is the ampere rating? | What is the fault withstand rating / AiC?");
            ImageRectQueryQuestion.Add("TRANSOFRMERS", "Who is the manufacturer? | What is the model number? | What is the catalog number? | What is the serial number? | What is the voltage rating (high / low)? |	What is the frequency? | What is the percentage impedance / z%? | What is the kVA rating? | What is the winding configuration?");
            ImageRectQueryQuestion.Add("UPS SYSTEMS", "Who is the manufacturer? | What is the model number? | What is the catalog number? | What is the serial number? | What is the ampere rating? | What is the voltage rating?");
            ImageRectQueryQuestion.Add("Default Questions", "Who is the manufacturer? | what is the model number? | what is the high voltage or HV rating? | what is the high voltage current? | what is the percent impedance? | what is the frequency? ");
            return ImageRectQueryQuestion[class_type];
        }
        public static string ThermalProbableCauseDropdown(int index_id)
        {
            Dictionary<int, string> NECViolationDropdown = new Dictionary<int, string>();
            NECViolationDropdown.Add(1, "Internal Flaw");
            NECViolationDropdown.Add(2, "Overload");
            NECViolationDropdown.Add(3, "Poor Connection");
            return NECViolationDropdown[index_id];
        }
        public static string VendorCategoryTypesENUMs(int index_id)
        {
            Dictionary<int, string> VendorCategoryTypesENUMs = new Dictionary<int, string>();
            VendorCategoryTypesENUMs.Add(0, null);
            VendorCategoryTypesENUMs.Add(1, "Manufacturer");
            VendorCategoryTypesENUMs.Add(2, "Electrical Contractor");
            VendorCategoryTypesENUMs.Add(3, "Thermographer");
            VendorCategoryTypesENUMs.Add(4, "Other");
            return VendorCategoryTypesENUMs[index_id];
        }
        public static string ContactCategoryTypesENUMs(int index_id)
        {
            Dictionary<int, string> ContactCategoryTypesENUMs = new Dictionary<int, string>();
            ContactCategoryTypesENUMs.Add(0, null);
            ContactCategoryTypesENUMs.Add(1, "Customer");
            ContactCategoryTypesENUMs.Add(2, "Vendor");
            ContactCategoryTypesENUMs.Add(3, "Internal");
            ContactCategoryTypesENUMs.Add(4, "Other");
            return ContactCategoryTypesENUMs[index_id];
        }
        public static string AssetHealthIndex(string type)
        {
            Dictionary<string, string> AssetHealthIndex = new Dictionary<string, string>();
            AssetHealthIndex.Add("na", "0");
            AssetHealthIndex.Add("acceptable", "1");
            AssetHealthIndex.Add("alert", "2");
            AssetHealthIndex.Add("danger", "3");
            return AssetHealthIndex[type];
        }
    }

    public enum AttributesValueTypes
    {
        SingleSelect = 1,
        MultiSelect = 2,
        Text = 3,
        Number = 4
    }

    public static class FileFormatExtentions
    {
        public const string Excel = ".xls";
        public const string NewExcel = ".xlsx";
        public const string CSV = ".csv";
    }

    public enum StatusTypes
    {
        ActiveInactive = 1,
        Asset = 2,
        Inspection = 3,
        WorkOrder = 4,
        AssetReport = 5,
        SiteTypes = 6,
        ExecutiveReportStatus = 7,
        PMTypeStatus = 8,
        PMByStatus = 9,
        PMDateTimeRepeatTypeStatus = 10,
        Priority = 14,
        MaintenanceRequestStatus = 15,
        MaintenanceRequestCreation = 16,
        WorkOrderStatus = 17,
        WorkOrderCreation = 18,
        FormIOTemplateCondition = 19
    }

    public enum Status
    {
        Active = 1,
        Deactive = 2,
        AssetActive = 3,
        AssetDeactive = 4,
        InMaintenanace = 5,
        Disposed = 6,
        InTransfer = 7,
        Pending = 8,
        Cencelled = 9,
        Approved = 10,
        InspectionMaintenance = 11,
        Rejected = 16,
        New = 12,
        InProgress = 13,
        Waiting = 14,
        Completed = 15,
        ReportInProgress = 17,
        ReportCompleted = 18,
        ReportFailed = 19,
        AllSiteType = 20,
        AllCompanyType = 21,
        DailyReport = 22,
        WeeklyReport = 23,
        FixedOneTime = 24,
        Recurring = 25,
        Time = 26,
        MeterHours = 27,
        TimeMeterHours = 28,
        Month = 29,
        Year = 30,
        OverDue = 31,
        Due = 32,
        PMInProgress = 33,
        PMWaiting = 42,
        TriggerNew = 43,
        TriggerCompleted = 44,
        TriggetTaskNew = 35,
        TriggetTaskInProgress = 36,
        TriggetTaskWaiting = 37,
        TriggetTaskCompleted = 38,
        Week = 39,
        Day = 40,
        //Open = 41,
        
        PMCompleted = 38,
        MROpen = 48,
        MRCancelled = 49,
        MRCompeleted = 50,
        MRWorkOrderCreated = 51,
        Manual = 52,
        Inspection = 53,
        Low = 45,
        Medium = 46,
        High = 47,
        WOOpen = 54,
        WOInProgress = 55,
        WOCompleted = 56,
        WOCancelled = 57,
        WOReOpen = 58,
        WOManual = 59,
        WOInspection = 60,
        WOManualMaintenaceRequest = 61,
        Normal = 62,
        Intermedate = 63,
        Serious = 64,
        Critical = 65,
        Acceptance_Test_WO = 66,
        Maintenance_WO = 67,
        open = 68,
        Hold = 69,
        Ready_for_review = 70,
        Recheck = 71,
        PlannedWO = 72,
        ReleasedOpenWO = 73,
        Draft = 74,
        Submitted = 75,
        Onboarding_WO = 76,
        TroubleShoot_WO = 77,
        IR_Scan_WO = 78,
        Schedule = 80,
        Defered = 81,
        QuoteAccepted = 82,
        QuoteRejected = 83,
        QuoteWO = 84,
        running = 85
    }

    public enum ResponseStatusNumber
    {
        Success = 1,
        Error = -1,
        NotFound = -2,
        AlreadyExists = -3,
        Exceeded = -4,
        DateRequired = -5,
        DeActiveRecord = -6,
        AlreadyUsedToken = -7,
        TokenExpired = -8,
        InvalidToken = -9,
        False = 0,
        DeviceNotAssignToUserCompany = -6,
        DeviceRecordNotFound = -7,
        UnauthorizedDevice = -8,
        NotValidUser = -9,
        NotFoundInspectionForReport = -10,
        NotExists = -12,
        NotFoundCompanyCode = -11,
        DeviceUnAuthorized=-13,
        Forbidden = -14,
        Unauthorized = -50,
        AccessRevoke = -51,
        NewPasswordMustBeDifferent = -12,
        InvalidStatus = -15,
        PMPlansExist = -16,
        InvalidData = -17,
        TaskInUse = -18,
        invalidworkprocedure = -19,
        can_not_delete_parent_task = -20,
        internal_asset_id_must_be_unique = -21,
        form_is_used = -22,
        form_have_no_task = -23,
        no_form_available = -24,
        identification_must_be_unique = -25,
        nameplate_info_not_found = -26,
        invalidformname = -27,
        draft_form = -28,
        app_force_update = -29,
        WO_completed = -30,
        form_is_not_completed = -31,
        duplicate_wo_number = -32,
        asset_class_not_found = -33,
        asset_class_already_used = -34,
        qr_code_must_be_unique = -35,
        fed_by_wo_ines_must_be_completes = -36,
        can_not_delete_fed_by_wo_line = -37,
        class_not_having_form = -38,
        issue_is_not_open = -39,
        issue_is_assigned = -40,
        PMPlanIsLinked = -41,
        compoentlevel_wo_ines_must_be_completes = -42,
        equipment_number_must_be_unique = -43,
        can_not_delete_sublevel_woline = -44,
        can_not_delete_toplevel_woline = -45,
        PM_Master_Form_not_available = -46,
        SubLevel_Fedby_is_NotAllowed = -47,
        can_not_change_top_to_sub = -48,
        NetaReportFailed = -49,
        can_not_update_future_assetpm = -50,
        asset_in_different_location = -51,
        InValidSessionId = 440,
        TimedOut = -52,
        user_exist_with_single_site = -53,
    }

    public enum ResponseHeaderStatus
    {
        Success = 1,
        UpdateAvailable = -1,
        DownTime = -3
    }

    public enum NotificationStatus
    {
        AutoApproveInspection = 1,
        PendingNewInspection = 2,
        ManagerApproveInspection = 3,
        NewWorkOrderForInspection = 4,
        NewWorkOrderWithoutInspection = 5,
        UpdateWorkOrderStatus = 6,
        NewWorkOrderWithApprovedAsset = 7,
        UpdateWorkOrderPriority = 8,
        FirstDueDateReminder = 9,
        FirstMeterHoursDueReminder = 10,
        SecondDueDateReminder = 11,
        SecondMeterHoursDueReminder = 12,
        OnDueDateReminder = 13,
        OnMeterHoursDueReminder = 14,
    }

    public enum ActivityTypes {
        PMDue = 1,
        PMOverDue = 2,
        OperatorRequestForAsset = 3,
        AssetAutoApprove = 4,
        ManagerAcceptInspection = 5,
        NewIssueCreated = 6,
        IssueResolved = 7,
        PMNotificationLog = 8,

        WorkOrderCreated = 9,
        WorkOrderUpdated = 10,
        WorkOrderDeleted = 11,
        WorkOrderIssueUnlink = 12,
        WorkOrderIssueLink = 13,
        AssetRepairRequestCreated = 14,
        AssetRepairRequestInprogress = 15,
        AssetRepairRequestCompleted = 16,
        AssetReplaceRequestCreated = 17,
        IssueConditionChange = 18
    }

    public enum FilterActivityTypes {
        All = 0,
        Inspection = 1,
        Issue = 2,
        PM = 3
    }

    public enum CreatePDFTypes
    {
        User = 1,
        Asset = 2
    }

    public enum AssetStatus
    {
        PendingInspection = -1,
        AssetInMaintenance = -2,
        AlreadyHaveInspection = -3
    }

    public enum WorkOrderPriority
    {
        Very_High = 1,
        High = 2,
        Medium = 3,
        Low = 4
    }

    public static class AllowAction
    {
        public const string Index = "Index";
        public const string Register = "Register";
        public const string GetAuthStatus = "GetAuthStatus";
        public const string UpdateStatus = "UpdateStatus";
    }

    public enum Language
    {
        english = 1,
        spanish = 2
    }

    public enum PlatForm
    {
        App = 1,
        Web = 2
    }

    public static class Roles
    {
        public const string SuperAdmin = "ff52a40a-b130-4388-bb1c-4237f8dae72e";
        public const string Admin = "";
        public const string Manager = "a988c901-d5ed-4564-a969-ef0ac64725c6";
        public const string Operator = "0c36634f-5df4-4187-aa44-7440e2fd5989";
        public const string MS = "6bca326e-c8d9-4ae8-8b83-ab0334d5cabf";
    }

    public static class AuthenticationConstants
    {
        public const string MobilePlatform = "mobile";
        public const string WebPlatform = "web";
        public const string QRAuthType = "qr";
        public const string CredentialsAuthType = "credentials";
    }

    public enum Notification_Status
    {
        New = 1,
        Read = 2
    }

    public enum PMDashboardFilterType
    {
        Weekly = 1,
        Monthly = 2,
        Quarterly = 3
    }

    public enum NotificationType {
        FirstDueDate = 1,
        FirstMeterHours = 2,
        SecondDueDate = 3,
        SecondMeterHours = 4,
        FinalDueDate = 5,
        FinamMeterHours = 6
    }

    public enum DashboardPMType {
        CurrentUpcoming = 1,
        Completed = 2
    }
    public enum AssetHierarchyType
    {
        By_level = 1,
        multi_level = 2
    }

    public enum FormioTypes
    {
        All = 22
    }

    public enum inspectionVerdictnumber
    {
        acceptable = 1,
        alert = 2,
        danger = 3,
        defective = 4
    }
    public enum inspectionVerdictdropdownname
    {
        Pass = 1,
        Repair_Needed = 2,
        Replacement_Needed = 3,
        Defective = 4
    }
    public static class inspectionVerdictname
    {
        public const string acceptable = "acceptable";
        public const string alert = "alert";
        public const string danger = "danger";
        public const string defective = "defective";
        
    }
    public static class inspectionVerdictname_2
    {
        public const string acceptable = "pass";
        public const string alert = "repair needed";
        public const string danger = "replacement needed";
        public const string defective = "defective";
    }

    public enum AWSBucketsEnums
    {
        Asset_images = 1,
        wo_attachments = 2,
        inspection_images = 3,
        IR_Visual_Images = 4,
    }
    public enum condition_index_type
    {
        Poor_Corrosive = 1,
        Average = 2,
        Good = 3,
        Poor_Dusty = 4,
    }
    public enum criticality_index_type
    {
        Low = 1,
        Medium = 2,
        High = 3
    }
    public enum thermal_classification
    {
        OK = 1,
        Nominal = 2,
        Intermidiate = 3,
        Serious = 4,
        Critical = 5,
        Alert = 6
    }
    public enum MWO_inspection_wo_type
    {
        OnBoarding = 1,
        Inspection = 2,
        Repair = 3,
        Replace = 4,
        Trouble_Call_Check = 5,
        Others = 6,
        PM = 7
    }
    public enum MWO_inspection_status
    {
        Open = 1,
        Inprogress = 2,
        Done = 3
    }
    public enum MWO_inspection_Repair_resolution
    {
        Repair_completed_successfully = 1,
        Repair_could_not_be_completed = 2
    }
    public enum MWO_inspection_replacement_resolution
    {
        Replacement_completed_successfully = 1,
        Replacement_could_not_be_completed = 2
    }
    public enum MWO_inspection_recommended_action
    {
        Inspection = 1,
        Repair = 2,
        Replace = 3,
        No_Action_required = 4
    }
    public enum MWO_inspection_recommended_action_reschedule
    {
        Today = 1,
        Future = 2
    }
    public enum MWO_inspection_general_issue_resolution
    {
        Issue_completed_successfully = 1,
        Issue_could_not_be_completed = 2
    }
    public enum AssetOperatingConduitionState
    {
        Operating_Normally = 1,
        Repair_Needed = 2,
        Replacement_Needed = 3,
        Repair_Scheduled = 4,
        Replacement_Scheduled = 5,
        Decomissioned = 6,
        Spare = 7,
        InspectionNeeded = 8,
        Defective = 9,
        Repair_Inprogress = 10,
        Replace_Inprogress = 11,
    }
    public enum Asset_Placement
    {
        Indoor = 1,
        Outdoor = 2
    }
    public enum IRWO_Flag_Issues
    {
        Thermal_Anamoly_Detected = 1,
        NEC_Violation = 2,
        OSHA_Violation = 3
    }
    public enum Thermal_Anomaly_Probable_Cause
    {
        Internal_Flaw = 1,
        Overload = 2,
        Poor_Connection = 3
    }
    public enum Thermal_Anomaly_Recommendation
    {
        Continue_to_Monitor = 1,
        Replace_Component = 2,
        Verify_Clea_and_Tighten = 3
    }
    public enum NEC_Violations
    {
        Circuit_Breaker_is_restricted_from_freely_operating = 1,
        Circuit_Exceeds_panel_limit = 2,
        Component_Visible_Corrosion = 3,
        Conduit_Improperly_fastened_or_secured = 4,
        Enclosure_Missing_dead_front_door_cover_etc = 5,
        Enclosure_Must_be_free_of_foreign_materials = 6,
        Enclosure_Unused_opening_must_be_sealed = 7,
        Fuse_Parallel_fuses_must_match = 8,
        General_Lack_of_component_integrity = 9,
        General_Not_installed_in_a_proper_worklike_manner = 10,
        Grounding_Need_earth_connection = 11,
        Marking_Labels_Missing_or_Insufficient_information = 12,
        Mechanics_Damaged_broken_parts = 13,
        Missing_arc_flash_and_shock_hazard_warning_labels = 14,
        Plug_Fuse_Exposed_energized_parts = 15,
        Temperaature_Inadequate_ventilation_cooling_for_component = 16,
        Terminals_Connection_made_without_damaging_wire = 17,
        Wire_1_wire_per_terminal = 18,
        Wire_Improper_Neutral_Conductor = 18,
        Wire_Not_Protected_from_damage = 20,
        Wire_Size_wrong_for_load = 21,
        Wire_Wire_bundle_should_have_listed_bushing = 22,
        Wire_Wire_burned_or_damaged = 23
    }
    public enum OSHA_Violations
    {
        Clearance_Insufficient_Access = 1,
        Enclosure_Broken_locking_mechanism = 2,
        Enclosure_Damaged = 3,
        Enclosure_Should_be_waterproof = 4,
        Equipment_Free_of_Hazards = 5,
        Grounding_Must_be_permanent_continuous = 6,
        Lighting_Inadequate_around_equipment = 7,
        Marking_Labels_Inadequate_or_missing_information_on_equipment = 8,
        Mounting_Should_be_secure = 9,
        Noise_Excessive = 10,
        Wire_Exposed = 11
    }
    public enum AssetPhotoType
    {

        Asset_Profile = 1,
        Nameplate_Photo = 2,
        Thermal_Anomly_Photo = 3,
        NEC_Violation_Photo = 4,
        OSHA_Violation_Photo = 5,
        PM_Additional_General_photo = 6,
        PM_Additional_Nameplate_photo = 7,
        PM_Additional_Before_photo = 8,
        PM_Additional_After_photo = 9,
        PM_Additional_Environment_photo = 10,
        Asset_IR_Image = 11,
        Asset_Visual_Image = 12,
        Exterior_Photo = 13,
        Schedule_Photos = 14,
        Additional_Photos = 15,
        Repair_Woline_Issue_Photo = 16,
        Replace_Woline_Issue_photo = 17,
        Other_Woline_Issue_photo = 18,
        Issue_before_photo = 19,
        Issue_after_photo = 20,
        Ultrasonic_anamoly_photo = 21,
        NFPA_70B_Violation_Photo = 22
    }
    public enum code_compliance
    {
        compliant  = 1,
        non_compliant = 2
    }
    public enum WOLine_Temp_Issue_Caused
    {
        OSha_Violation = 1,
        NEC_Violation = 2,
        Thermal_Anamoly_Violation = 3,
        Repair_Visual = 4,
        Repair_mechanical = 5,
        Repair_electrical = 6,
        Replace_Visual = 7,
        Replace_mechanical = 8,
        Replace_electrical = 9,
        PM = 10,
        OB_repair = 11,
        OB_replace = 12,
        Ultrasonic_Anamoly = 13,
        NFPA_70B_Violation = 14
    }
    public enum WOLine_Temp_Issue_Title
    {
        Osha_Violation = 1,
        NEC_Violation = 2,
        Thermal_Anamoly = 3,
        Repair_Visual = 4,
        Repair_Mechanical = 5,
        Repair_Electrical = 6,
        Replace_Visual = 7,
        Replace_Mechanical = 8,
        Replace_Electrical = 9,
    }
    public enum WOLine_Temp_Issue_Type
    {
      //  Osha_Violation = 1,
       // NEC_Violation = 2,
        Compliance = 1,  // this will be for Osha and NEC
        Thermal_Anamoly = 2,
        Repair = 3,
        Replace = 4,
        Other = 6,
        Osha_Violation = 7,
        NEC_Violation = 8,
        ultrasonic_anamoly = 9
    }
    public static class Issue_Title
    {
        public const string Osha_Violation = "Osha Violation";
        public const string NEC_Violation = "NEC Violation";
        public const string Thermal_Anamoly = "Thermal Anamoly";
        public const string Ultrasonic_Anamoly = "Ultrasonic Anomaly";
        public const string Repair_Visual = "Repair - Visual";
        public const string Repair_Mechanical = "Repair - Mechanical";
        public const string Repair_Electrical = "Repair - Electrical";
        public const string Replace_Visual = "Replace - Visual";
        public const string Replace_Mechanical = "Replace - Mechanical";
        public const string Replace_Electrical = "Replace - Electrical";
        public const string NFPA_70B_Violation = "NFPA 70B Violation";

    }
    public static class Issue_Description
    {
        public const string visual_issue = "Visual Issue";
        public const string mechanical_issue = "Mechanical Issue";
        public const string electrical_issue = "Electrical Issue";

    }
    public enum IssueImageDuration
    {
        before = 1,
        after = 2,
        ir_visual = 3,
    }

    public enum CompleteWOThreadStatus
    {
        Inprogress = 1,
        Completed = 2,
        Failed = 3
    }
    public enum AddLocationType
    {
        Building = 1 ,
        Floor = 2 ,
        Room = 3,
        Section = 4
    }
    public enum BulkDataImportStatus
    {
        Completed = 1,
        Inprogress = 2,
        Failed = 3
    }
    public enum ComponentLevelTypes
    {
        ToplevelComponent = 1,
        SublevelComponent = 2
    }
    public enum ClusterOneLinePdf
    {
        Completed = 1,
        Inprogress = 2,
        Failed = 3
    }
    public enum PMInspectionTypeId
    {
        IRThermography = 1,
    }
    public enum CalibrationStatus
    {
        Calibrated = 1,
        NotCalibrated = 2,
        NA = 3
    }

    public enum IssueCreationtype
    {
        existing_issue = 1,
        new_issue = 2
    }

    public enum NewIssueAssettype
    {
        NewAsset = 1,
        ExistingAsset = 2,
        VerifyOnField = 3
    }
    public enum WatcherRefType
    {
        Workorder = 1,
    }
    public enum WatcherUserRoleType
    {
        BackOffice_User = 1,
    }
    public enum NotificationUserRoleType
    {
        BackOffice_User = 1,
        Technician_User = 2
    }
    public enum NotificationType_Version_2
    {
        WorkOrderAssignedToTechnician = 1, 
        SiteAssignedToUser = 2,
        AllWOLinesCompletedORReadyForReviewOfWO = 3,
        WorkOrder_is_Completed_With_Issue_Created = 4,
        AssignedWorkorderIsDue = 5,
        WorkorderIsOverDue = 6,
    }
    public enum wo_due_overdue_flag
    {
        WO_Due = 1,
        WO_Overdue = 2,
        WO_OnTrack = 3
    }
    public enum PMWOlineAssettype
    {
        ExistingAsset = 1,
        TempAsset = 2 ,
        NewAsset = 3
    }
    public enum pm_due_overdue_flag
    {
        PM_Due = 1,
        PM_Overdue = 2,
        PM_OnTrack = 3
    }

    public enum TimeMaterials_Category_Type
    {
        Labor = 1,
        Materials = 2,
        Miscellaneous = 3,
        Subcontracts = 4,
        Indirect_Labor_Cost = 5,
        Indirect_Job_Cost = 6,
        Third_Party_Rental = 7
    }
    public enum Quantity_Unit_Type
    {
        Unit = 1,
        Feet = 2,
        Blank = 3
    }
    public enum BurdenType
    {
        Dollar = 1,
        Percentage = 2
    }
    public enum ConductorTypes
    {
        Copper = 1,
        Aluminum = 2
    }
    public enum RacewayTypes
    {
        Metallic = 1,
        NonMetallic = 2
    }

    public enum NetaInspectionReportStatusType
    {
        Completed = 1,
        InProgress = 2,
        PartialCompleted = 3,
        Failed = 4
    }

    public enum report_inspection_type
    {
        AcceptanceTest = 1,
        Maintenance = 2
    }

    public enum maintenance_condition_index_type
    {
        Serviceable = 1,
        LimitedService = 2,
        NonServiceable = 3
    }
    public enum ir_visual_camera_type
    {
        FLIR = 1,
        FLUKE = 2
    }
    public enum ir_visual_image_type
    {
        IR_Image_Only = 1,
        Visual_Image_Only = 2,
        IR_and_Visual = 3,
    }
    public enum WOLineActionsTypes
    {
        Save = 1,
        Submit = 2,
        Accept = 3,
        Reject = 4,
        Hold = 5,
        Delete = 6,
        WorkStartDate = 7
    }

    public static class ImageRectQueryQuestion
    {
        public const string ARRESTERS = "Who is the manufacturer? , What is the model / catalog #? , What is the ampere rating? , What is the voltage rating?";
        public const string BATTERIES = "Who is the manufacturer? , What is the model / catalog #? , What is the ampere rating? , What is the voltage rating?";
        public const string BUS = "Who is the manufacturer? , What is the model / catalog #? , What is the ampere rating? , What is the voltage rating?";
        public const string CAPACITORS = "Who is the manufacturer? , What is the model / catalog #? , What is the ampere rating? , What is the voltage rating?";
        public const string CIRCUIT_BREAKERS = "Who is the manufacturer? , What is the frame ampere rating? , What is the model / catalog #? , What is the frame fault withstand / AiC rating? , What is the trip unit model / catalog #? , What is the trip unit ampere rating? , What is the voltage rating?";
        public const string ELECTRICAL_PANELS = "Who is the manufacturer? , What is the model / catalog #? , What is the serial number? , What is the voltage rating? , What is the configuration? , What is the ampere rating? ";
        public const string Repair_Mechanical = "Repair - Mechanical";
        public const string Repair_Electrical = "Repair - Electrical";
        public const string Replace_Visual = "Replace - Visual";
        public const string Replace_Mechanical = "Replace - Mechanical";
        public const string Replace_Electrical = "Replace - Electrical";

    }
    public enum panel_schedule_type
    {
        current = 1,
        needs_updating = 2,
        missing = 3,
    }
    public enum Ultrasonic_Anamoly_type
    {
        Crack = 1,
        Void = 2,
        Delamination = 3,
        Inclusions = 4,
        Corrosion = 5,
        Porosity = 6
    }
    public enum SeverityCriteriaType
    {
        Similar = 1,
        Ambient = 2,
        Indirect = 3
    }
    public enum ReportStatus
    {
        ReportInProgress = 17,
        ReportCompleted = 18,
        ReportFailed = 19,
        ReportRequestTimeOut = 20,
    }
    public enum FeatureTypes
    {
        EstimatorFeature = 1,
    }
    public enum Vendor_CategoryTypes
    {
        Manufacturer = 1,
        ElectricalContractor = 2,
        Thermographer = 3,
        Other = 4
    }
    public enum Contact_CategoryTypes
    {
        Customer = 1,
        Vendor = 2,
        Internal = 3,
        Other = 4
    }
    public enum Contact_Invite_Status
    {
        Accepted = 1,
        Rejected = 2,
        Pending = 3,
        Maybe = 4
    }

    public enum InpsectionFormTypes
    {
        Acceptance_Test_Form,
        Maintenance_Test_Form,
        Preventative_Maintenance_Test_Form
    }

    public enum PM_Work_Procedure_Type
    {
        Default = 1,
        Custom = 2
    }
}


using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.Shared.Helper
{
    public class FormIOObject
    {
        // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
        public class Attr
        {
            public string attr { get; set; }
            public string value { get; set; }
        }

        public class Attributes
        {
        }

        public class BLongTimeElementSettingsB
        {
            public string textField { get; set; }
            public string textField1 { get; set; }
            public string ltpuRanges { get; set; }
            public string ltdRanges { get; set; }
            public string ltpuAsFound { get; set; }
            public string ltdAsFound { get; set; }
            public string ltpuAsLeft { get; set; }
            public string ltdAsLeft { get; set; }
            public string ltpuAsTested { get; set; }
            public string ltdAsTested { get; set; }
        }

        public class BLongTimeElementSettingsB1
        {
            public string textField { get; set; }
            public string textField1 { get; set; }
            public string textField2 { get; set; }
            public string stpuRanges { get; set; }
            public string stdRanges { get; set; }
            public string stiRanges { get; set; }
            public string stpuAsFound { get; set; }
            public string stdAsFound { get; set; }
            public string stiAsFound { get; set; }
            public string stpuAsLeft { get; set; }
            public string stdAsLeft { get; set; }
            public string stiAsLeft { get; set; }
            public string stpuAsTested { get; set; }
            public string stdAsTested { get; set; }
            public string stiAsTested { get; set; }
        }

        public class BLongTimeElementSettingsB2
        {
            public string textField { get; set; }
            public string textField1 { get; set; }
            public string textField2 { get; set; }
            public string gfpuRanges { get; set; }
            public string gfdRanges { get; set; }
            public string gfiRanges { get; set; }
            public string gfpuAsFound { get; set; }
            public string gfdAsFound { get; set; }
            public string gfiAsFound { get; set; }
            public string gfpuAsLeft { get; set; }
            public string gfdAsLeft { get; set; }
            public string gfiAsLeft { get; set; }
            public string gfpuAsTested { get; set; }
            public string gfdAsTested { get; set; }
            public string gfiAsTested { get; set; }
        }

        public class BLongTimeElementSettingsB3
        {
            public string textField { get; set; }
            public string instPickUpRanges { get; set; }
            public string instPickupAsFound { get; set; }
            public string instPickupAsLeft { get; set; }
            public string instPickupAsTested { get; set; }
        }

        public class Column
        {
            public List<Component> components { get; set; }
            public int width { get; set; }
            public int offset { get; set; }
            public int push { get; set; }
            public int pull { get; set; }
            public string size { get; set; }
            public int currentWidth { get; set; }
        }

        public class Component
        {
            public string label { get; set; }
            public string labelWidth { get; set; }
            public string labelMargin { get; set; }
            public string tag { get; set; }
            public string className { get; set; }
            public List<Attr> attrs { get; set; }
            public string content { get; set; }
            public bool refreshOnChange { get; set; }
            public string customClass { get; set; }
            public bool hidden { get; set; }
            public bool modalEdit { get; set; }
            public string key { get; set; }
            public List<object> tags { get; set; }
            public Properties properties { get; set; }
            public Conditional conditional { get; set; }
            public string customConditional { get; set; }
            public List<object> logic { get; set; }
            public Attributes attributes { get; set; }
            public Overlay overlay { get; set; }
            public string type { get; set; }
            public bool input { get; set; }
            public bool tableView { get; set; }
            public string placeholder { get; set; }
            public string prefix { get; set; }
            public string suffix { get; set; }
            public bool multiple { get; set; }
            public object defaultValue { get; set; }
            public bool @protected { get; set; }
            public bool unique { get; set; }
            public bool persistent { get; set; }
            public bool clearOnHide { get; set; }
            public string refreshOn { get; set; }
            public string redrawOn { get; set; }
            public bool dataGridLabel { get; set; }
            public string labelPosition { get; set; }
            public string description { get; set; }
            public string errorLabel { get; set; }
            public string tooltip { get; set; }
            public bool hideLabel { get; set; }
            public string tabindex { get; set; }
            public bool disabled { get; set; }
            public bool autofocus { get; set; }
            public bool dbIndex { get; set; }
            public string customDefaultValue { get; set; }
            public string calculateValue { get; set; }
            public bool calculateServer { get; set; }
            public Widget widget { get; set; }
            public string validateOn { get; set; }
            public Validate validate { get; set; }
            public bool allowCalculateOverride { get; set; }
            public bool encrypted { get; set; }
            public bool showCharCount { get; set; }
            public bool showWordCount { get; set; }
            public bool allowMultipleMasks { get; set; }
            public List<object> addons { get; set; }
            public string id { get; set; }
            public bool? tree { get; set; }
            public bool? lazyLoad { get; set; }
            public List<Component> components { get; set; }
            public string title { get; set; }
            public string theme { get; set; }
            public bool? collapsible { get; set; }
            public string breadcrumb { get; set; }
            public bool? collapsed { get; set; }
            public string size { get; set; }
            public bool? block { get; set; }
            public string action { get; set; }
            public bool? disableOnInvalid { get; set; }
            public string leftIcon { get; set; }
            public string rightIcon { get; set; }
            public List<Column> columns { get; set; }
            public bool autoAdjust { get; set; }
            public string inputMask { get; set; }
            public string displayMask { get; set; }
            public string autocomplete { get; set; }
            public bool? mask { get; set; }
            public bool? spellcheck { get; set; }
            public string inputFormat { get; set; }
            public string @case { get; set; }
            public bool? truncateMultipleSpaces { get; set; }
            public string errors { get; set; }
            public string inputType { get; set; }
            public string displayInTimezone { get; set; }
            public bool? useLocaleSettings { get; set; }
            public bool? allowInput { get; set; }
            public string format { get; set; }
            public List<object> shortcutButtons { get; set; }
            public bool? enableDate { get; set; }
            public DatePicker datePicker { get; set; }
            public bool? enableTime { get; set; }
            public TimePicker timePicker { get; set; }
            public string defaultDate { get; set; }
            public CustomOptions customOptions { get; set; }
            public bool? enableMinDateInput { get; set; }
            public bool? enableMaxDateInput { get; set; }
            public string timezone { get; set; }
            public string datepickerMode { get; set; }
        }

        public class Conditional
        {
            public object show { get; set; }
            public object when { get; set; }
            public string eq { get; set; }
            public string json { get; set; }
        }

        public class ContactResistanceTests
        {
            public ResistanceInMicroOhms resistanceInMicroOhms { get; set; }
        }

        public class CustomOptions
        {
        }

        public class Data
        {
         //   public string customer { get; set; }
       //     public string customerAddress { get; set; }
       //     public string owner { get; set; }
       //     public string ownerAddress { get; set; }
            public DateTime date { get; set; }
            public string workOrderNumber { get; set; }
            public string temperature { get; set; }
            public string humidity { get; set; }
          //  public string location { get; set; }
          //   public string identification { get; set; }
         //    public string assetId { get; set; }
            public NameplateInformation nameplateInformation { get; set; }
            public header header { get; set; } = new header();
            public TripUnitInformation tripUnitInformation { get; set; }
            public TripUnitSettings tripUnitSettings { get; set; }
            public VisualInspection visualInspection { get; set; }
            public OperationalTests operationalTests { get; set; }
            public InsulationResistanceTests insulationResistanceTests { get; set; }
            public ContactResistanceTests contactResistanceTests { get; set; }
            public TripTests tripTests { get; set; }
            public footer footer { get; set; }
            public bool submit { get; set; }
        }
        public class header
        {
            public string customer { get; set; }
            public string customerAddress { get; set; }
            public string owner { get; set; }
            public string ownerAddress { get; set; }
            public string parent { get; set; }
            //public string location { get; set; }
            public string identification { get; set; }
            public string assetId { get; set; }
            public string date { get; set; }
            public string workOrder { get; set; }
            public string building { get; set; }
            public string floor { get; set; }
            public string room { get; set; }
            public string section { get; set; }
            public string note { get; set; }
        }

        public class footer
        {
            public string comments { get; set; }
        }
        public class DatePicker
        {
            public string disable { get; set; }
            public string disableFunction { get; set; }
            public bool disableWeekends { get; set; }
            public bool disableWeekdays { get; set; }
            public object minDate { get; set; }
            public object maxDate { get; set; }
            public bool showWeeks { get; set; }
            public int startingDay { get; set; }
            public string initDate { get; set; }
            public string minMode { get; set; }
            public string maxMode { get; set; }
            public int yearRows { get; set; }
            public int yearColumns { get; set; }
        }

        public class GroundFaultElementsTripTest
        {
            public string LTEpole1AF { get; set; }
            public string LTEpole2AF { get; set; }
            public string LTEpole3AF { get; set; }
            public string LTEpole1AL { get; set; }
            public string LTEpole2AL { get; set; }
            public string LTEpole3AL { get; set; }
        }

        public class InstantaneousElementsTripTest
        {
            public string LTEpole1AF { get; set; }
            public string LTEpole2AF { get; set; }
            public string LTEpole3AF { get; set; }
            public string LTEpole1AL { get; set; }
            public string LTEpole2AL { get; set; }
            public string LTEpole3AL { get; set; }
        }

        public class InsulationResistancePoleToPole
        {
            public string p1AsLeft1 { get; set; }
            public string p2AsLeft1 { get; set; }
            public string p3AsLeft1 { get; set; }
            public string p1AsFound { get; set; }
            public string p2AsFound { get; set; }
            public string p3AsFound { get; set; }
            public InsulationResistancePoleToPole1 insulationResistancePoleToPole1 { get; set; }
        }

        public class InsulationResistancePoleToPole1
        {
            public string p1AsLeft1 { get; set; }
            public string p2AsLeft1 { get; set; }
            public string p3AsLeft1 { get; set; }
            public string p1AsFound { get; set; }
            public string p2AsFound { get; set; }
            public string p3AsFound { get; set; }
        }

        public class InsulationResistanceTests
        {
            public InsulationResistancePoleToPole insulationResistancePoleToPole { get; set; }
        }

        public class LongTimeElementsTripTest
        {
            public string LTEpole1AF { get; set; }
            public string LTEpole2AF { get; set; }
            public string LTEpole3AF { get; set; }
            public string LTEpole1AL { get; set; }
            public string LTEpole2AL { get; set; }
            public string LTEpole3AL { get; set; }
        }

        public class NameplateInformation
        {
            public string manufacturer { get; set; }
            public string model { get; set; }
            public string name { get; set; }
            public string catalogNumber { get; set; }
            public string frameAmpereRating { get; set; }
            public string trippingVoltage { get; set; }
            public string closingVoltage { get; set; }
            public string type { get; set; }
            public string serialNumber { get; set; }
            public string voltageRating { get; set; }
            public string interruptingKARating { get; set; }
            public string chargingVoltage { get; set; }
            public string shuntTripVoltageRating { get; set; }
        }

        public class OperationalTests
        {
            public string manualOpen { get; set; }
            public string electricallyOpen { get; set; }
            public string manualCharge { get; set; }
            public string tripWithProtectiveDevices { get; set; }
            public string manualClose { get; set; }
            public string electricallyClose { get; set; }
            public string electricallyCharge { get; set; }
        }

        public class Overlay
        {
            public string style { get; set; }
            public string page { get; set; }
            public string left { get; set; }
            public string top { get; set; }
            public string width { get; set; }
            public string height { get; set; }
        }

        public class Properties
        {
        }

        public class ResistanceInMicroOhms
        {
            public string p1AsLeft1 { get; set; }
            public string p2AsLeft1 { get; set; }
            public string p3AsLeft1 { get; set; }
            public string p1AsFound { get; set; }
            public string p2AsFound { get; set; }
            public string p3AsFound { get; set; }
        }

        public class Root
        {
            public string display { get; set; }
            public object components { get; set; }
            public Data data { get; set; }
        }

        public class ShortTimeElementsTripTest
        {
            public string LTEpole1AF { get; set; }
            public string LTEpole2AF { get; set; }
            public string LTEpole3AF { get; set; }
            public string LTEpole1AL { get; set; }
            public string LTEpole2AL { get; set; }
            public string LTEpole3AL { get; set; }
        }

        public class TimePicker
        {
            public int hourStep { get; set; }
            public int minuteStep { get; set; }
            public bool showMeridian { get; set; }
            public bool readonlyInput { get; set; }
            public bool mousewheel { get; set; }
            public bool arrowkeys { get; set; }
        }

        public class TripTests
        {
            public LongTimeElementsTripTest longTimeElementsTripTest { get; set; }
            public ShortTimeElementsTripTest shortTimeElementsTripTest { get; set; }
            public GroundFaultElementsTripTest groundFaultElementsTripTest { get; set; }
            public InstantaneousElementsTripTest instantaneousElementsTripTest { get; set; }
        }

        public class TripUnitInformation
        {
            public string manufacturer { get; set; }
            public string catalogNumber { get; set; }
            public string sensorCtAmpereRating { get; set; }
            public string tripModuleAmpereRating { get; set; }
            public string model { get; set; }
            public string plugAmpereRating { get; set; }
        }

        public class TripUnitSettings
        {
            public BLongTimeElementSettingsB bLongTimeElementSettingsB { get; set; }
            public BLongTimeElementSettingsB1 bLongTimeElementSettingsB1 { get; set; }
            public BLongTimeElementSettingsB2 bLongTimeElementSettingsB2 { get; set; }
            public BLongTimeElementSettingsB3 bLongTimeElementSettingsB3 { get; set; }
        }

        public class Validate
        {
            public bool required { get; set; }
            public string custom { get; set; }
            public bool customPrivate { get; set; }
            public bool strictDateValidation { get; set; }
            public bool multiple { get; set; }
            public bool unique { get; set; }
            public string minLength { get; set; }
            public string maxLength { get; set; }
            public string minWords { get; set; }
            public string maxWords { get; set; }
            public string pattern { get; set; }
            public string customMessage { get; set; }
            public string json { get; set; }
        }

        public class VisualInspection
        {
            public string circuitBreaker { get; set; }
            public string operatingMechanism { get; set; }
            public string electricalConnections { get; set; }
            public string mainContacts { get; set; }
            public string arcingContacts { get; set; }
            public string contactSequence { get; set; }
            public string auxiliaryContacts { get; set; }
            public string arcChutes { get; set; }
            public string cubicle { get; set; }
            public string grounded { get; set; }
            public string auxiliaryDevices { get; set; }
            public string panelLights { get; set; }
            public string rackingMechanism { get; set; }
            public string shuntTripOperation { get; set; }
        }

        public class Widget
        {
            public string type { get; set; }
            public string displayInTimezone { get; set; }
            public string locale { get; set; }
            public bool? useLocaleSettings { get; set; }
            public bool? allowInput { get; set; }
            public string mode { get; set; }
            public bool? enableTime { get; set; }
            public bool? noCalendar { get; set; }
            public string format { get; set; }
            public int? hourIncrement { get; set; }
            public int? minuteIncrement { get; set; }
            public bool? time_24hr { get; set; }
            public object minDate { get; set; }
            public string disabledDates { get; set; }
            public bool? disableWeekends { get; set; }
            public bool? disableWeekdays { get; set; }
            public string disableFunction { get; set; }
            public object maxDate { get; set; }
        }



    }
}

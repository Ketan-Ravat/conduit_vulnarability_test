using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels.RequestResponseViewModel
{
    public class GetAssetFormDataForBulkImportResponsemodel
    {
        public List<form_categoty_list_bulk_impoer> form_category_list { get; set; }
        public List<FormIOMasterFormsBulkimport> master_forms { get; set; }
    }
    public class form_categoty_list_bulk_impoer
    {
        public Guid wo_inspectionsTemplateFormIOAssignment_id { get; set; }
        public List<GetWOcategoryTaskByCategoryIDListBulkImport> task_list { get; set; }
    }
    public class GetWOcategoryTaskByCategoryIDListBulkImport
    {

        public Guid WOcategorytoTaskMapping_id { get; set; }
        public Guid wo_inspectionsTemplateFormIOAssignment_id { get; set; }
        public GetFormByWOTaskIDBulkImpoert task_form { get; set; }
    }
    public class GetFormByWOTaskIDBulkImpoert
    {
        public Guid asset_form_id { get; set; }
        public string asset_form_data { get; set; }


    }
    public class FormIOMasterFormsBulkimport
    {
        public Guid form_id { get; set; }
        public string form_data { get; set; }
    }
}

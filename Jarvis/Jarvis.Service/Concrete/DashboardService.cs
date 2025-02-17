using Amazon.Runtime.Internal.Util;
using AutoMapper;
using Jarvis.db.Models;
using Jarvis.Service.Abstract;
using Jarvis.Shared.StatusEnums;
using Jarvis.Shared.Utility;
using Jarvis.ViewModels.RequestResponseViewModel;
using Jarvis.ViewModels.ViewModels;
using Org.BouncyCastle.Asn1.Ocsp;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jarvis.Service.Concrete
{
    public class DashboardService : BaseService , IDashboardService
    {
        private readonly IMapper _mapper;
        private Shared.Utility.Logger _logger;
        public DashboardService(IMapper mapper) : base(mapper)
        {
            _mapper = mapper;
            _logger = Shared.Utility.Logger.GetInstance<DashboardService>();
        }


        public GetAdminDashboardCountResponseModel GetAdminDashboardCount()
        {
            GetAdminDashboardCountResponseModel response = new GetAdminDashboardCountResponseModel();
            try
            {
                int get_sites_count = _UoW.DashboardRepository.GetActiveSiteCount();
                int get_workorders_count = _UoW.DashboardRepository.GetActiveWorkordersCount();
                int get_technicians_count = _UoW.DashboardRepository.GetActiveTechniciansCount();

                int due_wo_count = _UoW.DashboardRepository.GetWODueOverdueFlagWiseCount((int)wo_due_overdue_flag.WO_Due);
                int overdue_wo_count = _UoW.DashboardRepository.GetWODueOverdueFlagWiseCount((int)wo_due_overdue_flag.WO_Overdue);
                int released_open_wo_count = _UoW.DashboardRepository.GetWOStatusWiseCount((int)Status.ReleasedOpenWO);
                int planned_wo_count = _UoW.DashboardRepository.GetWOStatusWiseCount((int)Status.PlannedWO);
                int hold_wo_count = _UoW.DashboardRepository.GetWOStatusWiseCount((int)Status.Hold);
                int completed_wo_count = _UoW.DashboardRepository.GetWOStatusWiseCount((int)Status.Completed);
                int inprogress_wo_count = _UoW.DashboardRepository.GetWOStatusWiseCount((int)Status.WOInProgress);

                int neta_forms_count = _UoW.DashboardRepository.GetNetaInspectionFormLineItemsCount();
                int regular_form_count = _UoW.DashboardRepository.GetRegularInspectionFormLineItemsCount();

                response.active_sites_count = get_sites_count;
                response.active_workorders_count = get_workorders_count;
                response.active_technicians_count = get_technicians_count;
                response.neta_inspection_line_items_count = neta_forms_count;
                response.regular_inspection_line_items_count = regular_form_count;

                response.wo_status_wise_count_object = new WorkorderStatusWiseCountObject();

                response.wo_status_wise_count_object.due_wo_count = due_wo_count;
                response.wo_status_wise_count_object.overdue_wo_count = overdue_wo_count;
                response.wo_status_wise_count_object.released_open_wo_count = released_open_wo_count;
                response.wo_status_wise_count_object.planned_wo_count = planned_wo_count;
                response.wo_status_wise_count_object.hold_wo_count = hold_wo_count;
                response.wo_status_wise_count_object.inprogress_wo_count = inprogress_wo_count;
                response.wo_status_wise_count_object.completed_wo_count = completed_wo_count;
            }
            catch (Exception ex)
            {
            }
            return response;
        }

        //Top 20 InProgress Workorders and Overdue Workorders
        public ListViewModel<GetInProgressWorkordersListResponseModel> GetAllWorkordersListByStatus(GetTopWorkordersListByStatusRequestModel request)
        {
            ListViewModel<GetInProgressWorkordersListResponseModel> response = new ListViewModel<GetInProgressWorkordersListResponseModel>();
            try
            {
                var get_wo_list = _UoW.DashboardRepository.GetAllWorkordersListByStatus(request);
                var get_mapped_wo_list = _mapper.Map<List<GetInProgressWorkordersListResponseModel>>(get_wo_list.Item1);

                response.list = get_mapped_wo_list;
                response.listsize = get_wo_list.Item2;
                response.pageIndex = request.pageindex;
                response.pageSize = request.pagesize;
            }
            catch (Exception ex)
            {
            }
            return response;
        }


        // Get All Line Items Ready For Review --- WoonboardingAssets and AssetFormIO 
        public ListViewModel<GetInProgressWorkordersListResponseModel> GetAllLineItemsByStatus(GetTopWorkordersListByStatusRequestModel request)
        {
            ListViewModel<GetInProgressWorkordersListResponseModel> response = new ListViewModel<GetInProgressWorkordersListResponseModel>();
            try
            {
                if (request.is_requested_for_neta_inspection_forms)
                {
                    request.status = (int)Status.Ready_for_review;
                    var get_asset_formio_list = _UoW.DashboardRepository.GetAllAssetFormIOByStatus(request);
                    var get_mapped_asset_formio_list = _mapper.Map<List<GetInProgressWorkordersListResponseModel>>(get_asset_formio_list.Item1);

                    /* //Time Elapsed Calculation
                    foreach (var wo in get_mapped_asset_formio_list)
                    {
                        string time_elapsed = null;
                        if (wo.modified_at != null)
                        {
                            time_elapsed = DateTimeUtil.GetDueInNewFlow(wo.modified_at.Value.Date, DateTime.UtcNow.Date);
                        }
                        else
                        {
                            time_elapsed = DateTimeUtil.GetDueInNewFlow(wo.created_at.Value.Date, DateTime.UtcNow.Date);
                        }
                        wo.time_elapsed = time_elapsed;
                    }*/

                    response.list = get_mapped_asset_formio_list;
                    response.listsize = get_asset_formio_list.Item2;
                }
                else
                {
                    request.status = (int)Status.Ready_for_review;
                    var get_woline_list = _UoW.DashboardRepository.GetAllWOOBAssetsByStatus(request);
                    var get_mapped_woline_list = _mapper.Map<List<GetInProgressWorkordersListResponseModel>>(get_woline_list.Item1);

                    response.list = get_mapped_woline_list;
                    response.listsize = get_woline_list.Item2;
                }
                response.pageIndex = request.pageindex;
                response.pageSize = request.pagesize;
            }
            catch (Exception ex)
            {
            }
            return response;
        }


        //Technicians LineItems Submitted Last 30 Days --- WoonboardingAssets and AssetFormIO
        public ListViewModel<GetTechniciansListWithSubmittedLinesCountResponseModel> GetTechniciansListWithSubmittedLinesCount(int pagesize, int pageindex)
        {
            ListViewModel<GetTechniciansListWithSubmittedLinesCountResponseModel> response = new ListViewModel<GetTechniciansListWithSubmittedLinesCountResponseModel>();
            try
            {
                var get_all_userids_who_submitted_wolines = _UoW.DashboardRepository.GetTechniciansWhoSubmittedWOLinesOfLast30Days();

                var get_all_userids_who_submitted_formio = _UoW.DashboardRepository.GetTechniciansWhoSubmittedAssetFormIOOfLast30Days();

                var all_technicians_ids = get_all_userids_who_submitted_wolines.Concat(get_all_userids_who_submitted_formio).Distinct().ToList();

                response.listsize = all_technicians_ids.Count; //List Size and Pagination
                if (pageindex > 0 && pagesize > 0) { all_technicians_ids = all_technicians_ids.Skip((pageindex - 1) * pagesize).Take(pagesize).ToList(); }

                var site = _UoW.UserRepository.GetSiteBySiteId(Guid.Parse(UpdatedGenericRequestmodel.CurrentUser.site_id));

                foreach (var technician_id in all_technicians_ids)
                {
                    GetTechniciansListWithSubmittedLinesCountResponseModel responseModel = new GetTechniciansListWithSubmittedLinesCountResponseModel();
                    responseModel.user_id = Guid.Parse(technician_id);

                    var username = _UoW.UserRepository.GetUserNameByID(technician_id);
                    responseModel.technician_user_name = username;

                    int count = get_all_userids_who_submitted_wolines.Count(x=>x == technician_id) + get_all_userids_who_submitted_formio.Count(x => x == technician_id);
                    responseModel.submitted_line_items_count = count;
                    
                    responseModel.site_id = Guid.Parse(UpdatedGenericRequestmodel.CurrentUser.site_id);
                    responseModel.site_name = site.site_name;

                    response.list.Add(responseModel);
                }
                response.pageIndex = pageindex;
                response.pageSize = pagesize;
            }
            catch (Exception ex)
            {
            }
            return response;
        }


        public UpcomingSiteOpportunitiesDashboardResponseModel UpcomingSiteOpportunitiesDashboard(UpcomingSiteOpportunitiesDashboardRequestModel requestmodel)
        {
            AssetPMService assetPMService = new AssetPMService(_mapper);
            UpcomingSiteOpportunitiesDashboardResponseModel response = new UpcomingSiteOpportunitiesDashboardResponseModel();
            response.graph1 = new List<class_code_due_month_wise_asset_pms>();
            response.graph2 = new List<class_code_due_month_wise_asset_pms>();
            response.graph3 = new List<class_code_due_month_wise_asset_pms>();

            try
            {
                var active_assets_count = _UoW.DashboardRepository.GetActiveAssetsCountBySites(requestmodel);
                
                var open_issues_count = _UoW.DashboardRepository.GetOpenAssetIssuesCountBySites(requestmodel);

                var open_pms_count = _UoW.DashboardRepository.GetOpenAssetPMsCountBySites(requestmodel);

                GetAssetPMListRequestmodel getAssetPMListRequestmodel = new GetAssetPMListRequestmodel();
                getAssetPMListRequestmodel.pagesize = 0;
                getAssetPMListRequestmodel.pageindex = 0;
                var get_all_asset_pms = assetPMService.GetAssetPMList(getAssetPMListRequestmodel);

                // Upcoming Visual/Mechanical/Cleaning/Lubrication Opportunities  -- Graph 1
                var graph1_list = get_all_asset_pms.list.Where(x=>x.status != (int)Status.Completed 
                        && (x.title.ToLower().Replace(" ","") == "visualinspection" || x.title.ToLower().Replace(" ", "") == "mechanicalservicing"
                                  || x.title.ToLower().Replace(" ", "") == "cleaning" || x.title.ToLower().Replace(" ", "") == "lubrication") ).ToList();

                var class_wise_pms_for_graph1 = graph1_list.GroupBy(x=>x.asset_class_code);

                foreach(var v1 in class_wise_pms_for_graph1)
                {
                    class_code_due_month_wise_asset_pms class_code_due_month_wise_asset_pms = new class_code_due_month_wise_asset_pms();
                    class_code_due_month_wise_asset_pms.asset_class_code = v1.Select(x => x.asset_class_code).FirstOrDefault();

                    class_code_due_month_wise_asset_pms.due_in_1 = v1.Where(x => x.due_date.Value.Month == 1 && x.due_date.Value.Year == DateTime.UtcNow.Year).Count();
                    class_code_due_month_wise_asset_pms.due_in_2 = v1.Where(x => x.due_date.Value.Month == 2 && x.due_date.Value.Year == DateTime.UtcNow.Year).Count();
                    class_code_due_month_wise_asset_pms.due_in_3 = v1.Where(x => x.due_date.Value.Month == 3 && x.due_date.Value.Year == DateTime.UtcNow.Year).Count();
                    class_code_due_month_wise_asset_pms.due_in_4 = v1.Where(x => x.due_date.Value.Month == 4 && x.due_date.Value.Year == DateTime.UtcNow.Year).Count();
                    class_code_due_month_wise_asset_pms.due_in_5 = v1.Where(x => x.due_date.Value.Month == 5 && x.due_date.Value.Year == DateTime.UtcNow.Year).Count();
                    class_code_due_month_wise_asset_pms.due_in_6 = v1.Where(x => x.due_date.Value.Month == 6 && x.due_date.Value.Year == DateTime.UtcNow.Year).Count();
                    class_code_due_month_wise_asset_pms.due_in_7 = v1.Where(x => x.due_date.Value.Month == 7 && x.due_date.Value.Year == DateTime.UtcNow.Year).Count();
                    class_code_due_month_wise_asset_pms.due_in_8 = v1.Where(x => x.due_date.Value.Month == 8 && x.due_date.Value.Year == DateTime.UtcNow.Year).Count();
                    class_code_due_month_wise_asset_pms.due_in_9 = v1.Where(x => x.due_date.Value.Month == 9 && x.due_date.Value.Year == DateTime.UtcNow.Year).Count();
                    class_code_due_month_wise_asset_pms.due_in_10 = v1.Where(x => x.due_date.Value.Month == 10 && x.due_date.Value.Year == DateTime.UtcNow.Year).Count();
                    class_code_due_month_wise_asset_pms.due_in_11 = v1.Where(x => x.due_date.Value.Month == 11 && x.due_date.Value.Year == DateTime.UtcNow.Year).Count();
                    class_code_due_month_wise_asset_pms.due_in_12 = v1.Where(x => x.due_date.Value.Month == 12 && x.due_date.Value.Year == DateTime.UtcNow.Year).Count();

                    response.graph1.Add(class_code_due_month_wise_asset_pms);
                }

                // Upcoming Electrical PM Opportunities -- Graph 2
                var graph2_list = get_all_asset_pms.list.Where(x => x.status != (int)Status.Completed && x.title.ToLower().Replace(" ", "") == "electricaltesting").ToList();
                var class_wise_pms_for_graph2 = graph2_list.GroupBy(x => x.asset_class_code);

                foreach (var v1 in class_wise_pms_for_graph2)
                {
                    class_code_due_month_wise_asset_pms class_code_due_month_wise_asset_pms = new class_code_due_month_wise_asset_pms();
                    class_code_due_month_wise_asset_pms.asset_class_code = v1.Select(x => x.asset_class_code).FirstOrDefault();

                    class_code_due_month_wise_asset_pms.due_in_1 = v1.Where(x => x.due_date.Value.Date > DateTime.UtcNow.Date && x.due_date.Value.Date < DateTime.UtcNow.AddMonths(3).Date).Count();
                    class_code_due_month_wise_asset_pms.due_in_2 = v1.Where(x => x.due_date.Value.Date > DateTime.UtcNow.AddMonths(3).Date && x.due_date.Value.Date < DateTime.UtcNow.AddMonths(6).Date).Count(); 
                    class_code_due_month_wise_asset_pms.due_in_3 = v1.Where(x => x.due_date.Value.Date > DateTime.UtcNow.AddMonths(6).Date && x.due_date.Value.Date < DateTime.UtcNow.AddMonths(9).Date).Count();
                    class_code_due_month_wise_asset_pms.due_in_4 = v1.Where(x => x.due_date.Value.Date > DateTime.UtcNow.AddMonths(9).Date && x.due_date.Value.Date < DateTime.UtcNow.AddMonths(12).Date).Count();
                    class_code_due_month_wise_asset_pms.due_in_5 = v1.Where(x => x.due_date.Value.Date > DateTime.UtcNow.AddMonths(12).Date && x.due_date.Value.Date < DateTime.UtcNow.AddMonths(15).Date).Count();
                    class_code_due_month_wise_asset_pms.due_in_6 = v1.Where(x => x.due_date.Value.Date > DateTime.UtcNow.AddMonths(15).Date && x.due_date.Value.Date < DateTime.UtcNow.AddMonths(18).Date).Count();
                    class_code_due_month_wise_asset_pms.due_in_7 = v1.Where(x => x.due_date.Value.Date > DateTime.UtcNow.AddMonths(18).Date && x.due_date.Value.Date < DateTime.UtcNow.AddMonths(21).Date).Count();
                    class_code_due_month_wise_asset_pms.due_in_8 = v1.Where(x => x.due_date.Value.Date > DateTime.UtcNow.AddMonths(21).Date && x.due_date.Value.Date < DateTime.UtcNow.AddMonths(24).Date).Count();
                    class_code_due_month_wise_asset_pms.due_in_9 = v1.Where(x => x.due_date.Value.Date > DateTime.UtcNow.AddMonths(24).Date && x.due_date.Value.Date < DateTime.UtcNow.AddMonths(27).Date).Count();
                    class_code_due_month_wise_asset_pms.due_in_10 = v1.Where(x => x.due_date.Value.Date > DateTime.UtcNow.AddMonths(27).Date && x.due_date.Value.Date < DateTime.UtcNow.AddMonths(30).Date).Count();
                    class_code_due_month_wise_asset_pms.due_in_11 = v1.Where(x => x.due_date.Value.Date > DateTime.UtcNow.AddMonths(30).Date && x.due_date.Value.Date < DateTime.UtcNow.AddMonths(33).Date).Count();
                    class_code_due_month_wise_asset_pms.due_in_12 = v1.Where(x => x.due_date.Value.Date > DateTime.UtcNow.AddMonths(33).Date && x.due_date.Value.Date < DateTime.UtcNow.AddMonths(36).Date).Count();

                    response.graph2.Add(class_code_due_month_wise_asset_pms);
                }

                // Upcoming Infrared Thermography Opportunities -- Graph 3
                var graph3_list = get_all_asset_pms.list.Where(x => x.status != (int)Status.Completed && x.title.ToLower().Replace(" ", "") == "infraredthermography").ToList();
                var class_wise_pms_for_graph3 = graph3_list.GroupBy(x => x.asset_class_code);

                foreach (var v1 in class_wise_pms_for_graph3)
                {
                    class_code_due_month_wise_asset_pms class_code_due_month_wise_asset_pms = new class_code_due_month_wise_asset_pms();
                    class_code_due_month_wise_asset_pms.asset_class_code = v1.Select(x => x.asset_class_code).FirstOrDefault();

                    class_code_due_month_wise_asset_pms.due_in_1 = v1.Where(x => x.due_date.Value.Month == 1 && x.due_date.Value.Year == DateTime.UtcNow.Year).Count();
                    class_code_due_month_wise_asset_pms.due_in_2 = v1.Where(x => x.due_date.Value.Month == 2 && x.due_date.Value.Year == DateTime.UtcNow.Year).Count();
                    class_code_due_month_wise_asset_pms.due_in_3 = v1.Where(x => x.due_date.Value.Month == 3 && x.due_date.Value.Year == DateTime.UtcNow.Year).Count();
                    class_code_due_month_wise_asset_pms.due_in_4 = v1.Where(x => x.due_date.Value.Month == 4 && x.due_date.Value.Year == DateTime.UtcNow.Year).Count();
                    class_code_due_month_wise_asset_pms.due_in_5 = v1.Where(x => x.due_date.Value.Month == 5 && x.due_date.Value.Year == DateTime.UtcNow.Year).Count();
                    class_code_due_month_wise_asset_pms.due_in_6 = v1.Where(x => x.due_date.Value.Month == 6 && x.due_date.Value.Year == DateTime.UtcNow.Year).Count();
                    class_code_due_month_wise_asset_pms.due_in_7 = v1.Where(x => x.due_date.Value.Month == 7 && x.due_date.Value.Year == DateTime.UtcNow.Year).Count();
                    class_code_due_month_wise_asset_pms.due_in_8 = v1.Where(x => x.due_date.Value.Month == 8 && x.due_date.Value.Year == DateTime.UtcNow.Year).Count();
                    class_code_due_month_wise_asset_pms.due_in_9 = v1.Where(x => x.due_date.Value.Month == 9 && x.due_date.Value.Year == DateTime.UtcNow.Year).Count();
                    class_code_due_month_wise_asset_pms.due_in_10 = v1.Where(x => x.due_date.Value.Month == 10 && x.due_date.Value.Year == DateTime.UtcNow.Year).Count();
                    class_code_due_month_wise_asset_pms.due_in_11 = v1.Where(x => x.due_date.Value.Month == 11 && x.due_date.Value.Year == DateTime.UtcNow.Year).Count();
                    class_code_due_month_wise_asset_pms.due_in_12 = v1.Where(x => x.due_date.Value.Month == 12 && x.due_date.Value.Year == DateTime.UtcNow.Year).Count();

                    response.graph3.Add(class_code_due_month_wise_asset_pms);
                }

                response.active_assets_count = active_assets_count;
                response.open_asset_issues_count = open_issues_count;
                response.open_asset_pms_count = open_pms_count;
                response.overdue_asset_pms_count = get_all_asset_pms.list.Where(x => x.is_overdue).Count();
                response.scheduled_asset_pms_count = get_all_asset_pms.list.Where(x => x.due_date != null 
                        && x.due_date.Value.Date > DateTime.UtcNow.Date ).Count();

            }
            catch (Exception ex)
            {
            }

            return response;
        }

        public GetFeatureWiseURLsResponseModel GetFeatureWiseURLs(int feature_type)
        {
            GetFeatureWiseURLsResponseModel response = new GetFeatureWiseURLsResponseModel();
            response.status = (int)ResponseStatusNumber.Error;
            try
            {
                string feature_url = null;
                if (feature_type == (int)FeatureTypes.EstimatorFeature)
                {
                    feature_url = ConfigurationManager.AppSettings["estimator_feature_url"];
                }
                if(!String.IsNullOrEmpty(feature_url))
                {
                    response.status = (int)ResponseStatusNumber.Success;
                    response.url = feature_url;
                }
            }
            catch(Exception ex)
            {
            }
            return response;
        }

    }
}

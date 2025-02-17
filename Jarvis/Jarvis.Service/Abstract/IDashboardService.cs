using Jarvis.ViewModels.RequestResponseViewModel;
using Jarvis.ViewModels.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Jarvis.Service.Abstract
{
    public interface IDashboardService
    {
        GetAdminDashboardCountResponseModel GetAdminDashboardCount();
        ListViewModel<GetInProgressWorkordersListResponseModel> GetAllWorkordersListByStatus(GetTopWorkordersListByStatusRequestModel request);
        ListViewModel<GetInProgressWorkordersListResponseModel> GetAllLineItemsByStatus(GetTopWorkordersListByStatusRequestModel request);
        ListViewModel<GetTechniciansListWithSubmittedLinesCountResponseModel> GetTechniciansListWithSubmittedLinesCount(int pagesize, int pageindex);
        UpcomingSiteOpportunitiesDashboardResponseModel UpcomingSiteOpportunitiesDashboard(UpcomingSiteOpportunitiesDashboardRequestModel requestmodel);
        GetFeatureWiseURLsResponseModel GetFeatureWiseURLs(int feature_type);
    }
}

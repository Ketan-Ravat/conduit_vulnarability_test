using Jarvis.db.Models;
using Jarvis.ViewModels.RequestResponseViewModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.Repo.Abstract
{
    public interface IDashboardRepository
    {
        int GetActiveSiteCount();
        int GetActiveWorkordersCount();
        int GetActiveTechniciansCount();
        int GetWOStatusWiseCount(int status);
        int GetWODueOverdueFlagWiseCount(int wo_due_overdue_flag);
        (List<WorkOrders>, int) GetAllWorkordersListByStatus(GetTopWorkordersListByStatusRequestModel request);
        (List<WOOnboardingAssets>,int) GetAllWOOBAssetsByStatus(GetTopWorkordersListByStatusRequestModel request);
        (List<AssetFormIO>,int) GetAllAssetFormIOByStatus(GetTopWorkordersListByStatusRequestModel request);
        List<string> GetTechniciansWhoSubmittedWOLinesOfLast30Days();
        List<string> GetTechniciansWhoSubmittedAssetFormIOOfLast30Days();
        int GetActiveAssetsCountBySites(UpcomingSiteOpportunitiesDashboardRequestModel request);
        int GetOpenAssetIssuesCountBySites(UpcomingSiteOpportunitiesDashboardRequestModel request);
        int GetOpenAssetPMsCountBySites(UpcomingSiteOpportunitiesDashboardRequestModel request);
        int GetNetaInspectionFormLineItemsCount();
        int GetRegularInspectionFormLineItemsCount();
    } 
}

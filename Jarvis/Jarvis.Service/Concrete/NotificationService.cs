using AutoMapper;
using Jarvis.db.Models;
using Jarvis.Repo;
using Jarvis.Service.Abstract;
using Jarvis.Service.Notification;
using Jarvis.Shared;
using Jarvis.Shared.StatusEnums;
using Jarvis.ViewModels;
using Jarvis.ViewModels.RequestResponseViewModel;
using Newtonsoft.Json;
using Org.BouncyCastle.Ocsp;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using ZXing.Common;

namespace Jarvis.Service.Concrete {
    public class NotificationService : BaseService {
        public readonly IMapper _mapper;

        public NotificationService(IMapper mapper) : base(mapper)
        {
            _mapper = mapper;
        }


        string url = "https://fcm.googleapis.com/fcm/send";
        //string customerAndroidAppKey = ConfigurationManager.AppSettings["customerAndroidAppKey"];
        //string customerAndroidAppKey = @"AAAAL2QoacM:APA91bGs_PVik_e0Bw9QnXrAKOloWuMy3AI0tyPqPyqY-9KflvvjJ5qH5kOGH_Mc5ODajRUoOc4MYOaLfZtPphBe3ZgQ3h2-qLuFPX3emItlEDTyfXPAIlHJxO6XnGQcpY9n2sZY0vra";
        string customerAndroidAppKey = @"AAAA8y37LzE:APA91bEmJZZRxnJNaM5BwiiSpmYC7kYhWgksBbDb0DfEjC5d_zNDUP5hekg3fOleiJ9kn8-addR4bfshXFisJXNaqF_hdoypaFupMznMFjyiczD3B86YFYDCG8Q19vPMZIQhbid5PZ5n";
        // "AAAAL2QoacM:APA91bGs_PVik_e0Bw9QnXrAKOloWuMy3AI0tyPqPyqY-9KflvvjJ5qH5kOGH_Mc5ODajRUoOc4MYOaLfZtPphBe3ZgQ3h2-qLuFPX3emItlEDTyfXPAIlHJxO6XnGQcpY9n2sZY0vra";
        string customerIosdAppKey = ConfigurationManager.AppSettings["customerIosdAppKey"];
        //"AAAAL2QoacM:APA91bGs_PVik_e0Bw9QnXrAKOloWuMy3AI0tyPqPyqY-9KflvvjJ5qH5kOGH_Mc5ODajRUoOc4MYOaLfZtPphBe3ZgQ3h2-qLuFPX3emItlEDTyfXPAIlHJxO6XnGQcpY9n2sZY0vra";
        string partnerAppKey = ConfigurationManager.AppSettings["partnerAppKey"];
        string driverAppKey = ConfigurationManager.AppSettings["driverAppKey"];
        string partnerIosAppKey = ConfigurationManager.AppSettings["partnerIosAppKey"];
        string driverIosAppKey = ConfigurationManager.AppSettings["driverIosAppKey"];
        string partnerAdminWebAppKey = ConfigurationManager.AppSettings["partnerAdminWebAppKey"];


        public async Task<bool> SendNotification(int NotificationStatusID, string ref_id, string userid)
        {
            bool response = false;
            
            /*
            //if (NotificationStatusID == (int)NotificationStatus.AutoApproveInspection || NotificationStatusID == (int)NotificationStatus.PendingNewInspection)
            //{
            //    var managerlist = _UoW.UserRepository.GetAllManager(ref_id);
            //    if (managerlist.Count > 0)
            //    {
            //        var userlist = _mapper.Map<List<UserSitesNotificationModel>>(managerlist);

            //        var inspectionDetail = _UoW.InspectionRepository.GetInspectionById(ref_id, userid);

            //        var operatorname = _UoW.UserRepository.GetUserFromUserId(userid);
            //        Notification.Notification notificationmessage = new Notification.Notification();
            //        if (NotificationStatusID == (int)NotificationStatus.AutoApproveInspection)
            //        {
            //            notificationmessage = NotificationGenerator.AutoApproedInspection(inspectionDetail.Asset.name, operatorname);
            //        }
            //        else if (NotificationStatusID == (int)NotificationStatus.PendingNewInspection)
            //        {
            //            notificationmessage = NotificationGenerator.PendingNewInspection(inspectionDetail.Asset.name, operatorname);
            //        }
            //        List<string> ListOfTokenAndroid = new List<string>();
            //        List<string> ListOfTokenIOS = new List<string>();

            //        userlist.ForEach(x =>
            //        {
            //            ListOfTokenAndroid.Add(x.User.notification_token);
            //        });

            //        RootRequestObj androidReq = new RootRequestObj()
            //        {
            //            registration_ids = ListOfTokenAndroid,
            //            priority = "high",
            //            data = new RequestData()
            //            {
            //                title = notificationmessage.heading,
            //                body = notificationmessage.message,
            //                type = NotificationStatusID,
            //                ref_id = ref_id,
            //                custom = inspectionDetail.inspection_id,
            //            },
            //            notification = new Notifications()
            //            {
            //                title = notificationmessage.heading,
            //                body = notificationmessage.message
            //            }
            //        };

            //        var responseandroid = await sendAndroid(androidReq);
            //        if (responseandroid > 0)
            //        {
            //            await AddNotificationData(userlist, NotificationStatusID, notificationmessage, ref_id);
            //            _UoW.SaveChanges();
            //        }


            //        if (NotificationStatusID == (int)NotificationStatus.AutoApproveInspection)
            //        {
            //            var operatordetails = _UoW.UserRepository.GetUserSiteById(userid);
            //            userlist.Clear();
            //            userlist = _mapper.Map<List<UserSitesNotificationModel>>(operatordetails);
            //            //var token = userlists.User.notification_token;
            //            ListOfTokenAndroid.Clear();
            //            userlist.ForEach(x =>
            //            {
            //                ListOfTokenAndroid.Add(x.User.notification_token);
            //            });

            //            notificationmessage = NotificationGenerator.AutoApproedInspectionOperator(inspectionDetail.Asset.name);

            //            RootRequestObj operatorReq = new RootRequestObj()
            //            {
            //                registration_ids = ListOfTokenAndroid,
            //                priority = "high",
            //                data = new RequestData()
            //                {
            //                    title = notificationmessage.heading,
            //                    body = notificationmessage.message,
            //                    type = NotificationStatusID,
            //                    ref_id = ref_id,
            //                    custom = inspectionDetail.inspection_id,
            //                },
            //                notification = new Notifications()
            //                {
            //                    title = notificationmessage.heading,
            //                    body = notificationmessage.message
            //                }
            //            };
            //            var responseandroidop = await sendAndroid(operatorReq);

            //            if (responseandroidop > 0)
            //            {
            //                await AddNotificationData(userlist, NotificationStatusID, notificationmessage, ref_id);
            //                _UoW.SaveChanges();
            //            }
            //        }


            //        if (responseandroid > 0)
            //        {
            //            response = true;
            //            _UoW.CommitTransaction();
            //        }
            //        else
            //        {
            //            response = false;
            //        }
            //    }
            //}

            //else if (NotificationStatusID == (int)NotificationStatus.ManagerApproveInspection)
            //{
            //    var operatorlist = _UoW.UserRepository.GetAllOperator(ref_id);

            //    if (operatorlist.Count > 0)
            //    {
            //        var userlist = _mapper.Map<List<UserSitesNotificationModel>>(operatorlist);

            //        var inspectionDetail = _UoW.InspectionRepository.GetInspectionById(ref_id, userid);

            //        var managername = _UoW.UserRepository.GetUserFromUserId(userid);

            //        Notification.Notification notificationmessage = new Notification.Notification();
            //        var notificationmessage = NotificationGenerator.ManagerApproveInspection(inspectionDetail.Asset.name, managername);

            //        List<string> ListOfToken = new List<string>();

            //        userlist.ForEach(x =>
            //        {
            //            ListOfToken.Add(x.User.notification_token);
            //        });

            //        RootRequestObj androidReq = new RootRequestObj()
            //        {
            //            registration_ids = ListOfToken,
            //            priority = "high",
            //            data = new RequestData()
            //            {
            //                title = notificationmessage.heading,
            //                body = notificationmessage.message,
            //                type = NotificationStatusID,
            //                ref_id = ref_id,
            //                custom = inspectionDetail.inspection_id,
            //            },
            //            notification = new Notifications()
            //            {
            //                title = notificationmessage.heading,
            //                body = notificationmessage.message
            //            }
            //        };
            //        var responseandroid = await sendAndroid(androidReq);
            //        if (responseandroid > 0)
            //        {
            //            await AddNotificationData(userlist, NotificationStatusID, notificationmessage, ref_id);
            //            _UoW.SaveChanges();
            //            _UoW.CommitTransaction();
            //            response = true;
            //        }
            //        else
            //        {
            //            response = false;
            //        }
            //    }
            //    else
            //    {
            //        response = false;
            //    }
            //}

            //else if(NotificationStatusID == (int)NotificationStatus.NewWorkOrderForInspection || NotificationStatusID == (int)NotificationStatus.NewWorkOrderWithApprovedAsset)
            //{
            //    var operatorandmaintencestafflist = _UoW.UserRepository.GetAllOperatorAndMaintenceStaff(ref_id);

            //    if (operatorandmaintencestafflist.Count > 0)
            //    {
            //        var userlist = _mapper.Map<List<UserSitesNotificationModel>>(operatorandmaintencestafflist);

            //        var inspectionDetail = _UoW.InspectionRepository.GetInspectionById(ref_id, userid);

            //        var managername = _UoW.UserRepository.GetUserFromUserId(userid);

            //        Notification.Notification notificationmessage = new Notification.Notification();
            //        if (NotificationStatusID == (int)NotificationStatus.NewWorkOrderForInspection)
            //        {
            //            notificationmessage = NotificationGenerator.NewWorkOrderForInspection(inspectionDetail.Asset.name, managername);
            //        }
            //        else if (NotificationStatusID == (int)NotificationStatus.NewWorkOrderWithApprovedAsset) 
            //        {
            //            notificationmessage = NotificationGenerator.NewWorkOrderWithAutoApprovedAsset(inspectionDetail.Asset.name, managername);
            //        }
            //            List<string> ListOfToken = new List<string>();

            //        userlist.ForEach(x =>
            //        {
            //            ListOfToken.Add(x.User.notification_token);
            //        });

            //        RootRequestObj androidReq = new RootRequestObj()
            //        {
            //            registration_ids = ListOfToken,
            //            priority = "high",
            //            data = new RequestData()
            //            {
            //                title = notificationmessage.heading,
            //                body = notificationmessage.message,
            //                type = NotificationStatusID,
            //                ref_id = ref_id,
            //                custom = inspectionDetail.inspection_id,
            //            },
            //            notification = new Notifications()
            //            {
            //                title = notificationmessage.heading,
            //                body = notificationmessage.message
            //            }
            //        };

            //        var responseandroid = await sendAndroid(androidReq);
            //        if (responseandroid > 0)
            //        {
            //            await AddNotificationData(userlist, NotificationStatusID, notificationmessage, ref_id);
            //            _UoW.SaveChanges();
            //            _UoW.CommitTransaction();
            //            response = true;
            //        }
            //        else
            //        {
            //            response = false;
            //        }
            //    }
            //    else
            //    {
            //        response = false;
            //    }
            //}
            */

            if (NotificationStatusID == (int)NotificationStatus.AutoApproveInspection || NotificationStatusID == (int)NotificationStatus.PendingNewInspection)
            {
                var managerlist = _UoW.UserRepository.GetAllManager(ref_id);
                if (managerlist.Count > 0)
                {
                    var userlist = _mapper.Map<List<UserSitesNotificationModel>>(managerlist);

                    var inspectionDetail = _UoW.InspectionRepository.GetInspectionById(ref_id, userid);

                    var operatorname = _UoW.UserRepository.GetUserFromUserId(userid);
                    Notification.Notification notificationmessage = new Notification.Notification();
                    if (NotificationStatusID == (int)NotificationStatus.AutoApproveInspection)
                    {
                        notificationmessage = NotificationGenerator.AutoApproedInspection(inspectionDetail.Asset.name, operatorname);
                    }
                    else if (NotificationStatusID == (int)NotificationStatus.PendingNewInspection)
                    {
                        notificationmessage = NotificationGenerator.PendingNewInspection(inspectionDetail.Asset.name, operatorname);
                    }
                    List<string> ListOfTokenAndroid = new List<string>();
                    List<string> ListOfTokenIOS = new List<string>();

                    userlist.ForEach(x =>
                    {
                        ListOfTokenAndroid.Add(x.User.notification_token);
                    });

                    var androidReq = GetRootRequestObj(ListOfTokenAndroid, notificationmessage, NotificationStatusID, ref_id, Roles.Manager);

                    var responseandroid = await sendAndroid(androidReq);
                    if (responseandroid > 0)
                    {
                        await AddNotificationData(userlist, NotificationStatusID, notificationmessage, ref_id, Roles.Manager);
                        //_UoW.SaveChanges();
                    }


                    if (NotificationStatusID == (int)NotificationStatus.AutoApproveInspection)
                    {
                        var operatordetails = _UoW.UserRepository.GetUserSiteById(userid);
                        userlist.Clear();
                        userlist = _mapper.Map<List<UserSitesNotificationModel>>(operatordetails);
                        //var token = userlists.User.notification_token;
                        ListOfTokenAndroid.Clear();
                        userlist.ForEach(x =>
                        {
                            ListOfTokenAndroid.Add(x.User.notification_token);
                        });

                        notificationmessage = NotificationGenerator.AutoApproedInspectionOperator(inspectionDetail.Asset.name);

                        var operatorReq = GetRootRequestObj(ListOfTokenAndroid, notificationmessage, NotificationStatusID, ref_id, Roles.Operator);

                        var responseandroidop = await sendAndroid(operatorReq);

                        if (responseandroidop > 0)
                        {
                            await AddNotificationData(userlist, NotificationStatusID, notificationmessage, ref_id, Roles.Operator);
                            //_UoW.SaveChanges();
                        }
                    }


                    if (responseandroid > 0)
                    {
                        response = true;
                        _UoW.CommitTransaction();
                    }
                    else
                    {
                        Logger.Log("Error in SendNotification ", responseandroid.ToString());
                        response = false;
                    }
                }
            }

            else if (NotificationStatusID == (int)NotificationStatus.ManagerApproveInspection)
            {
                Notification.Notification notificationmessage = new Notification.Notification();

                var operatorlist = _UoW.UserRepository.GetOperatorForNotification(ref_id);

                var managername = _UoW.UserRepository.GetUserFromUserId(userid);

                //List<WorkOrder> workorderlist = _UoW.WorkOrderRepository.GetWorkOrdersByInspectionId(ref_id);

                if (operatorlist.Count > 0)
                {
                    var userlist = _mapper.Map<List<UserSitesNotificationModel>>(operatorlist);

                    var inspectionDetail = _UoW.InspectionRepository.GetInspectionById(ref_id, userid);

                    //Notification.Notification notificationmessage = new Notification.Notification();
                    notificationmessage = NotificationGenerator.ManagerApproveInspection(inspectionDetail.Asset.name, managername);

                    List<string> ListOfToken = new List<string>();

                    userlist.ForEach(x =>
                    {
                        ListOfToken.Add(x.User.notification_token);
                    });

                    var androidReq = GetRootRequestObj(ListOfToken, notificationmessage, NotificationStatusID, ref_id, Roles.Operator);

                    var responseandroid = await sendAndroid(androidReq);
                    if (responseandroid > 0)
                    {
                        await AddNotificationData(userlist, NotificationStatusID, notificationmessage, ref_id, Roles.Operator);
                        //_UoW.SaveChanges();
                        //_UoW.CommitTransaction();
                        response = true;
                    }
                    else
                    {
                        Logger.Log("Error in SendNotification ", responseandroid.ToString());
                        response = false;
                    }
                }

                //if (workorderlist.Count > 0)
                //{
                //    Guid workorder_id = workorderlist.Select(x => x.work_order_uuid).FirstOrDefault();

                //    var ms = _UoW.UserRepository.GetAllMaintenanceStaffByWorkOrderID(workorder_id.ToString());

                //    var msuserlist = _mapper.Map<List<UserSitesNotificationModel>>(ms);
                //    List<string> ListOfToken = new List<string>();

                //    msuserlist.ForEach(x =>
                //    {
                //        ListOfToken.Add(x.User.notification_token);
                //    });

                //    workorderlist.ForEach(x =>
                //    {
                //        notificationmessage = NotificationGenerator.WorkOrderCreated(x.Asset.name, managername,x.Attributes.name);
                //        var androidReq = GetRootRequestObj(ListOfToken, notificationmessage, NotificationStatusID, ref_id);
                //        var responseandroid = sendAndroid(androidReq);
                //        if (responseandroid != null && responseandroid.Result > 0)
                //        {
                //            AddNotificationData(msuserlist, NotificationStatusID, notificationmessage, ref_id);
                //            _UoW.SaveChanges();
                //            _UoW.CommitTransaction();
                //            response = true;
                //        }
                //        else
                //        {
                //            Logger.Log("Error in SendNotification ", responseandroid.ToString());
                //            response = false;
                //        }
                //    });
                //}
            }

            else if (NotificationStatusID == (int)NotificationStatus.NewWorkOrderForInspection || NotificationStatusID == (int)NotificationStatus.NewWorkOrderWithApprovedAsset)
            {
                //var operatorandmaintencestafflist = _UoW.UserRepository.GetAllOperatorAndMaintenceStaff(ref_id);
                Notification.Notification notificationmessage = new Notification.Notification();

                var operatorlist = _UoW.UserRepository.GetOperatorForNotification(ref_id);

                if (operatorlist.Count > 0)
                {
                    var userlist = _mapper.Map<List<UserSitesNotificationModel>>(operatorlist);

                    var inspectionDetail = _UoW.InspectionRepository.GetInspectionById(ref_id, userid);

                    var managername = _UoW.UserRepository.GetUserFromUserId(userid);

                    if (NotificationStatusID == (int)NotificationStatus.NewWorkOrderForInspection)
                    {
                        //notificationmessage = NotificationGenerator.NewWorkOrderForInspection(inspectionDetail.Asset.name, managername);
                        notificationmessage = NotificationGenerator.RejecteInspection(inspectionDetail.Asset.name, managername);
                    }
                    else if (NotificationStatusID == (int)NotificationStatus.NewWorkOrderWithApprovedAsset)
                    {
                        notificationmessage = NotificationGenerator.NewWorkOrderWithAutoApprovedAsset(inspectionDetail.Asset.name, managername);
                    }

                    List<string> ListOfToken = new List<string>();

                    userlist.ForEach(x =>
                    {
                        ListOfToken.Add(x.User.notification_token);
                    });

                    var androidReq = GetRootRequestObj(ListOfToken, notificationmessage, NotificationStatusID, ref_id, Roles.Operator);

                    var responseandroid = await sendAndroid(androidReq);

                    if (responseandroid > 0)
                    {
                        await AddNotificationData(userlist, NotificationStatusID, notificationmessage, ref_id, Roles.Operator);
                        //_UoW.SaveChanges();
                        //_UoW.CommitTransaction();
                        response = true;
                    }
                    else
                    {
                        Logger.Log("Error in SendNotification ", responseandroid.ToString());
                        response = false;
                    }

                    var workorder = _UoW.IssueRepository.GetIssueByInspectionId(ref_id);
                    if (workorder != null && workorder.issue_uuid != null && workorder.issue_uuid != Guid.Empty)
                    {
                        notificationmessage = NotificationGenerator.NewWorkOrderForInspection(inspectionDetail.Asset.name, managername);
                        var maintencestafflist = _UoW.UserRepository.GetAllMaintenanceStaffByIssueID(workorder.issue_uuid.ToString());
                        userlist.Clear();
                        ListOfToken.Clear();
                        if (maintencestafflist.Count > 0)
                        {
                            userlist = _mapper.Map<List<UserSitesNotificationModel>>(maintencestafflist);
                            userlist.ForEach(x =>
                            {
                                ListOfToken.Add(x.User.notification_token);
                            });

                            var androidReqMS = GetRootRequestObj(ListOfToken, notificationmessage, NotificationStatusID, workorder.issue_uuid.ToString(), Roles.MS);


                            var responseandroidMs = await sendAndroid(androidReqMS);
                            if (responseandroidMs > 0)
                            {
                                await AddNotificationData(userlist, NotificationStatusID, notificationmessage, workorder.issue_uuid.ToString(), Roles.MS);
                                //_UoW.SaveChanges();
                                //_UoW.CommitTransaction();
                                response = true;
                            }
                            else
                            {
                                Logger.Log("Error in SendNotification ", responseandroid.ToString());
                                response = false;
                            }
                        }
                        else
                        {
                            Logger.Log("Error in SendNotification Not Found maintencestafflist");
                            response = false;
                        }
                    }
                    else
                    {
                        Logger.Log("Error in SendNotification Not Found WorkOrder");
                        response = false;
                    }
                }
                else
                {
                    Logger.Log("Error in SendNotification Not Found operatorlist ");
                    response = false;
                }
            }

            else if (NotificationStatusID == (int)NotificationStatus.UpdateWorkOrderStatus)
            {
                var managerlist = _UoW.UserRepository.GetAllManagerByIssueID(ref_id);
                var workorderdetails = _UoW.IssueRepository.GetIssueById(ref_id, userid);
                var userdetails = await _UoW.UserRepository.GetUserByID(userid);
                List<UserSitesNotificationModel> userlist = new List<UserSitesNotificationModel>();
                Notification.Notification notificationmessage = new Notification.Notification();

                if (NotificationStatusID == (int)NotificationStatus.UpdateWorkOrderStatus)
                {
                    notificationmessage = NotificationGenerator.UpdateWorkOrderStatus(workorderdetails.Inspection.Asset.name, userdetails.username, workorderdetails.StatusMaster.status_name, workorderdetails.Attributes.name);
                }

                userlist.Clear();
                userlist = _mapper.Map<List<UserSitesNotificationModel>>(managerlist);
                if (userlist.Count > 0)
                {
                    List<string> ListOfToken = new List<string>();
                    userlist.ForEach(x =>
                    {
                        ListOfToken.Add(x.User.notification_token);
                    });

                    var androidReq = GetRootRequestObj(ListOfToken, notificationmessage, NotificationStatusID, ref_id, Roles.Manager);

                    var responseandroid = await sendAndroid(androidReq);
                    if (responseandroid > 0)
                    {
                        await AddNotificationData(userlist, NotificationStatusID, notificationmessage, ref_id, Roles.Manager);
                        //_UoW.SaveChanges();
                        //_UoW.CommitTransaction();
                        response = true;
                    }
                    else
                    {
                        Logger.Log("Error in SendNotification ", responseandroid.ToString());
                        response = false;
                    }
                }
                else
                {
                    Logger.Log("Error in SendNotification Not Found Manager ");
                }
            }

            else if (NotificationStatusID == (int)NotificationStatus.UpdateWorkOrderPriority)
            {
                List<UserSites> maintenancestafflist = new List<UserSites>();
                var workorderdetails = _UoW.IssueRepository.GetIssueById(ref_id, userid);
                if (workorderdetails.status != (int)Status.New)
                {
                    maintenancestafflist = _UoW.UserRepository.GetMaintenanceStaffByIssueID(ref_id);
                }
                else
                {
                    maintenancestafflist = _UoW.UserRepository.GetAllMaintenanceStaffByIssueID(ref_id);
                }

                var userdetails = await _UoW.UserRepository.GetUserByID(userid);

                var userlist = _mapper.Map<List<UserSitesNotificationModel>>(maintenancestafflist);
                if (userlist.Count > 0)
                {
                    string priority = string.Empty;
                    if (workorderdetails.priority == 1)
                    {
                        priority = "Very High";
                    }
                    else if (workorderdetails.priority == 2)
                    {
                        priority = "High";
                    }
                    else if (workorderdetails.priority == 3)
                    {
                        priority = "Medium";
                    }
                    else if (workorderdetails.priority == 4)
                    {
                        priority = "Low";
                    }

                    var notificationmessage = NotificationGenerator.UpdateWorkOrderPriority(workorderdetails.Inspection.Asset.name, userdetails.username, priority);

                    List<string> ListOfToken = new List<string>();
                    userlist.ForEach(x =>
                    {
                        ListOfToken.Add(x.User.notification_token);
                    });

                    var androidReq = GetRootRequestObj(ListOfToken, notificationmessage, NotificationStatusID, ref_id, Roles.MS);

                    var responseandroid = await sendAndroid(androidReq);
                    if (responseandroid > 0)
                    {
                        await AddNotificationData(userlist, NotificationStatusID, notificationmessage, ref_id, Roles.MS);
                        //_UoW.SaveChanges();
                        //_UoW.CommitTransaction();
                        response = true;
                    }
                    else
                    {
                        Logger.Log("Error in SendNotification ", responseandroid.ToString());
                        response = false;
                    }
                }
                else
                {
                    Logger.Log("Error in SendNotification Not Found maintenancestafflist");
                }
            }

            else if (NotificationStatusID == (int)NotificationStatus.FirstDueDateReminder || NotificationStatusID == (int)NotificationStatus.SecondDueDateReminder || NotificationStatusID == (int)NotificationStatus.OnDueDateReminder || NotificationStatusID == (int)NotificationStatus.FirstMeterHoursDueReminder || NotificationStatusID == (int)NotificationStatus.SecondMeterHoursDueReminder || NotificationStatusID == (int)NotificationStatus.OnMeterHoursDueReminder)
            {
                //var managerlist = _UoW.UserRepository.GetAllManagerByAssetPMID(ref_id);
                var assetPM = await _UoW.AssetPMsRepository.GetAssetPMById(Guid.Parse(ref_id));
                var Trigger = assetPM.PMTriggers.Where(x => x.status != (int)Status.TriggerCompleted && !x.is_archive).FirstOrDefault();
                List<UserSitesNotificationModel> userlist = new List<UserSitesNotificationModel>();
                Notification.Notification notificationmessage = new Notification.Notification();
                SentPMNotification pmNotification = new SentPMNotification();
                pmNotification.manager_id = Guid.Parse(userid);
                var timeIntext = String.Empty;
                var due_meter_hours = 0;
                if (NotificationStatusID == (int)NotificationStatus.FirstDueDateReminder)
                {
                    //timeIntext = Shared.Utility.DateTimeUtil.GetBeforetimeText(Trigger.due_datetime.Value);
                    timeIntext = Shared.Utility.DateTimeUtil.GetDueIn(Trigger.due_datetime.Value);
                    notificationmessage = NotificationGenerator.PMNotificationFirstReminder(assetPM.Asset.name, timeIntext, assetPM.title);
                }
                else if (NotificationStatusID == (int)NotificationStatus.SecondDueDateReminder)
                {
                    timeIntext = Shared.Utility.DateTimeUtil.GetDueIn(Trigger.due_datetime.Value);
                    notificationmessage = NotificationGenerator.PMNotificationSecondReminder(assetPM.Asset.name, timeIntext, assetPM.title);
                }
                else if (NotificationStatusID == (int)NotificationStatus.OnDueDateReminder)
                {
                    timeIntext = Shared.Utility.DateTimeUtil.GetDueIn(Trigger.due_datetime.Value);
                    notificationmessage = NotificationGenerator.PMNotificationDueDateReminder(assetPM.Asset.name, assetPM.title, timeIntext);
                }
                else if (NotificationStatusID == (int)NotificationStatus.FirstMeterHoursDueReminder)
                {
                    due_meter_hours = Trigger.due_meter_hours.Value;
                    notificationmessage = NotificationGenerator.PMNotificationFirstMeterHoursReminder(assetPM.Asset.name, assetPM.Asset.meter_hours.ToString(), assetPM.title, Trigger.due_meter_hours.Value.ToString());
                }
                else if (NotificationStatusID == (int)NotificationStatus.SecondMeterHoursDueReminder)
                {
                    due_meter_hours = Trigger.due_meter_hours.Value;
                    notificationmessage = NotificationGenerator.PMNotificationSecondMeterHoursReminder(assetPM.Asset.name, assetPM.Asset.meter_hours.ToString(), assetPM.title, Trigger.due_meter_hours.Value.ToString());
                }
                else if (NotificationStatusID == (int)NotificationStatus.OnMeterHoursDueReminder)
                {
                    due_meter_hours = Trigger.due_meter_hours.Value;
                    notificationmessage = NotificationGenerator.PMNotificationFinalMeterHoursReminder(assetPM.Asset.name, assetPM.Asset.meter_hours.ToString(), assetPM.title, Trigger.due_meter_hours.Value.ToString());
                }
                pmNotification.notification_type = NotificationStatusID;
                pmNotification.trigger_id = Trigger.pm_trigger_id;
                userlist.Clear();
                var user = _UoW.UserRepository.GetUserSiteById(userid);
                if (user?.Count > 0)
                {
                    userlist = _mapper.Map<List<UserSitesNotificationModel>>(user);
                }
                if (userlist.Count > 0)
                {
                    var sendNot = await _UoW.PMNotificationRepository.GetSentPMNotification(Guid.Parse(userid), Trigger.pm_trigger_id, NotificationStatusID);
                    if (sendNot != null)
                    {
                        return response;
                    }
                    var disabled = await _UoW.PMNotificationRepository.GetPMItemNotificationConfig(Guid.Parse(userid), Trigger.pm_trigger_id);
                    if (disabled != null)
                    {
                        if(disabled.is_disabled)
                        {
                            return response;
                        }
                    }
                    List<string> ListOfToken = new List<string>();
                    userlist.ForEach(x =>
                    {
                        ListOfToken.Add(x.User.notification_token);
                    });

                    var androidReq = GetRootRequestObj(ListOfToken, notificationmessage, NotificationStatusID, ref_id, Roles.Manager);

                    var responseandroid = await sendAndroid(androidReq);
                    if (responseandroid > 0)
                    {
                        await AddNotificationData(userlist, NotificationStatusID, notificationmessage, Trigger.pm_trigger_id.ToString(), Roles.Manager);

                        await _UoW.BaseGenericRepository<SentPMNotification>().Update(pmNotification);

                        var activityLogs = NotificationGenerator.SendPMNotificationLog(assetPM.Asset.name, assetPM.title, assetPM.Asset.meter_hours.ToString(), NotificationStatusID, timeIntext, due_meter_hours > 0 ? due_meter_hours.ToString(): "");
                        activityLogs.asset_id = assetPM.asset_id;
                        activityLogs.created_at = DateTime.UtcNow;
                        activityLogs.ref_id = Trigger.pm_trigger_id.ToString();
                        activityLogs.site_id = assetPM.Asset.site_id;
                        var res = await _UoW.BaseGenericRepository<AssetActivityLogs>().Update(activityLogs);
                        if (res == true)
                        {
                            _UoW.SaveChanges();
                        }
                        //_UoW.SaveChanges();
                        _UoW.CommitTransaction();
                        response = true;
                    }
                    else
                    {
                        Logger.Log("Error in SendNotification ", responseandroid.ToString());
                        response = false;
                    }
                }
                else
                {
                    Logger.Log("Error in SendNotification Not Found Manager ");
                }
            }
            return response;
        }


        //public async Task<int> sendAndroidSingle(RootRequestObjSingle request, string platform = PlatformTypes.Android)
        //{
        //    HttpClient client = new HttpClient();
        //    string reqJson = JsonConvert.SerializeObject(request);
        //    var buffer = System.Text.Encoding.UTF8.GetBytes(reqJson);
        //    var byteContent = new ByteArrayContent(buffer);
        //    byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
        //    string key = string.Empty;
        //    if (platform == Notification.PlatformTypes.Android)
        //    {
        //        key = customerAndroidAppKey;
        //    }
        //    else if (platform == Notification.PlatformTypes.iOS)
        //    {
        //        key = customerIosdAppKey;
        //    }

        //    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("key", "=" + key);
        //    //byteContent.Headers.Add("Authorization", customerAppKey);
        //    Logger.Log("send noti res " + key + " key " + reqJson);
        //    var res = await client.PostAsync(url, byteContent);
        //    if (res.StatusCode == System.Net.HttpStatusCode.OK)
        //    {
        //        var restring = await res.Content.ReadAsStringAsync();
        //        Logger.Log("Noti res " + restring);
        //        return (int)ResponseStatusNumber.Success;
        //    }
        //    else
        //    {
        //        Logger.Log("Noti res error");
        //        return (int)ResponseStatusNumber.Error;
        //    }
        //}


        public RootRequestObj GetRootRequestObj(List<string> ListOfTokenAndroid, Notification.Notification notificationmessage, int NotificationStatusID, string ref_id, string role_id)
        {
            RootRequestObj androidReq = new RootRequestObj()
            {
                registration_ids = ListOfTokenAndroid,
                priority = "high",
                data = new RequestData()
                {
                    title = notificationmessage.heading,
                    body = notificationmessage.message,
                    type = NotificationStatusID,
                    ref_id = ref_id,
                    custom = ref_id,
                    target_role = role_id

                },
                notification = new Notifications()
                {
                    title = notificationmessage.heading,
                    body = notificationmessage.message,
                    click_action = "https://app.dev.sensaii.com/dashboard",
                    target_role = role_id
                }
            };

            return androidReq;
        }


        public async Task<int> sendAndroid(RootRequestObj request, string platform = PlatformTypes.Android)
        {
            HttpClient client = new HttpClient();
            string reqJson = JsonConvert.SerializeObject(request);
            var buffer = System.Text.Encoding.UTF8.GetBytes(reqJson);
            var byteContent = new ByteArrayContent(buffer);
            byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            string key = string.Empty;
            if (platform == Notification.PlatformTypes.Android)
            {
                key = customerAndroidAppKey;
            }
            else if (platform == Notification.PlatformTypes.iOS)
            {
                key = customerIosdAppKey;
            }

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("key", "=" + key);
            //byteContent.Headers.Add("Authorization", customerAppKey);
            Logger.Log("send noti res " + key + " key " + reqJson);
            var res = await client.PostAsync(url, byteContent);
            if (res.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var restring = await res.Content.ReadAsStringAsync();
                Logger.Log("Noti res " + restring);
                return (int)ResponseStatusNumber.Success;
            }
            else
            {
                Logger.Log("Noti res error");
                return (int)ResponseStatusNumber.Error;
            }
        }

        public async Task<int> AddNotificationData(List<UserSitesNotificationModel> userlist, int NotificationStatusID, Notification.Notification notificationmessage, string ref_id, string role_id)
        {
            foreach (var userlists in userlist)
            {
                NotificationData notification = new NotificationData();
                notification.user_id = userlists.User.uuid;
                notification.device_key = userlists.User.notification_token;
                notification.ref_id = ref_id;
                notification.message = notificationmessage.message;
                notification.heading = notificationmessage.heading;
                notification.notification_type = NotificationStatusID;
                notification.status = 1;
                notification.sendDate = DateTime.UtcNow;
                notification.createdDate = DateTime.UtcNow;
                notification.OS = userlists.User.OS;
                notification.target_role = role_id;
                notification.notification_status = (int)Notification_Status.New;
                await _UoW.BaseGenericRepository<NotificationData>().Update(notification);
            }
            return 1;
        }

        public async Task<int> sendApple(RootAppleRequestObj request, string platform = PlatformTypes.Android)
        {
            HttpClient client = new HttpClient();
            string reqJson = JsonConvert.SerializeObject(request);
            var buffer = System.Text.Encoding.UTF8.GetBytes(reqJson);
            var byteContent = new ByteArrayContent(buffer);
            byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            string key = string.Empty;
            if (platform == Notification.PlatformTypes.Android)
            {
                key = customerAndroidAppKey;
            }
            else if (platform == Notification.PlatformTypes.iOS)
            {
                key = customerIosdAppKey;
            }

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("key", "=" + key);
            //byteContent.Headers.Add("Authorization", customerAppKey);
            Logger.Log("send noti res " + key + " key " + reqJson);
            var res = await client.PostAsync(url, byteContent);
            if (res.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var restring = await res.Content.ReadAsStringAsync();
                Logger.Log("Noti res " + restring);
                return (int)ResponseStatusNumber.Success;
            }
            else
            {
                Logger.Log("Noti res error");
                return (int)ResponseStatusNumber.Error;
            }
        }

        public async Task<int> sendWeb(RootRequestObj request, int appType, string platform = PlatformTypes.Web)
        {
            HttpClient client = new HttpClient();
            string reqJson = JsonConvert.SerializeObject(request);
            var buffer = System.Text.Encoding.UTF8.GetBytes(reqJson);
            var byteContent = new ByteArrayContent(buffer);
            byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            string key = string.Empty;
            if (platform == PlatformTypes.Web)
            {
                key = partnerAdminWebAppKey;
            }

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("key", "=" + key);
            //byteContent.Headers.Add("Authorization", customerAppKey);
            Logger.Log("send noti res " + key + " key " + reqJson);
            var res = await client.PostAsync(url, byteContent);
            if (res.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var restring = await res.Content.ReadAsStringAsync();
                Logger.Log("Noti res " + restring);
                return (int)ResponseStatusNumber.Success;
            }
            else
            {
                Logger.Log("Noti res error");
                return (int)ResponseStatusNumber.Error;
            }
        }

        //   ------- Conduit New Flow For Notifications -------------------
        //===================================================================================================================

        
        public async Task<bool> SendNotificationGenericNewFlow(int notification_type, List<string>? ref_id_list, List<string>? userid_list)
        {
            bool res = false;
            try
            {
                // Workorder is Assigned to Multiple Technicians
                if (notification_type == (int)NotificationType_Version_2.WorkOrderAssignedToTechnician)
                {
                    var ref_id = ref_id_list.FirstOrDefault();//wo_id
                    var get_wo = _UoW.WorkOrderRepository.GetWOByidforUpdate(Guid.Parse(ref_id));

                    AddNotificationDataForUsersRequestModel AddNotificationDataForUsersRequestModel = new AddNotificationDataForUsersRequestModel();
                    AddNotificationDataForUsersRequestModel.users_list = new List<UserDetailsWithRole>();

                    foreach (var techi in userid_list)
                    {
                        UserDetailsWithRole UserDetailsWithRole = new UserDetailsWithRole();
                        UserDetailsWithRole.user_id = Guid.Parse(techi);
                        UserDetailsWithRole.notification_user_role_type = ((int)NotificationUserRoleType.Technician_User).ToString();

                        AddNotificationDataForUsersRequestModel.users_list.Add(UserDetailsWithRole);
                    }
                    AddNotificationDataForUsersRequestModel.ref_id = ref_id;
                    AddNotificationDataForUsersRequestModel.notification_type = (int)NotificationType_Version_2.WorkOrderAssignedToTechnician;
                    AddNotificationDataForUsersRequestModel.heading = "New Work Order is assigned to you";
                    AddNotificationDataForUsersRequestModel.message = "Work Order " + get_wo.manual_wo_number + "(" + get_wo.Sites.site_name + ")" + " has been assigned to you.";

                    int insert_notification = await AddNotificationDataForUsers(AddNotificationDataForUsersRequestModel);
                    if(insert_notification == (int)ResponseStatusNumber.Success)
                    {
                        res = true;
                    }
                }

                // --- Assign Multiple Sites to One-User --- site_id is ref_id
                else if (notification_type == (int)NotificationType_Version_2.SiteAssignedToUser)
                {
                    var userid = userid_list.FirstOrDefault();
                    string this_user_role = null;
                    var get_roles = _UoW.UserRepository.GetUserRolesById(Guid.Parse(userid));

                    if (get_roles.Contains(GlobalConstants.Technician_Role_id) && get_roles.Contains(GlobalConstants.BackOffice_Role_Id))
                    {
                        this_user_role = "[ 1, 2,]";    // User has Technician and Back-Office both Roles
                    }
                    else if (get_roles.Contains(GlobalConstants.Technician_Role_id)) //  Technician
                    {
                        this_user_role = ((int)NotificationUserRoleType.Technician_User).ToString();
                    }
                    else if (get_roles.Contains(GlobalConstants.BackOffice_Role_Id)) //  Back-Office
                    {
                        this_user_role = ((int)NotificationUserRoleType.BackOffice_User).ToString();
                    }

                    if (ref_id_list != null && ref_id_list.Count > 0)
                    {
                        foreach (var ref_id in ref_id_list)
                        {
                            var get_site = _UoW.UserRepository.GetSiteBySiteId(Guid.Parse(ref_id));

                            AddNotificationDataForUsersRequestModel AddNotificationDataForUsersRequestModel = new AddNotificationDataForUsersRequestModel();

                            AddNotificationDataForUsersRequestModel.single_user = new UserDetailsWithRole();
                            AddNotificationDataForUsersRequestModel.single_user.user_id = Guid.Parse(userid);
                            AddNotificationDataForUsersRequestModel.single_user.notification_user_role_type = this_user_role;
                            AddNotificationDataForUsersRequestModel.ref_id = ref_id;
                            AddNotificationDataForUsersRequestModel.notification_type = (int)NotificationType_Version_2.SiteAssignedToUser;
                            AddNotificationDataForUsersRequestModel.heading = "New site is assigned to you";
                            AddNotificationDataForUsersRequestModel.message = "Access granted for site : " + get_site.site_name+".";

                            await AddNotificationDataForSingleUser(AddNotificationDataForUsersRequestModel);
                        }
                    }
                }

                //---All WOLines Completed/ReadyForReview of Workorder then send Notification to All BackOffice Users of that site
                else if (notification_type == (int)NotificationType_Version_2.AllWOLinesCompletedORReadyForReviewOfWO)
                {
                    var ref_id = ref_id_list.FirstOrDefault(); //wo_id
                    WorkOrderService workOrderService = new WorkOrderService(_mapper);
                    var get_woline_count = workOrderService.GetStatusWiseWOLineCountByWOId(Guid.Parse(ref_id));
                    var get_wo = _UoW.WorkOrderRepository.GetWOByidforUpdate(Guid.Parse(ref_id));

                    if (get_woline_count.total_count == (get_woline_count.completed_obwo_asset + get_woline_count.ready_for_review_obwo_asset))
                    {
                        var get_all_back_office_users = _UoW.UserRepository.GetAllBackOfficeUsersBySiteId(get_wo.site_id);

                        AddNotificationDataForUsersRequestModel AddNotificationDataForUsersRequestModel = new AddNotificationDataForUsersRequestModel();
                        AddNotificationDataForUsersRequestModel.users_list = new List<UserDetailsWithRole>();

                        foreach (var BO_user in get_all_back_office_users)
                        {
                            UserDetailsWithRole UserDetailsWithRole = new UserDetailsWithRole();
                            UserDetailsWithRole.user_id = BO_user;
                            UserDetailsWithRole.notification_user_role_type = ((int)NotificationUserRoleType.BackOffice_User).ToString();

                            AddNotificationDataForUsersRequestModel.users_list.Add(UserDetailsWithRole);
                        }
                        AddNotificationDataForUsersRequestModel.ref_id = ref_id;
                        AddNotificationDataForUsersRequestModel.notification_type = (int)NotificationType_Version_2.AllWOLinesCompletedORReadyForReviewOfWO;
                        AddNotificationDataForUsersRequestModel.heading = "All items completed or in ready for review state in work order";
                        AddNotificationDataForUsersRequestModel.message = "Work Order " + get_wo.manual_wo_number + "(" + get_wo.Sites.site_name + ")" + " is ready for review.";

                        var insert_notification = await AddNotificationDataForUsers(AddNotificationDataForUsersRequestModel);
                        if (insert_notification == (int)ResponseStatusNumber.Success)
                        {
                            res = true;
                        }
                    }
                }

                //--- WorkOrder is Completed with Issue Created then send Notification to All BackOffice Users of that site
                else if (notification_type == (int)NotificationType_Version_2.WorkOrder_is_Completed_With_Issue_Created)
                {
                    var ref_id = ref_id_list.FirstOrDefault(); //wo_id
                    var get_wo = _UoW.WorkOrderRepository.GetWOByidforUpdate(Guid.Parse(ref_id));
                    var open_issues_count = get_wo.WOLineIssue.Where(x => x.issue_status == (int)Status.open && !x.is_deleted).Count();
                    var get_all_back_office_users = _UoW.UserRepository.GetAllBackOfficeUsersBySiteId(get_wo.site_id);

                    AddNotificationDataForUsersRequestModel AddNotificationDataForUsersRequestModel = new AddNotificationDataForUsersRequestModel();
                    AddNotificationDataForUsersRequestModel.ref_id = ref_id;

                    AddNotificationDataForUsersRequestModel.users_list = new List<UserDetailsWithRole>();
                    foreach (var BO_user in get_all_back_office_users)
                    {
                        UserDetailsWithRole UserDetailsWithRole = new UserDetailsWithRole();
                        UserDetailsWithRole.user_id = BO_user;
                        UserDetailsWithRole.notification_user_role_type = ((int)NotificationUserRoleType.BackOffice_User).ToString();

                        AddNotificationDataForUsersRequestModel.users_list.Add(UserDetailsWithRole);
                    }
                    AddNotificationDataForUsersRequestModel.notification_type = (int)NotificationType_Version_2.WorkOrder_is_Completed_With_Issue_Created;
                    AddNotificationDataForUsersRequestModel.heading = "Work Order is completed with issues created";
                    AddNotificationDataForUsersRequestModel.message = "Work Order " + get_wo.manual_wo_number + "(" + get_wo.Sites.site_name + ")" + " is now completed. There are " + open_issues_count + " open issues to review.";

                    var insert_notification = await AddNotificationDataForUsers(AddNotificationDataForUsersRequestModel);
                    if (insert_notification == (int)ResponseStatusNumber.Success)
                    {
                        res = true;
                    }
                }

                //--- WorkOrder is Due then send Notification to All Technician-Users and Watcher(BackOffice) Users of that Workorder
                else if (notification_type == (int)NotificationType_Version_2.AssignedWorkorderIsDue)
                {
                    var ref_id = ref_id_list.FirstOrDefault();//wo_id
                    var get_wo = _UoW.WorkOrderRepository.GetWOByidforUpdate(Guid.Parse(ref_id));
                    var get_wo_technicians_ids = get_wo.WorkOrderTechnicianMapping.Where(x=>!x.is_deleted).Select(x=>x.user_id.ToString()).ToList();
                    var get_wo_watcher_ids = _UoW.WorkOrderRepository.GetWorkorderWatcherByWOId(Guid.Parse(ref_id));

                    var due_in_difference = get_wo.due_at.Date - DateTime.UtcNow.Date;

                    AddNotificationDataForUsersRequestModel AddNotificationDataForUsersRequestModel = new AddNotificationDataForUsersRequestModel();
                    AddNotificationDataForUsersRequestModel.users_list = new List<UserDetailsWithRole>();

                    foreach (var techi in get_wo_technicians_ids)
                    {
                        UserDetailsWithRole UserDetailsWithRole = new UserDetailsWithRole();
                        UserDetailsWithRole.user_id = Guid.Parse(techi);
                        UserDetailsWithRole.notification_user_role_type = ((int)NotificationUserRoleType.Technician_User).ToString();

                        AddNotificationDataForUsersRequestModel.users_list.Add(UserDetailsWithRole);
                    }
                    foreach (var BO_user in get_wo_watcher_ids)
                    {
                        var get_tech_BO = AddNotificationDataForUsersRequestModel.users_list.Where(x => x.user_id == Guid.Parse(BO_user)).Any();
                        if (get_tech_BO)
                        {
                            AddNotificationDataForUsersRequestModel.users_list.Where(x => x.user_id == Guid.Parse(BO_user))
                                .FirstOrDefault().notification_user_role_type = ((int)NotificationUserRoleType.BackOffice_User).ToString() + ((int)NotificationUserRoleType.Technician_User).ToString();
                        }
                        else
                        {
                            UserDetailsWithRole UserDetailsWithRole = new UserDetailsWithRole();
                            UserDetailsWithRole.user_id = Guid.Parse(BO_user);
                            UserDetailsWithRole.notification_user_role_type = ((int)NotificationUserRoleType.BackOffice_User).ToString();

                            AddNotificationDataForUsersRequestModel.users_list.Add(UserDetailsWithRole);
                        }
                    }
                    AddNotificationDataForUsersRequestModel.ref_id = ref_id;
                    AddNotificationDataForUsersRequestModel.notification_type = (int)NotificationType_Version_2.AssignedWorkorderIsDue;
                    AddNotificationDataForUsersRequestModel.heading = "Work Order is Due";
                    if (due_in_difference.Days == 0) { AddNotificationDataForUsersRequestModel.message = "Work Order " + get_wo.manual_wo_number + "(" + get_wo.Sites.site_name + ")" + " is due Today!"; }
                    else { AddNotificationDataForUsersRequestModel.message = "Work Order " + get_wo.manual_wo_number + "(" + get_wo.Sites.site_name + ")" + " is due by " + due_in_difference.Days.ToString() +" days." ; }
                    
                    var insert_notification = await AddNotificationDataForUsers(AddNotificationDataForUsersRequestModel);
                    if (insert_notification == (int)ResponseStatusNumber.Success)
                    {
                        res = true;
                    }
                }

                //--- WorkOrder is OverDue then send Notification to All BackOffice-Users of that Workorder
                else if (notification_type == (int)NotificationType_Version_2.WorkorderIsOverDue)
                {
                    var ref_id = ref_id_list.FirstOrDefault(); //wo_id
                    var get_wo = _UoW.WorkOrderRepository.GetWOByidforUpdate(Guid.Parse(ref_id));
                    var open_issues_count = get_wo.WOLineIssue.Where(x => x.issue_status == (int)Status.open && !x.is_deleted).Count();
                    var get_all_back_office_users = _UoW.UserRepository.GetAllBackOfficeUsersBySiteId(get_wo.site_id);

                    var overdue_in_difference = DateTime.UtcNow.Date - get_wo.due_at.Date;

                    AddNotificationDataForUsersRequestModel AddNotificationDataForUsersRequestModel = new AddNotificationDataForUsersRequestModel();
                    AddNotificationDataForUsersRequestModel.users_list = new List<UserDetailsWithRole>();

                    foreach (var BO_user in get_all_back_office_users)
                    {
                        UserDetailsWithRole UserDetailsWithRole = new UserDetailsWithRole();
                        UserDetailsWithRole.user_id = BO_user;
                        UserDetailsWithRole.notification_user_role_type = ((int)NotificationUserRoleType.BackOffice_User).ToString();

                        AddNotificationDataForUsersRequestModel.users_list.Add(UserDetailsWithRole);
                    }
                    AddNotificationDataForUsersRequestModel.ref_id = ref_id;
                    AddNotificationDataForUsersRequestModel.notification_type = (int)NotificationType_Version_2.WorkorderIsOverDue;
                    AddNotificationDataForUsersRequestModel.heading = "Work Order is overdue";
                    if (overdue_in_difference.Days == 0) { AddNotificationDataForUsersRequestModel.message = "Work Order " + get_wo.manual_wo_number + "(" + get_wo.Sites.site_name + ")" + " is overdue Today"; }
                    else { AddNotificationDataForUsersRequestModel.message = "Work Order " + get_wo.manual_wo_number + "(" + get_wo.Sites.site_name + ")" + " is overdue by " + overdue_in_difference.Days.ToString() + " days." ; }
                    
                    var insert_notification = await AddNotificationDataForUsers(AddNotificationDataForUsersRequestModel);
                    if (insert_notification == (int)ResponseStatusNumber.Success)
                    {
                        res = true;
                    }
                }

            }
            catch(Exception ex)
            {
            }
            return res;
        }

        public async Task<int> AddNotificationDataForUsers(AddNotificationDataForUsersRequestModel requestModel)
        {
            int response = (int)ResponseStatusNumber.Error;
            try
            {
                if (requestModel.users_list != null && requestModel.users_list.Count > 0)
                {
                    foreach (var this_user in requestModel.users_list)
                    {
                        NotificationData notification = new NotificationData();
                        notification.user_id = this_user.user_id;
                        notification.ref_id = requestModel.ref_id;
                        notification.message = requestModel.message;
                        notification.heading = requestModel.heading;
                        notification.notification_type = requestModel.notification_type;
                        notification.status = (int)Status.Active;
                        notification.sendDate = DateTime.UtcNow;
                        notification.createdDate = DateTime.UtcNow;
                        notification.notification_status = (int)Notification_Status.New;
                        notification.notification_user_role = this_user.notification_user_role_type;
                        notification.is_visible = true;

                        if (requestModel.notification_type == (int)NotificationType_Version_2.WorkorderIsOverDue)
                        {
                            /*var is_Overdue_by_week = _UoW.UserRepository.CheckIsWOOverdueByWeekOrNot(requestModel.ref_id);
                            if (is_Overdue_by_week)
                                notification.notification_status = (int)Notification_Status.Read;*/

                            var get_old_notif = _UoW.UserRepository.GetAllNewNotificationsByTypeUserId(this_user.user_id, requestModel.notification_type,requestModel.ref_id);
                            foreach (var item in get_old_notif)
                            {
                                // mark as read all previous notifications other than today's
                                item.notification_status = (int)Notification_Status.Read;

                                //hide if it is older than a week
                                if(item.sendDate.Date < DateTime.UtcNow.AddDays(-7).Date)
                                    item.is_visible = false;

                                await _UoW.BaseGenericRepository<NotificationData>().Update(item);
                                _UoW.SaveChanges();
                            }
                        }

                        //notification.OS = userlists.User.OS;
                        //notification.device_key = userlists.User.notification_token;
                        //notification.target_role = this_user.role_id.ToString();

                        var insert_notification = await _UoW.BaseGenericRepository<NotificationData>().Insert(notification);
                        if (insert_notification)
                        {
                            response = (int)ResponseStatusNumber.Success;
                        }
                    }
                    _UoW.SaveChanges();
                }

            }
            catch (Exception e)
            {
            }
            return response;
        }

        public async Task<int> AddNotificationDataForSingleUser(AddNotificationDataForUsersRequestModel requestModel)
        {
            int response = (int)ResponseStatusNumber.Error;
            try
            {
                if (requestModel.single_user != null)
                {
                    NotificationData notification = new NotificationData();
                    notification.user_id = requestModel.single_user.user_id;
                    notification.ref_id = requestModel.ref_id;
                    notification.message = requestModel.message;
                    notification.heading = requestModel.heading;
                    notification.notification_type = requestModel.notification_type;
                    notification.status = (int)Status.Active;
                    notification.sendDate = DateTime.UtcNow;
                    notification.createdDate = DateTime.UtcNow;
                    notification.notification_status = (int)Notification_Status.New;
                    notification.notification_user_role = requestModel.single_user.notification_user_role_type;
                    notification.is_visible = true;
                    //notification.OS = userlists.User.OS;
                    //notification.device_key = userlists.User.notification_token;
                    //notification.target_role = this_user.role_id.ToString();

                    var insert_notification = await _UoW.BaseGenericRepository<NotificationData>().Insert(notification);
                    if (insert_notification)
                    {
                        response = (int)ResponseStatusNumber.Success;
                    }
                    _UoW.SaveChanges();
                }
            }
            catch (Exception e)
            {
            }
            return response;
        }

    }
}

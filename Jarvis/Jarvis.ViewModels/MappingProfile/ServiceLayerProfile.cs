using AutoMapper;
using Jarvis.db.ExcludePropertiesfromDBHelper;
using Jarvis.db.Models;
using Jarvis.Shared.Helper;
using Jarvis.Shared.StatusEnums;
using Jarvis.Shared.Utility;
using Jarvis.ViewModels;
using Jarvis.ViewModels.RequestResponseViewModel;
using Jarvis.ViewModels.Utility;
using Jarvis.ViewModels.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jarvis.Service.MappingProfile
{
    public class ServiceLayerProfile : Profile
    {
        public ServiceLayerProfile()
        {

            CreateMap<User, LoginResponceModel>().ForMember(des => des.Userroles, opt => opt.MapFrom(src => src.Userroles))
                .ForMember(des => des.rolename, opt => opt.MapFrom(src => src.Active_Role_App.name))
                .ForMember(des => des.default_rolename_app, opt => opt.MapFrom(src => src.ac_default_role_app))
                .ForMember(des => des.active_rolename_app, opt => opt.MapFrom(src => src.ac_active_role_app))
                .ForMember(des => des.default_rolename_app_name, opt => opt.MapFrom(src => src.Role_App.name))
                .ForMember(des => des.active_rolename_app_name, opt => opt.MapFrom(src => src.Active_Role_App.name))
                .ForMember(des => des.default_rolename_web, opt => opt.MapFrom(src => src.ac_default_role_web))
                .ForMember(des => des.default_rolename_web_name, opt => opt.MapFrom(src => src.Role_Web.name))
                .ForMember(des => des.active_rolename_web, opt => opt.MapFrom(src => src.ac_active_role_web))
                .ForMember(des => des.active_rolename_web_name, opt => opt.MapFrom(src => src.Active_Role_Web.name))
                .ForMember(des => des.default_site_id, opt => opt.MapFrom(src => src.ac_default_site))
                .ForMember(des => des.default_company_id, opt => opt.MapFrom(src => src.ac_default_company))
                .ForMember(des => des.active_site_id, opt => opt.MapFrom(src => src.ac_active_site))
                .ForMember(des => des.active_company_id, opt => opt.MapFrom(src => src.ac_active_company))
                .ForMember(des => des.default_site_name, opt => opt.MapFrom(src => src.Site.site_name))
                .ForMember(des => des.active_site_name, opt => opt.MapFrom(src => src.Active_Site.site_name))
                .ForMember(des => des.default_company_name, opt => opt.MapFrom(src => src.Company.company_name))
                .ForMember(des => des.active_company_name, opt => opt.MapFrom(src => src.Active_Company.company_name))
                .ForMember(des => des.active_site_status, opt => opt.MapFrom(src => src.Active_Site.status))
                .ForMember(des => des.active_company_status, opt => opt.MapFrom(src => src.Active_Company.status))
                .ForMember(des => des.default_site_status, opt => opt.MapFrom(src => src.Site.status))
                .ForMember(des => des.default_company_status, opt => opt.MapFrom(src => src.Company.status))
                .ForPath(des => des.client_company, opt => opt.MapFrom(src => src.Company.ClientCompany.Where(x => x.status == (int)Status.Active)))
                .ForPath(des => des.default_client_company_Usersites, opt => opt.MapFrom(src => src.default_client_Company.Sites.Where(x=>x.status == (int)Status.Active)))
                .ForPath(des => des.ac_default_client_company_name, opt => opt.MapFrom(src => src.default_client_Company.client_company_name))
                .ForPath(des => des.ac_active_client_company_name, opt => opt.MapFrom(src => src.Active_client_Company.client_company_name))
                //.ForPath(des => des.Usersites, opt => opt.MapFrom(src => src.Usersites!=null ? src.Usersites.Where(x=>x.Sites.status == (int)Status.Active):null))
                //.ForMember(des => des.default_app_name, opt => opt.MapFrom(src => src.AppMaster.app_name))
                //.ForMember(des => des.ti_default_role, opt => opt.MapFrom(src => src.ti_default_role))
                //.ForMember(des => des.ti_default_site, opt => opt.MapFrom(src => src.ti_default_site))
                .ForMember(des => des.prefer_language_name, opt => opt.MapFrom(src => src.LanguageMaster.language_name))
                .ForPath(des => des.profile_picture_url, opt => opt.MapFrom(src => UrlGenerator.GetProfilePictureURL(src.profile_picture_name)));

            CreateMap<LoginResponceModel, User>();




            CreateMap<UserRoles, UserRole>()
                .ForMember(des => des.role_name, opt => opt.MapFrom(src => src.Role.name));

            CreateMap<UserPoolResponseModel, Company>();
            CreateMap<Company, UserPoolResponseModel>();

            CreateMap<Company, GetCompanyLogosResponsemodel>()
                .ForMember(des => des.company_favicon_logo, opt => opt.MapFrom(src => src.company_thumbnail_logo))
                ;

            CreateMap<Role, RoleViewModel>();
            CreateMap<RoleViewModel, Role>();

            CreateMap<CompanyRequestModel, Company>();
            CreateMap<Company, CompanyRequestModel>();

            CreateMap<SitesViewModel, Sites>();
            CreateMap<Sites, SitesViewModel>();

            CreateMap<SiteRequestModel, Sites>();
            CreateMap<Sites, SiteRequestModel>();

            CreateMap<UsersitesResponseModel, UserSites>();
            CreateMap<UserSites, UsersitesResponseModel>();

            CreateMap<UserSites, UserSite>()
                .ForMember(des => des.company_id, opt => opt.MapFrom(src => src.Sites.company_id))
                .ForMember(des => des.main_site_status, opt => opt.MapFrom(src => src.Sites.status))
                .ForMember(des => des.company_name, opt => opt.MapFrom(src => src.Sites.Company.company_name));
            CreateMap<UserSite, UserSites>();

            CreateMap<UserRequestModel, User>()
                .ForMember(des => des.phone_number, opt => opt.MapFrom(src => src.mobile_number));
            CreateMap<User, UserRequestModel>()
                ;

            CreateMap<OperatorsListResponseModel, User>();
            CreateMap<User, OperatorsListResponseModel>();

            CreateMap<UserRoles, RoleViewModel>();
            CreateMap<RoleViewModel, UserRoles>();

            CreateMap<UsersitesRequestModel, UserSites>();
            CreateMap<UserSites, UsersitesRequestModel>();

            CreateMap<AttributesJsonObjectViewModel, AttributeValueJsonObject>();
            CreateMap<AttributeValueJsonObject, AttributesJsonObjectViewModel>()
                .ForMember(des => des.spanish_name, opt => opt.MapFrom(src => PreferLanguageSingleton.Instance.GetLanguageKeyByName(src.name, (int)Language.spanish).Result));

            CreateMap<FormAttributesViewModel, InspectionFormAttributes>();
            CreateMap<InspectionFormAttributes, FormAttributesViewModel>()
                .ForMember(des => des.categoryname, opt => opt.MapFrom(src => src.InspectionAttributeCategory.name));

            CreateMap<SitesViewModel, Sites>();
            CreateMap<Sites, SitesViewModel>();

            CreateMap<CompanyViewModel, Company>();
            CreateMap<Company, CompanyViewModel>();

            CreateMap<User, UserViewModel>();
            CreateMap<UserViewModel, User>();

            CreateMap<UserSites, SitesViewModel>();
            CreateMap<SitesViewModel, UserSites>();

            CreateMap<FormAttributesJsonObjectViewModel, FormAttributesJsonObject>();
            CreateMap<FormAttributesJsonObject, FormAttributesJsonObjectViewModel>()
                    .ForMember(des => des.spanish_name, opt => opt.MapFrom(src => PreferLanguageSingleton.Instance.GetLanguageKeyByName(src.name, (int)Language.spanish).Result));

            CreateMap<InspectionAttributesJsonObjectViewModel, FormAttributesJsonObjectViewModel>();
            CreateMap<FormAttributesJsonObjectViewModel, InspectionAttributesJsonObjectViewModel>();

            CreateMap<InspectionForms, InspectionFormRequestViewModel>();
            CreateMap<InspectionFormRequestViewModel, InspectionForms>();

            CreateMap<RoleRequestModel, Role>();
            CreateMap<Role, RoleRequestModel>();

            CreateMap<InspectionFormAttributes, InspectionFormAttributesRequestModel>();
            CreateMap<InspectionFormAttributesRequestModel, InspectionFormAttributes>();


            CreateMap<InspectionAttributeCategoryViewModel, InspectionAttributeCategory>().ForMember(des => des.InspectionFormAttributes, opt => opt.MapFrom(src => src.name));
            CreateMap<InspectionAttributeCategory, InspectionAttributeCategoryViewModel>();

            CreateMap<AssetRequestModel, Asset>();
            CreateMap<Asset, AssetRequestModel>();

            CreateMap<AssetsResponseModel, Asset>();
            CreateMap<Asset, AssetsResponseModel>()
                .ForMember(des => des.client_company_id, opt => opt.MapFrom(src => src.Sites.client_company_id))
                .ForMember(des => des.client_company_name, opt => opt.MapFrom(src => src.Sites.ClientCompany.client_company_name))
                .ForMember(des => des.site_name, opt => opt.MapFrom(src => src.Sites.site_name))
                .ForMember(des => des.company_name, opt => opt.MapFrom(src => src.Sites.Company.company_name))
                .ForMember(des => des.Inspections, opt => opt.MapFrom(src => src.Inspection))
                .ForMember(des => des.company_code, opt => opt.MapFrom(src => src.Sites.Company.company_code))
                .ForMember(des => des.site_code, opt => opt.MapFrom(src => src.Sites.site_code))
                .ForMember(des => des.asset_photo, opt => opt.MapFrom(src => src.asset_photo != null ? UrlGenerator.GetAssetImagesURL(src.asset_photo) : null))
                .ForMember(des => des.status_name, opt => opt.MapFrom(src => src.StatusMaster.status_name))
                .ForMember(des => des.isAutoApprove, opt => opt.MapFrom(src => src.Sites.isAutoApprove))
                .ForMember(des => des.timeelapsed, opt => opt.MapFrom(src => src.created_at!=null ? DateTimeUtil.GetBeforetimeText(src.created_at.Value) : null))
                .ForMember(des => des.timezone, opt => opt.MapFrom(src => src.Sites.timezone))
                .ForMember(des => des.inspection_verdict_name, opt => opt.MapFrom(src => src.inspection_verdict != null ? ((inspectionVerdictdropdownname)src.inspection_verdict.Value).ToString() : null))
                .ForMember(des => des.condition_index_type_name, opt => opt.MapFrom(src => src.condition_index_type != null ? ((condition_index_type)src.condition_index_type.Value).ToString() : null))
                .ForMember(des => des.criticality_index_type_name, opt => opt.MapFrom(src => src.criticality_index_type != null ? ((criticality_index_type)src.criticality_index_type.Value).ToString() : null))
                .ForMember(des => des.thermal_classification_name, opt => opt.MapFrom(src => src.thermal_classification_id != null ? ((thermal_classification)src.thermal_classification_id.Value).ToString() : null))
                .ForMember(des => des.AssetLocationHierarchy, opt => opt.MapFrom(src => src.AssetFormIOBuildingMappings))
                .ForMember(des => des.formio_building_name, opt => opt.MapFrom(src => src.AssetFormIOBuildingMappings.FormIOBuildings.formio_building_name))
                .ForMember(des => des.formio_floor_name, opt => opt.MapFrom(src => src.AssetFormIOBuildingMappings.FormIOFloors.formio_floor_name))
                .ForMember(des => des.formio_room_name, opt => opt.MapFrom(src => src.AssetFormIOBuildingMappings.FormIORooms.formio_room_name))
                .ForMember(des => des.formio_location_notes, opt => opt.MapFrom(src => src.AssetFormIOBuildingMappings.FormIOSections.FormIOLocationNotes.notes))
                .ForMember(des => des.formio_section_name, opt => opt.MapFrom(src => src.AssetFormIOBuildingMappings.FormIOSections.formio_section_name))
                .ForMember(des => des.asset_class_code, opt => opt.MapFrom(src => src.InspectionTemplateAssetClass.asset_class_code))
                .ForMember(des => des.asset_class_name, opt => opt.MapFrom(src => src.InspectionTemplateAssetClass.asset_class_name))
                .ForMember(des => des.asset_class_type, opt => opt.MapFrom(src => src.InspectionTemplateAssetClass.FormIOType != null ? src.InspectionTemplateAssetClass.FormIOType.form_type_name : null))
                .ForMember(des => des.asset_profile_images, opt => opt.MapFrom(src => src.AssetProfileImages.Where(x => (x.asset_photo_type == 1 || x.asset_photo_type == (int)AssetPhotoType.PM_Additional_General_photo ||
                                                                                                                            x.asset_photo_type == (int)AssetPhotoType.PM_Additional_Nameplate_photo || 
                                                                                                                            x.asset_photo_type == (int)AssetPhotoType.PM_Additional_Before_photo ||
                                                                                                                            x.asset_photo_type == (int)AssetPhotoType.PM_Additional_After_photo ||
                                                                                                                            x.asset_photo_type == (int)AssetPhotoType.Additional_Photos ||
                                                                                                                            x.asset_photo_type == (int)AssetPhotoType.Exterior_Photo ||
                                                                                                                            x.asset_photo_type == (int)AssetPhotoType.Nameplate_Photo || //Adding nameplate photos in same list for FE requirement
                                                                                                                            x.asset_photo_type == (int)AssetPhotoType.Schedule_Photos || //Adding Schedule photos in same list for FE requirement
                                                                                                                            x.asset_photo_type == (int)AssetPhotoType.PM_Additional_Environment_photo) && !x.is_deleted).ToList()))
                .ForMember(des => des.asset_nameplate_images, opt => opt.MapFrom(src => src.AssetProfileImages.Where(x => x.asset_photo_type == 2 && !x.is_deleted).ToList()))
                .ForMember(des => des.asset_IR_scan_images, opt => opt.MapFrom(src => src.AssetProfileImages.Where(x => x.asset_photo_type == 3 && !x.is_deleted).ToList()))
                .ForMember(des => des.ob_ir_Image_label_list, opt => opt.MapFrom(src => src.AssetIRWOImagesLabelMapping))
                .ForMember(des => des.asset_parent_mapping_list, opt => opt.MapFrom(src => src.AssetParentHierarchyMapping))
                .ForMember(des => des.WorkOrders, opt => opt.MapFrom(src => src.Issues))
                .ForMember(des => des.asset_subcomponents_mapping_list, opt => opt.MapFrom(src => src.AssetSubLevelcomponentMapping))
                .ForMember(des => des.issue_count, opt => opt.MapFrom(src => src.AssetIssue!=null ? src.AssetIssue.Where(x=>!x.is_deleted).Count() : 0))
                .ForMember(des => des.asset_profile_image, opt => opt.MapFrom(src => src.asset_profile_image != null
                ? src.asset_profile_image : src.AssetProfileImages.Where(x => x.asset_photo_type == 1&&!x.is_deleted).FirstOrDefault() != null
                ? UrlGenerator.GetAssetImagesURL(src.AssetProfileImages.Where(x => x.asset_photo_type == 1 && !x.is_deleted).Select(x => x.asset_photo).FirstOrDefault()) : null))
                ;

            CreateMap<AssetProfileImages, subcomponentasset_image_list_class>()
                .ForMember(des => des.asset_photo_url, opt => opt.MapFrom(src => src.asset_photo != null ? UrlGenerator.GetAssetImagesURL(src.asset_photo) : null))
                ;

            CreateMap<AssetSubLevelcomponentMapping, asset_subcomponents_mapping_listview_class>();

            CreateMap<AssetParentHierarchyMapping, AssetParentsMapping>();

            CreateMap<AssetIRWOImagesLabelMapping, Asset_OBIRImage_labels>()
                .ForMember(des => des.ir_image_label_url, opt => opt.MapFrom(src => src.ir_image_label != null ? UrlGenerator.GetIRImagesURL(src.ir_image_label, src.s3_image_folder_name) : null))
                .ForMember(des => des.visual_image_label_url, opt => opt.MapFrom(src => src.visual_image_label != null ? UrlGenerator.GetIRImagesURL(src.visual_image_label, src.s3_image_folder_name) : null))
                ;

            CreateMap<AssetProfileImages, AssetProfileImageList>()
               .ForMember(des => des.asset_photo, opt => opt.MapFrom(src => src.asset_photo != null ? UrlGenerator.GetAssetImagesURL(src.asset_photo) : null))
               .ForMember(des => des.asset_thumbnail_photo, opt => opt.MapFrom(src => src.asset_photo != null ? UrlGenerator.GetAssetImagesURL(src.asset_thumbnail_photo) : null))
               ;

            CreateMap<AssetProfileImages, AssetNameplateImageList>()
               .ForMember(des => des.asset_photo, opt => opt.MapFrom(src => src.asset_photo != null ? UrlGenerator.GetAssetImagesURL(src.asset_photo) : null))
               .ForMember(des => des.asset_thumbnail_photo, opt => opt.MapFrom(src => src.asset_thumbnail_photo != null ? UrlGenerator.GetAssetImagesURL(src.asset_thumbnail_photo) : src.asset_photo !=null ? UrlGenerator.GetAssetImagesURL(src.asset_photo) : null))// if thumbnail is null then retrun actual image
               ;
            CreateMap<AssetProfileImages, AssetIRScanImageList>()
               .ForMember(des => des.asset_photo, opt => opt.MapFrom(src => src.asset_photo != null ? UrlGenerator.GetAssetImagesURL(src.asset_photo) : null))
               .ForMember(des => des.asset_thumbnail_photo, opt => opt.MapFrom(src => src.asset_photo != null ? UrlGenerator.GetAssetImagesURL(src.asset_thumbnail_photo) : null))
               ;
            CreateMap<Asset, MobileAssetsResponseModel>()
                .ForMember(des => des.status_name, opt => opt.MapFrom(src => src.StatusMaster.status_name))
                .ForMember(des => des.asset_class_name, opt => opt.MapFrom(src => src.InspectionTemplateAssetClass.asset_class_name))
                .ForMember(des => des.asset_class_code, opt => opt.MapFrom(src => src.InspectionTemplateAssetClass.asset_class_code))
                .ForMember(des => des.asset_profile_images, opt => opt.MapFrom(src => src.AssetProfileImages.Where(x => x.asset_photo_type == 1 && !x.is_deleted).ToList()))
                .ForMember(des => des.asset_nameplate_images, opt => opt.MapFrom(src => src.AssetProfileImages.Where(x => x.asset_photo_type == 2 && !x.is_deleted).ToList()))
                .ForMember(des => des.asset_IR_scan_images, opt => opt.MapFrom(src => src.AssetProfileImages.Where(x => x.asset_photo_type == 3 && !x.is_deleted).ToList()))
                .ForMember(des => des.site_name, opt => opt.MapFrom(src => src.Sites.site_name))
                .ForMember(des => des.formio_building_name, opt => opt.MapFrom(src => src.AssetFormIOBuildingMappings.FormIOBuildings.formio_building_name))
                .ForMember(des => des.formio_floor_name, opt => opt.MapFrom(src => src.AssetFormIOBuildingMappings.FormIOFloors.formio_floor_name))
                .ForMember(des => des.formio_room_name, opt => opt.MapFrom(src => src.AssetFormIOBuildingMappings.FormIORooms.formio_room_name))
                .ForMember(des => des.formio_section_name, opt => opt.MapFrom(src => src.AssetFormIOBuildingMappings.FormIOSections.formio_section_name))
                .ForMember(des => des.condition_index_type_name, opt => opt.MapFrom(src => src.condition_index_type != null ? ((condition_index_type)src.condition_index_type.Value).ToString() : null))
                .ForMember(des => des.criticality_index_type_name, opt => opt.MapFrom(src => src.criticality_index_type != null ? ((criticality_index_type)src.criticality_index_type.Value).ToString() : null))
                .ForMember(des => des.inspection_verdict_name, opt => opt.MapFrom(src => src.inspection_verdict != null ? ((inspectionVerdictdropdownname)src.inspection_verdict.Value).ToString() : null))
                ;

            CreateMap<AssetFormIOBuildingMappings, AssetLocationHierarchy>()
                .ForMember(des => des.formio_building_name, opt => opt.MapFrom(src => src.FormIOBuildings.formio_building_name))
                .ForMember(des => des.formio_floor_name, opt => opt.MapFrom(src => src.FormIOFloors.formio_floor_name))
                .ForMember(des => des.formio_room_name, opt => opt.MapFrom(src => src.FormIORooms.formio_room_name))
                .ForMember(des => des.formio_location_notes, opt => opt.MapFrom(src => src.FormIOSections.FormIOLocationNotes.notes))
                .ForMember(des => des.formio_section_name, opt => opt.MapFrom(src => src.FormIOSections.formio_section_name));

            CreateMap<PendingInspectionCheckoutAssetsResponseModel, Asset>();
            CreateMap<Asset, PendingInspectionCheckoutAssetsResponseModel>().ForMember(des => des.site_name, opt => opt.MapFrom(src => src.Sites.site_name))
                .ForMember(des => des.company_name, opt => opt.MapFrom(src => src.Sites.Company.company_name))
                .ForMember(des => des.company_code, opt => opt.MapFrom(src => src.Sites.Company.company_code))
                .ForMember(des => des.site_code, opt => opt.MapFrom(src => src.Sites.site_code))
                .ForMember(des => des.timezone, opt => opt.MapFrom(src => src.Sites.timezone))
                .ForMember(des => des.asset_photo, opt => opt.MapFrom(src => src.asset_photo != null ? UrlGenerator.GetAssetImagesURL(src.asset_photo) : null));

            CreateMap<InspectionResponseModel, Inspection>();
            CreateMap<Inspection, InspectionResponseModel>()
                .ForMember(des => des.operator_name, opt => opt.MapFrom(src => src.User.username))
                .ForMember(des => des.internal_asset_id, opt => opt.MapFrom(src => src.Asset.internal_asset_id))
                .ForMember(des => des.asset_name, opt => opt.MapFrom(src => src.Asset.name))
                .ForMember(des => des.operator_firstname, opt => opt.MapFrom(src => src.User.firstname))
                .ForMember(des => des.operator_lastname, opt => opt.MapFrom(src => src.User.lastname))
                .ForMember(des => des.status_name, opt => opt.MapFrom(src => src.StatusMaster.status_name))
                .ForMember(des => des.site_name, opt => opt.MapFrom(src => src.Sites == null ? null : src.Sites.site_name))
                .ForMember(des => des.time_elapsed, opt => opt.MapFrom(src => DateTimeUtil.GetBeforetimeText(src.created_at)));

            CreateMap<Inspection, MobileInspectionResponseModel>()
                // .ForMember(des => des.operator_name, opt => opt.MapFrom(src => src.User.username))
                //   .ForMember(des => des.internal_asset_id, opt => opt.MapFrom(src => src.Asset.internal_asset_id))
                //  .ForMember(des => des.asset_name, opt => opt.MapFrom(src => src.Asset.name))
                .ForMember(des => des.operator_firstname, opt => opt.MapFrom(src => src.User.firstname))
                .ForMember(des => des.internal_asset_id, opt => opt.MapFrom(src => src.Asset.internal_asset_id))
                .ForMember(des => des.asset_name, opt => opt.MapFrom(src => src.Asset.name))
                .ForMember(des => des.asset_id, opt => opt.MapFrom(src => src.Asset.asset_id))
                .ForMember(des => des.timezone, opt => opt.MapFrom(src => src.Sites.timezone))

                .ForMember(des => des.operator_lastname, opt => opt.MapFrom(src => src.User.lastname));
            //  .ForMember(des => des.status_name, opt => opt.MapFrom(src => src.StatusMaster.status_name))
            //  .ForMember(des => des.site_name, opt => opt.MapFrom(src => src.Sites == null ? null : src.Sites.site_name))
            //  .ForMember(des => des.time_elapsed, opt => opt.MapFrom(src => DateTimeUtil.GetBeforetimeText(src.created_at)));

            CreateMap<InspectionDetails, Inspection>();
            CreateMap<Inspection, InspectionDetails>()
                .ForMember(des => des.operator_name, opt => opt.MapFrom(src => src.User.username))
                .ForMember(des => des.site_name, opt => opt.MapFrom(src => src.Sites.site_name))
                .ForMember(des => des.internal_asset_id, opt => opt.MapFrom(src => src.Asset.internal_asset_id))
                .ForMember(des => des.asset_name, opt => opt.MapFrom(src => src.Asset.name))
                .ForMember(des => des.meter_hours_at_inspection, opt => opt.MapFrom(src => src.meter_hours))
                .ForMember(des => des.time_elapsed, opt => opt.MapFrom(src => DateTimeUtil.GetBeforetimeText(src.created_at)));

            CreateMap<PendingAndCheckoutInspViewModel, Inspection>();
            CreateMap<Inspection, PendingAndCheckoutInspViewModel>().ForMember(des => des.site_name, opt => opt.MapFrom(src => src.Sites.site_name))
                .ForMember(des => des.company_name, opt => opt.MapFrom(src => src.Sites.Company.company_name))
                .ForMember(des => des.company_code, opt => opt.MapFrom(src => src.Sites.Company.company_code))
                .ForMember(des => des.site_code, opt => opt.MapFrom(src => src.Sites.site_code))
                .ForMember(des => des.site_status, opt => opt.MapFrom(src => src.Sites.status))
                .ForMember(des => des.internal_asset_id, opt => opt.MapFrom(src => src.Asset.internal_asset_id))
                .ForMember(des => des.asset_status, opt => opt.MapFrom(src => src.Asset.status))
                .ForMember(des => des.inspectionform_id, opt => opt.MapFrom(src => src.Asset.inspectionform_id))
                .ForMember(des => des.notes, opt => opt.MapFrom(src => src.Asset.notes))
                .ForMember(des => des.asset_request_status, opt => opt.MapFrom(src => src.Asset.asset_request_status))
                .ForMember(des => des.asset_requested_by, opt => opt.MapFrom(src => src.Asset.asset_requested_by))
                .ForMember(des => des.asset_requested_on, opt => opt.MapFrom(src => src.Asset.asset_requested_on))
                .ForMember(des => des.asset_approved_by, opt => opt.MapFrom(src => src.Asset.asset_approved_by))
                .ForMember(des => des.asset_approved_on, opt => opt.MapFrom(src => src.Asset.asset_approved_on))
                .ForMember(des => des.usage, opt => opt.MapFrom(src => src.Asset.usage))
                .ForMember(des => des.asset_name, opt => opt.MapFrom(src => src.Asset.name))
                .ForMember(des => des.asset_type, opt => opt.MapFrom(src => src.Asset.asset_type))
                .ForMember(des => des.product_name, opt => opt.MapFrom(src => src.Asset.product_name))
                .ForMember(des => des.model_name, opt => opt.MapFrom(src => src.Asset.model_name))
                .ForMember(des => des.asset_serial_number, opt => opt.MapFrom(src => src.Asset.asset_serial_number))
                .ForMember(des => des.model_year, opt => opt.MapFrom(src => src.Asset.model_year))
                .ForMember(des => des.current_stage, opt => opt.MapFrom(src => src.Asset.current_stage))
                .ForMember(des => des.company_status, opt => opt.MapFrom(src => src.Sites.Company.status))
                .ForMember(des => des.timeelapsed, opt => opt.MapFrom(src => DateTimeUtil.GetBeforetimeText(src.created_at)))
                .ForMember(des => des.location, opt => opt.MapFrom(src => src.Asset.site_location))
                .ForMember(des => des.status_name, opt => opt.MapFrom(src => src.StatusMaster.status_name))
                .ForMember(des => des.operator_firstname, opt => opt.MapFrom(src => src.User.firstname))
                .ForMember(des => des.operator_lastname, opt => opt.MapFrom(src => src.User.lastname))
                .ForMember(des => des.operator_name, opt => opt.MapFrom(src => src.User.username))
                .ForMember(des => des.isManagerNotes, opt => opt.MapFrom(src => src.Sites.isManagerNotes))
                .ForMember(des => des.showHideApprove, opt => opt.MapFrom(src => src.Sites.showHideApprove))
                .ForMember(des => des.asset_photo, opt => opt.MapFrom(src => src.Asset.asset_photo != null ? UrlGenerator.GetAssetImagesURL(src.Asset.asset_photo) : null));

            CreateMap<AssetInspectionViewModel, Inspection>();
            CreateMap<Inspection, AssetInspectionViewModel>()
                .ForMember(des => des.status_name, opt => opt.MapFrom(src => src.StatusMaster.status_name))
                .ForMember(des => des.operator_firstname, opt => opt.MapFrom(src => src.User.firstname))
                .ForMember(des => des.operator_lastname, opt => opt.MapFrom(src => src.User.lastname))
                .ForMember(des => des.operator_name, opt => opt.MapFrom(src => src.User.username));

            CreateMap<InspectionStatus, Inspection>();
            CreateMap<Inspection, InspectionStatus>()
                .ForMember(des => des.Duration, opt => opt.MapFrom(src => src.status == (int)Status.Approved ? DateTimeUtil.GetBeforetimeText(src.modified_at) : DateTimeUtil.GetBeforetimeText(src.created_at)));

            CreateMap<InspectionSiteViewModel, Company>();
            CreateMap<Company, InspectionSiteViewModel>();

            CreateMap<InspectionSiteViewModel, Sites>();
            CreateMap<Sites, InspectionSiteViewModel>().ForMember(des => des.company_code, opt => opt.MapFrom(src => src.Company.company_code))
                .ForMember(des => des.company_name, opt => opt.MapFrom(src => src.Company.company_name));

            CreateMap<Inspection, InspectionRequestModel>();
            CreateMap<InspectionRequestModel, Inspection>();

            CreateMap<AssetsValueJsonObject, AssetsValueJsonObjectViewModel>();
            CreateMap<AssetsValueJsonObjectViewModel, AssetsValueJsonObject>();

            CreateMap<ImagesListObjectViewModel, ImagesListObject>();
            CreateMap<ImagesListObject, ImagesListObjectViewModel>()
                .ForMember(des => des.image_names, opt => opt.MapFrom(src => UrlGenerator.GetInspectionImagesURL(src.image_names)))
                .ForMember(des => des.thumbnail_image_names, opt => opt.MapFrom(src => UrlGenerator.GetInspectionThumbnailImagesURL(src.image_names)));

            CreateMap<AssetViewModel, Asset>();
            CreateMap<Asset, AssetViewModel>().ForMember(des => des.asset_photo, opt => opt.MapFrom(src => src.asset_photo != null ? UrlGenerator.GetAssetImagesURL(src.asset_photo) : null))
                .ForMember(des => des.status_name, opt => opt.MapFrom(src => src.StatusMaster.status_name));

            CreateMap<InspectionForms, AssetInspectionFormResponseModel>().ForMember(des => des.status_name, opt => opt.MapFrom(src => src.StatusMaster == null ? null : src.StatusMaster.status_name));
            CreateMap<AssetInspectionFormResponseModel, InspectionForms>();

            CreateMap<Issue, IssueResponseModel>()
                .ForMember(des => des.inspections, opt => opt.MapFrom(src => src.Inspection))
                .ForMember(des => des.status_name, opt => opt.MapFrom(src => src.StatusMaster == null ? null : src.StatusMaster.status_name))
                .ForMember(des => des.timeelapsed, opt => opt.MapFrom(src => src.created_at == null ? "" : DateTimeUtil.GetBeforetimeText(src.created_at.Value)))
                .ForMember(des => des.datetime_requested, opt => opt.MapFrom(src => src.Inspection.datetime_requested))
                .ForMember(des => des.site_name, opt => opt.MapFrom(src => src.Sites == null ? null : src.Sites.site_name))
                .ForMember(des => des.asset_name, opt => opt.MapFrom(src => src.Asset == null ? null : src.Asset.name))
                .ForMember(des => des.work_order_uuid, opt => opt.MapFrom(src => src.issue_uuid))
                .ForMember(des => des.timezone, opt => opt.MapFrom(src => src.Sites == null ? null : src.Sites.timezone))
                .ForMember(des => des.work_order_number, opt => opt.MapFrom(src => src.issue_number));
            CreateMap<IssueResponseModel, Issue>();

            CreateMap<Issue, MobileIssueResponseModel>()
                .ForMember(des => des.status_name, opt => opt.MapFrom(src => src.StatusMaster.status_name))
                .ForMember(des => des.asset_id, opt => opt.MapFrom(src => src.asset_id))
                .ForMember(des => des.asset_name, opt => opt.MapFrom(src => src.Asset.name))
                .ForMember(des => des.internal_asset_id, opt => opt.MapFrom(src => src.Asset.internal_asset_id))
                .ForMember(des => des.notes, opt => opt.MapFrom(src => src.notes))
                .ForMember(des => des.timezone, opt => opt.MapFrom(src => src.Sites.timezone))
                .ForMember(des => des.inspection_id, opt => opt.MapFrom(src => src.inspection_id))
                ;

            CreateMap<Issue, IssuesNameResponseModel>();
            CreateMap<IssuesNameResponseModel, Issue>();

            CreateMap<InspectionAttributesJsonObjectViewModel, AssetsValueJsonObject>();
            CreateMap<AssetsValueJsonObject, InspectionAttributesJsonObjectViewModel>()
                .ForMember(des => des.value_spanish_name, opt => opt.MapFrom(src => PreferLanguageSingleton.Instance.GetLanguageKeyByName(src.value, (int)Language.spanish).Result))
                .ForMember(des => des.attribute_spanish_name, opt => opt.MapFrom(src => PreferLanguageSingleton.Instance.GetLanguageKeyByName(src.name, (int)Language.spanish).Result));

            CreateMap<GetAllCompanyResponseModel, Company>();
            CreateMap<Company, GetAllCompanyResponseModel>();

            CreateMap<GetCompanySitesViewModel, Sites>();
            CreateMap<Sites, GetCompanySitesViewModel>();

            CreateMap<Issue, IssueRequestModel>();
            CreateMap<IssueRequestModel, Issue>();

            CreateMap<CommentJsonObjectViewModel, CommentJsonObject>();
            CreateMap<CommentJsonObject, CommentJsonObjectViewModel>();

            CreateMap<IssueRequestModel, IssueStatus>();
            CreateMap<IssueStatus, IssueRequestModel>();


            CreateMap<UserSitesNotificationModel, UserSites>();
            CreateMap<UserSites, UserSitesNotificationModel>();

            CreateMap<UsersData, User>();
            CreateMap<User, UsersData>();

            CreateMap<NotificationViewModel, NotificationData>();
            CreateMap<NotificationData, NotificationViewModel>();

            CreateMap<GetUserResponseModel, User>();
            CreateMap<User, GetUserResponseModel>().ForMember(des => des.status_name, opt => opt.MapFrom(src => src.StatusMaster.status_name))
                .ForMember(des => des.default_rolename_app_name, opt => opt.MapFrom(src => src.Role_App.name))
                .ForMember(des => des.default_rolename_web_name, opt => opt.MapFrom(src => src.Role_Web.name))
                .ForMember(des => des.default_rolename_web, opt => opt.MapFrom(src => src.ac_default_role_web))
                .ForMember(des => des.default_rolename_app, opt => opt.MapFrom(src => src.ac_default_role_app))
                //.ForMember(des => des.default_app_name, opt => opt.MapFrom(src => src.AppMaster.app_name))
                .ForMember(des => des.default_site_id, opt => opt.MapFrom(src => src.ac_default_site))
                .ForMember(des => des.default_site_name, opt => opt.MapFrom(src => src.Site.site_name))
                .ForMember(des => des.user_profile_image, opt => opt.MapFrom(src => src.profile_picture_name != null ? UrlGenerator.GetProfilePictureURL(src.profile_picture_name) : null))
                .ForMember(des => des.prefer_language_name, opt => opt.MapFrom(src =>  src.LanguageMaster.language_name))
                .ForMember(des => des.mobile_number, opt => opt.MapFrom(src =>  src.phone_number));

            CreateMap<User, FilterUsersOptimizedResponsemodel>()
                .ForMember(des => des.default_rolename_app_name, opt => opt.MapFrom(src => src.Role_App.name));

            CreateMap<GetUserDetailsResponseModel, User>();
            CreateMap<User, GetUserDetailsResponseModel>().ForMember(des => des.status_name, opt => opt.MapFrom(src => src.StatusMaster.status_name))
                .ForMember(des => des.role_name, opt => opt.MapFrom(src => src.Role_App.name))
                .ForMember(des => des.default_rolename_web_name, opt => opt.MapFrom(src => src.Role_Web.name))
                .ForMember(des => des.default_rolename_app, opt => opt.MapFrom(src => src.ac_default_role_app))
                .ForMember(des => des.default_rolename_web, opt => opt.MapFrom(src => src.ac_default_role_web))
                .ForMember(des => des.default_rolename_app_name, opt => opt.MapFrom(src => src.Role_App.name))
                .ForMember(des => des.prefer_language_id, opt => opt.MapFrom(src => src.LanguageMaster.language_id))
                .ForMember(des => des.prefer_language_name, opt => opt.MapFrom(src => src.LanguageMaster.language_name))
                .ForMember(des => des.default_site_id, opt => opt.MapFrom(src => src.ac_default_site))
                .ForMember(des => des.default_site_name, opt => opt.MapFrom(src => src.Site.site_name))
                //.ForMember(des => des.default_app_name, opt => opt.MapFrom(src => src.AppMaster.app_name))
                .ForMember(des => des.prefer_language_name, opt => opt.MapFrom(src => src.LanguageMaster.language_name));

            CreateMap<UserSitesViewModel, UserSites>();
            CreateMap<UserSites, UserSitesViewModel>()
                .ForMember(des => des.status_name, opt => opt.MapFrom(src => src.StatusMaster.status_name))
                .ForMember(des => des.company_id, opt => opt.MapFrom(src => src.Sites.company_id))
                .ForMember(des => des.comapny_name, opt => opt.MapFrom(src => src.Sites.Company.company_name))
                .ForMember(des => des.site_name, opt => opt.MapFrom(src => src.Sites.site_name))
                .ForMember(des => des.site_code, opt => opt.MapFrom(src => src.Sites.site_code))
                .ForMember(des => des.location, opt => opt.MapFrom(src => src.Sites.location))
                ;

            //CreateMap<UserSitesViewModel, Sites>();
            //CreateMap<Sites, UserSitesViewModel>().ForMember(des => des.status_name, opt => opt.MapFrom(src => src.StatusMaster.status_name));

            CreateMap<AssetsValueJsonObjectViewModel, FormAttributesJsonObject>();
            CreateMap<FormAttributesJsonObject, AssetsValueJsonObjectViewModel>().ForMember(des => des.id, opt => opt.MapFrom(src => src.attributes_id));

            CreateMap<GetRolesResponseModel, Role>();
            CreateMap<Role, GetRolesResponseModel>();

            CreateMap<MasterDataResponseModel, MasterData>();
            CreateMap<MasterData, MasterDataResponseModel>();

            CreateMap<IssueViewModel, Issue>();
            CreateMap<Issue, IssueViewModel>()
                    .ForMember(des => des.status_name, opt => opt.MapFrom(src => src.StatusMaster.status_name))
                    .ForMember(des => des.work_order_uuid, opt => opt.MapFrom(src => src.issue_uuid))
                    .ForMember(des => des.work_order_number, opt => opt.MapFrom(src => src.issue_number));

            CreateMap<DashboardOutstandingIssuesResponseModel, DashboardOutstandingIssues>();
            CreateMap<DashboardOutstandingIssues, DashboardOutstandingIssuesResponseModel>();

            CreateMap<ReportJsonDatas, ReportJsonData>();
            CreateMap<ReportJsonData, ReportJsonDatas>()
                .ForMember(des => des.time_elapsed, opt => opt.MapFrom(src => DateTimeUtil.GetBeforetimeText(src.not_ok_since.Value)));

            CreateMap<ReportJsonDatas, Issue>();
            CreateMap<Issue, ReportJsonDatas>()
                .ForMember(des => des.asset_name, opt => opt.MapFrom(src => src.Asset.name))
                .ForMember(des => des.attribute_name, opt => opt.MapFrom(src => src.Attributes.name))
                .ForMember(des => des.internal_asset_id, opt => opt.MapFrom(src => src.Asset.internal_asset_id))
                .ForMember(des => des.not_ok_since, opt => opt.MapFrom(src => src.created_at))
                .ForMember(des => des.site_id, opt => opt.MapFrom(src => src.Asset.site_id))
                .ForMember(des => des.site_name, opt => opt.MapFrom(src => src.Asset.Sites.site_name))
                .ForMember(des => des.time_elapsed, opt => opt.MapFrom(src => DateTimeUtil.GetBeforetimeText(src.requested_datetime)));

            CreateMap<AssetIssueResponseModel, Asset>();
            CreateMap<Asset, AssetIssueResponseModel>()
                .ForMember(des => des.status_name, opt => opt.MapFrom(src => src.StatusMaster.status_name));

            CreateMap<DeviceInfo, DeviceInfoRequestModel>();
            CreateMap<DeviceInfoRequestModel, DeviceInfo>()
                .ForMember(des => des.modified_by, opt => opt.MapFrom(src => src.requested_by))
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            CreateMap<RecordSyncInformation, InsertSyncRecordRequestModel>();
            CreateMap<InsertSyncRecordRequestModel, RecordSyncInformation>()
                .ForMember(des => des.created_by, opt => opt.MapFrom(src => src.requested_by));

            CreateMap<AssetDetailsViewModel, Asset>();
            CreateMap<Asset, AssetDetailsViewModel>()
                .ForMember(des => des.site_name, opt => opt.MapFrom(src => src.Sites.site_name))
                .ForMember(des => des.company_name, opt => opt.MapFrom(src => src.Sites.Company.company_name))
                .ForMember(des => des.company_code, opt => opt.MapFrom(src => src.Sites.Company.company_code))
                .ForMember(des => des.site_code, opt => opt.MapFrom(src => src.Sites.site_code))
                .ForMember(des => des.asset_photo, opt => opt.MapFrom(src => src.asset_photo != null ? UrlGenerator.GetAssetImagesURL(src.asset_photo) : null))
                .ForMember(des => des.status_name, opt => opt.MapFrom(src => src.StatusMaster.status_name))
                .ForMember(des => des.timezone, opt => opt.MapFrom(src => src.Sites.timezone))
                .ForMember(des => des.timeelapsed, opt => opt.MapFrom(src => DateTimeUtil.GetBeforetimeText(src.created_at.Value)));

            CreateMap<InspectionFormDataViewModel, InspectionForms>();
            CreateMap<InspectionForms, InspectionFormDataViewModel>()
                .ForMember(des => des.status_name, opt => opt.MapFrom(src => src.StatusMaster == null ? null : src.StatusMaster.status_name));

            CreateMap<InspectionRequestModel, OfflineInspectionRequestModel>();
            CreateMap<OfflineInspectionRequestModel, InspectionRequestModel>()
                .ForMember(des => des.created_by, opt => opt.MapFrom(src => src.user_id))
                .ForMember(des => des.datetime_requested, opt => opt.MapFrom(src => src.requested_datetime))
                .ForMember(des => des.operator_id, opt => opt.MapFrom(src => src.user_id))
                .ForMember(des => des.modified_at, opt => opt.MapFrom(src => src.created_at));

            CreateMap<UpdateIssueByMaintenanceRequestModel, OfflineIssueRequestModel>();
            CreateMap<OfflineIssueRequestModel, UpdateIssueByMaintenanceRequestModel>();

            CreateMap<AssetInspectionReportResponseModel, AssetInspectionReport>();
            CreateMap<AssetInspectionReport, AssetInspectionReportResponseModel>()
                .ForMember(des => des.asset_name, opt => opt.MapFrom(src => src.Asset.name))
                .ForMember(des => des.status_name, opt => opt.MapFrom(src => src.StatusMaster.status_name));


            CreateMap<AssetListResponseModel, Asset>();
            CreateMap<Asset, AssetListResponseModel>();

            CreateMap<DeviceInfoViewModel, DeviceInfo>();
            CreateMap<DeviceInfo, DeviceInfoViewModel>();

            CreateMap<AssetActivityLogsViewModel, AssetActivityLogs>();
            CreateMap<AssetActivityLogs, AssetActivityLogsViewModel>()
                .ForMember(des => des.timezone, opt => opt.MapFrom(src => src.Asset.Sites.timezone));

            #region PMCategory

            CreateMap<PMCategoryRequestModel, PMCategory>();
            CreateMap<PMCategory, PMCategoryRequestModel>();

            CreateMap<PMCategoryResponseModel, PMCategory>();
            CreateMap<PMCategory, PMCategoryResponseModel>()
                .ForMember(des => des.pmPlansCount, opt => opt.MapFrom(src => src.PMPlans != null ? UpdatedGenericRequestmodel.CurrentUser.role_id == GlobalConstants.SuperAdmin_Role_id ? src.PMPlans.Where(x => x.status == (int)Status.Active).ToList().Count : src.PMPlans.Where(x => x.status == (int)Status.Active && x.plan_name != "70B-STANDARD").ToList().Count : 0));

            CreateMap<PMPlansRequestModel, PMPlans>();
            CreateMap<PMPlans, PMPlansRequestModel>();

            CreateMap<PMPlansResponseModel, PMPlans>();
            CreateMap<PMPlans, PMPlansResponseModel>()
                .ForMember(des => des.pmCount, opt => opt.MapFrom(src => src.PMs != null ? src.PMs.Where(x => !x.is_archive).ToList().Count : 0));

            CreateMap<TaskRequestModel, Tasks>();
            CreateMap<Tasks, TaskRequestModel>();

            CreateMap<TaskResponseModel, Tasks>();
            CreateMap<Tasks, TaskResponseModel>();

            CreateMap<PMResponseModel, PMs>();
            CreateMap<PMs, PMResponseModel>()
                .ForMember(des => des.status_name, opt => opt.MapFrom(src => src.StatusMaster != null ? src.StatusMaster.status_name : null))
                .ForMember(des => des.pm_type_status_name, opt => opt.MapFrom(src => src.PMTypeStatus != null ? src.PMTypeStatus.status_name : null))
                .ForMember(des => des.pm_by_status_name, opt => opt.MapFrom(src => src.PMByStatus != null ? src.PMByStatus.status_name : null))
                .ForMember(des => des.pm_datetime_repeat_type_status_name, opt => opt.MapFrom(src => src.PMDateTimeRepeatTypeStatus != null ? src.PMDateTimeRepeatTypeStatus.status_name : null))
                .ForPath(des => des.pm_trigger_condition_mapping_response_model, opt => opt.MapFrom(src => src.PMsTriggerConditionMapping))
                .ForPath(des => des.pm_attachments, opt => opt.MapFrom(src => src.PMAttachments))
                ;
            CreateMap<PMsTriggerConditionMapping, PMsTriggerConditionMappingResponsemodel>();


            CreateMap<PMs, GetAllPMsByPlanIdResponsemodel>()
                 .ForPath(des => des.pm_trigger_condition_mapping_response_model, opt => opt.MapFrom(src => src.PMsTriggerConditionMapping));

            CreateMap<PMsTriggerConditionMapping, PMConditionmapping>();

            CreateMap<AddPMRequestModel, PMs>()
                .ForPath(des => des.PMsTriggerConditionMapping, opt => opt.MapFrom(src => src.pm_trigger_condition_mapping_request_model))
                .ForPath(des => des.PMAttachments, opt => opt.MapFrom(src => src.pm_attachments))
                ;
            CreateMap<PMsTriggerConditionMappingRequestmodel, PMsTriggerConditionMapping>();
            CreateMap<PMs, AddPMRequestModel>();

            CreateMap<PMTasksRequestModel, PMTasks>();
            CreateMap<PMTasks, PMTasksRequestModel>();

            CreateMap<PMTasksResponseModel, PMTasks>();
            CreateMap<PMTasks, PMTasksResponseModel>();

            CreateMap<TaskResponseModel, Tasks>();
            CreateMap<Tasks, TaskResponseModel>()
                .ForPath(des => des.AssetTasks, opt => opt.MapFrom(src => src.AssetTasks))
                .ForPath(des => des.form_name, opt => opt.MapFrom(src => src.FormIO.form_name))
                .ForPath(des => des.form_type, opt => opt.MapFrom(src => src.FormIO.FormIOType.form_type_name))
                .ForPath(des => des.work_procedure, opt => opt.MapFrom(src => src.FormIO.work_procedure))
                ;
            CreateMap<Tasks, MobileTaskResponseModel>()
                //  .ForPath(des => des.AssetTasks, opt => opt.MapFrom(src => src.AssetTasks))
                .ForPath(des => des.form_name, opt => opt.MapFrom(src => src.FormIO.form_name))
                .ForPath(des => des.form_type, opt => opt.MapFrom(src => src.FormIO.FormIOType.form_type_name))
                //  .ForPath(des => des.work_procedure, opt => opt.MapFrom(src => src.FormIO.work_procedure))
                ;



            CreateMap<AssetPMPlans, PMPlans>();
            CreateMap<PMPlans, AssetPMPlans>();

            CreateMap<PMPlansResponseModel, AssetPMPlans>();
            CreateMap<AssetPMPlans, PMPlansResponseModel>();
            //.ForMember(des => des.AssetPMs, opt => opt.MapFrom(src => src.PMs));

            CreateMap<AssetPMs, PMs>();
            CreateMap<PMs, AssetPMs>()
                .ForMember(des => des.AssetPMTasks, opt => opt.MapFrom(src => src.PMTasks))
                .ForMember(des => des.AssetPMAttachments, opt => opt.MapFrom(src => src.PMAttachments))
                .ForMember(des => des.AssetPMsTriggerConditionMapping, opt => opt.MapFrom(src => src.PMsTriggerConditionMapping))
                ;

            CreateMap<PMsTriggerConditionMapping, AssetPMsTriggerConditionMapping>();

            CreateMap<AssetPMTasks, PMTasks>();
            CreateMap<PMTasks, AssetPMTasks>();

            CreateMap<AssetPMResponseModel, AssetPMs>();
            CreateMap<AssetPMs, AssetPMResponseModel>()
                .ForMember(des => des.status_name, opt => opt.MapFrom(src => src.StatusMaster != null ? src.StatusMaster.status_name : null))
                .ForMember(des => des.pm_type_status_name, opt => opt.MapFrom(src => src.PMTypeStatus != null ? src.PMTypeStatus.status_name : null))
                .ForMember(des => des.pm_by_status_name, opt => opt.MapFrom(src => src.PMByStatus != null ? src.PMByStatus.status_name : null))
                .ForMember(des => des.pm_datetime_repeat_type_status_name, opt => opt.MapFrom(src => src.PMDateTimeRepeatTypeStatus != null ? src.PMDateTimeRepeatTypeStatus.status_name : null))
                .ForMember(des => des.asset_pm_plan_name, opt => opt.MapFrom(src => src.AssetPMPlans != null ? src.AssetPMPlans.plan_name : null))
                .ForMember(des => des.asset_pm_trigger_condition_mapping, opt => opt.MapFrom(src => src.AssetPMsTriggerConditionMapping))
                .ForMember(des => des.asset_pm_attachments, opt => opt.MapFrom(src => src.AssetPMAttachments))
                ;

            CreateMap<AssetPMsTriggerConditionMapping, AssetPMsTriggerConditionMappingResponsemodel>();

            CreateMap<UpdateAssetPMRequestModel, AssetPMs>();
            CreateMap<AssetPMs, UpdateAssetPMRequestModel>();

            CreateMap<AssetPMTasksRequestModel, AssetPMTasks>();
            CreateMap<AssetPMTasks, AssetPMTasksRequestModel>();

            CreateMap<AssetPMTasksResponseModel, AssetPMTasks>();
            CreateMap<AssetPMTasks, AssetPMTasksResponseModel>();

            CreateMap<PMTriggers, PMTriggersResponseModel>();
            CreateMap<PMTriggersResponseModel, PMTriggers>();

            CreateMap<PMTriggersTasks, PMTriggersTasksResponseModel>();
            CreateMap<PMTriggersTasksResponseModel, PMTriggersTasks>();

            CreateMap<PMTriggersRemarks, PMMarkCompletedRequestModel>();
            CreateMap<PMMarkCompletedRequestModel, PMTriggersRemarks>();

            CreateMap<CompanyPMNotificationRequestModel, CompanyPMNotificationConfigurations>();
            CreateMap<CompanyPMNotificationConfigurations, CompanyPMNotificationRequestModel>();

            CreateMap<CompanyPMNotificationResponseModel, CompanyPMNotificationConfigurations>();
            CreateMap<CompanyPMNotificationConfigurations, CompanyPMNotificationResponseModel>();

            CreateMap<AssetPMNotificationRequestModel, AssetPMNotificationConfigurations>();
            CreateMap<AssetPMNotificationConfigurations, AssetPMNotificationRequestModel>();

            CreateMap<AssetPMNotificationResponseModel, AssetPMNotificationConfigurations>();
            CreateMap<AssetPMNotificationConfigurations, AssetPMNotificationResponseModel>();

            CreateMap<AssetPMNotificationResponseModel, CompanyPMNotificationConfigurations>();
            CreateMap<CompanyPMNotificationConfigurations, AssetPMNotificationResponseModel>();

            CreateMap<PMTriggers, DashboardPendingPMItems>()
                .ForMember(des => des.title, opt => opt.MapFrom(src => src.AssetPMs != null ? src.AssetPMs.title : null))
                .ForMember(des => des.internal_asset_id, opt => opt.MapFrom(src => src.Asset != null ? src.Asset.internal_asset_id : null))
                .ForMember(des => des.asset_name, opt => opt.MapFrom(src => src.Asset != null ? src.Asset.name : null))
                .ForMember(des => des.pm_plan_name, opt => opt.MapFrom(src => src.AssetPMs != null && src.AssetPMs.AssetPMPlans != null ? src.AssetPMs.AssetPMPlans.plan_name : null))
                .ForMember(des => des.site_name, opt => opt.MapFrom(src => src.Asset != null && src.Asset.Sites != null && src.Asset.Sites.site_name != null ? src.Asset.Sites.site_name : null))
                .ForMember(des => des.due_in, opt => opt.MapFrom(src => src.due_datetime != null ? DateTimeUtil.GetDueIn(src.due_datetime.Value) : null))
                .ForMember(des => des.due_in, opt => opt.MapFrom(src => src.PMTriggersRemarks != null ? DateTimeUtil.DateFormatToShortDate(src.PMTriggersRemarks.completed_on) : src.due_datetime != null ? DateTimeUtil.GetDueIn(src.due_datetime.Value) : null))
                .ForMember(des => des.asset_pm_status_name, opt => opt.MapFrom(src => src.AssetPMStatusMaster != null ? src.AssetPMStatusMaster.status_name : null))
                .ForMember(des => des.status_name, opt => opt.MapFrom(src => src.StatusMaster != null ? src.StatusMaster.status_name : null));

            CreateMap<PMTriggers, PMDueReportEmailResponse>()
                .ForMember(des => des.pm_title, opt => opt.MapFrom(src => src.AssetPMs != null ? src.AssetPMs.title : null))
                .ForMember(des => des.status, opt => opt.MapFrom(src => src.StatusMaster != null ? src.StatusMaster.status_name : null))
                .ForMember(des => des.internal_asset_id, opt => opt.MapFrom(src => src.Asset != null ? src.Asset.internal_asset_id : null))
                .ForMember(des => des.asset_name, opt => opt.MapFrom(src => src.Asset != null ? src.Asset.name : null))
                .ForMember(des => des.pm_plan_name, opt => opt.MapFrom(src => src.AssetPMs != null && src.AssetPMs.AssetPMPlans != null ? src.AssetPMs.AssetPMPlans.plan_name : null))
                .ForMember(des => des.site_name, opt => opt.MapFrom(src => src.Asset != null && src.Asset.Sites != null && src.Asset.Sites.site_name != null ? src.Asset.Sites.site_name : null))
                .ForMember(des => des.time_elapsed, opt => opt.MapFrom(src => src.due_datetime != null ? DateTimeUtil.GetDueIn(src.due_datetime.Value) : src.datetime_when_pm_due != null ? DateTimeUtil.GetDueIn(src.datetime_when_pm_due.Value) : null))
                .ForMember(des => des.meters_at_due, opt => opt.MapFrom(src => src.meter_hours_when_pm_due))
                .ForMember(des => des.current_meters, opt => opt.MapFrom(src => src.Asset != null ? src.Asset.meter_hours : null))
                .ForMember(des => des.Total_meters_run_after_PM_overdue, opt => opt.MapFrom(src => (src.Asset.meter_hours - src.meter_hours_when_pm_due)))
                .ForMember(des => des.service_dealer_id, opt => opt.MapFrom(src => src.AssetPMs.ServiceDealers != null ? src.AssetPMs.ServiceDealers.service_dealer_id : Guid.Empty))
                .ForMember(des => des.service_dealer_name, opt => opt.MapFrom(src => src.AssetPMs != null && src.AssetPMs.ServiceDealers != null ? src.AssetPMs.ServiceDealers.name : null))
                .ForMember(des => des.service_dealer_email, opt => opt.MapFrom(src => src.AssetPMs != null && src.AssetPMs.ServiceDealers != null ? src.AssetPMs.ServiceDealers.email : null));
            //.ForMember(des => des.due_in, opt => opt.MapFrom(src => src.due_datetime != null ? src.due_datetime.Value.Date.ToShortDateString() : src.due_meter_hours != null ? src.due_meter_hours.ToString() + "Meter Hours" : null));

            CreateMap<UserEmailNotificationConfigurationSettings, ExecutivePMDueEmailConfigRequestModel>();
            CreateMap<ExecutivePMDueEmailConfigRequestModel, UserEmailNotificationConfigurationSettings>();

            CreateMap<UserEmailNotificationConfigurationSettings, ExecutivePMDueEmailConfigResponseModel>();
            CreateMap<ExecutivePMDueEmailConfigResponseModel, UserEmailNotificationConfigurationSettings>();

            #endregion

            #region Asset Type

            CreateMap<AssetTypeRequestModel, AssetType>();
            CreateMap<AssetType, AssetTypeRequestModel>();

            CreateMap<AssetTypeResponseModel, AssetType>();
            CreateMap<AssetType, AssetTypeResponseModel>();

            #endregion

            #region Upload PM Attachment

            CreateMap<AssetPMAttachments, PMAttachments>();
            CreateMap<PMAttachments, AssetPMAttachments>();

            CreateMap<PMAttachmentsRequestModel, PMAttachments>();
            CreateMap<PMAttachments, PMAttachmentsRequestModel>();

            CreateMap<PMAttachmentsResponseModel, PMAttachments>();
            CreateMap<PMAttachments, PMAttachmentsResponseModel>();

            CreateMap<AssetPMAttachmentsRequestModel, AssetPMAttachments>();
            CreateMap<AssetPMAttachments, AssetPMAttachmentsRequestModel>();

            CreateMap<AssetPMAttachmentsResponseModel, AssetPMAttachments>();
            CreateMap<AssetPMAttachments, AssetPMAttachmentsResponseModel>();

            CreateMap<PMAttachmentsResponseModel, PMAttachments>();
            CreateMap<PMAttachments, PMAttachmentsResponseModel>()
                .ForMember(des => des.file_url, opt => opt.MapFrom(src => src.filename != null ? UrlGenerator.GetPMAttachmentURL(src.filename) : null));

            CreateMap<AssetPMAttachmentsResponseModel, AssetPMAttachments>();
            CreateMap<AssetPMAttachments, AssetPMAttachmentsResponseModel>()
                .ForMember(des => des.file_url, opt => opt.MapFrom(src => src.filename != null ? UrlGenerator.GetPMAttachmentURL(src.filename) : null));

            #endregion

            CreateMap<ServiceDealerViewModel, ServiceDealers>();
            CreateMap<ServiceDealers, ServiceDealerViewModel>()
                .ForMember(des => des.status_name, opt => opt.MapFrom(src => src.StatusMaster != null ? src.StatusMaster.status_name : null));


            CreateMap<VendorEmailExcelDetails, ToSendVendorEmailExcelDetails>();
            CreateMap<ToSendVendorEmailExcelDetails, VendorEmailExcelDetails>();


            #region Maintenance Request

            CreateMap<MRResponseModel, MaintenanceRequests>();
            CreateMap<MaintenanceRequests, MRResponseModel>()
                .ForMember(des => des.status_name, opt => opt.MapFrom(src => src.StatusMaster.status_name))
                .ForMember(des => des.mr_type_name, opt => opt.MapFrom(src => src.MRTypeStatusMaster != null ? src.MRTypeStatusMaster.status_name : null))
                .ForMember(des => des.priority_name, opt => opt.MapFrom(src => src.PriorityStatusMaster != null ? src.PriorityStatusMaster.status_name : null))
                .ForMember(des => des.time_elapsed, opt => opt.MapFrom(src => DateTimeUtil.GetBeforetimeText(src.created_at.Value)));

            CreateMap<AddMRRequestModel, MaintenanceRequests>();
            CreateMap<MaintenanceRequests, AddMRRequestModel>();

            CreateMap<MRCancelRequestModel, MaintenanceReqCancelRequests>();
            CreateMap<MaintenanceReqCancelRequests, MRCancelRequestModel>();

            CreateMap<WorkOrderMRResponseModel, MaintenanceRequests>();
            CreateMap<MaintenanceRequests, WorkOrderMRResponseModel>()
                .ForMember(des => des.status_name, opt => opt.MapFrom(src => src.StatusMaster.status_name))
                .ForMember(des => des.mr_type_name, opt => opt.MapFrom(src => src.MRTypeStatusMaster != null ? src.MRTypeStatusMaster.status_name : null))
                .ForMember(des => des.priority_name, opt => opt.MapFrom(src => src.PriorityStatusMaster != null ? src.PriorityStatusMaster.status_name : null))
                .ForMember(des => des.time_elapsed, opt => opt.MapFrom(src => DateTimeUtil.GetBeforetimeText(src.created_at.Value)));
            #endregion

            #region Work Orders

            CreateMap<WorkOrderResponseModel, WorkOrders>();
            CreateMap<WorkOrders, WorkOrderResponseModel>()
                .ForMember(des => des.status_name, opt => opt.MapFrom(src => src.StatusMaster != null ? src.StatusMaster.status_name : null))
                .ForMember(des => des.wo_type_name, opt => opt.MapFrom(src => src.WOTypeStatusMaster != null ? src.WOTypeStatusMaster.status_name : null))
                .ForMember(des => des.priority_name, opt => opt.MapFrom(src => src.PriorityStatusMaster != null ? src.PriorityStatusMaster.status_name : null));

            CreateMap<AddWorkOrderRequestModel, WorkOrders>();
            CreateMap<WorkOrders, AddWorkOrderRequestModel>();

            CreateMap<WorkOrderTasksRequestModel, WorkOrderTasks>();
            CreateMap<WorkOrderTasks, WorkOrderTasksRequestModel>();

            CreateMap<WorkOrderTasksResponseModel, WorkOrderTasks>();
            CreateMap<WorkOrderTasks, WorkOrderTasksResponseModel>();

            CreateMap<WorkOrderAttachmentsRequestModel, WorkOrderAttachments>();
            CreateMap<WorkOrderAttachments, WorkOrderAttachmentsRequestModel>();

            CreateMap<WorkOrderAttachmentsResponseModel, WorkOrderAttachments>();
            CreateMap<WorkOrderAttachments, WorkOrderAttachmentsResponseModel>()
                .ForMember(des => des.file_url, opt => opt.MapFrom(src => src.filename != null ? UrlGenerator.GetWorkOrderAttachmentURL(src.filename) : null));

            CreateMap<WorkOrderAttachments, MobileWorkOrderAttachmentsResponseModel>()
                .ForMember(des => des.file_url, opt => opt.MapFrom(src => src.filename != null ? UrlGenerator.GetWorkOrderAttachmentURL(src.filename) : null));

            #endregion

            #region
            CreateMap<AssetMeterHourHistoryResponseModel, AssetMeterHourHistory>();
            CreateMap<AssetMeterHourHistory, AssetMeterHourHistoryResponseModel>()
                .ForMember(des => des.timezone, opt => opt.MapFrom(src => src.Asset.Sites.timezone));
            #endregion

            #region FormIO

            CreateMap<FormIOResponseModel, InspectionsTemplateFormIO>();
            CreateMap<InspectionsTemplateFormIO, FormIOResponseModel>()
                .ForMember(des => des.form_type, opt => opt.MapFrom(src => src.FormIOType.form_type_name))
                .ForMember(des => des.status_name, opt => opt.MapFrom(src => src.StatusMaster.status_name))
                ;

            CreateMap<FormIOFormsExcludedProprties, GetAllFormIOFormResponsemodel>()
                .ForMember(des => des.form_type, opt => opt.MapFrom(src => src.form_type))
                ;

            CreateMap<InspectionsTemplateFormIO, MobileFormIOResponseModel>()
               .ForMember(des => des.form_type, opt => opt.MapFrom(src => src.FormIOType.form_type_name))
               .ForMember(des => des.status_name, opt => opt.MapFrom(src => src.StatusMaster.status_name))
               ;
            CreateMap<AddFormIORequestModel, InspectionsTemplateFormIO>();
            CreateMap<InspectionsTemplateFormIO, AddFormIORequestModel>();

            CreateMap<AssetFormIOResponseModel, AssetFormIO>();
            CreateMap<AssetFormIO, AssetFormIOResponseModel>()
                .ForMember(des => des.timezone, opt => opt.MapFrom(src => src.WorkOrders.Sites.timezone))
                .ForMember(des => des.asset_name, opt => opt.MapFrom(src => src.form_retrived_asset_name))
                .ForMember(des => des.pdf_report_status_id, opt => opt.MapFrom(src => src.pdf_report_status))
                .ForMember(des => des.pdf_report_status_name, opt => opt.MapFrom(src => src.PDFReportStatusMaster.status_name))
                .ForMember(des => des.pdf_report_url, opt => opt.MapFrom(src => src.pdf_report_url))
                .ForMember(des => des.status_name, opt => opt.MapFrom(src => src.StatusMaster.status_name))
                .ForMember(des => des.inspection_verdict_name, opt => opt.MapFrom(src => src.inspection_verdict != null ? ((inspectionVerdictdropdownname)src.inspection_verdict).ToString() : null))
                .ForMember(des => des.wo_number, opt => opt.MapFrom(src => src.WorkOrders.wo_number))
                .ForMember(des => des.manual_wo_number, opt => opt.MapFrom(src => src.WorkOrders.manual_wo_number))
                ;

            CreateMap<AssetFormIO, MobileAssetFormIOResponseModel>()
                 .ForMember(des => des.timezone, opt => opt.MapFrom(src => src.Sites.timezone))
               ;

            CreateMap<AssetFormIO, GetAssetFormIOByAssetFormIdResponsemodel>()
                .ForMember(des => des.task_rejected_notes, opt => opt.MapFrom(src => src.WOcategorytoTaskMapping.task_rejected_notes))
                .ForMember(des => des.wo_type, opt => opt.MapFrom(src => src.WorkOrders.wo_type))
                .ForMember(des => des.wo_status_id, opt => opt.MapFrom(src => src.WorkOrders.status))
                //.ForMember(des => des.task_rejected_notes, opt => opt.MapFrom(src => src.WOcategorytoTaskMapping.task_rejected_notes))

                ;

            CreateMap<WorkOrders, WorkOrder>();

            CreateMap<AssetFormIORequestModel, AssetFormIO>();
            CreateMap<AssetFormIO, AssetFormIORequestModel>();

            CreateMap<AssetTasksRequestModel, AssetTasks>();
            CreateMap<AssetTasks, AssetTasksRequestModel>();

            CreateMap<AssetTasksResponseModel, AssetTasks>();
            CreateMap<AssetTasks, AssetTasksResponseModel>()
                .ForPath(des => des.asset_name, opt => opt.MapFrom(src => src.Asset.name))
                .ForPath(des => des.internal_asset_id, opt => opt.MapFrom(src => src.Asset.internal_asset_id))
                ;

            #endregion FormIO

            #region Form IO Type

            CreateMap<FormTypeResponseModel, FormIOType>();
            CreateMap<FormIOType, FormTypeResponseModel>();

            #endregion

            #region client_company
            CreateMap<ClientCompany, Client_Company>()
                .ForPath(des => des.client_company_Usersites, opt => opt.MapFrom(src => src.Sites));

            CreateMap<Sites, UserSite>()
                .ForPath(des => des.company_name, opt => opt.MapFrom(src => src.ClientCompany.client_company_name))
                .ForPath(des => des.company_id, opt => opt.MapFrom(src => src.ClientCompany.client_company_id))
                ;
            ;
            #endregion client_company

            CreateMap<Asset, GetAllHierarchyAssetsResponseModel>()
                .ForMember(des => des.site_id, opt => opt.MapFrom(src => src.site_id))
                .ForMember(des => des.condition_index_type_name, opt => opt.MapFrom(src => src.condition_index_type != null ? ((condition_index_type)src.condition_index_type.Value).ToString() : null))
                .ForMember(des => des.site_name, opt => opt.MapFrom(src => src.Sites.site_name))
                .ForMember(des => des.parent_internal_asset_id, opt => opt.MapFrom(src => src.parent))
                .ForMember(des => des.site_code, opt => opt.MapFrom(src => src.Sites.site_code))
                ;

            CreateMap<WorkOrders, NewFlowWorkorderListResponseModel>()
                .ForMember(des => des.wo_number, opt => opt.MapFrom(src => src.wo_number.ToString()))
                .ForMember(des => des.manual_wo_number, opt => opt.MapFrom(src => src.manual_wo_number))
                .ForMember(des => des.site_name, opt => opt.MapFrom(src => src.Sites.site_name))
                .ForMember(des => des.wo_type_name, opt => opt.MapFrom(src => src.WOTypeStatusMaster.status_name.Replace("WO", "")))
                .ForMember(des => des.wo_status, opt => opt.MapFrom(src => src.StatusMaster.status_name))
                .ForMember(des => des.quote_status_name, opt => opt.MapFrom(src => src.QuoteStatusMaster.status_name))
                .ForMember(des => des.wo_status_id, opt => opt.MapFrom(src => src.status))
                .ForMember(des => des.quote_status_id, opt => opt.MapFrom(src => src.quote_status))
                .ForMember(des => des.timezone, opt => opt.MapFrom(src => src.Sites.timezone))
                .ForMember(des => des.client_company_id, opt => opt.MapFrom(src => src.Sites.client_company_id))
                .ForMember(des => des.client_company_name, opt => opt.MapFrom(src => src.Sites.ClientCompany.client_company_name))
                .ForMember(des => des.wo_status_id, opt => opt.MapFrom(src => src.status))
                .ForMember(des => des.due_date, opt => opt.MapFrom(src => src.due_at))
                .ForMember(des => des.due_in, opt => opt.MapFrom(src => src.wo_due_time_duration))
                .ForMember(des => des.ir_wo_pdf_report, opt => opt.MapFrom(src => UrlGenerator.GetOBAssetPdfUrl(src.ir_wo_pdf_report, src.wo_id.ToString())))
                //.ForMember(des => des.technician_id, opt => opt.MapFrom(src => src.technician_user_id))
                //.ForMember(des => des.technician_name, opt => opt.MapFrom(src => (src.TechnicianUser.firstname + src.TechnicianUser.lastname)))
                .ForMember(des => des.WorkOrderAttachments_list, opt => opt.MapFrom(src => src.WorkOrderAttachments))
                ;

            CreateMap<WorkOrderAttachments, WorkOrderAttachments_data>()
                 .ForMember(dest => dest.wo_attachment_id, opt => opt.MapFrom(src => src.wo_attachment_id))
                 .ForMember(dest => dest.filename, opt => opt.MapFrom(src => src.filename))
                 .ForMember(dest => dest.user_uploaded_name, opt => opt.MapFrom(src => src.user_uploaded_name));

            CreateMap<WorkOrders, MobileNewFlowWorkorderListResponseModel>()
                .ForMember(des => des.wo_number, opt => opt.MapFrom(src => src.wo_number.ToString()))
                .ForMember(des => des.manual_wo_number, opt => opt.MapFrom(src => src.manual_wo_number))
                .ForMember(des => des.site_name, opt => opt.MapFrom(src => src.Sites.site_name))
                .ForMember(des => des.wo_type_name, opt => opt.MapFrom(src => src.WOTypeStatusMaster.status_name))
                .ForMember(des => des.wo_status, opt => opt.MapFrom(src => src.StatusMaster.status_name))
                .ForMember(des => des.wo_status_id, opt => opt.MapFrom(src => src.status))
                .ForMember(des => des.timezone, opt => opt.MapFrom(src => src.Sites.timezone))
                //.ForMember(des => des.client_company_id, opt => opt.MapFrom(src => src.Sites.client_company_id))
                //.ForMember(des => des.client_company_name, opt => opt.MapFrom(src => src.Sites.ClientCompany.client_company_name))

                ;

            CreateMap<InspectionsTemplateFormIO, GetAllCatagoryForWOResponseModel>()
                .ForMember(des => des.form_category_name, opt => opt.MapFrom(src => src.FormIOType.form_type_name))
                .ForMember(des => des.form_description, opt => opt.MapFrom(src => src.form_description))
                .ForMember(des => des.form_id, opt => opt.MapFrom(src => src.form_id));

            CreateMap<WorkOrders, ViewWorkOrderDetailsByIdResponsemodel>()
               .ForMember(des => des.wo_number, opt => opt.MapFrom(src => src.wo_number.ToString()))
               .ForMember(des => des.description, opt => opt.MapFrom(src => src.description))
               .ForMember(des => des.site_name, opt => opt.MapFrom(src => src.Sites.site_name))
               .ForMember(des => des.wo_type_name, opt => opt.MapFrom(src => src.WOTypeStatusMaster.status_name))
               .ForMember(des => des.wo_status, opt => opt.MapFrom(src => src.StatusMaster.status_name))
               .ForMember(des => des.quote_status_name, opt => opt.MapFrom(src => src.QuoteStatusMaster.status_name))
               .ForMember(des => des.wo_status_id, opt => opt.MapFrom(src => src.status))
               .ForMember(des => des.quote_status_id, opt => opt.MapFrom(src => src.quote_status))
            .ForMember(des => des.ir_wo_pdf_report, opt => opt.MapFrom(src => UrlGenerator.GetOBAssetPdfUrl(src.ir_wo_pdf_report, src.wo_id.ToString())))
            // .ForMember(des => des.technician_id, opt => opt.MapFrom(src => src.technician_user_id))
               .ForMember(des => des.client_company_name, opt => opt.MapFrom(src => src.ClientCompany.client_company_name))
                              //  .ForMember(des => des.technician_name, opt => opt.MapFrom(src => (src.TechnicianUser.firstname + src.TechnicianUser.lastname)))
              .ForMember(des => des.due_date, opt => opt.MapFrom(src => src.due_at == DateTime.MinValue ? (DateTime?)null : src.due_at))
              .ForMember(des => des.due_in, opt => opt.MapFrom(src => src.wo_due_time_duration))
              .ForMember(des => des.technician_mapping_list, opt => opt.MapFrom(src => src.WorkOrderTechnicianMapping.Where(x=>!x.is_deleted)))
              .ForMember(des => des.backoffice_mapping_list, opt => opt.MapFrom(src => src.WorkOrderBackOfficeUserMapping.Where(x=>!x.is_deleted)))
              .ForMember(des => des.responsible_party_name, opt => opt.MapFrom(src => src.ResponsibleParty!=null ? src.ResponsibleParty.responsible_party_name : null))
              ;
            CreateMap<WorkOrderTechnicianMapping, WorkOrder_TechnicianUser_Details_Class>()
               .ForMember(des => des.firstname, opt => opt.MapFrom(src => src.TechnicianUser.firstname))
               .ForMember(des => des.lastname, opt => opt.MapFrom(src => src.TechnicianUser.lastname))
                ;
            CreateMap<WorkOrderBackOfficeUserMapping, WorkOrder_BOUser_Details_Class>()
               .ForMember(des => des.firstname, opt => opt.MapFrom(src => src.BackOfficeUser.firstname))
               .ForMember(des => des.lastname, opt => opt.MapFrom(src => src.BackOfficeUser.lastname))
                ;
            CreateMap<WorkOrders, ViewOBWODetailsByIdResponsemodel>()
              .ForMember(des => des.wo_number, opt => opt.MapFrom(src => src.wo_number.ToString()))
              .ForMember(des => des.description, opt => opt.MapFrom(src => src.description))
              .ForMember(des => des.site_name, opt => opt.MapFrom(src => src.Sites.site_name))
              .ForMember(des => des.wo_type_name, opt => opt.MapFrom(src => src.WOTypeStatusMaster.status_name))
              .ForMember(des => des.wo_status, opt => opt.MapFrom(src => src.StatusMaster.status_name))
              .ForMember(des => des.quote_status_name, opt => opt.MapFrom(src => src.QuoteStatusMaster.status_name))
              .ForMember(des => des.wo_status_id, opt => opt.MapFrom(src => src.status))
              .ForMember(des => des.quote_status_id, opt => opt.MapFrom(src => src.quote_status))
              .ForMember(des => des.due_date, opt => opt.MapFrom(src => src.due_at == DateTime.MinValue ? (DateTime?)null : src.due_at))
              .ForMember(des => des.due_in, opt => opt.MapFrom(src => src.wo_due_time_duration))
              .ForMember(des => des.client_company_name, opt => opt.MapFrom(src => src.ClientCompany.client_company_name))
              .ForMember(des => des.ir_wo_pdf_report, opt => opt.MapFrom(src => UrlGenerator.GetOBAssetPdfUrl(src.ir_wo_pdf_report, src.wo_id.ToString())))
              .ForMember(des => des.asset_details, opt => opt.MapFrom(src => src.WOOnboardingAssets.OrderBy(x=>x.building).ThenBy(x=>x.floor).ThenBy(x=>x.room)))
              .ForMember(des => des.asset_details_v2, opt => opt.MapFrom(src => src.WOOnboardingAssets.OrderBy(x => x.building).ThenBy(x => x.floor).ThenBy(x => x.room)))
              .ForMember(des => des.responsible_party_name, opt => opt.MapFrom(src => src.ResponsibleParty != null ? src.ResponsibleParty.responsible_party_name : null))
              .ForMember(des => des.technician_mapping_list, opt => opt.MapFrom(src => src.WorkOrderTechnicianMapping.Where(x => !x.is_deleted)))
              .ForMember(des => des.backoffice_mapping_list, opt => opt.MapFrom(src => src.WorkOrderBackOfficeUserMapping.Where(x => !x.is_deleted)))
              ;

            /*CreateMap<WOOnboardingAssets, OBWOAssetDetails>()
                .ForMember(des => des.status_name, opt => opt.MapFrom(src => src.StatusMaster.status_name))
                .ForMember(des => des.temp_formiobuilding_id, opt => opt.MapFrom(src => src.WOOBAssetTempFormIOBuildingMapping.temp_formiobuilding_id))
                .ForMember(des => des.temp_formiofloor_id, opt => opt.MapFrom(src => src.WOOBAssetTempFormIOBuildingMapping.temp_formiofloor_id))
                .ForMember(des => des.temp_formioroom_id, opt => opt.MapFrom(src => src.WOOBAssetTempFormIOBuildingMapping.temp_formioroom_id))
                .ForMember(des => des.temp_formiosection_id, opt => opt.MapFrom(src => src.WOOBAssetTempFormIOBuildingMapping.temp_formiosection_id))
                .ForMember(des => des.building, opt => opt.MapFrom(src => src.WOOBAssetTempFormIOBuildingMapping.TempFormIOBuildings.temp_formio_building_name))
                .ForMember(des => des.floor, opt => opt.MapFrom(src => src.WOOBAssetTempFormIOBuildingMapping.TempFormIOFloors.temp_formio_floor_name))
                .ForMember(des => des.room, opt => opt.MapFrom(src => src.WOOBAssetTempFormIOBuildingMapping.TempFormIORooms.temp_formio_room_name))
                //.ForMember(des => des.section, opt => opt.MapFrom(src => src.WOOBAssetTempFormIOBuildingMapping.TempFormIOSections.temp_formio_section_name))
                .ForMember(des => des.section, opt => opt.MapFrom(src => !String.IsNullOrEmpty(src.section) ? src.section : src.WOOBAssetTempFormIOBuildingMapping != null ? src.WOOBAssetTempFormIOBuildingMapping.TempFormIOSections.temp_formio_section_name : ""))

                .ForMember(des => des.status_name, opt => opt.MapFrom(src => src.StatusMaster.status_name))
                
                .ForMember(des => des.asset_id, opt => opt.MapFrom(src => src.Asset.asset_id))
                .ForMember(des => des.site_name, opt => opt.MapFrom(src => src.Sites.site_name))
                .ForMember(des => des.asset_class_type, opt => opt.MapFrom(src => src.TempAsset.InspectionTemplateAssetClass.FormIOType.form_type_name))
                //.ForMember(des => des.asset_name, opt => opt.MapFrom(src => src.asset_id !=null ? src.Asset.name: src.asset_name))
                ;*/
            CreateMap<WOOnboardingAssets, OBWOAssetDetails>()
                //.ForMember(des => des.status_name, opt => opt.MapFrom(src => src.StatusMaster.status_name))
                .ForMember(des => des.temp_formiobuilding_id, opt => opt.MapFrom(src => src.TempAsset.temp_formiobuilding_id))
                .ForMember(des => des.temp_formiofloor_id, opt => opt.MapFrom(src => src.TempAsset.temp_formiofloor_id))
                .ForMember(des => des.temp_formioroom_id, opt => opt.MapFrom(src => src.TempAsset.temp_formioroom_id))
                .ForMember(des => des.temp_formiosection_id, opt => opt.MapFrom(src => src.TempAsset.temp_formiosection_id))
                .ForMember(des => des.building, opt => opt.MapFrom(src => src.TempAsset.TempFormIOBuildings.temp_formio_building_name))
                .ForMember(des => des.temp_master_building, opt => opt.MapFrom(src => src.TempAsset.TempMasterBuilding!=null?src.TempAsset.TempMasterBuilding.temp_master_building_name:src.building))
                .ForMember(des => des.floor, opt => opt.MapFrom(src => src.TempAsset.TempFormIOFloors.temp_formio_floor_name))
                .ForMember(des => des.temp_master_floor, opt => opt.MapFrom(src => src.TempAsset.TempMasterFloor!=null?src.TempAsset.TempMasterFloor.temp_master_floor_name:src.floor))
                .ForMember(des => des.room, opt => opt.MapFrom(src => src.TempAsset.TempFormIORooms.temp_formio_room_name))
                .ForMember(des => des.temp_master_room, opt => opt.MapFrom(src => src.TempAsset.TempMasterRoom!=null?src.TempAsset.TempMasterRoom.temp_master_room_name:src.room))
                .ForMember(des => des.section, opt => opt.MapFrom(src => !String.IsNullOrEmpty(src.section) ? src.section : src.TempAsset.TempFormIOSections.temp_formio_section_name))
                .ForMember(des => des.temp_master_section, opt => opt.MapFrom(src => src.TempAsset.temp_master_section))
                .ForMember(des => des.asset_id, opt => opt.MapFrom(src => src.TempAsset.asset_id))
                .ForMember(des => des.site_name, opt => opt.MapFrom(src => src.TempAsset.Sites.site_name))
                .ForMember(des => des.asset_class_type, opt => opt.MapFrom(src => src.TempAsset.InspectionTemplateAssetClass.FormIOType.form_type_name))
                .ForMember(des => des.asset_class_name, opt => opt.MapFrom(src => src.TempAsset.InspectionTemplateAssetClass.asset_class_name))
                .ForMember(des => des.asset_class_code, opt => opt.MapFrom(src => src.TempAsset.InspectionTemplateAssetClass.asset_class_code))
                .ForMember(des => des.inspectiontemplate_asset_class_id, opt => opt.MapFrom(src => src.TempAsset.inspectiontemplate_asset_class_id))
                .ForMember(des => des.QR_code, opt => opt.MapFrom(src => src.TempAsset.QR_code))
                .ForMember(des => des.asset_name, opt => opt.MapFrom(src => src.TempAsset!=null ? src.TempAsset.asset_name:src.asset_name))
                .ForMember(des => des.component_level_type_id, opt => opt.MapFrom(src => src.TempAsset.component_level_type_id))
                .ForMember(des => des.maintenance_index_type, opt => opt.MapFrom(src => src.TempAsset.maintenance_index_type))
                .ForMember(des => des.arc_flash_label_valid, opt => opt.MapFrom(src => src.TempAsset.arc_flash_label_valid))
                .ForMember(des => des.form_nameplate_info, opt => opt.MapFrom(src => src.TempAsset.form_nameplate_info))
                .ForMember(des => des.status_name, opt => opt.MapFrom(src => src.StatusMaster.status_name))
                .ForMember(des => des.issues_title_list, opt => opt.MapFrom(src => src.WOLineIssue.Where(x=>!x.is_deleted).Select(x=>x.issue_title)))
                //.ForMember(des => des.asset_name, opt => opt.MapFrom(src => src.asset_id !=null ? src.Asset.name: src.asset_name))
                ;

            CreateMap<WOOnboardingAssets, GetOBWOAssetDetailsByIdResponsemodel>()
              .ForMember(des => des.status_name, opt => opt.MapFrom(src => src.StatusMaster.status_name))
              .ForMember(des => des.qr_code, opt => opt.MapFrom(src => src.QR_code))
              .ForMember(des => des.inspection_type, opt => opt.MapFrom(src => src.inspection_type != null || src.inspection_type > 0 ? src.inspection_type : 1))
              .ForMember(des => des.mwo_asset_id, opt => opt.MapFrom(src => src.asset_id))
              .ForMember(des => des.mwo_asset_name, opt => opt.MapFrom(src => src.Asset.name))
              .ForMember(des => des.formiobuilding_id, opt => opt.MapFrom(src => src.WOLineBuildingMapping.formiobuilding_id))
              .ForMember(des => des.formiofloor_id, opt => opt.MapFrom(src => src.WOLineBuildingMapping.formiofloor_id))
              .ForMember(des => des.formioroom_id, opt => opt.MapFrom(src => src.WOLineBuildingMapping.formioroom_id))
              .ForMember(des => des.temp_formiobuilding_id, opt => opt.MapFrom(src => src.TempAsset != null ? src.TempAsset.temp_formiobuilding_id : src.WOOBAssetTempFormIOBuildingMapping.temp_formiobuilding_id))
              .ForMember(des => des.temp_formiofloor_id, opt => opt.MapFrom(src => src.WOOBAssetTempFormIOBuildingMapping != null ? src.WOOBAssetTempFormIOBuildingMapping.temp_formiofloor_id : src.TempAsset.temp_formiofloor_id))
              .ForMember(des => des.temp_formioroom_id, opt => opt.MapFrom(src => src.WOOBAssetTempFormIOBuildingMapping != null ? src.WOOBAssetTempFormIOBuildingMapping.temp_formioroom_id : src.TempAsset.temp_formioroom_id))
              .ForMember(des => des.temp_formiosection_id, opt => opt.MapFrom(src => src.WOOBAssetTempFormIOBuildingMapping!=null ? src.WOOBAssetTempFormIOBuildingMapping.temp_formiosection_id : src.TempAsset.temp_formiosection_id))
              
              .ForMember(des => des.temp_master_building_id, opt => opt.MapFrom(src => src.TempAsset.temp_master_building_id))
              .ForMember(des => des.temp_master_floor_id, opt => opt.MapFrom(src =>  src.TempAsset.temp_master_floor_id))
              .ForMember(des => des.temp_master_room_id, opt => opt.MapFrom(src =>  src.TempAsset.temp_master_room_id))

              .ForMember(des => des.building, opt => opt.MapFrom(src => !String.IsNullOrEmpty(src.building) ? src.building : src.WOOBAssetTempFormIOBuildingMapping != null ? src.WOOBAssetTempFormIOBuildingMapping.TempFormIOBuildings.temp_formio_building_name : ""))
              .ForMember(des => des.floor, opt => opt.MapFrom(src => !String.IsNullOrEmpty(src.floor) ? src.floor : src.WOOBAssetTempFormIOBuildingMapping != null ? src.WOOBAssetTempFormIOBuildingMapping.TempFormIOFloors.temp_formio_floor_name : ""))
              .ForMember(des => des.room, opt => opt.MapFrom(src => !String.IsNullOrEmpty(src.room) ? src.room : src.WOOBAssetTempFormIOBuildingMapping != null ? src.WOOBAssetTempFormIOBuildingMapping.TempFormIORooms.temp_formio_room_name : ""))
              .ForMember(des => des.section, opt => opt.MapFrom(src => !String.IsNullOrEmpty(src.section) ? src.section : src.WOOBAssetTempFormIOBuildingMapping != null ? src.WOOBAssetTempFormIOBuildingMapping.TempFormIOSections.temp_formio_section_name : ""))

              .ForMember(des => des.temp_master_building, opt => opt.MapFrom(src => src.TempAsset.TempMasterBuilding!=null? src.TempAsset.TempMasterBuilding.temp_master_building_name:src.building))
              .ForMember(des => des.temp_master_floor, opt => opt.MapFrom(src => src.TempAsset.TempMasterFloor!=null? src.TempAsset.TempMasterFloor.temp_master_floor_name:src.floor))
              .ForMember(des => des.temp_master_room, opt => opt.MapFrom(src => src.TempAsset.TempMasterRoom!=null? src.TempAsset.TempMasterRoom.temp_master_room_name:src.room))
              .ForMember(des => des.temp_master_section, opt => opt.MapFrom(src => src.TempAsset.temp_master_section))


              .ForMember(des => des.temp_building, opt => opt.MapFrom(src => !String.IsNullOrEmpty(src.building) ? src.building : src.TempAsset.TempFormIOBuildings.temp_formio_building_name ))
              .ForMember(des => des.temp_floor, opt => opt.MapFrom(src => !String.IsNullOrEmpty(src.floor) ? src.floor : src.TempAsset.TempFormIOFloors.temp_formio_floor_name))
              .ForMember(des => des.temp_room, opt => opt.MapFrom(src => !String.IsNullOrEmpty(src.room) ? src.room : src.TempAsset.TempFormIORooms.temp_formio_room_name ))
              .ForMember(des => des.temp_section, opt => opt.MapFrom(src => !String.IsNullOrEmpty(src.section) ? src.section : src.TempAsset.TempFormIOSections.temp_formio_section_name))
              .ForMember(des => des.condition_index_type_name, opt => opt.MapFrom(src => src.condition_index_type != null ? ((condition_index_type)src.condition_index_type.Value).ToString() : null))
              .ForMember(des => des.criticality_index_type_name, opt => opt.MapFrom(src => src.criticality_index_type != null && src.criticality_index_type > 0 ? ((criticality_index_type)src.criticality_index_type.Value).ToString() : null))
              .ForMember(des => des.thermal_classification_name, opt => opt.MapFrom(src => src.thermal_classification_id != null && src.thermal_classification_id>0 ? ((thermal_classification)src.thermal_classification_id.Value).ToString() : null))
              .ForMember(des => des.asset_image_list, opt => opt.MapFrom(src => src.WOOnboardingAssetsImagesMapping))
              //.ForMember(des => des.asset_image_list, opt => opt.MapFrom(src => src.WOLineIssue))
              .ForMember(des => des.ob_ir_Image_label_list, opt => opt.MapFrom(src => src.IRWOImagesLabelMapping))
              .ForMember(des => des.wo_ob_asset_fed_by_mapping, opt => opt.MapFrom(src => src.WOOBAssetFedByMapping))
              .ForMember(des => des.wo_ob_asset_toplevelcomponent_mapping, opt => opt.MapFrom(src => src.WOlineTopLevelcomponentMapping))
              .ForMember(des => des.wo_ob_asset_sublevelcomponent_mapping, opt => opt.MapFrom(src => src.WOlineSubLevelcomponentMapping))
              .ForMember(des => des.asset_class_code, opt => opt.MapFrom(src => src.Asset != null ? src.Asset.InspectionTemplateAssetClass.asset_class_code : src.asset_class_code))
              .ForMember(des => des.asset_class_name, opt => opt.MapFrom(src => src.Asset != null ? src.Asset.InspectionTemplateAssetClass.asset_class_name : src.asset_class_name))
              .ForMember(des => des.dynmic_fields_json, opt => opt.MapFrom(src => src.dynmic_fields_json != null ? src.dynmic_fields_json : "{}"))
              .ForPath(des => des.woline_issue_list, opt => opt.MapFrom(src => src.WOLineIssue))
              .ForMember(des => des.temp_asset_details, opt => opt.MapFrom(src => src.TempAsset))
              .ForMember(des => des.inspectiontemplate_asset_class_id, opt => opt.MapFrom(src => src.TempAsset.inspectiontemplate_asset_class_id))
              .ForMember(des => des.maintenance_index_type, opt => opt.MapFrom(src => src.TempAsset.maintenance_index_type))
              .ForMember(des => des.work_time_spend, opt => opt.MapFrom(src => src.WOOnboardingAssetsDateTimeTracking!=null ? src.WOOnboardingAssetsDateTimeTracking.work_time_spend : null))
              .ForMember(des => des.asset_class_type, opt => opt.MapFrom(src => src.TempAsset.InspectionTemplateAssetClass!=null&&src.TempAsset.InspectionTemplateAssetClass.FormIOType!=null ? src.TempAsset.InspectionTemplateAssetClass.FormIOType.form_type_name : null))
              .ForMember(des => des.asset_class_type_id, opt => opt.MapFrom(src => src.TempAsset.InspectionTemplateAssetClass!=null ? src.TempAsset.InspectionTemplateAssetClass.form_type_id : null))
              .ForMember(des => des.asset_group_id, opt => opt.MapFrom(src => src.TempAsset!=null ? src.TempAsset.asset_group_id : null))
              .ForMember(des => des.asset_group_name, opt => opt.MapFrom(src => src.TempAsset!=null && src.TempAsset.AssetGroup !=null ? src.TempAsset.AssetGroup.asset_group_name : null))
              //.ForMember(des => des.pm_plan_id, opt => opt.MapFrom(src => src.SitewalkthroughTempPmEstimation!=null ? src.SitewalkthroughTempPmEstimation.FirstOrDefault().pm_plan_id : null))
              //.ForMember(des => des.plan_name, opt => opt.MapFrom(src => src.SitewalkthroughTempPmEstimation != null && src.SitewalkthroughTempPmEstimation.PMPlans? src.SitewalkthroughTempPmEstimation.FirstOrDefault().pm_plan_id : null))
              .ForMember(des => des.pm_estimation_list, opt => opt.MapFrom(src => src.SitewalkthroughTempPmEstimation.Where(x=>!x.is_deleted)))
               ;

            CreateMap<SitewalkthroughTempPmEstimation, PMEstimationdata>()
            .ForMember(dest => dest.pm_id, opt => opt.MapFrom(src => src.pm_id)) // Mapping pm_id directly
            .ForMember(dest => dest.estimation_time, opt => opt.MapFrom(src => src.estimation_time)) // Mapping estimation_time
            .ForMember(dest => dest.sitewalkthrough_temp_pm_estimation_id, opt => opt.MapFrom(src => src.sitewalkthrough_temp_pm_estimation_id)) // Mapping sitewalkthrough_temp_pm_estimation_id
            .ForMember(dest => dest.title, opt => opt.MapFrom(src => src.PMs != null ? src.PMs.title : null)); // Mapping title from the related PMs entity
            

           CreateMap<WOLineIssue, woline_issue_response_obj>()
                .ForMember(des => des.nfpa_violation, opt => opt.MapFrom(src => src.nfpa_70b_violation))
                .ForMember(des => des.woline_issue_image_list, opt => opt.MapFrom(src => src.WOlineIssueImagesMapping.Where(x=>!x.is_deleted)))
                ;
            CreateMap<WOlineIssueImagesMapping, woline_issue_image_mapping>()
                .ForMember(des => des.image_file_name, opt => opt.MapFrom(src => UrlGenerator.GetIssueImagesURL(src.image_file_name)))
                ;

            //CreateMap<WOLineIssue, OBWOAssetImages>()
                //.ForMember(des => des.woline_issue_image_list, opt => opt.MapFrom(src => src.WOlineIssueImagesMapping));


            CreateMap<WOlineIssueImagesMapping, OBWOAssetImages>()
                //.ForMember(des => des.image_thumbnail_file_name, opt => opt.MapFrom(src => UrlGenerator.GetIssueImagesURL(src.image_thumbnail_file_name)))
                .ForMember(des => des.asset_photo, opt => opt.MapFrom(src => UrlGenerator.GetIssueImagesURL(src.image_file_name)))
                //.ForMember(des => des.asset_photo_type, opt => opt.MapFrom(src =>  ))
                ;

            CreateMap<TempAsset, TempAssetDetailsForWoline>()
              .ForMember(des => des.temp_formio_building_name, opt => opt.MapFrom(src => src.TempFormIOBuildings.temp_formio_building_name))
              .ForMember(des => des.temp_formio_floor_name, opt => opt.MapFrom(src => src.TempFormIOFloors.temp_formio_floor_name))
              .ForMember(des => des.temp_formio_room_name, opt => opt.MapFrom(src => src.TempFormIORooms.temp_formio_room_name))
              .ForMember(des => des.temp_formio_section_name, opt => opt.MapFrom(src => src.TempFormIOSections.temp_formio_section_name))

              .ForMember(des => des.temp_master_building, opt => opt.MapFrom(src => src.TempMasterBuilding != null ? src.TempMasterBuilding.temp_master_building_name : null))
              .ForMember(des => des.temp_master_floor, opt => opt.MapFrom(src => src.TempMasterFloor != null ? src.TempMasterFloor.temp_master_floor_name : null))
              .ForMember(des => des.temp_master_room, opt => opt.MapFrom(src => src.TempMasterRoom != null ? src.TempMasterRoom.temp_master_room_name : null))
              .ForMember(des => des.temp_master_section, opt => opt.MapFrom(src => src.temp_master_section))

              .ForMember(des => des.asset_class_code, opt => opt.MapFrom(src => src.InspectionTemplateAssetClass.asset_class_code))
              .ForMember(des => des.panel_schedule, opt => opt.MapFrom(src => src.panel_schedule))
              .ForMember(des => des.arc_flash_label_valid, opt => opt.MapFrom(src => src.arc_flash_label_valid))
              .ForMember(des => des.asset_class_name, opt => opt.MapFrom(src => src.InspectionTemplateAssetClass.asset_class_name));

            CreateMap<WOlineTopLevelcomponentMapping, WOOBAssetToplevelAssetMapping>();
            CreateMap<WOlineSubLevelcomponentMapping, WOOBAssetSublevelAssetMapping>()
                .ForMember(des => des.image_url, opt => opt.MapFrom(src => src.image_name!=null ? UrlGenerator.GetAssetImagesURL(src.image_name) : null))
                ;

            CreateMap<WOOnboardingAssetsImagesMapping, subcomponent_image_list_class>()
                .ForMember(des => des.image_url, opt => opt.MapFrom(src => UrlGenerator.GetAssetImagesURL(src.asset_photo)))
                .ForMember(des => des.thumbnail_url, opt => opt.MapFrom(src => src.asset_thumbnail_photo!=null ? UrlGenerator.GetAssetImagesURL(src.asset_thumbnail_photo):null))
                .ForMember(des => des.image_name, opt => opt.MapFrom(src => src.asset_photo))
                .ForMember(des => des.image_type, opt => opt.MapFrom(src => src.asset_photo_type))
                ;

            CreateMap<WOOnboardingAssetsImagesMapping, OBWOAssetImages>()
                .ForMember(des => des.asset_photo, opt => opt.MapFrom(src => UrlGenerator.GetAssetImagesURL(src.asset_photo)))
                .ForMember(des => des.asset_thumbnail_photo, opt => opt.MapFrom(src => UrlGenerator.GetAssetImagesURL(src.asset_thumbnail_photo)))
                ;
            CreateMap<IRWOImagesLabelMapping, View_OBIRImage_label>()
                 .ForMember(des => des.ir_image_label_url, opt => opt.MapFrom(src => UrlGenerator.GetIRImagesURL(src.ir_image_label, src.s3_image_folder_name)))
                 .ForMember(des => des.visual_image_label_url, opt => opt.MapFrom(src => UrlGenerator.GetIRImagesURL(src.visual_image_label, src.s3_image_folder_name)))
                ;
            CreateMap<IRWOImagesLabelMapping, wo_ob_asset_image_label>()
                 .ForMember(des => des.ir_image_label_url, opt => opt.MapFrom(src => UrlGenerator.GetIRImagesURL(src.ir_image_label, src.s3_image_folder_name)))
                 .ForMember(des => des.visual_image_label_url, opt => opt.MapFrom(src => UrlGenerator.GetIRImagesURL(src.visual_image_label, src.s3_image_folder_name)))
                ;
            CreateMap<WOOBAssetFedByMapping, WOOBAssetFedByMap>();

            CreateMap<WorkOrders, MobileViewWorkOrderDetailsByIdResponsemodel>()
               .ForMember(des => des.wo_number, opt => opt.MapFrom(src => src.wo_number.ToString()))
               .ForMember(des => des.manual_wo_number, opt => opt.MapFrom(src => src.manual_wo_number))
               .ForMember(des => des.description, opt => opt.MapFrom(src => src.description))
               .ForMember(des => des.site_name, opt => opt.MapFrom(src => src.Sites.site_name))
               .ForMember(des => des.wo_type_name, opt => opt.MapFrom(src => src.WOTypeStatusMaster.status_name))
               .ForMember(des => des.wo_status, opt => opt.MapFrom(src => src.StatusMaster.status_name))
               .ForMember(des => des.WorkOrderAttachments, opt => opt.MapFrom(src => src.WorkOrderAttachments))
               .ForMember(des => des.wo_status_id, opt => opt.MapFrom(src => src.status));
            ///.ForMember(des => des.client_company_name, opt => opt.MapFrom(src => src.ClientCompany.client_company_name));

            CreateMap<WorkOrders, WorkOrderDetailsByIdForExportPDFResponsemodel>()
               .ForMember(des => des.wo_number, opt => opt.MapFrom(src => src.wo_number.ToString()))
               .ForMember(des => des.manual_wo_number, opt => opt.MapFrom(src => src.manual_wo_number))
               .ForMember(des => des.description, opt => opt.MapFrom(src => src.description))
               .ForMember(des => des.site_name, opt => opt.MapFrom(src => src.Sites.site_name))
               .ForMember(des => des.wo_type_name, opt => opt.MapFrom(src => src.WOTypeStatusMaster.status_name))
               .ForMember(des => des.wo_status, opt => opt.MapFrom(src => src.StatusMaster.status_name))
               .ForMember(des => des.wo_status_id, opt => opt.MapFrom(src => src.status))
               // .ForMember(des => des.technician_id, opt => opt.MapFrom(src => src.technician_user_id))
               .ForMember(des => des.client_company_name, opt => opt.MapFrom(src => src.ClientCompany.client_company_name))
               //  .ForMember(des => des.technician_name, opt => opt.MapFrom(src => (src.TechnicianUser.firstname + src.TechnicianUser.lastname)))
               .ForMember(des => des.isCalibrationDateEnabled, opt => opt.MapFrom(src => src.Sites.Company.isCalibrationDateEnabled));



            CreateMap<User, GetAllTechnicianResponsemodel>()
               .ForMember(des => des.user_id, opt => opt.MapFrom(src => src.uuid))
               .ForMember(des => des.email, opt => opt.MapFrom(src => src.email))
               .ForMember(des => des.first_name, opt => opt.MapFrom(src => src.firstname))
               .ForMember(des => des.last_name, opt => opt.MapFrom(src => src.lastname))
               ;

            CreateMap<WOcategorytoTaskMapping, GetWOcategoryTaskByCategoryIDListResponsemodel>()
            //  .ForMember(des => des.task_code, opt => opt.MapFrom(src => src.Tasks.task_code))
              .ForMember(des => des.description, opt => opt.MapFrom(src => src.AssetFormIO.asset_form_description))
              .ForMember(des => des.asset_id, opt => opt.MapFrom(src => src.AssetFormIO.form_retrived_asset_id))
              .ForMember(des => des.asset_name, opt => opt.MapFrom(src => src.AssetFormIO.form_retrived_asset_name))
              .ForMember(des => des.location, opt => opt.MapFrom(src => src.AssetFormIO.form_retrived_location))
              .ForMember(des => des.inspection_verdict, opt => opt.MapFrom(src => src.AssetFormIO.inspection_verdict))
              .ForMember(des => des.defects, opt => opt.MapFrom(src => src.AssetFormIO.defects))
              .ForMember(des => des.assigned_asset_id, opt => opt.MapFrom(src => src.assigned_asset != null ? src.assigned_asset : src.woonboardingassets_id))
              .ForMember(des => des.assigned_asset_name, opt => opt.MapFrom(src => src._assigned_asset != null ? src._assigned_asset.name : src.AssetFormIO.form_retrived_asset_name))// if asset is assigned then take from assigned_asset or else take from formretrived_asset name
              .ForMember(des => des.WOcategorytoTaskMapping_id, opt => opt.MapFrom(src => src.WOcategorytoTaskMapping_id))
              .ForMember(des => des.status_id, opt => opt.MapFrom(src => src.AssetFormIO.status))
              // .ForMember(des => des.technician_id, opt => opt.MapFrom(src => src.technician_user_id))
              //.ForMember(des => des.technician_name, opt => opt.MapFrom(src => src.AssetFormIO.requested_by + src.User.lastname))
              .ForMember(des => des.WP, opt => opt.MapFrom(src => src.WOInspectionsTemplateFormIOAssignment.InspectionsTemplateFormIO.work_procedure))
              .ForMember(des => des.asset_class_name, opt => opt.MapFrom(src => src.WOInspectionsTemplateFormIOAssignment.InspectionTemplateAssetClass.asset_class_name))
              .ForMember(des => des.asset_class_code, opt => opt.MapFrom(src => src.WOInspectionsTemplateFormIOAssignment.InspectionTemplateAssetClass.asset_class_code))
              .ForMember(des => des.asset_class_type, opt => opt.MapFrom(src => src.WOInspectionsTemplateFormIOAssignment.InspectionTemplateAssetClass.FormIOType.form_type_name))
              .ForMember(des => des.technician_id, opt => opt.MapFrom(src => src.AssetFormIO.requested_by))
              .ForMember(des => des.form_name, opt => opt.MapFrom(src => src.AssetFormIO.asset_form_name))
              .ForMember(des => des.serial_number, opt => opt.MapFrom(src => src.serial_number))
              .ForMember(des => des.task_id, opt => opt.MapFrom(src => src.WOInspectionsTemplateFormIOAssignment.task_id))
              .ForMember(des => des.form_id, opt => opt.MapFrom(src => src.AssetFormIO.form_id))
              .ForMember(des => des.form_type, opt => opt.MapFrom(src => src.WOInspectionsTemplateFormIOAssignment.InspectionsTemplateFormIO.FormIOType.form_type_name))
              .ForMember(des => des.status_name, opt => opt.MapFrom(src => src.AssetFormIO.StatusMaster.status_name))
              .ForMember(des => des.asset_form_id, opt => opt.MapFrom(src => src.AssetFormIO.asset_form_id))
              .ForMember(des => des.building, opt => opt.MapFrom(src => src.AssetFormIO.building))
              .ForMember(des => des.floor, opt => opt.MapFrom(src => src.AssetFormIO.floor))
              .ForMember(des => des.room, opt => opt.MapFrom(src => src.AssetFormIO.room))
              .ForMember(des => des.section, opt => opt.MapFrom(src => src.AssetFormIO.section))
              .ForMember(des => des.site_id, opt => opt.MapFrom(src => src.AssetFormIO.site_id))
              .ForMember(des => des.site_name, opt => opt.MapFrom(src => src.AssetFormIO.Sites.site_name))
              ;

            CreateMap<WOcategorytoTaskMapping, MobileGetWOcategoryTaskByCategoryIDListResponsemodel>()
              // .ForMember(des => des.task_code, opt => opt.MapFrom(src => src.Tasks.task_code))
              // .ForMember(des => des.description, opt => opt.MapFrom(src => src.Tasks.description))
              .ForMember(des => des.asset_id, opt => opt.MapFrom(src => src.AssetFormIO.form_retrived_asset_id))
              .ForMember(des => des.asset_name, opt => opt.MapFrom(src => src.AssetFormIO.form_retrived_asset_name))
              .ForMember(des => des.location, opt => opt.MapFrom(src => src.AssetFormIO.form_retrived_location))
              .ForMember(des => des.assigned_asset_id, opt => opt.MapFrom(src => src.assigned_asset))
              .ForMember(des => des.assigned_asset_name, opt => opt.MapFrom(src => src._assigned_asset.name))
              .ForMember(des => des.WOcategorytoTaskMapping_id, opt => opt.MapFrom(src => src.WOcategorytoTaskMapping_id))
              .ForMember(des => des.status_id, opt => opt.MapFrom(src => src.AssetFormIO.status))

              // .ForMember(des => des.WP, opt => opt.MapFrom(src => src.WOInspectionsTemplateFormIOAssignment.InspectionsTemplateFormIO.work_procedure))
              .ForMember(des => des.technician_id, opt => opt.MapFrom(src => src.AssetFormIO.requested_by))
              // .ForMember(des => des.form_name, opt => opt.MapFrom(src => src.AssetFormIO.asset_form_name))
              .ForMember(des => des.serial_number, opt => opt.MapFrom(src => src.serial_number))
              // .ForMember(des => des.task_id, opt => opt.MapFrom(src => src.WOInspectionsTemplateFormIOAssignment.task_id))
              .ForMember(des => des.form_id, opt => opt.MapFrom(src => src.WOInspectionsTemplateFormIOAssignment.form_id))
              // .ForMember(des => des.form_type, opt => opt.MapFrom(src => src.WOInspectionsTemplateFormIOAssignment.InspectionsTemplateFormIO.FormIOType.form_type_name))
              .ForMember(des => des.status_name, opt => opt.MapFrom(src => src.AssetFormIO.StatusMaster.status_name));

            CreateMap<WOcategorytoTaskMapping, GetWOcategoryTaskByCategoryIDListResponsemodelExport>()
             .ForMember(des => des.task_code, opt => opt.MapFrom(src => src.Tasks.task_code))
             .ForMember(des => des.description, opt => opt.MapFrom(src => src.Tasks.description))
             .ForMember(des => des.asset_id, opt => opt.MapFrom(src => src.AssetFormIO.form_retrived_asset_id))
             .ForMember(des => des.asset_name, opt => opt.MapFrom(src => src.AssetFormIO.form_retrived_asset_name))
             .ForMember(des => des.location, opt => opt.MapFrom(src => src.AssetFormIO.form_retrived_location))
             .ForMember(des => des.assigned_asset_id, opt => opt.MapFrom(src => src.assigned_asset))
             .ForMember(des => des.assigned_asset_name, opt => opt.MapFrom(src => src._assigned_asset.name))
             .ForMember(des => des.WOcategorytoTaskMapping_id, opt => opt.MapFrom(src => src.WOcategorytoTaskMapping_id))
             .ForMember(des => des.status_id, opt => opt.MapFrom(src => src.AssetFormIO.status))
             // .ForMember(des => des.technician_id, opt => opt.MapFrom(src => src.technician_user_id))
             //.ForMember(des => des.technician_name, opt => opt.MapFrom(src => src.AssetFormIO.requested_by + src.User.lastname))
             .ForMember(des => des.WP, opt => opt.MapFrom(src => src.WOInspectionsTemplateFormIOAssignment.InspectionsTemplateFormIO.work_procedure))
             .ForMember(des => des.technician_id, opt => opt.MapFrom(src => src.AssetFormIO.requested_by))
             .ForMember(des => des.form_name, opt => opt.MapFrom(src => src.AssetFormIO.asset_form_name))
             .ForMember(des => des.form_type, opt => opt.MapFrom(src => src.WOInspectionsTemplateFormIOAssignment.InspectionsTemplateFormIO.FormIOType.form_type_name))
             .ForMember(des => des.status_name, opt => opt.MapFrom(src => src.AssetFormIO.StatusMaster.status_name));

            CreateMap<WOcategorytoTaskMapping, GetWOcategoryTaskByCategoryIDListBulkImport>();

            CreateMap<WOcategorytoTaskMapping, GetWOGridViewTaskResponsemodel>()
            .ForMember(des => des.task_code, opt => opt.MapFrom(src => src.Tasks.task_code))
            .ForMember(des => des.description, opt => opt.MapFrom(src => src.Tasks.description))
            .ForMember(des => des.asset_id, opt => opt.MapFrom(src => src.AssetFormIO.form_retrived_asset_id))
            .ForMember(des => des.asset_name, opt => opt.MapFrom(src => src.AssetFormIO.form_retrived_asset_name))
            .ForMember(des => des.location, opt => opt.MapFrom(src => src.AssetFormIO.form_retrived_location))
            .ForMember(des => des.assigned_asset_id, opt => opt.MapFrom(src => src.assigned_asset))
            .ForMember(des => des.assigned_asset_name, opt => opt.MapFrom(src => src._assigned_asset.name))
            .ForMember(des => des.WOcategorytoTaskMapping_id, opt => opt.MapFrom(src => src.WOcategorytoTaskMapping_id))
            .ForMember(des => des.status_id, opt => opt.MapFrom(src => src.AssetFormIO.status))
            // .ForMember(des => des.technician_id, opt => opt.MapFrom(src => src.technician_user_id))
            //.ForMember(des => des.technician_name, opt => opt.MapFrom(src => src.AssetFormIO.requested_by + src.User.lastname))
            .ForMember(des => des.WP, opt => opt.MapFrom(src => src.WOInspectionsTemplateFormIOAssignment.InspectionsTemplateFormIO.work_procedure))
            .ForMember(des => des.technician_id, opt => opt.MapFrom(src => src.AssetFormIO.requested_by))
            .ForMember(des => des.form_name, opt => opt.MapFrom(src => src.AssetFormIO.asset_form_name))
            .ForMember(des => des.asset_form_id, opt => opt.MapFrom(src => src.AssetFormIO.asset_form_id))
            .ForMember(des => des.asset_form_description, opt => opt.MapFrom(src => src.AssetFormIO.asset_form_description))
            .ForMember(des => des.form_type, opt => opt.MapFrom(src => src.WOInspectionsTemplateFormIOAssignment.InspectionsTemplateFormIO.FormIOType.form_type_name))
            .ForMember(des => des.status_name, opt => opt.MapFrom(src => src.AssetFormIO.StatusMaster.status_name));

            CreateMap<AssetFormIO, GetFormByWOTaskIDResponsemodel>()
                .ForMember(des => des.asset_name, opt => opt.MapFrom(src => src.Asset.name))
                 .ForMember(des => des.timezone, opt => opt.MapFrom(src => src.Asset.Sites.timezone))
                 .ForMember(des => des.wo_inspectionsTemplateFormIOAssignment_id, opt => opt.MapFrom(src => src.WOcategorytoTaskMapping.wo_inspectionsTemplateFormIOAssignment_id))
                ;

            CreateMap<AssetFormIOExclude, GetFormByWOTaskIDResponsemodel>()
                .ForMember(des => des.asset_name, opt => opt.MapFrom(src => src.asset_name))
                 .ForMember(des => des.timezone, opt => opt.MapFrom(src => src.timezone))
                ;
            CreateMap<AssetFormIOExclude, MobileGetFormByWOTaskIDResponsemodel>()
                .ForMember(des => des.asset_name, opt => opt.MapFrom(src => src.asset_name))
                //.ForMember(des => des.timezone, opt => opt.MapFrom(src => src.timezone))
                ;

            CreateMap<AssetFormIO, GetFormByWOTaskIDResponsemodelExport>()
                .ForMember(des => des.asset_name, opt => opt.MapFrom(src => src.Asset.name))
                 .ForMember(des => des.timezone, opt => opt.MapFrom(src => src.Asset.Sites.timezone))
                ;
            CreateMap<AssetFormIO, GetFormByWOTaskIDBulkImpoert>();
            CreateMap<MultiCopyWOTaskRequestModel, MultiCopyWOTaskRequestModel>();

            CreateMap<AssetFormIOExclude, AssetFormIO>();

            CreateMap<AddWOCategoryMappingOfflineRequestModel, WOInspectionsTemplateFormIOAssignment>();

            CreateMap<AddWOCategoryTaskMappingOfflineRequestModel, WOcategorytoTaskMapping>()
                .ForMember(x => x._assigned_asset, y => y.Ignore());
            ;

            CreateMap<AddAssetFormioOfflineRequestModel, AssetFormIO>();

            CreateMap<FormIOBuildings, Building>()
                .ForMember(des => des.floor, opt => opt.MapFrom(src => src.FormIOFloors))
                ;
            CreateMap<FormIOFloors, floor>()
                 .ForMember(des => des.rooms, opt => opt.MapFrom(src => src.FormIORooms))
                ;
            CreateMap<FormIORooms, rooms>()
                 .ForMember(des => des.section, opt => opt.MapFrom(src => src.FormIOSections))
                ;
            CreateMap<FormIOSections, section>().ForMember(des => des.asset_detail, opt => opt.MapFrom(src => src.AssetFormIOBuildingMappings))
                ;
            CreateMap<AssetFormIOBuildingMappings, asset_detail>()
                 .ForMember(des => des.asset_name, opt => opt.MapFrom(src => src.Asset.name))
                 .ForMember(des => des.asset_internal_id, opt => opt.MapFrom(src => src.Asset.internal_asset_id));

            CreateMap<FormIOInsulationResistanceTestMapping, FormIOInsulationResistanceTestResponseModel>();

            CreateMap<InspectionTemplateAssetClass, GetAllAssetClassResponsemodel>()
                .ForMember(des => des.form_type_name, opt => opt.MapFrom(src => src.FormIOType.form_type_name))
                .ForMember(des => des.pm_category_id, opt => opt.MapFrom(src => src.PMCategory!=null ? src.PMCategory.pm_category_id : Guid.Empty))
                .ForMember(des => des.pmplans_list, opt => opt.MapFrom(src => src.PMCategory.PMPlans.Where(x => x.status != (int)Status.Deactive)))
                ;

            CreateMap<PMPlans, PMPlans_for_Class_Obj>();

            CreateMap<InspectionTemplateFormIoExclude, GetFormsByAssetclassIDResponsemodel>();

            CreateMap<InspectionsTemplateFormIO, GetFormIOFormByIdResponsemodel>()
                .ForMember(des => des.status_name, opt => opt.MapFrom(src => src.StatusMaster.status_name))
                ;

            CreateMap<InspectionTemplateFormIoExclude, GetFormIOFormByIdResponsemodel>()
               ;

            CreateMap<InspectionTemplateFormIoExclude, GetFormPropertiesByAssetclassIDResponsemodel>();
            CreateMap<Asset, GetAssetsToAssignResponsemodel>()
                 .ForMember(des => des.name, opt => opt.MapFrom(src => src.name))
                 .ForMember(des => des.asset_id, opt => opt.MapFrom(src => src.asset_id))
                 .ForMember(des => des.asset_class_name, opt => opt.MapFrom(src => src.InspectionTemplateAssetClass.asset_class_name))
                 .ForMember(des => des.inspectiontemplate_asset_class_id, opt => opt.MapFrom(src => src.inspectiontemplate_asset_class_id))
                ;

            CreateMap<AssetClassFormIOMapping, mobile_asset_class_form_io_mapping>()
                 .ForMember(des => des.asset_class_formio_mapping_id, opt => opt.MapFrom(src => src.asset_class_formio_mapping_id))
                 .ForMember(des => des.inspectiontemplate_asset_class_id, opt => opt.MapFrom(src => src.inspectiontemplate_asset_class_id))
                 .ForMember(des => des.form_id, opt => opt.MapFrom(src => src.form_id))
                 .ForMember(des => des.isarchive, opt => opt.MapFrom(src => src.isarchive))
                ;
            CreateMap<AssetClassFormIOMappingExcludeProperty, GetAssetclassFormToAddcategoryResponsemodel>();
            CreateMap<uploadassetONWOdata, WOOnboardingAssets>()
                .ForMember(des => des.site_id, opt => opt.MapFrom(src => Guid.Parse(GenericRequestModel.site_id)))
                .ForMember(des => des.status, opt => opt.MapFrom(src => (int)Status.open))
                .ForMember(des => des.inspection_type, opt => opt.MapFrom(src => (int)MWO_inspection_wo_type.OnBoarding))
                ;

            CreateMap<InspectionTemplateAssetClass, MobileAssetClassResponsemodel>()
                .ForMember(des => des.classType, opt => opt.MapFrom(src => src.FormIOType.form_type_name))
                ;
            CreateMap<AssetClassFormIOMapping, MobileAssetClassFormMappingResponsemodel>();

            CreateMap<InspectionTemplateAssetClass, GetAllAssetClassCodesResponsemodel>()
                .ForMember(des => des.label, opt => opt.MapFrom(src => src.asset_class_code))
                .ForMember(des => des.value, opt => opt.MapFrom(src => src.inspectiontemplate_asset_class_id))
                .ForMember(des => des.className, opt => opt.MapFrom(src => src.asset_class_name))
                .ForMember(des => des.classType, opt => opt.MapFrom(src => src.FormIOType.form_type_name))
                ;



            CreateMap<WOOnboardingAssets, MobileOBWOAssetListResponsemodel>()
                .ForMember(des => des.status_name, opt => opt.MapFrom(src => src.StatusMaster.status_name))
                 .ForMember(des => des.thermal_classification_name, opt => opt.MapFrom(src => src.thermal_classification_id != null ? ((thermal_classification)src.thermal_classification_id.Value).ToString() : null))
                 .ForMember(des => des.condition_index_type_name, opt => opt.MapFrom(src => src.condition_index_type != null ? ((condition_index_type)src.condition_index_type.Value).ToString() : null))
                .ForMember(des => des.criticality_index_type_name, opt => opt.MapFrom(src => src.criticality_index_type != null ? ((criticality_index_type)src.criticality_index_type.Value).ToString() : null))
                ;
            CreateMap<WOOnboardingAssetsImagesMapping, MobileOBWOAssetImagesResponsemodel>()
                .ForMember(des => des.asset_photo, opt => opt.MapFrom(src => src.asset_photo != null ? UrlGenerator.GetAssetImagesURL(src.asset_photo) : null))
                .ForMember(des => des.asset_thumbnail_photo, opt => opt.MapFrom(src => src.asset_photo != null ? UrlGenerator.GetAssetImagesURL(src.asset_thumbnail_photo) : null))
                ;

            CreateMap<OBWOAssetDetailsOfflineRequestmodel, WOOnboardingAssets>()
                .ForMember(des => des.building, opt => opt.MapFrom(src => src.building != null ? src.building : null))
                .ForMember(des => des.floor, opt => opt.MapFrom(src => src.floor != null ? src.floor : null))
                .ForMember(des => des.room, opt => opt.MapFrom(src => src.room != null ? src.room : null))
                .ForMember(des => des.section, opt => opt.MapFrom(src => src.section != null ? src.section : null))
                ;

            CreateMap<ExportCompletedAssetsByWOExcludedProperty, ExportCompletedAssets>();
            CreateMap<WOOnboardingAssets, ExportCompletedAssets>()
                .ForMember(des => des.form_retrived_asset_name, opt => opt.MapFrom(src => src.TempAsset != null? src.TempAsset.asset_name : src.asset_name!= null ? src.asset_name : src.Asset.name))
                .ForMember(des => des.intial_form_filled_date, opt => opt.MapFrom(src => src.inspected_at));


            CreateMap<AssetNotes, GetAssetNotesResponsemodel>();


            CreateMap<FormIOBuildings, FilterAssetBuildingLocationOptionsmapping>()
                .ForMember(des => des.label, opt => opt.MapFrom(src => src.formio_building_name))
                .ForMember(des => des.value, opt => opt.MapFrom(src => src.formiobuilding_id));

            CreateMap<FormIOFloors, FilterAssetBuildingLocationOptionsmapping>()
                .ForMember(des => des.label, opt => opt.MapFrom(src => src.formio_floor_name))
                .ForMember(des => des.value, opt => opt.MapFrom(src => src.formiofloor_id));
            CreateMap<FormIORooms, FilterAssetBuildingLocationOptionsmapping>()
                .ForMember(des => des.label, opt => opt.MapFrom(src => src.formio_room_name))
                .ForMember(des => des.value, opt => opt.MapFrom(src => src.formioroom_id));
            CreateMap<FormIOSections, FilterAssetBuildingLocationOptionsmapping>()
                .ForMember(des => des.label, opt => opt.MapFrom(src => src.formio_section_name))
                .ForMember(des => des.value, opt => opt.MapFrom(src => src.formiosection_id));
            CreateMap<AssetFormIOBuildingMappings, FilterAssetRoomFloorBuildingLocationOptionsmapping>()
                .ForMember(des => des.room_name, opt => opt.MapFrom(src => src.FormIORooms.formio_room_name))
                .ForMember(des => des.room_id, opt => opt.MapFrom(src => src.FormIORooms.formioroom_id))
                .ForMember(des => des.building_name, opt => opt.MapFrom(src => src.FormIOBuildings.formio_building_name))
                .ForMember(des => des.building_id, opt => opt.MapFrom(src => src.formiobuilding_id))
                .ForMember(des => des.floor_name, opt => opt.MapFrom(src => src.FormIOFloors.formio_floor_name))
                .ForMember(des => des.floor_id, opt => opt.MapFrom(src => src.formiofloor_id))
                ;

            CreateMap<AssetFormIOBuildingMappings, FilterAssetRoomFloorBuildingLocationOptionsmapping>()
                .ForMember(des => des.room_name, opt => opt.MapFrom(src => src.FormIORooms.formio_room_name))
                .ForMember(des => des.room_id, opt => opt.MapFrom(src => src.FormIORooms.formioroom_id))
                .ForMember(des => des.building_name, opt => opt.MapFrom(src => src.FormIOBuildings.formio_building_name))
                .ForMember(des => des.building_id, opt => opt.MapFrom(src => src.formiobuilding_id))
                .ForMember(des => des.floor_name, opt => opt.MapFrom(src => src.FormIOFloors.formio_floor_name))
                .ForMember(des => des.floor_id, opt => opt.MapFrom(src => src.formiofloor_id))
                ;


            CreateMap<Asset, GetChildrenByAssetIDResponsemodel>()
               .ForMember(des => des.asset_name, opt => opt.MapFrom(src => src.name))
               .ForMember(des => des.site_name, opt => opt.MapFrom(src => src.Sites.site_name));

            CreateMap<WOOBAssetFedByMapping, mobile_fed_by_asset_mapping>();
            CreateMap<AssetParentHierarchyMapping, MobileAssetParentsMapping>();

            CreateMap<IRScanWOImageFileMapping, GetIRScanImagesFilesResponsemodel>()
              .ForMember(des => des.img_file_url, opt => opt.MapFrom(src => src.img_file_name != null ? UrlGenerator.GetIRImagesURL(src.img_file_name, src.manual_wo_number) : null))
                ;
            CreateMap<Asset, GetAllAssetsForClusterResponsemodel>()
                 .ForPath(des => des.children_list, opt => opt.MapFrom(src => src.AssetChildrenHierarchyMapping))
                 .ForPath(des => des.subcomponent_list, opt => opt.MapFrom(src => src.AssetSubLevelcomponentMapping.Where(x=>!x.is_deleted)))
                 .ForPath(des => des.is_asset_temp, opt => opt.MapFrom(src => false))
                 .ForPath(des => des.asset_class_type, opt => opt.MapFrom(src => src.InspectionTemplateAssetClass.FormIOType.form_type_name))
                 .ForPath(des => des.formiobuilding_id, opt => opt.MapFrom(src => src.AssetFormIOBuildingMappings.formiobuilding_id))
                 .ForPath(des => des.formiofloor_id, opt => opt.MapFrom(src => src.AssetFormIOBuildingMappings.formiofloor_id))
                 .ForPath(des => des.formioroom_id, opt => opt.MapFrom(src => src.AssetFormIOBuildingMappings.formioroom_id))
                 .ForPath(des => des.building, opt => opt.MapFrom(src => src.AssetFormIOBuildingMappings.FormIOBuildings.formio_building_name))
                 .ForPath(des => des.floor, opt => opt.MapFrom(src => src.AssetFormIOBuildingMappings.FormIOFloors.formio_floor_name))
                 .ForPath(des => des.room, opt => opt.MapFrom(src => src.AssetFormIOBuildingMappings.FormIORooms.formio_room_name))
                 .ForPath(des => des.section, opt => opt.MapFrom(src => src.AssetFormIOBuildingMappings.FormIOSections.formio_section_name))
                 .ForPath(des => des.asset_class_code, opt => opt.MapFrom(src => src.InspectionTemplateAssetClass.asset_class_code))
                 .ForPath(des => des.asset_class_name, opt => opt.MapFrom(src => src.InspectionTemplateAssetClass.asset_class_name))
                 ;
            CreateMap<AssetChildrenHierarchyMapping, AssetChildrenMappingForCluster>()
                 .ForPath(des => des.is_asset_temp, opt => opt.MapFrom(src => false))
                ;
            CreateMap<AssetSubLevelcomponentMapping, ClusterSubcomponents>()
                 .ForPath(des => des.is_asset_temp, opt => opt.MapFrom(src => false))
                 .ForPath(des => des.asset_id, opt => opt.MapFrom(src => src.sublevelcomponent_asset_id))
                ;
            CreateMap<AssetFormIO, asset_form_data_bulk_report>();
            CreateMap<InspectionsTemplateFormIO, master_form_data_bulk_report>();
            CreateMap<ClientCompany, ClientCompanyListResponseModel>();
            CreateMap<Sites, SiteListResponseModel>();
            CreateMap<AssetFormIOExcludeNew, AssetFormIOResponseModel>()
                 .ForMember(des => des.asset_name, opt => opt.MapFrom(src => src.asset_id != null ? src.asset_name : src.form_retrived_asset_name))
                ;

            CreateMap<WOLineIssue, GetAllWOLineTempIssuesResponsemodel>()
                 .ForMember(des => des.asset_name, opt => opt.MapFrom(src => src.WOOnboardingAssets.TempAsset != null ? src.WOOnboardingAssets.TempAsset.asset_name : src.form_retrived_asset_name))
                 .ForMember(des => des.issue_status_name, opt => opt.MapFrom(src => src.StatusMaster.status_name))
                 .ForMember(des => des.origin_wo_id, opt => opt.MapFrom(src => src.original_wo_id))
                 .ForMember(des => des.origin_wo_line_id, opt => opt.MapFrom(src => src.original_asset_form_id != null ? src.original_asset_form_id : src.original_woonboardingassets_id))
                ;

            CreateMap<WOLineIssue, AssetIssue>()
                .ForMember(des => des.AssetFormIO, opt => opt.Ignore())
                .ForMember(des => des.WorkOrders, opt => opt.Ignore())
                .ForMember(des => des.WOOnboardingAssets, opt => opt.Ignore())
                .ForMember(des => des.StatusMaster, opt => opt.Ignore())
                .ForMember(des => des.AssetIssueImagesMapping, opt => opt.MapFrom(src => src.WOlineIssueImagesMapping))
                ;

            CreateMap<WOlineIssueImagesMapping, AssetIssueImagesMapping>();

            CreateMap<AssetIssue, link_main_issue_list>()
                 .ForMember(des => des.asset_name, opt => opt.MapFrom(src => src.WOOnboardingAssets.TempAsset.asset_name))
                 .ForMember(des => des.origin_wo_id, opt => opt.MapFrom(src => src.WOLineIssue.original_wo_id))
                 .ForMember(des => des.origin_wo_line_id, opt => opt.MapFrom(src => src.WOLineIssue.original_asset_form_id != null ? src.WOLineIssue.original_asset_form_id : src.WOLineIssue.original_woonboardingassets_id))
                ;

            CreateMap<WOLineIssue, link_temp_issue_list>()
                 .ForMember(des => des.asset_name, opt => opt.MapFrom(src => src.asset_id != null ? src.Asset.name : src.form_retrived_asset_name)); ;

            CreateMap<AssetIssue, GetAllAssetIssuesResponsemodel>()
                 .ForMember(des => des.asset_name, opt => opt.MapFrom(src => src.Asset.name))
                 .ForMember(des => des.asset_class_type, opt => opt.MapFrom(src => src.Asset.InspectionTemplateAssetClass.FormIOType.form_type_name))
                 .ForMember(des => des.issue_time_elapsed, opt => opt.MapFrom(src => (src.created_at == null || src.issue_status == (int)Status.Completed) ? "" : DateTimeUtil.GetBeforetimeText(src.created_at.Value)))
                 .ForMember(des => des.issue_status_name, opt => opt.MapFrom(src => src.StatusMaster.status_name))
                 .ForMember(des => des.manual_wo_number, opt => opt.MapFrom(src => src.WorkOrders.manual_wo_number))
                 .ForMember(des => des.issue_created_wo_id, opt => opt.MapFrom(src => src.WOLineIssue.wo_id))
                 .ForMember(des => des.issue_created_asset_form_id, opt => opt.MapFrom(src => src.WOLineIssue.asset_form_id))
                 .ForMember(des => des.issue_created_woonboardingassets_id, opt => opt.MapFrom(src => src.WOLineIssue.woonboardingassets_id))
                 .ForMember(des => des.issue_image_list, opt => opt.MapFrom(src => src.AssetIssueImagesMapping.Where(x=>!x.is_deleted).ToList()))
                ;

            CreateMap<AssetIssueComments, GetAllAssetIssueCommentsResponsemodel>()
                 .ForMember(des => des.comment_user_name, opt => opt.MapFrom(src => (src.User.firstname + src.User.lastname)))
                 .ForMember(des => des.comment_user_role_name, opt => opt.MapFrom(src => src.Roles.name))
                ;

            CreateMap<AssetIssue, ViewAssetIssueDetailsByIdResponsemodel>()
                .ForMember(des => des.client_company_id, opt => opt.MapFrom(src => src.Sites.client_company_id))
                .ForMember(des => des.site_name, opt => opt.MapFrom(src => src.Sites.site_name))
                .ForMember(des => des.client_company_name, opt => opt.MapFrom(src => src.Sites.ClientCompany.client_company_name))
                .ForMember(des => des.manual_wo_number, opt => opt.MapFrom(src => src.WorkOrders.manual_wo_number))
                .ForMember(des => des.asset_name, opt => opt.MapFrom(src => src.Asset.name))
                .ForMember(des => des.issue_created_wo_id, opt => opt.MapFrom(src => src.WOLineIssue.wo_id))
                .ForMember(des => des.issue_image_list, opt => opt.MapFrom(src => src.AssetIssueImagesMapping.Where(x => !x.is_deleted).ToList()))
                ;

            CreateMap<AssetIssueImagesMapping, AssetIssueImagesListResponse>()
                .ForMember(des => des.image_thumbnail_file_name_url, opt => opt.MapFrom(src => src.image_thumbnail_file_name != null ? UrlGenerator.GetIssueImagesURL(src.image_thumbnail_file_name) : null))
                .ForMember(des => des.image_file_name_url, opt => opt.MapFrom(src => src.image_file_name != null ? UrlGenerator.GetIssueImagesURL(src.image_file_name) : null));

            CreateMap<PMPlans, GetPMPlansByClassIdResponsemodel>()
                .ForMember(des => des.pm_count, opt => opt.MapFrom(src => src.PMs.Where(x => !x.is_archive).Count()))
                ;

            CreateMap<AssetPMs, GetAssetPMListMobileResponsemodel>();


            CreateMap<FormIOBuildings, WOlineBuilding>()
                 .ForPath(des => des.floors, opt => opt.MapFrom(src => src.FormIOFloors.Where(x=>x.site_id != Guid.Empty && x.site_id!=null)));

            CreateMap<FormIOFloors, WOlineFloors>()
                 .ForPath(des => des.rooms, opt => opt.MapFrom(src => src.FormIORooms.Where(x => x.site_id != Guid.Empty && x.site_id != null)));

            CreateMap<FormIORooms, WOlineRooms>();

            CreateMap<WOLineBuildingMapping, WOLineBuildingMappingOffline>();

            CreateMap<FormIOBuildings, MobileFormIOMasterBuildings>();
            CreateMap<FormIOFloors, MobileFormIOMasterFloors>();
            CreateMap<FormIORooms, MobileFormIOMasterRooms>();
            CreateMap<FormIOSections, MobileFormIOMasterSection>();

            CreateMap<InspectionsTemplateFormIO, InspectionsTemplateFormIO>()
               .ForMember(x => x.form_id, y => y.Ignore())
               .ForMember(des => des.company_id, opt => opt.MapFrom(src => Guid.Parse("0b2b3b98-f141-40f1-88b9-fa8de7224c0f")))
            ;

            CreateMap<InspectionTemplateAssetClass, InspectionTemplateAssetClass>()
               .ForMember(x => x.inspectiontemplate_asset_class_id, y => y.Ignore())
               .ForMember(des => des.company_id, opt => opt.MapFrom(src => Guid.Parse("0b2b3b98-f141-40f1-88b9-fa8de7224c0f")))
               .ForPath(des => des.AssetClassFormIOMapping, opt => opt.MapFrom(src => src.AssetClassFormIOMapping));


            CreateMap<AssetClassFormIOMapping, AssetClassFormIOMapping>()
               .ForMember(x => x.asset_class_formio_mapping_id, y => y.Ignore())
               .ForMember(x => x.inspectiontemplate_asset_class_id, y => y.Ignore());

            CreateMap<AssetPMs, AssetPmsOffline>();

            CreateMap<AssetIssue, AssetIssueListOffline>()
                   .ForMember(des => des.asset_name, opt => opt.MapFrom(src => src.Asset.name));
            CreateMap<WOLineIssue, AssetWOlineIssueListOffline>()
                .ForMember(des => des.asset_name, opt => opt.MapFrom(src => src.Asset.name))
                ;
            CreateMap<AssetIssueImagesMapping, AssetIssueImageMapping>();
            CreateMap<AssetIRWOImagesLabelMapping, AssetIRVisualImageMappingList>();

            CreateMap<UpdateAssetIssueOffline, AssetIssue>();

            CreateMap<AssetAttachmentMapping, GetAssetAttachmentsResponsemodel>()
                .ForMember(des => des.file_url, opt => opt.MapFrom(src => src.file_name != null ? UrlGenerator.GetAssetAttachmentURL(src.file_name) : null));

            CreateMap<AssetAttachmentMapping, AssetAttachmentsOffline>()
               .ForMember(des => des.file_url, opt => opt.MapFrom(src => src.file_name != null ? UrlGenerator.GetAssetAttachmentURL(src.file_name) : null));

            CreateMap<Asset, WOOnboardingAssets>()
                 .ForMember(des => des.created_at, opt => opt.MapFrom(src => DateTime.UtcNow))
                 .ForMember(des => des.status, opt => opt.MapFrom(src => (int)Status.open))
                 .ForMember(des => des.inspection_type, opt => opt.MapFrom(src => (int)MWO_inspection_wo_type.OnBoarding))
                 .ForMember(des => des.asset_name, opt => opt.MapFrom(src => src.name))
                 .ForMember(des => des.back_office_note, opt => opt.MapFrom(src => src.notes))
                 .ForMember(des => des.is_wo_line_for_exisiting_asset, opt => opt.MapFrom(src => true))
                 .ForMember(des => des.location, opt => opt.MapFrom(src => src.asset_placement))
                 .ForMember(des => des.form_nameplate_info, opt => opt.MapFrom(src => src.form_retrived_nameplate_info))
                 .ForMember(des => des.asset_class_code, opt => opt.MapFrom(src => src.InspectionTemplateAssetClass.asset_class_code))
                 .ForMember(des => des.asset_class_name, opt => opt.MapFrom(src => src.InspectionTemplateAssetClass.asset_class_name))
                 .ForMember(des => des.building, opt => opt.MapFrom(src => src.AssetFormIOBuildingMappings.FormIOBuildings.formio_building_name))
                 .ForMember(des => des.floor, opt => opt.MapFrom(src => src.AssetFormIOBuildingMappings.FormIOFloors.formio_floor_name))
                 .ForMember(des => des.room, opt => opt.MapFrom(src => src.AssetFormIOBuildingMappings.FormIORooms.formio_room_name))
                 .ForMember(des => des.section, opt => opt.MapFrom(src => src.AssetFormIOBuildingMappings.FormIOSections.formio_section_name))
                 .ForMember(des => des.WOLineBuildingMapping, opt => opt.MapFrom(src => src.AssetFormIOBuildingMappings))
                 .ForMember(des => des.WOOBAssetFedByMapping, opt => opt.MapFrom(src => src.AssetParentHierarchyMapping))
                 .ForMember(des => des.WOOnboardingAssetsImagesMapping, opt => opt.MapFrom(src => src.AssetProfileImages))
                 .ForMember(des => des.IRWOImagesLabelMapping, opt => opt.MapFrom(src => src.AssetIRWOImagesLabelMapping))
                 .ForMember(des => des.WOlineTopLevelcomponentMapping, opt => opt.MapFrom(src => src.AssetTopLevelcomponentMapping))
                 .ForMember(des => des.WOlineSubLevelcomponentMapping, opt => opt.MapFrom(src => src.AssetSubLevelcomponentMapping))
                 .ForMember(x => x.AssetIssue, y => y.Ignore())
                 .ForMember(x => x.TempAsset, y => y.Ignore())
                 ;

            CreateMap<AssetFormIOBuildingMappings, WOLineBuildingMapping>()
                 .ForMember(des => des.created_at, opt => opt.MapFrom(src => DateTime.UtcNow));

            CreateMap<AssetParentHierarchyMapping, WOOBAssetFedByMapping>()
                .ForMember(des => des.created_at, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(des => des.via_subcomponant_asset_id, opt => opt.MapFrom(src => src.via_subcomponent_asset_id))
                ;

            CreateMap<AssetProfileImages, WOOnboardingAssetsImagesMapping>()
                .ForMember(des => des.created_at, opt => opt.MapFrom(src => DateTime.UtcNow));

            CreateMap<AssetIRWOImagesLabelMapping, IRWOImagesLabelMapping>()
               .ForMember(des => des.created_at, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(des => des.s3_image_folder_name, opt => opt.MapFrom(src => (src.site_id + "/" + src.s3_image_folder_name)));

            CreateMap<AssetTopLevelcomponentMapping, WOlineTopLevelcomponentMapping>()
                .ForMember(des => des.toplevelcomponent_asset_id, opt => opt.MapFrom(src => src.toplevelcomponent_asset_id))
                .ForMember(des => des.is_toplevelcomponent_from_ob_wo, opt => opt.MapFrom(src => false))
                 .ForMember(des => des.created_at, opt => opt.MapFrom(src => DateTime.UtcNow));

            CreateMap<AssetSubLevelcomponentMapping, WOlineSubLevelcomponentMapping>()
                .ForMember(des => des.sublevelcomponent_asset_id, opt => opt.MapFrom(src => src.sublevelcomponent_asset_id))
                .ForMember(des => des.is_sublevelcomponent_from_ob_wo, opt => opt.MapFrom(src => false))
                .ForMember(des => des.created_at, opt => opt.MapFrom(src => DateTime.UtcNow));

            CreateMap<Asset, GetAssetsToAssignOBWOResponsemodel>()
                .ForMember(des => des.asset_class_code, opt => opt.MapFrom(src => src.InspectionTemplateAssetClass.asset_class_code))
                .ForMember(des => des.asset_name, opt => opt.MapFrom(src => src.name))
                 .ForMember(des => des.asset_class_name, opt => opt.MapFrom(src => src.InspectionTemplateAssetClass.asset_class_name))
                 .ForMember(des => des.building, opt => opt.MapFrom(src => src.AssetFormIOBuildingMappings.FormIOBuildings.formio_building_name))
                 .ForMember(des => des.floor, opt => opt.MapFrom(src => src.AssetFormIOBuildingMappings.FormIOFloors.formio_floor_name))
                 .ForMember(des => des.room, opt => opt.MapFrom(src => src.AssetFormIOBuildingMappings.FormIORooms.formio_room_name))
                 .ForMember(des => des.section, opt => opt.MapFrom(src => src.AssetFormIOBuildingMappings.FormIOSections.formio_section_name))
                ;

            CreateMap<Asset, MainAssets>()
                .ForMember(des => des.asset_name, opt => opt.MapFrom(src => src.name))
                .ForMember(des => des.building_name, opt => opt.MapFrom(src => src.AssetFormIOBuildingMappings.FormIOBuildings.formio_building_name))
                .ForMember(des => des.floor_name, opt => opt.MapFrom(src => src.AssetFormIOBuildingMappings.FormIOFloors.formio_floor_name))
                .ForMember(des => des.room_name, opt => opt.MapFrom(src => src.AssetFormIOBuildingMappings.FormIORooms.formio_room_name))
                
                ;


            CreateMap<WOOnboardingAssets, TempAssets>()
                .ForMember(des => des.asset_name, opt => opt.MapFrom(src => src.asset_id != null ? src.Asset.name : src.asset_name))
                //.ForMember(des => des.building_name, opt => opt.MapFrom(src => src.WOOBAssetTempFormIOBuildingMapping.TempFormIOBuildings.temp_formio_building_name))
                .ForMember(des => des.building_name, opt => opt.MapFrom(src => src.TempAsset.TempMasterBuilding!=null? src.TempAsset.TempMasterBuilding.temp_master_building_name : src.building))
                //.ForMember(des => des.floor_name, opt => opt.MapFrom(src => src.WOOBAssetTempFormIOBuildingMapping.TempFormIOFloors.temp_formio_floor_name))
                .ForMember(des => des.floor_name, opt => opt.MapFrom(src => src.TempAsset.TempMasterFloor != null ? src.TempAsset.TempMasterFloor.temp_master_floor_name : src.floor))
                //.ForMember(des => des.room_name, opt => opt.MapFrom(src => src.WOOBAssetTempFormIOBuildingMapping.TempFormIORooms.temp_formio_room_name))
                .ForMember(des => des.room_name, opt => opt.MapFrom(src => src.TempAsset.TempMasterRoom != null ? src.TempAsset.TempMasterRoom.temp_master_room_name : src.room))
                ;


            CreateMap<AssetSubLevelcomponentMapping, GetSubcomponentsByAssetIdResponsemodel>()
                 .ForMember(des => des.image_url, opt => opt.MapFrom(src => src.image_name != null ? UrlGenerator.GetAssetImagesURL(src.image_name) : null));

            CreateMap<Asset, GetSubcomponentAssetsToAddinAssetResponsemodel>()
                .ForMember(des => des.asset_name, opt => opt.MapFrom(src => src.name));


            CreateMap<Equipment, GetAllEquipmentListResponsemodel>()
                .ForMember(des => des.equipment_id, opt => opt.MapFrom(src => src.equipment_id));
            CreateMap<Equipment, FormEquipmentsOffline>();
            CreateMap<Asset, GetAssetsLocationDetailsResponseModel>()

                .ForMember(des => des.asset_details_url, opt => opt.MapFrom(src => UrlGenerator.GetAssetDetailsURL(src.asset_id, GenericRequestModel.domain_refer)))
                .ForMember(des => des.formio_building_name, opt => opt.MapFrom(src => src.AssetFormIOBuildingMappings.FormIOBuildings.formio_building_name))
                .ForMember(des => des.formio_floor_name, opt => opt.MapFrom(src => src.AssetFormIOBuildingMappings.FormIOFloors.formio_floor_name))
                .ForMember(des => des.formio_room_name, opt => opt.MapFrom(src => src.AssetFormIOBuildingMappings.FormIORooms.formio_room_name))
                .ForMember(des => des.formio_section_name, opt => opt.MapFrom(src => src.AssetFormIOBuildingMappings.FormIOSections.formio_section_name))
            .ForMember(des => des.asset_class_name, opt => opt.MapFrom(src => src.InspectionTemplateAssetClass.asset_class_name))
            .ForMember(des => des.asset_class_code, opt => opt.MapFrom(src => src.InspectionTemplateAssetClass.asset_class_code))
            .ForMember(des => des.pm_plan_name, opt => opt.MapFrom(src => src.AssetPMs.Where(x=>!x.is_archive && x.status != (int)Status.Completed).FirstOrDefault().AssetPMPlans.plan_name));


            CreateMap<Asset, GetTopLevelAssetsResponseModel>()
                .ForMember(des => des.formio_building_name, opt => opt.MapFrom(src => src.AssetFormIOBuildingMappings.FormIOBuildings.formio_building_name))
                .ForMember(des => des.formio_floor_name, opt => opt.MapFrom(src => src.AssetFormIOBuildingMappings.FormIOFloors.formio_floor_name))
                .ForMember(des => des.formio_room_name, opt => opt.MapFrom(src => src.AssetFormIOBuildingMappings.FormIORooms.formio_room_name))
                .ForMember(des => des.formio_section_name, opt => opt.MapFrom(src => src.AssetFormIOBuildingMappings.FormIOSections.formio_section_name))
                .ForMember(des => des.subLevel_components, opt => opt.MapFrom(src => src.AssetSubLevelcomponentMapping));

            CreateMap<WolineToplevelAssetOffline, WOlineTopLevelcomponentMapping>();
            CreateMap<WolineSublevelAssetOffline, WOlineSubLevelcomponentMapping>();
            CreateMap<AssetToplevelAssetOffline, AssetTopLevelcomponentMapping>();
            CreateMap<AssetSublevelAssetOffline, AssetSubLevelcomponentMapping>();

            CreateMap< WOlineTopLevelcomponentMapping, WolineToplevelAssetOffline>();
            CreateMap<WOlineSubLevelcomponentMapping, WolineSublevelAssetOffline>();
            CreateMap<AssetTopLevelcomponentMapping, AssetToplevelAssetOffline>();
            CreateMap<AssetSubLevelcomponentMapping, AssetSublevelAssetOffline>();

            CreateMap<AssetSubLevelcomponentMapping, SubLevelComponentAssets>()
                .ForMember(des => des.image_name_url, opt => opt.MapFrom(src => src.image_name != null ? UrlGenerator.GetAssetImagesURL(src.image_name) : null));


            CreateMap<Equipment, FormEquipmentsOffline>();

            CreateMap<Asset, GetAssetsbyLocationHierarchyResponsemodel>()
                .ForMember(des => des.asset_class_code, opt => opt.MapFrom(src => src.InspectionTemplateAssetClass.asset_class_code))
                .ForMember(des => des.asset_class_type, opt => opt.MapFrom(src => src.InspectionTemplateAssetClass.FormIOType != null ? src.InspectionTemplateAssetClass.FormIOType.form_type_name : null))
                .ForMember(des => des.asset_name, opt => opt.MapFrom(src => src.name))
                 .ForMember(des => des.asset_class_name, opt => opt.MapFrom(src => src.InspectionTemplateAssetClass.asset_class_name))
                 .ForMember(des => des.building, opt => opt.MapFrom(src => src.AssetFormIOBuildingMappings.FormIOBuildings.formio_building_name))
                 .ForMember(des => des.floor, opt => opt.MapFrom(src => src.AssetFormIOBuildingMappings.FormIOFloors.formio_floor_name))
                 .ForMember(des => des.room, opt => opt.MapFrom(src => src.AssetFormIOBuildingMappings.FormIORooms.formio_room_name))
                 .ForMember(des => des.section, opt => opt.MapFrom(src => src.AssetFormIOBuildingMappings.FormIOSections.formio_section_name))
                 .ForMember(des => des.formiosection_id, opt => opt.MapFrom(src => src.AssetFormIOBuildingMappings.FormIOSections.formiosection_id))
                 .ForMember(des => des.subLevel_components, opt => opt.MapFrom(src => src.AssetSubLevelcomponentMapping.Where(x=>!x.is_deleted).ToList()));

            CreateMap<TempFormIOBuildings, temp_WOlineBuildings>()
                 .ForPath(des => des.temp_floors, opt => opt.MapFrom(src => src.TempFormIOFloors.Where(x => x.site_id != Guid.Empty && x.site_id != null && !x.is_deleted).OrderBy(x=>x.temp_formio_floor_name)));

            CreateMap<FormIOBuildings, temp_WOlineBuildings>()
                 .ForMember(des => des.temp_formio_building_name, opt => opt.MapFrom(src => src.formio_building_name));

            CreateMap<TempFormIOFloors, temp_WOlineFloors>()
                 .ForPath(des => des.temp_formio_building_name, opt => opt.MapFrom(src => src.TempFormIOBuildings.temp_formio_building_name))
                 .ForPath(des => des.temp_rooms, opt => opt.MapFrom(src => src.TempFormIORooms.Where(x => x.site_id != Guid.Empty && x.site_id != null && !x.is_deleted).OrderBy(x=>x.temp_formio_room_name)));

            CreateMap<FormIOFloors, temp_WOlineFloors>()
                 .ForMember(des => des.temp_formio_floor_name, opt => opt.MapFrom(src => src.formio_floor_name));

            CreateMap<TempFormIORooms, temp_WOlineRooms>()
                 .ForPath(des => des.temp_formio_floor_name, opt => opt.MapFrom(src => src.TempFormIOFloors.temp_formio_floor_name));

            CreateMap<FormIORooms, temp_WOlineRooms>()
                 .ForMember(des => des.temp_formio_room_name, opt => opt.MapFrom(src => src.formio_room_name));

            //temp master locations
            CreateMap<TempMasterBuilding, temp_master_building_class>()
                .ForMember(des => des.building_name, opt => opt.MapFrom(src => src.temp_master_building_name))
                 .ForPath(des => des.temp_master_floor, opt => opt.MapFrom(src => src.TempMasterFloor.Where(x => x.site_id != Guid.Empty && x.site_id != null && !x.is_deleted)));

            CreateMap<FormIOBuildings, temp_master_building_class>()
                 .ForMember(des => des.building_name, opt => opt.MapFrom(src => src.formio_building_name));

            CreateMap<TempMasterFloor, temp_master_floor_class>()
                .ForMember(des => des.floor_name, opt => opt.MapFrom(src => src.temp_master_floor_name))
                 .ForPath(des => des.temp_master_rooms, opt => opt.MapFrom(src => src.TempMasterRoom.Where(x => x.site_id != Guid.Empty && x.site_id != null && !x.is_deleted)));

            CreateMap<FormIOFloors, temp_master_floor_class>()
                 .ForMember(des => des.floor_name, opt => opt.MapFrom(src => src.formio_floor_name));

            CreateMap<TempMasterRoom, temp_master_room_class>()
                .ForMember(des => des.room_name, opt => opt.MapFrom(src => src.temp_master_room_name));

            CreateMap<FormIORooms, temp_master_room_class>()
                 .ForMember(des => des.room_name, opt => opt.MapFrom(src => src.formio_room_name));

            CreateMap<WOOnboardingAssets, GetWOOBAssetsbyLocationHierarchyResponseModel>()
                .ForMember(des => des.temp_building, opt => opt.MapFrom(src => src.WOOBAssetTempFormIOBuildingMapping.TempFormIOBuildings.temp_formio_building_name))
                .ForMember(des => des.temp_floor, opt => opt.MapFrom(src => src.WOOBAssetTempFormIOBuildingMapping.TempFormIOFloors.temp_formio_floor_name))
                .ForMember(des => des.temp_room, opt => opt.MapFrom(src => src.WOOBAssetTempFormIOBuildingMapping.TempFormIORooms.temp_formio_room_name))
                .ForMember(des => des.temp_section, opt => opt.MapFrom(src => src.WOOBAssetTempFormIOBuildingMapping.TempFormIOSections.temp_formio_section_name))
                .ForMember(des => des.temp_master_building, opt => opt.MapFrom(src => src.TempAsset.TempMasterBuilding!=null?src.TempAsset.TempMasterBuilding.temp_master_building_name:src.building))
                .ForMember(des => des.temp_master_floor, opt => opt.MapFrom(src => src.TempAsset.TempMasterFloor!=null?src.TempAsset.TempMasterFloor.temp_master_floor_name:src.floor))
                .ForMember(des => des.temp_master_room, opt => opt.MapFrom(src => src.TempAsset.TempMasterRoom!=null?src.TempAsset.TempMasterRoom.temp_master_room_name:src.room))
                .ForMember(des => des.temp_master_section, opt => opt.MapFrom(src => src.TempAsset.temp_master_section))
                .ForMember(des => des.asset_profile_image, opt => opt.MapFrom(src => src.WOOnboardingAssetsImagesMapping.Where(x => x.asset_photo_type == (int)AssetPhotoType.Asset_Profile&&!x.is_deleted).FirstOrDefault()!=null ? UrlGenerator.GetAssetImagesURL(src.WOOnboardingAssetsImagesMapping.Where(x => x.asset_photo_type == (int)AssetPhotoType.Asset_Profile && !x.is_deleted).FirstOrDefault().asset_photo) : null))
                .ForMember(des => des.status_name, opt => opt.MapFrom(src => src.StatusMaster.status_name))
                 .ForMember(des => des.arc_flash_label_valid, opt => opt.MapFrom(src => src.TempAsset.arc_flash_label_valid))
                 .ForMember(des => des.maintenance_index_type, opt => opt.MapFrom(src => src.TempAsset.maintenance_index_type))
                 .ForMember(des => des.form_nameplate_info, opt => opt.MapFrom(src => src.TempAsset.form_nameplate_info))
                 ;

            CreateMap<PMItemMasterForms, GetPMMasterFormsOffline>();
            CreateMap<PMs, PMmasterOffline>();
            CreateMap<PMPlans, PMPlanMasterOfffline>();
            CreateMap<PMCategory, PMCategoryMasterOffline>();
            CreateMap<ActiveAssetPMWOlineMapping, GetPMSubmittedDataOffline>();
            CreateMap<AssetPMsTriggerConditionMapping, AssetPMTriggerConditionmappingoffline>();

            CreateMap<TempFormIOBuildings, TempLocationBuildingOffline>();
            CreateMap<TempFormIOFloors, TempLocationFloorOffline>();
            CreateMap<TempFormIORooms, TempLocationRoomOffline>();
            CreateMap<TempFormIOSections, TempLocationSectionOffline>();
           
            CreateMap<TempMasterBuilding, TempMasterBuildingMappingOffline>();
            CreateMap<TempMasterFloor, TempMasterFloorMappingOffline>();
            CreateMap<TempMasterRoom, TempMasterRoomMappingOffline>();
            CreateMap<TempMasterBuildingWOMapping, TempMasterBuildingWOMappingOffline>();
            CreateMap<TempMasterFloorWOMapping, TempMasterFloorWOMappingOffline>();
            CreateMap<TempMasterRoomWOMapping, TempMasterRoomWOMappingOffline>();
          
            CreateMap<WorkOrderTechnicianMapping, workorder_technician_mapping_class>();
           
            CreateMap<WOOBAssetTempFormIOBuildingMapping, TempLocationWolineMappingOffline>();
            CreateMap<WorkOrderTechnicianMapping, workorder_technician_mapping_class>();

            CreateMap<SitewalkthroughTempPmEstimation, SitewalkthroughTempPmEstimationOffline>()
            .ForMember(dest => dest.sitewalkthrough_temp_pm_estimation_id, opt => opt.MapFrom(src => src.sitewalkthrough_temp_pm_estimation_id))
            .ForMember(dest => dest.tempasset_id, opt => opt.MapFrom(src => src.tempasset_id))
            .ForMember(dest => dest.pm_plan_id, opt => opt.MapFrom(src => src.pm_plan_id))
            .ForMember(dest => dest.title, opt => opt.MapFrom(src => src.PMPlans != null ? src.PMPlans.plan_name : null))
            .ForMember(dest => dest.pm_id, opt => opt.MapFrom(src => src.pm_id))
            .ForMember(dest => dest.pm_name, opt => opt.MapFrom(src => src.PMs != null ? src.PMs.title : null))
            .ForMember(dest => dest.woonboardingassets_id, opt => opt.MapFrom(src => src.woonboardingassets_id))
            .ForMember(dest => dest.estimation_time, opt => opt.MapFrom(src => src.estimation_time!= null ? src.estimation_time : null))
            .ForMember(dest => dest.inspectiontemplate_asset_class_id, opt => opt.MapFrom(src => src.inspectiontemplate_asset_class_id))
            .ForMember(dest => dest.is_deleted, opt => opt.MapFrom(src => src.is_deleted));

            CreateMap<UpdateAssetPMOffline, AssetPMs>();

            CreateMap<TempAssetPMs, TempAssetPMsMappingOffline>();


            CreateMap<TempActiveAssetPMWOlineMapping, TempActiveAssetPMWOlineMappingOffline>();
            CreateMap<TempActiveAssetPMWOlineMappingOffline, TempActiveAssetPMWOlineMapping>();

            CreateMap<TempAsset, TempAssetWOlineMappingOffline>();

            CreateMap<WOlineIssueImagesMapping, asset_woline_issue_images>();

            CreateMap<TempLocationBuildingOffline , TempFormIOBuildings>();
            CreateMap<TempLocationFloorOffline, TempFormIOFloors>();
            CreateMap<TempLocationRoomOffline , TempFormIORooms>();

            CreateMap<TempLocationWolineMappingOffline , WOOBAssetTempFormIOBuildingMapping>();

            CreateMap<ClientCompany, GetAllClientCompanyWithSitesResponseModel>()
                .ForMember(des => des.client_company_code, opt => opt.MapFrom(src => src.clientcompany_code))
                .ForMember(des => des.list_of_site, opt => opt.MapFrom(src => src.Sites.Where(x => x.status != (int)Status.Disposed)));
                 

            CreateMap<Sites, Site_Data>()
                 .ForMember(des => des.customer, opt => opt.MapFrom(src => src.customer))
                 .ForMember(des => des.customer_address, opt => opt.MapFrom(src => src.customer_address))
                 .ForMember(des => des.is_add_asset_class_enabled, opt => opt.MapFrom(src => src.isAddAssetClassEnabled))
                 .ForMember(des => des.site_id, opt => opt.MapFrom(src => src.site_id))
                 .ForMember(des => des.company_id, opt => opt.MapFrom(src => src.company_id))
                 .ForMember(des => des.site_code, opt => opt.MapFrom(src => src.site_code))
                 .ForMember(des => des.status, opt => opt.MapFrom(src => src.status))
                .ForMember(des => des.site_name, opt => opt.MapFrom(src => src.site_name))
                .ForMember(des => des.profile_image, opt => opt.MapFrom(src => src.profile_image))
                .ForMember(des => des.site_projectmanager_list, opt => opt.MapFrom(src => src.SiteProjectManagerMapping.Where(x=>!x.is_deleted)));

            CreateMap<SiteProjectManagerMapping, SiteProjectManagerMapping_View_Class>()
                .ForMember(des => des.email, opt => opt.MapFrom(src => src.User.email))
                .ForMember(des => des.name, opt => opt.MapFrom(src => src.User.firstname+" "+src.User.lastname))
                ;

            CreateMap<UpdatePMSubmittedDataOffline, ActiveAssetPMWOlineMapping>();

            CreateMap<WOOnboardingAssets, GetOBWOAssetsOfRequestedAssetResponseModel>()
                 .ForMember(des => des.manual_wo_number, opt => opt.MapFrom(src => src.WorkOrders.manual_wo_number))
                 .ForMember(des => des.asset_name, opt => opt.MapFrom(src => src.Asset!=null ? src.Asset.name : src.asset_name))
                 .ForMember(des => des.workOrderStatus, opt => opt.MapFrom(src => src.WorkOrders.status))
                 .ForMember(des => des.asset_class_code, opt => opt.MapFrom(src => src.asset_class_code))
                 .ForMember(des => des.asset_class_name, opt => opt.MapFrom(src => src.asset_class_name))
                 //.ForMember(des => des.inspection_verdict, opt => opt.MapFrom(src => src.insp))
                 //.ForMember(des => des.inspection_verdict, opt => opt.MapFrom(src => src.insp))
                 .ForMember(des => des.inspected_at, opt => opt.MapFrom(src => src.inspected_at != null ? src.inspected_at : src.modified_at))
                 .ForMember(des => des.inspection_type, opt => opt.MapFrom(src => src.inspection_type != null || src.inspection_type > 0 ? src.inspection_type : 1))
                 .ForMember(des => des.wo_type, opt => opt.MapFrom(src => src.WorkOrders.wo_type))
                 .ForMember(des => des.asset_pm_id, opt => opt.MapFrom(src => src.ActiveAssetPMWOlineMapping.asset_pm_id))
                 .ForMember(des => des.pm_id, opt => opt.MapFrom(src => src.ActiveAssetPMWOlineMapping.AssetPMs.pm_id))
                 .ForMember(des => des.asset_pm_title, opt => opt.MapFrom(src => src.ActiveAssetPMWOlineMapping.AssetPMs.title))
                 //.ForMember(des => des.building, opt => opt.MapFrom(src => !String.IsNullOrEmpty(src.building) ? src.building : src.TempAsset.TempFormIOBuildings.temp_formio_building_name))
                 //.ForMember(des => des.floor, opt => opt.MapFrom(src => !String.IsNullOrEmpty(src.floor) ? src.floor : src.TempAsset.TempFormIOFloors.temp_formio_floor_name))
                 //.ForMember(des => des.room, opt => opt.MapFrom(src => !String.IsNullOrEmpty(src.room) ? src.room : src.TempAsset.TempFormIORooms.temp_formio_room_name))
                 //.ForMember(des => des.section, opt => opt.MapFrom(src => !String.IsNullOrEmpty(src.section) ? src.section : src.TempAsset.TempFormIOSections.temp_formio_section_name))
                 .ForMember(des => des.building, opt => opt.MapFrom(src => src.TempAsset.TempMasterBuilding!=null ? src.TempAsset.TempMasterBuilding.temp_master_building_name:src.building))
                 .ForMember(des => des.floor, opt => opt.MapFrom(src => src.TempAsset.TempMasterFloor!=null?src.TempAsset.TempMasterFloor.temp_master_floor_name:src.floor))
                 .ForMember(des => des.room, opt => opt.MapFrom(src => src.TempAsset.TempMasterRoom!=null ? src.TempAsset.TempMasterRoom.temp_master_room_name:src.room))
                 .ForMember(des => des.section, opt => opt.MapFrom(src => src.TempAsset.temp_master_section))
                 ;

            CreateMap<WOOnboardingAssets, GetAllWOOBAssetsByAssetIdResponseModel>()
                  //.ForMember(des => des.temp_building, opt => opt.MapFrom(src => src.WOOBAssetTempFormIOBuildingMapping.TempFormIOBuildings.temp_formio_building_name))
                  //.ForMember(des => des.temp_floor, opt => opt.MapFrom(src => src.WOOBAssetTempFormIOBuildingMapping.TempFormIOFloors.temp_formio_floor_name))
                  //.ForMember(des => des.temp_room, opt => opt.MapFrom(src => src.WOOBAssetTempFormIOBuildingMapping.TempFormIORooms.temp_formio_room_name))
                  //.ForMember(des => des.temp_section, opt => opt.MapFrom(src => src.WOOBAssetTempFormIOBuildingMapping.TempFormIOSections.temp_formio_section_name))
                  ;

            CreateMap<AssetFormIO, GetAssetFormIOByAssetIdResponseModel>()
                  .ForMember(des => des.wo_type, opt => opt.MapFrom(src => src.WorkOrders.wo_type))
                  .ForMember(des => des.WOcategorytoTaskMapping_id, opt => opt.MapFrom(src => src.WOcategorytoTaskMapping.WOcategorytoTaskMapping_id))
                ;
            CreateMap<WOOnboardingAssets, GetOBIRImagesByWOIdResponseModel>()
                  .ForMember(des => des.ob_ir_Image_label_list, opt => opt.MapFrom(src => src.IRWOImagesLabelMapping.Where(x=>!x.is_deleted)))
                  .ForMember(des => des.asset_name, opt => opt.MapFrom(src => src.TempAsset!=null ? src.TempAsset.asset_name : src.asset_name))
                  .ForMember(des => des.asset_class_code, opt => opt.MapFrom(src => src.TempAsset!=null && src.TempAsset.InspectionTemplateAssetClass!=null ? src.TempAsset.InspectionTemplateAssetClass.asset_class_code : src.asset_class_code))
                  .ForMember(des => des.asset_class_name, opt => opt.MapFrom(src => src.TempAsset!=null && src.TempAsset.InspectionTemplateAssetClass!=null ? src.TempAsset.InspectionTemplateAssetClass.asset_class_name : src.asset_class_name))
                  ;

            CreateMap<Asset, GetAssetPMListAssetWiseResponsemodel>()
                  .ForMember(des => des.asset_id, opt => opt.MapFrom(src => src.asset_id))
                  .ForMember(des => des.name, opt => opt.MapFrom(src => src.name))
                  .ForMember(des => des.asset_pms_list, opt => opt.MapFrom(src => src.AssetPMs));

            CreateMap<AssetPMs, asset_pm_of_asset>()
                   .ForMember(des => des.asset_pm_id, opt => opt.MapFrom(src => src.asset_pm_id))
                   .ForMember(des => des.asset_pm_plan_id, opt => opt.MapFrom(src => src.asset_pm_plan_id))
                   .ForMember(des => des.pm_id, opt => opt.MapFrom(src => src.pm_id))
                   .ForMember(des => des.title, opt => opt.MapFrom(src => src.title))
                   .ForMember(des => des.status, opt => opt.MapFrom(src => src.status));


            CreateMap<PMs, GetPMsListByAssetClassIdResponseModel>()
                 .ForPath(des => des.pm_plan_id, opt => opt.MapFrom(src => src.PMPlans.pm_plan_id))
                 .ForPath(des => des.plan_name, opt => opt.MapFrom(src => src.PMPlans.plan_name))
                 .ForPath(des => des.pm_category_id, opt => opt.MapFrom(src => src.PMPlans.pm_category_id))
                 .ForPath(des => des.category_name, opt => opt.MapFrom(src => src.PMPlans.PMCategory.category_name));

            CreateMap<AssetProfileImages, AssetImageDetails>()
                .ForMember(des => des.asset_image_name, opt => opt.MapFrom(src => src.asset_photo))
                .ForMember(des => des.asset_image_url, opt => opt.MapFrom(src => src.asset_photo != null ? UrlGenerator.GetAssetImagesURL(src.asset_photo) : null))
                ;

            CreateMap<InspectionTemplateAssetClass, FilterDropdownAssetPMListClassCodeList>()
              .ForMember(des => des.inspectiontemplate_asset_class_id, opt => opt.MapFrom(src => src.inspectiontemplate_asset_class_id))
              .ForMember(des => des.asset_class_name, opt => opt.MapFrom(src => src.asset_class_name))
              .ForMember(des => des.asset_class_code, opt => opt.MapFrom(src => src.asset_class_code))
              ;

            CreateMap<AssetFormIOBuildingMappings, GetLocationDataByAssetIdResponseModel>()
               .ForMember(des => des.formio_building_name, opt => opt.MapFrom(src => src.FormIOBuildings.formio_building_name))
               .ForMember(des => des.formio_floor_name, opt => opt.MapFrom(src => src.FormIOFloors.formio_floor_name))
               .ForMember(des => des.formio_room_name, opt => opt.MapFrom(src => src.FormIORooms.formio_room_name))
               .ForMember(des => des.formio_section_name, opt => opt.MapFrom(src => src.FormIOSections.formio_section_name));

            CreateMap<Asset, MainAssetListtoAddIssue>()
                .ForMember(des => des.asset_name, opt => opt.MapFrom(src => src.name))
                .ForMember(des => des.QR_code, opt => opt.MapFrom(src => src.QR_code))
                ;

            CreateMap<WOOnboardingAssets, TempAssetListtoAddIssue>()
                .ForMember(des => des.asset_name, opt => opt.MapFrom(src => src.asset_name))
                .ForMember(des => des.QR_code, opt => opt.MapFrom(src => src.QR_code))
                ;


            CreateMap<InspectionTemplateAssetClass, FilterDropdownAssetPMListClassCodeList>()
                .ForMember(des => des.inspectiontemplate_asset_class_id, opt => opt.MapFrom(src => src.inspectiontemplate_asset_class_id))
                .ForMember(des => des.asset_class_name, opt => opt.MapFrom(src => src.asset_class_name))
                .ForMember(des => des.asset_class_code, opt => opt.MapFrom(src => src.asset_class_code))
                ;


            CreateMap<WorkOrders, GetAllCalanderWorkordersResponseModel>()
                .ForMember(des => des.wo_number, opt => opt.MapFrom(src => src.wo_number.ToString()))
                .ForMember(des => des.due_date, opt => opt.MapFrom(src => src.due_at))
                .ForMember(des => des.due_in, opt => opt.MapFrom(src => src.wo_due_time_duration))
                .ForMember(des => des.manual_wo_number, opt => opt.MapFrom(src => src.manual_wo_number))
                .ForMember(des => des.status, opt => opt.MapFrom(src => src.status))
                .ForMember(des => des.responsible_party_name, opt => opt.MapFrom(src => src.ResponsibleParty!=null ? src.ResponsibleParty.responsible_party_name:null))
                .ForMember(des => des.wo_assigned_leads_list, opt => opt.MapFrom(src => src.WorkOrderBackOfficeUserMapping.Where(x=>!x.is_deleted)))
                .ForMember(des => des.wo_assigned_technicians_list, opt => opt.MapFrom(src => src.WorkOrderTechnicianMapping.Where(x => !x.is_deleted)))
                .ForMember(des => des.wo_vendors_list, opt => opt.MapFrom(src => src.WorkordersVendorContactsMapping.Where(x => !x.is_deleted).GroupBy(g=>g.vendor_id).Select(x=>x.First())))
                ;
            CreateMap<WorkOrderTechnicianMapping, workorder_assigned_users_list>()
                .ForMember(des => des.name, opt => opt.MapFrom(src => src.TechnicianUser.firstname + " " + src.TechnicianUser.lastname))
                .ForMember(des => des.email, opt => opt.MapFrom(src => src.TechnicianUser.email));
            CreateMap<WorkOrderBackOfficeUserMapping, workorder_assigned_users_list>()
                .ForMember(des => des.name, opt => opt.MapFrom(src => src.BackOfficeUser.firstname + " " + src.BackOfficeUser.lastname))
                .ForMember(des => des.email, opt => opt.MapFrom(src => src.BackOfficeUser.email));
            CreateMap<WorkordersVendorContactsMapping, wo_vendors_list_class>()
                .ForMember(des => des.vendor_id, opt => opt.MapFrom(src => src.vendor_id))
                .ForMember(des => des.vendor_name, opt => opt.MapFrom(src => src.Vendors.vendor_name))
                .ForMember(des => des.vendor_email, opt => opt.MapFrom(src => src.Vendors.vendor_email));

            CreateMap<AssetFormIOBuildingMappings, GetLocationDataByAssetIdResponseModel>()
               .ForMember(des => des.formio_building_name, opt => opt.MapFrom(src => src.FormIOBuildings.formio_building_name))
               .ForMember(des => des.formio_floor_name, opt => opt.MapFrom(src => src.FormIOFloors.formio_floor_name))
               .ForMember(des => des.formio_room_name, opt => opt.MapFrom(src => src.FormIORooms.formio_room_name))
               .ForMember(des => des.formio_section_name, opt => opt.MapFrom(src => src.FormIOSections.formio_section_name));


            CreateMap<Asset, GetOBWOAssetDetailsByIdResponsemodel>()
                  .ForMember(des => des.asset_id, opt => opt.MapFrom(src => src.asset_id))
                  .ForMember(des => des.asset_name, opt => opt.MapFrom(src => src.name))
                  .ForMember(des => des.status_name, opt => opt.MapFrom(src => src.StatusMaster.status_name))
                  .ForMember(des => des.qr_code, opt => opt.MapFrom(src => src.QR_code))
                  .ForMember(des => des.location, opt => opt.MapFrom(src => src.asset_placement))
                  //.ForMember(des => des.mwo_asset_id, opt => opt.MapFrom(src => src.asset_id))
                  //.ForMember(des => des.mwo_asset_name, opt => opt.MapFrom(src => src.Asset.name))
                  .ForMember(des => des.formiobuilding_id, opt => opt.MapFrom(src => src.AssetFormIOBuildingMappings.formiobuilding_id))
                  .ForMember(des => des.formiofloor_id, opt => opt.MapFrom(src => src.AssetFormIOBuildingMappings.formiofloor_id))
                  .ForMember(des => des.formioroom_id, opt => opt.MapFrom(src => src.AssetFormIOBuildingMappings.formioroom_id))

                  .ForMember(des => des.building, opt => opt.MapFrom(src => src.AssetFormIOBuildingMappings.FormIOBuildings.formio_building_name))
                  .ForMember(des => des.floor, opt => opt.MapFrom(src => src.AssetFormIOBuildingMappings.FormIOFloors.formio_floor_name))
                  .ForMember(des => des.room, opt => opt.MapFrom(src => src.AssetFormIOBuildingMappings.FormIORooms.formio_room_name))
                  .ForMember(des => des.section, opt => opt.MapFrom(src => src.AssetFormIOBuildingMappings.FormIOSections.formio_section_name))

                  //.ForMember(des => des.temp_master_building, opt => opt.MapFrom(src => src.TempAsset.TempMasterBuilding != null ? src.TempAsset.TempMasterBuilding.temp_master_building_name : src.building))
                  //.ForMember(des => des.temp_master_floor, opt => opt.MapFrom(src => src.TempAsset.TempMasterFloor != null ? src.TempAsset.TempMasterFloor.temp_master_floor_name : src.floor))
                  //.ForMember(des => des.temp_master_room, opt => opt.MapFrom(src => src.TempAsset.TempMasterRoom != null ? src.TempAsset.TempMasterRoom.temp_master_room_name : src.room))
                  //.ForMember(des => des.temp_master_section, opt => opt.MapFrom(src => src.TempAsset.temp_master_section))

                  //.ForMember(des => des.temp_formiobuilding_id, opt => opt.MapFrom(src => src.WOOBAssetTempFormIOBuildingMapping.temp_formiobuilding_id))
                  //.ForMember(des => des.temp_formiofloor_id, opt => opt.MapFrom(src => src.WOOBAssetTempFormIOBuildingMapping.temp_formiofloor_id))
                  //.ForMember(des => des.temp_formioroom_id, opt => opt.MapFrom(src => src.WOOBAssetTempFormIOBuildingMapping.temp_formioroom_id))
                  //.ForMember(des => des.temp_formiosection_id, opt => opt.MapFrom(src => src.WOOBAssetTempFormIOBuildingMapping.temp_formiosection_id))

                  //.ForMember(des => des.building, opt => opt.MapFrom(src => !String.IsNullOrEmpty(src.building) ? src.building : src.WOOBAssetTempFormIOBuildingMapping != null ? src.WOOBAssetTempFormIOBuildingMapping.TempFormIOBuildings.temp_formio_building_name : ""))
                  //.ForMember(des => des.floor, opt => opt.MapFrom(src => !String.IsNullOrEmpty(src.floor) ? src.floor : src.WOOBAssetTempFormIOBuildingMapping != null ? src.WOOBAssetTempFormIOBuildingMapping.TempFormIOFloors.temp_formio_floor_name : ""))
                  //.ForMember(des => des.room, opt => opt.MapFrom(src => !String.IsNullOrEmpty(src.room) ? src.room : src.WOOBAssetTempFormIOBuildingMapping != null ? src.WOOBAssetTempFormIOBuildingMapping.TempFormIORooms.temp_formio_room_name : ""))
                  //.ForMember(des => des.section, opt => opt.MapFrom(src => !String.IsNullOrEmpty(src.section) ? src.section : src.WOOBAssetTempFormIOBuildingMapping != null ? src.WOOBAssetTempFormIOBuildingMapping.TempFormIOSections.temp_formio_section_name : ""))

                  //.ForMember(des => des.temp_building, opt => opt.MapFrom(src => src.WOOBAssetTempFormIOBuildingMapping.TempFormIOBuildings.temp_formio_building_name))
                  //.ForMember(des => des.temp_floor, opt => opt.MapFrom(src => src.WOOBAssetTempFormIOBuildingMapping.TempFormIOFloors.temp_formio_floor_name))
                  //.ForMember(des => des.temp_room, opt => opt.MapFrom(src => src.WOOBAssetTempFormIOBuildingMapping.TempFormIORooms.temp_formio_room_name))
                  //.ForMember(des => des.temp_section, opt => opt.MapFrom(src => src.WOOBAssetTempFormIOBuildingMapping.TempFormIOSections.temp_formio_section_name))

                  .ForMember(des => des.condition_index_type_name, opt => opt.MapFrom(src => src.condition_index_type > 0  ? ((condition_index_type)src.condition_index_type.Value).ToString() : null))
                  .ForMember(des => des.criticality_index_type_name, opt => opt.MapFrom(src => src.criticality_index_type > 0 ? ((criticality_index_type)src.criticality_index_type.Value).ToString() : null))
                  .ForMember(des => des.thermal_classification_name, opt => opt.MapFrom(src => src.thermal_classification_id > 0 ? ((thermal_classification)src.thermal_classification_id.Value).ToString() : null))
                  .ForMember(des => des.asset_image_list, opt => opt.MapFrom(src => src.AssetProfileImages))
                  //.ForMember(des => des.ob_ir_Image_label_list, opt => opt.MapFrom(src => src.AssetIRWOImagesLabelMapping))
                  .ForMember(des => des.wo_ob_asset_fed_by_mapping, opt => opt.MapFrom(src => src.AssetParentHierarchyMapping))
                  .ForMember(des => des.wo_ob_asset_toplevelcomponent_mapping, opt => opt.MapFrom(src => src.AssetTopLevelcomponentMapping))
                  .ForMember(des => des.wo_ob_asset_sublevelcomponent_mapping, opt => opt.MapFrom(src => src.AssetSubLevelcomponentMapping))
                  .ForMember(des => des.asset_class_code, opt => opt.MapFrom(src => src.InspectionTemplateAssetClass.asset_class_code))
                  .ForMember(des => des.asset_class_name, opt => opt.MapFrom(src => src.InspectionTemplateAssetClass.asset_class_name))
                  //.ForMember(des => des.dynmic_fields_json, opt => opt.MapFrom(src => src.dynmic_fields_json != null ? src.dynmic_fields_json : "{}"))
                  .ForMember(des => des.form_nameplate_info, opt => opt.MapFrom(src => src.form_retrived_nameplate_info ))
                  ;

            CreateMap<AssetProfileImages, OBWOAssetImages>()
                .ForMember(des => des.asset_photo, opt => opt.MapFrom(src => UrlGenerator.GetAssetImagesURL(src.asset_photo)))
                .ForMember(des => des.asset_thumbnail_photo, opt => opt.MapFrom(src => UrlGenerator.GetAssetImagesURL(src.asset_thumbnail_photo)))
                ;
            //CreateMap<AssetIRWOImagesLabelMapping, View_OBIRImage_label>()
            //    .ForMember(des => des.ir_image_label_url, opt => opt.MapFrom(src => UrlGenerator.GetAssetImagesURL(src.ir_image_label)))
            //    .ForMember(des => des.visual_image_label_url, opt => opt.MapFrom(src => UrlGenerator.GetAssetImagesURL(src.visual_image_label)))
            //    ;

            CreateMap<AssetParentHierarchyMapping, WOOBAssetFedByMap>();
            CreateMap<AssetTopLevelcomponentMapping, WOOBAssetToplevelAssetMapping>()
                 .ForMember(des => des.is_toplevelcomponent_from_ob_wo, opt => opt.MapFrom(src => false))
                ;
            CreateMap<AssetSubLevelcomponentMapping, WOOBAssetSublevelAssetMapping>()
                //.ForMember(des => des.image_url, opt => opt.MapFrom(src => UrlGenerator.GetAssetImagesURL(src.image_name)))
                ;

            CreateMap<WorkOrders, GetInProgressWorkordersListResponseModel>()
                 .ForMember(des => des.site_name, opt => opt.MapFrom(src => src.Sites.site_name))
                 .ForMember(des => des.time_elapsed, opt => opt.MapFrom(src => src.modified_at != null ? DateTimeUtil.GetDueInNewFlow(src.modified_at.Value.Date, DateTime.UtcNow.Date) : DateTimeUtil.GetDueInNewFlow(src.created_at.Value.Date, DateTime.UtcNow.Date)))
                 .ForMember(des => des.time_elapsed_for_overdue_wo, opt => opt.MapFrom(src => (src.due_at != null && src.due_at != DateTime.MinValue) ? DateTimeUtil.GetDueInNewFlow(src.due_at.Date, DateTime.UtcNow.Date) : null));

            CreateMap<WOOnboardingAssets, GetInProgressWorkordersListResponseModel>()
                 .ForMember(des => des.site_name, opt => opt.MapFrom(src => src.Sites.site_name))
                 .ForMember(des => des.asset_name, opt => opt.MapFrom(src => src.Asset != null ? src.Asset.name : src.asset_name ))
                 .ForMember(des => des.time_elapsed, opt => opt.MapFrom(src => src.modified_at != null ? DateTimeUtil.GetDueInNewFlow(src.modified_at.Value.Date, DateTime.UtcNow.Date) : DateTimeUtil.GetDueInNewFlow(src.created_at.Value.Date, DateTime.UtcNow.Date)))
                  ;

            CreateMap<AssetFormIO, GetInProgressWorkordersListResponseModel>()
                 .ForMember(des => des.site_name, opt => opt.MapFrom(src => src.Sites.site_name))
                 .ForMember(des => des.asset_name, opt => opt.MapFrom(src => src.Asset != null ? src.Asset.name : src.asset_form_name))
                 .ForMember(des => des.time_elapsed, opt => opt.MapFrom(src => src.modified_at != null ? DateTimeUtil.GetDueInNewFlow(src.modified_at.Value.Date, DateTime.UtcNow.Date) : DateTimeUtil.GetDueInNewFlow(src.created_at.Value.Date, DateTime.UtcNow.Date)))
                 ;
            CreateMap<GetOBWOAssetDetailsByIdResponsemodel, UpdateOBWOAssetDetailsRequestmodel>()
                ;

            CreateMap<TempIssueUpdateOffline, WOLineIssue>();
            CreateMap<UpdateTempAssetOffline, TempAsset>();

            CreateMap<TempIssueImageMappingUpdtaeoffline, WOlineIssueImagesMapping>();

            CreateMap<TimeMaterials, TimeMaterials_Data>();
            
            CreateMap<User, watcher_users_list>()
                .ForMember(des => des.user_id, opt => opt.MapFrom(src => src.uuid))
                .ForMember(des => des.username, opt => opt.MapFrom(src => src.firstname + " " + src.lastname))
                ;

            CreateMap<WOOnboardingAssets, WOOnboardingAssets>()
                .ForMember(x => x.woonboardingassets_id, y => y.Ignore())
                .ForMember(x => x.AssetIssue, y => y.Ignore());

            CreateMap<Asset, TopLevelAssetData>()
                .ForMember(des => des.asset_name, opt => opt.MapFrom(src => src.name));

            CreateMap<Asset, SubLevelAssetData>()
                .ForMember(des => des.asset_name, opt => opt.MapFrom(src => src.name))
                .ForMember(des => des.toplevelcomponent_asset_id, opt => opt.MapFrom(src => src.AssetTopLevelcomponentMapping
                        .Where(x=>!x.is_deleted).Select(x=>x.toplevelcomponent_asset_id).FirstOrDefault())) ;

            CreateMap<WOOnboardingAssets, TopLevelAssetData>();

            CreateMap<WOOnboardingAssets, SubLevelAssetData>()
                .ForMember(des => des.toplevelcomponent_asset_id, opt => opt.MapFrom(src => src.WOlineTopLevelcomponentMapping
                        .Where(x => !x.is_deleted).Select(x => x.toplevelcomponent_asset_id).FirstOrDefault()));

            CreateMap<ResponsibleParty, GetAllResponsibleParty_Data>();

            CreateMap<NetaInspectionBulkReportTracking, GetAllNetaInspectionBulkReportTrackingListResponseModel>();
           
            CreateMap<Sites, ActiveUserSites_Data>()
                .ForMember(des => des.site_contact_details, opt => opt.MapFrom(src => src.SiteContact))
                .ForMember(des => des.client_company_name, opt => opt.MapFrom(src => src.ClientCompany.client_company_name));


            CreateMap<SiteContact, Site_Contact_Details_Obj>();
                //.ForMember(des => des.sitecontact_id, opt => opt.MapFrom(src => src.sitecontact_id))
                //.ForMember(des => des.sitecontact_name, opt => opt.MapFrom(src => src.sitecontact_name))
                //.ForMember(des => des.sitecontact_email, opt => opt.MapFrom(src => src.sitecontact_email))
                //.ForMember(des => des.sitecontact_phone, opt => opt.MapFrom(src => src.sitecontact_phone))
                //.ForMember(des => des.sitecontact_title, opt => opt.MapFrom(src => src.sitecontact_title));


            CreateMap<Role, ActiveUserRoles_Data>()
                .ForMember(des => des.role_name, opt => opt.MapFrom(src => src.name));
            CreateMap<ClientCompany, ActiveClintCompany_Data>()
                .ForMember(des => des.client_company_Usersites, opt => opt.MapFrom(src => src.Sites.Where(x=>x.status==(int)Status.Active)))
                ;
            CreateMap<Sites, ClientCompany_Site_Data>();


            CreateMap<User, GetActiveUserSitesAndRolesResponseModel>()
                .ForMember(des => des.active_site_id, opt => opt.MapFrom(src => src.ac_active_site))
                .ForMember(des => des.default_site_id, opt => opt.MapFrom(src => src.ac_default_site))
                .ForMember(des => des.active_site_name, opt => opt.MapFrom(src => src.Active_Site.site_name))
                .ForMember(des => des.default_site_name, opt => opt.MapFrom(src => src.Site.site_name))
                .ForMember(des => des.default_client_company_id, opt => opt.MapFrom(src => src.ac_default_client_company))
                .ForMember(des => des.active_client_company_id, opt => opt.MapFrom(src => src.ac_active_client_company))
                .ForPath(des => des.default_client_company_name, opt => opt.MapFrom(src => src.default_client_Company.client_company_name))
                .ForPath(des => des.active_client_company_name, opt => opt.MapFrom(src => src.Active_client_Company.client_company_name))
                .ForMember(des => des.default_role_id_web, opt => opt.MapFrom(src => src.ac_default_role_web))
                .ForMember(des => des.default_role_name_web, opt => opt.MapFrom(src => src.Role_Web.name))
                .ForMember(des => des.active_role_id_web, opt => opt.MapFrom(src => src.ac_active_role_web))
                .ForMember(des => des.active_role_name_web, opt => opt.MapFrom(src => src.Active_Role_Web.name))
                ;

            CreateMap<WOOnboardingAssets, GetOBAssetWithQRCode_Class>()
                .ForMember(des => des.asset_name, opt => opt.MapFrom(src => src.TempAsset != null ? src.TempAsset.asset_name : src.asset_name!=null?src.asset_name:src.Asset!=null?src.Asset.name:null))
                .ForMember(des => des.QR_code, opt => opt.MapFrom(src => src.TempAsset != null ? src.TempAsset.QR_code : src.QR_code))
                ;

            CreateMap<AssetListExclude, FilterAssetOptimizedResponsemodel>();
            CreateMap<AssetlocationHierarchyExclude, FilterAssetRoomFloorBuildingLocationOptionsmapping>();

            CreateMap<User, GetAllTechniciansListResponseModel>();

            CreateMap<WOlistExcludeProperties, GetAllWorkOrdersNewflowOptimizedResponsemodel>()
                .ForMember(des => des.wo_type_name, opt => opt.MapFrom(src => src.wo_type_name.Replace("WO","")));

            CreateMap<AssetPMs, AssetPMs>()
                .ForMember(x => x.asset_pm_id, y => y.Ignore())
                //.ForMember(des => des.AssetPMTasks, opt => opt.MapFrom(src => src.AssetPMTasks))
                .ForMember(des => des.AssetPMAttachments, opt => opt.MapFrom(src => src.AssetPMAttachments))
                .ForMember(des => des.AssetPMsTriggerConditionMapping, opt => opt.MapFrom(src => src.AssetPMsTriggerConditionMapping));

            CreateMap<AssetPMAttachments, AssetPMAttachments>()
                .ForMember(x => x.asset_pm_attachment_id, y => y.Ignore());

            CreateMap<AssetPMsTriggerConditionMapping, AssetPMsTriggerConditionMapping>()
                .ForMember(x => x.asset_pm_trigger_condition_mapping_id, y => y.Ignore());
            CreateMap<AssetPMListExcludeProperties, GetAssetPMListOptimizedResponsemodel>();

            //CreateMap<Sites, ActiveUserSites_Data>()
            //    .ForMember(des => des.client_company_name, opt => opt.MapFrom(src => src.ClientCompany.client_company_name));
            CreateMap<Role, ActiveUserRoles_Data>()
                .ForMember(des => des.role_name, opt => opt.MapFrom(src => src.name));

            CreateMap<ClientCompany, ActiveClintCompany_Data>()
                .ForMember(des => des.client_company_Usersites, opt => opt.MapFrom(src => src.Sites.Where(x => x.status == (int)Status.Active)));

            CreateMap<Sites, ClientCompany_Site_Data>();

            CreateMap<User, GetActiveUserSitesAndRolesResponseModel>()
                .ForMember(des => des.active_site_id, opt => opt.MapFrom(src => src.ac_active_site))
                .ForMember(des => des.default_site_id, opt => opt.MapFrom(src => src.ac_default_site))
                .ForMember(des => des.active_site_name, opt => opt.MapFrom(src => src.Active_Site.site_name))
                .ForMember(des => des.default_site_name, opt => opt.MapFrom(src => src.Site.site_name))
                .ForMember(des => des.default_client_company_id, opt => opt.MapFrom(src => src.ac_default_client_company))
                .ForMember(des => des.active_client_company_id, opt => opt.MapFrom(src => src.ac_active_client_company))
                .ForPath(des => des.default_client_company_name, opt => opt.MapFrom(src => src.default_client_Company.client_company_name))
                .ForPath(des => des.active_client_company_name, opt => opt.MapFrom(src => src.Active_client_Company.client_company_name))
                .ForMember(des => des.default_role_id_web, opt => opt.MapFrom(src => src.ac_default_role_web))
                .ForMember(des => des.default_role_name_web, opt => opt.MapFrom(src => src.Role_Web.name))
                .ForMember(des => des.active_role_id_web, opt => opt.MapFrom(src => src.ac_active_role_web))
                .ForMember(des => des.active_role_name_web, opt => opt.MapFrom(src => src.Active_Role_Web.name))
                ;

            CreateMap<SiteDocuments, GetAllSiteDocumentResponsemodel>()
                .ForPath(des => des.file_url, opt => opt.MapFrom(src => UrlGenerator.GetSiteDocumentUrl(src.file_name , src.s3_folder_name)))
                ;

            CreateMap<TempMasterBuilding, temp_master_building_class>()
                 .ForPath(des => des.building_name, opt => opt.MapFrom(src => src.temp_master_building_name));

            CreateMap<Vendors, ViewVendorDetailsByIdResponseModel>()
                 .ForPath(des => des.contacts_list, opt => opt.MapFrom(src => src.Contacts.Where(x=>!x.is_deleted).OrderByDescending(x=>x.created_at)));
           
            CreateMap<Contacts, Contacts_Data_Model>();
            CreateMap<Vendors, Vendors_Data>()
                .ForMember(dest => dest.vendor_id, opt => opt.MapFrom(src => src.vendor_id))
                .ForMember(dest => dest.vendor_name, opt => opt.MapFrom(src => src.vendor_name))
                .ForMember(dest => dest.vendor_email, opt => opt.MapFrom(src => src.vendor_email))
                .ForMember(dest => dest.vendor_phone_number, opt => opt.MapFrom(src => src.vendor_phone_number))
                .ForMember(dest => dest.vendor_category, opt => opt.MapFrom(src => src.vendor_category))
                .ForMember(dest => dest.vendor_address, opt => opt.MapFrom(src => src.vendor_address))
                ;

            CreateMap<AssetGroup, AssetGroupClass_Obj>();

            CreateMap<Asset, Asset_Obj>()
                .ForMember(dest => dest.asset_id, opt => opt.MapFrom(src => src.asset_id))
                .ForMember(dest => dest.asset_name, opt => opt.MapFrom(src => src.name))
                ;
        }
    }
}

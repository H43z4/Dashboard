using Inquiry.Services;
using Inquiry.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models.ViewModels.Identity;
using SharedLib.APIs;
using SharedLib.Common;
using SharedLib.Security;

namespace Reports.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = AuthenticationSchemes.JWT_BEARER_TOKEN_STATELESS)]
    public class InquiryController : Controller
    {
        private readonly IInquiryService _service;
        public VwUser User
        {
            get
            {
                return (VwUser)this.Request.HttpContext.Items["User"];
            }
        }

        public InquiryController(IInquiryService service)
        {
            this._service = service;
        }
        [HttpGet(Name = "GetVehicleDetailsAgainstRegNo")]
        public async Task<ApiResponse> GetVehicleDetailsAgainstRegNo(string registrationNo)
        {
            this._service.VwUser = this.User;

            var ds = await this._service.GetVehicleDetailsAgainstRegNo(registrationNo);

            if (ds.Tables.Count == 0 || ds.Tables[0].Rows.Count == 0)
            {
                return ApiResponse.GetApiResponse(ApiResponseType.FAILED, null, Constants.NOT_FOUND_MESSAGE);
            }

            var vehicle = SharedLib.Common.Extentions.ToList<ImVehicle>(ds.Tables[0]).FirstOrDefault();


            return ApiResponse.GetApiResponse(ApiResponseType.SUCCESS, vehicle, Constants.RECORD_FOUND_MESSAGE);
        }
        [HttpGet(Name = "GetOwnersAgainstRegNo")]
        public async Task<ApiResponse> GetOwnersAgainstRegNo(string registrationNo)
        {
            this._service.VwUser = this.User;

            var ds = await this._service.GetOwnersAgainstRegNo(registrationNo);

            if (ds.Tables.Count == 0 || ds.Tables[0].Rows.Count == 0)
            {
                return ApiResponse.GetApiResponse(ApiResponseType.FAILED, null, Constants.NOT_FOUND_MESSAGE);
            }

            var owner = SharedLib.Common.Extentions.ToList<OwnerApplicationInfo>(ds.Tables[0]).FirstOrDefault();
            if (owner is not null)
            {
                if (owner.OwnerTypeId == 1 )
                {
                owner.Persons = SharedLib.Common.Extentions.ToList<ImPerson>(ds.Tables[1]);
                } else if(owner.OwnerTypeId == 2 )
                {
                    owner.Business = SharedLib.Common.Extentions.ToList<ImBusiness>(ds.Tables[1]).FirstOrDefault();
                }
            }

            return ApiResponse.GetApiResponse(ApiResponseType.SUCCESS, owner, Constants.RECORD_FOUND_MESSAGE);
        }
        [HttpGet(Name = "GetPersonDetails")]
        public async Task<ApiResponse> GetPersonDetails(string personId)
        {
            this._service.VwUser = this.User;

            var ds = await this._service.GetPersonDetails(personId);

            if (ds.Tables.Count == 0 || ds.Tables[0].Rows.Count == 0)
            {
                return ApiResponse.GetApiResponse(ApiResponseType.FAILED, null, Constants.NOT_FOUND_MESSAGE);
            }

            var person = SharedLib.Common.Extentions.ToList<ImPerson>(ds.Tables[0]).FirstOrDefault();
            if (person is not null)
            {
                person.Addresses = SharedLib.Common.Extentions.ToList<ImAddress>(ds.Tables[1]);
                person.PhoneNumbers = SharedLib.Common.Extentions.ToList<ImPhoneNumber>(ds.Tables[2]);
            }

            return ApiResponse.GetApiResponse(ApiResponseType.SUCCESS, person, Constants.RECORD_FOUND_MESSAGE);
        }       
        [HttpGet(Name = "GetBusinessDetails")]
        public async Task<ApiResponse> GetBusinessDetails(string businessId)
        {
            this._service.VwUser = this.User;

            var ds = await this._service.GetBusinessDetails(businessId);

            if (ds.Tables.Count == 0 || ds.Tables[0].Rows.Count == 0)
            {
                return ApiResponse.GetApiResponse(ApiResponseType.FAILED, null, Constants.NOT_FOUND_MESSAGE);
            }

            var business = SharedLib.Common.Extentions.ToList<ImBusiness>(ds.Tables[0]).FirstOrDefault();
            if (business is not null)
            {
                business.Addresses = SharedLib.Common.Extentions.ToList<ImAddress>(ds.Tables[1]);
                business.PhoneNumbers = SharedLib.Common.Extentions.ToList<ImPhoneNumber>(ds.Tables[2]);
            }

            return ApiResponse.GetApiResponse(ApiResponseType.SUCCESS, business, Constants.RECORD_FOUND_MESSAGE);
        }
        [HttpGet(Name = "GetOwnershipHistory")]
        public async Task<ApiResponse> GetOwnershipHistory(string registrationNo)
        {
            this._service.VwUser = this.User;

            var ds = await this._service.GetOwnershipHistory(registrationNo);

            if (ds.Tables.Count == 0 || ds.Tables[0].Rows.Count == 0)
            {
                return ApiResponse.GetApiResponse(ApiResponseType.FAILED, null, Constants.NOT_FOUND_MESSAGE);
            }

            var ownershipHistory = SharedLib.Common.Extentions.ToList<ImOwnershipHistory>(ds.Tables[0]);

            return ApiResponse.GetApiResponse(ApiResponseType.SUCCESS, ownershipHistory, Constants.RECORD_FOUND_MESSAGE);
        }
        [HttpGet(Name = "GetTaxHistory")]
        public async Task<ApiResponse> GetTaxHistory(string registrationNo)
        {
            this._service.VwUser = this.User;

            var ds = await this._service.GetTaxHistory(registrationNo);

            if (ds.Tables.Count == 0 || ds.Tables[0].Rows.Count == 0)
            {
                return ApiResponse.GetApiResponse(ApiResponseType.FAILED, null, Constants.NOT_FOUND_MESSAGE);
            }

            var ownershipHistory = SharedLib.Common.Extentions.ToList<ImTaxHistory>(ds.Tables[0]);

            return ApiResponse.GetApiResponse(ApiResponseType.SUCCESS, ownershipHistory, Constants.RECORD_FOUND_MESSAGE);
        }
        [HttpGet(Name = "GetApplicationsListAgainstRegNo")]
        public async Task<ApiResponse> GetApplicationsListAgainstRegNo(string registrationNo)
        {
            this._service.VwUser = this.User;

            var ds = await this._service.GetApplicationsAgainstRegNo(registrationNo);

            if (ds.Tables.Count == 0 || ds.Tables[0].Rows.Count == 0)
            {
                return ApiResponse.GetApiResponse(ApiResponseType.FAILED, null, Constants.NOT_FOUND_MESSAGE);
            }

            var ownershipHistory = SharedLib.Common.Extentions.ToList<ImApplicationsList>(ds.Tables[0]);

            return ApiResponse.GetApiResponse(ApiResponseType.SUCCESS, ownershipHistory, Constants.RECORD_FOUND_MESSAGE);
        }
        [HttpGet(Name = "GetApplicationsLogsListAgainstRegNo")]
        public async Task<ApiResponse> GetApplicationsLogsListAgainstRegNo(long appId)
        {
            this._service.VwUser = this.User;

            var ds = await this._service.GetApplicationsLogsListAgainstRegNo(appId);

            if (ds.Tables.Count == 0 || ds.Tables[0].Rows.Count == 0)
            {
                return ApiResponse.GetApiResponse(ApiResponseType.FAILED, null, Constants.NOT_FOUND_MESSAGE);
            }

            var ownershipHistory = SharedLib.Common.Extentions.ToList<ImApplicationsList>(ds.Tables[0]);

            return ApiResponse.GetApiResponse(ApiResponseType.SUCCESS, ownershipHistory, Constants.RECORD_FOUND_MESSAGE);
        }
        [HttpGet(Name = "GetPrintingAgainstRegNo")]
        public async Task<ApiResponse> GetPrintingAgainstRegNo(string registrationNo)
        {
            this._service.VwUser = this.User;

            var ds = await this._service.GetPrintingAgainstRegNo(registrationNo);

            if (ds.Tables.Count == 0 || ds.Tables[0].Rows.Count == 0)
            {
                return ApiResponse.GetApiResponse(ApiResponseType.FAILED, null, Constants.NOT_FOUND_MESSAGE);
            }

            var ownershipHistory = SharedLib.Common.Extentions.ToList<ImApplicationsList>(ds.Tables[0]);

            return ApiResponse.GetApiResponse(ApiResponseType.SUCCESS, ownershipHistory, Constants.RECORD_FOUND_MESSAGE);
        }

        [HttpGet(Name = "GetStatusHistoryAgainstRegNo")]
        public async Task<ApiResponse> GetStatusHistoryAgainstRegNo(string registrationNo)
        {
            this._service.VwUser = this.User;

            var ds = await this._service.GetStatusHistoryAgainstRegNo(registrationNo);

            if (ds.Tables.Count == 0 || ds.Tables[0].Rows.Count == 0)
            {
                return ApiResponse.GetApiResponse(ApiResponseType.FAILED, null, Constants.NOT_FOUND_MESSAGE);
            }

            var data = new {
                applicationsList = ds.Tables[0]
            };

            return ApiResponse.GetApiResponse(ApiResponseType.SUCCESS, data, Constants.RECORD_FOUND_MESSAGE);
        }
        [HttpGet(Name = "GetVHLRemarksAgainstRegNo")]
        public async Task<ApiResponse> GetVHLRemarksAgainstRegNo(string registrationNo)
        {
            this._service.VwUser = this.User;

            var ds = await this._service.GetVHLRemarksAgainstRegNo(registrationNo);

            if (ds.Tables.Count == 0 || ds.Tables[0].Rows.Count == 0)
            {
                return ApiResponse.GetApiResponse(ApiResponseType.FAILED, null, Constants.NOT_FOUND_MESSAGE);
            }

            var data = new
            {
                VHLRemarksList = ds.Tables[0]
            };

            return ApiResponse.GetApiResponse(ApiResponseType.SUCCESS, data, Constants.RECORD_FOUND_MESSAGE);
        }
    }
}

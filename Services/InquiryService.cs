using Database;
using Models.ViewModels.Identity;
using SharedLib.Interfaces;
using System.Data;

namespace Inquiry.Services
{
    public interface IInquiryService : ICurrentUser
    {
        Task<DataSet> GetVehicleDetailsAgainstRegNo(string registrationNo);
        Task<DataSet> GetOwnersAgainstRegNo(string registrationNo);
        Task<DataSet> GetPersonDetails(string personId);
        Task<DataSet> GetBusinessDetails(string businessId);
        Task<DataSet> GetOwnershipHistory(string registrationNo);
        Task<DataSet> GetTaxHistory(string registrationNo);
        Task<DataSet> GetApplicationsAgainstRegNo(string registrationNo);
        Task<DataSet> GetApplicationsLogsListAgainstRegNo(long appId);
        Task<DataSet> GetPrintingAgainstRegNo(string registrationNo);
        Task<DataSet> GetDashboardCounts(string startDate, string endDate, int? districtId);
        Task<DataSet> GetDashboardTaxCounts(string startDate, string endDate, int? districtId);
        Task<DataSet> GetDashboardMonthlyCounts(string startDate, string endDate, int? districtId);
        Task<DataSet> GetDistrictsDropDowns();
        Task<DataSet> GetStatusHistoryAgainstRegNo(string registrationNo);
        Task<DataSet> GetVHLRemarksAgainstRegNo(string registrationNo);
    }
    public class InquiryService:IInquiryService
    {
        readonly IDBHelper dbHelper;
        public VwUser VwUser { get; set; }
        public InquiryService(IDBHelper dbHelper)
        {
            this.dbHelper = dbHelper;
        }

        public async Task<DataSet> GetVehicleDetailsAgainstRegNo(string registrationNo)
        {
            Dictionary<string, object> paramDict = new Dictionary<string, object>();

            paramDict.Add("@VehicleRegNo", registrationNo);

            var ds = await this.dbHelper.GetDataSetByStoredProcedure("[Reports].[GetVehicleDetails]", paramDict);

            return ds;
        }
        public async Task<DataSet> GetOwnersAgainstRegNo(string registrationNo)
        {
            Dictionary<string, object> paramDict = new Dictionary<string, object>();

            paramDict.Add("@VehicleRegNo", registrationNo);

            var ds = await this.dbHelper.GetDataSetByStoredProcedure("[Reports].[GetVehicleOwners]", paramDict);

            return ds;
        }        
        public async Task<DataSet> GetPersonDetails(string personId)
        {
            Dictionary<string, object> paramDict = new Dictionary<string, object>();

            paramDict.Add("@PersonId", personId);

            var ds = await this.dbHelper.GetDataSetByStoredProcedure("[Reports].[GetPersonDetail]", paramDict);

            return ds;
        }
        public async Task<DataSet> GetBusinessDetails(string businessId)
        {
            Dictionary<string, object> paramDict = new Dictionary<string, object>();

            paramDict.Add("@BusinessId", businessId);

            var ds = await this.dbHelper.GetDataSetByStoredProcedure("[Reports].[GetBusinessDetail]", paramDict);

            return ds;
        }        
        public async Task<DataSet> GetOwnershipHistory(string registrationNo)
        {
            Dictionary<string, object> paramDict = new Dictionary<string, object>();

            paramDict.Add("@VehicleRegNo", registrationNo);

            var ds = await this.dbHelper.GetDataSetByStoredProcedure("[Reports].[GetOwnershipHistory]", paramDict);

            return ds;
        }   
        public async Task<DataSet> GetTaxHistory(string registrationNo)
        {
            Dictionary<string, object> paramDict = new Dictionary<string, object>();

            paramDict.Add("@VehicleRegNo", registrationNo);

            var ds = await this.dbHelper.GetDataSetByStoredProcedure("[Reports].[GetTaxHistory]", paramDict);

            return ds;
        }        
        public async Task<DataSet> GetApplicationsAgainstRegNo(string registrationNo)
        {
            Dictionary<string, object> paramDict = new Dictionary<string, object>();

            paramDict.Add("@VehicleRegNo", registrationNo);

            var ds = await this.dbHelper.GetDataSetByStoredProcedure("[Reports].[GetApplicationsAgainstVehicle]", paramDict);

            return ds;
        }        
        public async Task<DataSet> GetApplicationsLogsListAgainstRegNo(long appId)
        {
            Dictionary<string, object> paramDict = new Dictionary<string, object>();

            paramDict.Add("@ApplicationId", appId);

            var ds = await this.dbHelper.GetDataSetByStoredProcedure("[Reports].[GetApplicationLogsAgainstVehicle]", paramDict);

            return ds;
        }       
        public async Task<DataSet> GetPrintingAgainstRegNo(string registrationNo)
        {
            Dictionary<string, object> paramDict = new Dictionary<string, object>();

            paramDict.Add("@VehicleRegNo", registrationNo);

            var ds = await this.dbHelper.GetDataSetByStoredProcedure("[Reports].[GetPrintingAgainstVehicle]", paramDict);

            return ds;
        }
        public async Task<DataSet> GetStatusHistoryAgainstRegNo(string registrationNo)
        {
            Dictionary<string, object> paramDict = new Dictionary<string, object>();

            paramDict.Add("@VehicleRegNo", registrationNo);

            var ds = await this.dbHelper.GetDataSetByStoredProcedure("[Reports].[GetStatusHistoryAgainstVehicle]", paramDict);

            return ds;
        }
        public async Task<DataSet> GetVHLRemarksAgainstRegNo(string registrationNo)
        {
            Dictionary<string, object> paramDict = new Dictionary<string, object>();

            paramDict.Add("@VehicleRegNo", registrationNo);

            var ds = await this.dbHelper.GetDataSetByStoredProcedure("[Reports].[GetVHLRemarksAgainstVehicle]", paramDict);

            return ds;
        }
        public async Task<DataSet> GetDistrictsDropDowns()
        {
            var ds = await this.dbHelper.GetDataSetByStoredProcedure("[Setup].[GetDistrictList]", null);

            return ds;
        }
        public async Task<DataSet> GetDashboardCounts(string startDate, string endDate, int? districtId)
        {
            Dictionary<string, object> paramDict = new Dictionary<string, object>();

            paramDict.Add("@DistrictId", districtId == 0 ? null: districtId);
            paramDict.Add("@StartDate", startDate);
            paramDict.Add("@EndDate", endDate);

            var ds = await this.dbHelper.GetDataSetByStoredProcedure("[Reports].[GetDashboardCounts]", paramDict);

            return ds;
        }
        public async Task<DataSet> GetDashboardTaxCounts(string startDate, string endDate, int? districtId)
        {
            Dictionary<string, object> paramDict = new Dictionary<string, object>();

            paramDict.Add("@DistrictId", districtId == 0 ? null: districtId);
            paramDict.Add("@StartDate", startDate);
            paramDict.Add("@EndDate", endDate);

            var ds = await this.dbHelper.GetDataSetByStoredProcedure("[Reports].[GetDashboardTaxCounts]", paramDict);

            return ds;
        }
        public async Task<DataSet> GetDashboardMonthlyCounts(string startDate, string endDate, int? districtId)
        {
            Dictionary<string, object> paramDict = new Dictionary<string, object>();

            paramDict.Add("@DistrictId", districtId == 0 ? null : districtId);

            var ds = await this.dbHelper.GetDataSetByStoredProcedure("[Reports].[GetDashboardMonthlyCounts]", paramDict);

            return ds;
        }
    }
}

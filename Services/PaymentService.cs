using Models.ViewModels.Identity;
using Database;
using SharedLib.Interfaces;
using System.Data;
using Models.ViewModels.Payment;
//using Models.DatabaseModels.VehicleRegistration.Core;

namespace Reports.Services
{
    public interface IPaymentService : ICurrentUser
    {
        Task<DataSet> GenerateChallan(long applicationId);
        Task<DataSet> GenerateChallan(long businessProcessId, long applicationId, long assessmentBaseId);
        Task<DataSet> GetPayeeInfo(long applicationId);
        Task<VwPayeeInfo> GetPSId(VwPayeeInfo payeeInfo);
        Task<DataSet> SavePSId(VwPayeeInfo payeeInfo);
        Task<DataSet> SaveEPayTask(long businessProcessId, long applicationId, long challanId);
        Task<DataSet> GetChallanApplications(long id, DateTime? _appDate);
        Task<DataSet> GetPSIDStatusDropDown();
        Task<DataSet> UpdatePsidStatus(long epayTaskId, long statusId);
    }

    public class PaymentService : IPaymentService
    {
        readonly IDBHelper dbHelper;
        public VwUser VwUser { get; set; }

        public PaymentService(IDBHelper dbHelper)
        {
            this.dbHelper = dbHelper;
        }

        public async Task<DataSet> GenerateChallan(long applicationId)
        {
            Dictionary<string, object> paramDict = new Dictionary<string, object>();
            paramDict.Add("@ApplicationId", applicationId);
            paramDict.Add("@UserId", this.VwUser.UserId);

            var ds = await this.dbHelper.GetDataSetByStoredProcedure("[Payments].[GenerateChallan]", paramDict);

            return ds;
        }

        public async Task<DataSet> GenerateChallan(long businessProcessId, long applicationId, long assessmentBaseId)
        {
            Dictionary<string, object> paramDict = new Dictionary<string, object>();
            paramDict.Add("@BusinessProcessId", businessProcessId);
            paramDict.Add("@ApplicationId", applicationId);
            paramDict.Add("@AssessmentBaseId", assessmentBaseId);
            paramDict.Add("@UserId", this.VwUser.UserId);

            var ds = await this.dbHelper.GetDataSetByStoredProcedure("[Common].[GenerateChallan]", paramDict);

            return ds;
        }

        public async Task<DataSet> GetPayeeInfo(long applicationId)
        {
            Dictionary<string, object> paramDict = new Dictionary<string, object>();
            paramDict.Add("@ApplicationId", applicationId);

            var ds = await this.dbHelper.GetDataSetByStoredProcedure("[Payments].[GetPayeeInfo]", paramDict);

            return ds;
        }

        public async Task<VwPayeeInfo> GetPSId(VwPayeeInfo payeeInfo)
        {
            payeeInfo.PSId = new Random().NextInt64(1, 9999999).ToString();
            return payeeInfo;
        }

        public async Task<DataSet> SavePSId(VwPayeeInfo payeeInfo)
        {
            Dictionary<string, object> paramDict = new Dictionary<string, object>();
            paramDict.Add("@ApplicationId", payeeInfo.ApplicationId);
            paramDict.Add("@ChallanId", payeeInfo.ChallanId);
            paramDict.Add("@PSId", payeeInfo.PSId);
            paramDict.Add("@UserId", this.VwUser.UserId);

            var ds = await this.dbHelper.GetDataSetByStoredProcedure("[Payments].[SavePSId]", paramDict);

            return ds;
        }

        public async Task<DataSet> SaveEPayTask(long businessProcessId, long applicationId, long challanId)
        {
            Dictionary<string, object> paramDict = new Dictionary<string, object>();
            paramDict.Add("@BusinessProcessId", businessProcessId);
            paramDict.Add("@ApplicationId", applicationId);
            paramDict.Add("@ChallanId", challanId);
            paramDict.Add("@UserId", this.VwUser.UserId);

            var ds = await this.dbHelper.GetDataSetByStoredProcedure("[epay].[SaveEPayTask]", paramDict);

            return ds;
        }

        public async Task<DataSet> GetChallanApplications(long id, DateTime? _appDate)
        {
            Dictionary<string, object> paramDict = new Dictionary<string, object>();
            string x = _appDate?.Date.ToString("yyyy-MM-dd");
            paramDict.Add("@PSIDStatus", id);
            paramDict.Add("@CreatedAt", _appDate);
            paramDict.Add("@CreatedBy", this.VwUser.UserId);

            var ds = await this.dbHelper.GetDataSetByStoredProcedure("[epay].[GetChallanApplications]", paramDict);
            return ds;
        }

        public async Task<DataSet> GetPSIDStatusDropDown()
        {
            var ds = await this.dbHelper.GetDataSetByStoredProcedure("[epay].[GetPSIDStatus]", null);
            return ds;
        }

        public async Task<DataSet> UpdatePsidStatus(long epayTaskId, long statusId)
        {
            Dictionary<string, object> paramDict = new Dictionary<string, object>();
            paramDict.Add("@UserId", this.VwUser.UserId);
            paramDict.Add("@EpayTaskId", epayTaskId);
            paramDict.Add("@statusId", statusId);


            var ds = await this.dbHelper.GetDataSetByStoredProcedure("[epay].[UpdatePSIDStatus]", paramDict);

            return ds;

        }

    }
}

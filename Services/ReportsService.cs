//using Models.DatabaseModels.VehicleRegistration.Core;
using Models.ViewModels.Identity;
using Models.ViewModels.VehicleRegistration.Core;
using Microsoft.EntityFrameworkCore;
using Database;
using SharedLib.Interfaces;
using System.Data;
using Microsoft.Data.SqlClient;
using SharedLib.Common;
//using Models.DatabaseModels.VehicleRegistration.Setup;

namespace Reports.Services
{
    public interface IReportsService : ICurrentUser
    {
        Task<DataSet> GetApplicationDetails(long applicationId);
        Task<DataSet> GetChallanDetail(long applicationId);
        Task<DataSet> SaveApplicationPhase(VwApplicationPhaseChange vwApplicationPhaseChange);
    }

    public class ReportsService : IReportsService
    {
        readonly IDBHelper dbHelper;
        public VwUser VwUser { get; set; }

        public ReportsService(IDBHelper dbHelper)
        {
            this.dbHelper = dbHelper;
        }
        
        public async Task<DataSet> GetApplicationDetails(long applicationId)
        {
            Dictionary<string, object> paramDict = new Dictionary<string, object>();
            paramDict.Add("@ApplicationId", applicationId);
            paramDict.Add("@UserId", this.VwUser.UserId);

            var ds = await this.dbHelper.GetDataSetByStoredProcedure("[Core].[GetApplicationDetailExt]", paramDict);

            return ds;
        }
        
        public async Task<DataSet> GetChallanDetail(long applicationId)
        {
            Dictionary<string, object> paramDict = new Dictionary<string, object>();
            paramDict.Add("@ApplicationId", applicationId);

            var ds = await this.dbHelper.GetDataSetByStoredProcedure("[Core].[GetChallanDetail]", paramDict);

            return ds;
        }

        public async Task<DataSet> SaveApplicationPhase(VwApplicationPhaseChange vwApplicationPhaseChange)
        {
            Dictionary<string, object> paramDict = new Dictionary<string, object>();
            paramDict.Add("@ApplicationId", vwApplicationPhaseChange.ApplicationId);
            paramDict.Add("@BusinessEventId", vwApplicationPhaseChange.BusinessEventId);
            paramDict.Add("@UserId", this.VwUser.UserId);
            paramDict.Add("@Remarks", vwApplicationPhaseChange.RemarksStatement);

            var ds = await this.dbHelper.GetDataSetByStoredProcedure("[Core].[SaveApplicationPhase]", paramDict);

            return ds;
        }
    }
}

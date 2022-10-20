using Database;
using Models.ViewModels.Identity;
using Biometric.ViewModels;
using SharedLib.Interfaces;
using System.Data;

namespace Biometric.Services
{
    public interface IBiometricService : ICurrentUser
    {
        Task<DataSet> GetVehicleInfo(InputModel inputModel);
        Task<DataSet> SaveBiometricInfo(InputModel inputModel);
    }

    public class BiometricService : IBiometricService
    {
        readonly IDBHelper dbHelper;
        public VwUser VwUser { get; set; }
        public BiometricService(IDBHelper dbHelper)
        {
            this.dbHelper = dbHelper;
        }


        #region public-Methods

        public async Task<DataSet> GetVehicleInfo(InputModel inputModel)
        {
            Dictionary<string, object> paramDict = new Dictionary<string, object>();

            paramDict.Add("@RegNo", inputModel.RegNo);
            paramDict.Add("@ChasisNo", inputModel.ChasisNo);
            paramDict.Add("@EngineNo", inputModel.EngineNo);
            paramDict.Add("@UserId", this.VwUser.UserId);

            var ds = await this.dbHelper.GetDataSetByStoredProcedure("[Biometric].[GetVehicleInfo]", paramDict);

            return ds;
        }
        
        public async Task<DataSet> SaveBiometricInfo(InputModel inputModel)
        {
            Dictionary<string, object> paramDict = new Dictionary<string, object>();

            paramDict.Add("@RegNo", inputModel.RegNo);
            paramDict.Add("@ChasisNo", inputModel.ChasisNo);
            paramDict.Add("@EngineNo", inputModel.EngineNo);
            paramDict.Add("@UserId", this.VwUser.UserId);

            var ds = await this.dbHelper.GetDataSetByStoredProcedure("[Biometric].[SaveBiometricInfo]", paramDict);

            return ds;
        }

        #endregion
    }
}

using Models.DatabaseModels.VehicleRegistration.Core;
using Models.ViewModels.Identity;
using Models.ViewModels.VehicleRegistration.Core;
using Microsoft.EntityFrameworkCore;
using Database;
using SharedLib.Interfaces;
using System.Data;
using Microsoft.Data.SqlClient;
using SharedLib.Common;
using Models.DatabaseModels.VehicleRegistration.Setup;

namespace Reports.Services
{
    public interface IRegistrationService : ICurrentUser
    {
        Task<DataSet> GetOwnersDropDowns();
        Task<DataSet> GetVehiclesDropDowns();
        Task<DataSet> GetPurchaseDropDowns();
        Task<DataSet> GetDocumentDropDowns();
        Task<DataSet> GetAPFDropDowns();

        Task<DataSet> GetApplications(long? applicationId, long applicationTypeId, long applicationStatusId);
        Task<DataSet> GetApplicationDetails(long applicationId);
        Task<DataSet> GetOwnerDetails(long ownerId);
        Task<DataSet> GetVehicle(long vehicleId);
        Task<DataSet> GetVehiclePurchaseInfo(long vehicleId);
        Task<DataSet> GetVehicleLocalPurchaseInfo(long vehicleId);
        Task<DataSet> GetVehicleImportInfo(long vehicleId);
        Task<DataSet> GetVehicleAuctionInfo(long vehicleId);
        Task<DataSet> GetHPA(long applicationId);
        Task<DataSet> GetKeeper(long ownerId);


        Task<DataSet> GetPersonByCNIC(string cnic);
        Task<VwBusiness> GetBusinessByTaxNumber(string ntn, string ftn, string stn);
        Task<DataSet> GetAuctionedNumber(string chassisNumber);
        Task<DataSet> GetRemarks(long applicationId);
        Task<DataSet> GetAssessment(long applicationId);
        Task<DataSet> GetChallanDetail(long applicationId);
        Task<DataSet> GenerateChallan(long applicationId);
        Task<DataSet> GetVehicleOwnerInfo(string SearchCriteria); //Either Vehicle Reg_No or Application ID, Delivery Article Service
        Task<DataSet> GetVehicleOwnerInfoForScanning(string SearchCriteria);
        Task<VwApplication> SaveOwner(VwTempOwner_v1 owner);
        Task<DataSet> NRSaveOwner(VwTempOwner_v1 owner);
        Task<DataSet> NRSaveVehicle(VwVehicle vwVehicle);
        Task<DataSet> NRSaveVehiclePurchaseInfo(VwVehiclePurchaseInfo vehiclePurchaseInfo);
        Task<DataSet> NRSaveVehicleDocument(VwVehicleDocument vwVehicleDocument);
        Task<DataSet> NRSaveHPA(VwHPA vwTempHPA);
        Task<DataSet> NRSaveKeeper(VwKeeper vwTempKeeper);
        Task<Vehicle> SaveVehicle(VwVehicle vehicle);
        Task<VwVehiclePurchaseInfo> SaveVehiclePurchaseInfo(VwVehiclePurchaseInfo vehiclePurchaseInfo);
        Task<bool> SaveVehicleDocument(VwVehicleDocument vwVehicleDocument);
        Task<HPA> SaveHPA(VwHPA vwTempHPA);
        public Task<Keeper> SaveKeeper(VwKeeper vwTempKeeper);
        Task<Remarks> SaveRemarks(VwRemarks remarks);
        Task<DataSet> SaveApplicationPhase(VwApplicationPhaseChange vwApplicationPhaseChange);

        Task<long> SaveVehicleArticleDelivery(VwVehicleArticleDelivery vwVehicleArticleDelivery);
        Task<long> SaveVehicleDocumentUploadInfo(VehicleDocument vehicleDocument);

    }

    public class RegistrationService : IRegistrationService
    {
        readonly AppDbContext appDbContext;
        readonly IAdoNet adoNet;
        readonly IDBHelper dbHelper;
        public VwUser VwUser { get; set; }

        public RegistrationService(AppDbContext appDbContext, IAdoNet adoNet, IDBHelper dbHelper)
        {
            this.appDbContext = appDbContext;
            this.adoNet = adoNet;
            this.dbHelper = dbHelper;
        }


        #region private-Methods

        private async Task CommitTransaction()
        {
            using (var transaction = this.appDbContext.Database.BeginTransaction())
            {
                var rowsAffected = await this.appDbContext.SaveChangesAsync();
                await transaction.CommitAsync();
            }
        }

        #endregion


        #region public-Get-Dropdowns

        public async Task<DataSet> GetOwnersDropDowns()
        {
            var ds = await this.adoNet.ExecuteUsingDataAdapter("[Setup].[GetOwnersDropDowns]", null);

            return ds;
        }
        public async Task<DataSet> GetVehiclesDropDowns()
        {
            var ds = await this.adoNet.ExecuteUsingDataAdapter("[Setup].[GetVehiclesDropDowns]", null);

            return ds;
        }
        public async Task<DataSet> GetPurchaseDropDowns()
        {
            var ds = await this.adoNet.ExecuteUsingDataAdapter("[Setup].[GetPurchaseDropDowns]", null);

            return ds;
        }
        public async Task<DataSet> GetDocumentDropDowns()
        {
            var ds = await this.adoNet.ExecuteUsingDataAdapter("[Setup].[GetDocumentDropDowns]", null);

            return ds;
        }
        public async Task<DataSet> GetAPFDropDowns()
        {
            var ds = await this.adoNet.ExecuteUsingDataAdapter("[Setup].[GetAPFDropDowns]", null);

            return ds;
        }

        #endregion

        #region public-Get-Methods

        public async Task<DataSet> GetApplications(long? applicationId, long businessProcessId, long applicationStatusId)
        {
            SqlParameter[] sql = new SqlParameter[6];
            sql[0] = new SqlParameter("@ApplicationId", applicationId);
            sql[1] = new SqlParameter("@ApplicationStatusId", applicationStatusId);
            sql[2] = new SqlParameter("@BusinessProcessId", businessProcessId);
            sql[3] = new SqlParameter("@BusinessPhaseId", DBNull.Value);
            sql[4] = new SqlParameter("@BusinessPhaseStatusId", DBNull.Value);
            sql[5] = new SqlParameter("@CreatedBy", this.VwUser.UserId);

            var ds = await this.adoNet.ExecuteUsingDataAdapter("[Core].[GetApplications]", sql);

            return ds;
        }
        
        public async Task<DataSet> GetApplicationDetails(long applicationId)
        {
            SqlParameter[] sql = new SqlParameter[2];
            sql[0] = new SqlParameter("@ApplicationId", applicationId);
            sql[1] = new SqlParameter("@UserId", this.VwUser.UserId);

            var ds = await this.adoNet.ExecuteUsingDataAdapter("[Core].[GetApplicationDetailExt]", sql);

            return ds;
        }

        private async Task<DataSet> GetBusinessProcessInitialState(long businessProcessId)
        {
            SqlParameter[] sql = new SqlParameter[2];
            sql[0] = new SqlParameter("@BusinessProcessId", businessProcessId);
            sql[1] = new SqlParameter("@CreatedBy", this.VwUser.UserId);

            var ds = await this.adoNet.ExecuteUsingDataAdapter("[Core].[GetBusinessProcessInitialState]", sql);

            return ds;
        }

        public async Task<DataSet> GetOwnerDetails(long ownerId)
        {
            SqlParameter[] sql = new SqlParameter[1];
            sql[0] = new SqlParameter("@OwnerId", ownerId);

            var ds = await this.adoNet.ExecuteUsingDataAdapter("[Core].[GetOwnerDetail]", sql);

            return ds;
        }
        
        public async Task<DataSet> GetVehicle(long vehicleId)
        {
            SqlParameter[] sql = new SqlParameter[1];
            sql[0] = new SqlParameter("@VehicleId", vehicleId);

            var ds = await this.adoNet.ExecuteUsingDataAdapter("[Core].[GetVehicle]", sql);

            return ds;
        }
        
        public async Task<DataSet> GetVehiclePurchaseInfo(long vehicleId)
        {
            SqlParameter[] sql = new SqlParameter[1];
            sql[0] = new SqlParameter("@VehicleId", vehicleId);

            var ds = await this.adoNet.ExecuteUsingDataAdapter("[Core].[GetVehiclePurchaseInfo]", sql);

            return ds;
        }
        
        public async Task<DataSet> GetVehicleLocalPurchaseInfo(long vehicleId)
        {
            SqlParameter[] sql = new SqlParameter[1];
            sql[0] = new SqlParameter("@VehicleId", vehicleId);

            var ds = await this.adoNet.ExecuteUsingDataAdapter("[Core].[GetVehicleLocalPurchaseInfo]", sql);

            return ds;
        }
        
        public async Task<DataSet> GetVehicleImportInfo(long vehicleId)
        {
            SqlParameter[] sql = new SqlParameter[1];
            sql[0] = new SqlParameter("@VehicleId", vehicleId);

            var ds = await this.adoNet.ExecuteUsingDataAdapter("[Core].[GetVehicleImportInfo]", sql);

            return ds;
        }
        
        public async Task<DataSet> GetVehicleAuctionInfo(long vehicleId)
        {
            SqlParameter[] sql = new SqlParameter[1];
            sql[0] = new SqlParameter("@VehicleId", vehicleId);

            var ds = await this.adoNet.ExecuteUsingDataAdapter("[Core].[GetVehicleAuctionInfo]", sql);

            return ds;
        }
        
        public async Task<DataSet> GetHPA(long applicationId)
        {
            SqlParameter[] sql = new SqlParameter[1];
            sql[0] = new SqlParameter("@ApplicationId", applicationId);

            var ds = await this.adoNet.ExecuteUsingDataAdapter("[Core].[GetHPA]", sql);

            return ds;
        }

        public async Task<DataSet> GetKeeper(long ownerId)
        {
            SqlParameter[] sql = new SqlParameter[1];
            sql[0] = new SqlParameter("@OwnerId", ownerId);

            var ds = await this.adoNet.ExecuteUsingDataAdapter("[Core].[GetKeeper]", sql);

            return ds;
        }



        public async Task<DataSet> GetPersonByCNIC(string cnic)
        {
            SqlParameter[] sql = new SqlParameter[2];
            sql[0] = new SqlParameter("@PersonId", DBNull.Value);
            sql[1] = new SqlParameter("@CNIC", cnic);

            var ds = await this.adoNet.ExecuteUsingDataAdapter("[Core].[GetPerson]", sql);

            return ds;
        }
        
        public async Task<DataSet> GenerateChallan(long applicationId)
        {
            SqlParameter[] sql = new SqlParameter[1];
            sql[0] = new SqlParameter("@ApplicationId", applicationId);

            var ds = await this.adoNet.ExecuteUsingDataAdapter("[Payments].[GenerateChallan]", sql);

            return ds;
        }

        public Task<VwBusiness> GetBusinessByTaxNumber(string ntn, string ftn, string stn)
        {
            //var business = this.appDbContext.Business.SingleOrDefault(x => x.NTN == ntn || x.FTN == ftn || x.STN == stn);
            var business = this.appDbContext.Business.FirstOrDefault(x => x.NTN == ntn);

            var vwBusiness = new VwBusiness()
            {
                Addresses = new List<VwAddress>(),
                PhoneNumbers = new List<VwPhoneNumber>()
            };

            if (business is not null)
            {
                SharedLib.Common.EntityMapper<Business, VwBusiness>.CopyByPropertyName(business, vwBusiness);

                var addresses = this.appDbContext.Address.Where(x => x.BusinessId == business.BusinessId).ToList();
                var phoneNumbers = this.appDbContext.PhoneNumber.Where(x => x.BusinessId == business.BusinessId).ToList();

                foreach (var address in addresses)
                {
                    VwAddress vwTempAddress = new VwAddress();

                    SharedLib.Common.EntityMapper<Address, VwAddress>.CopyByPropertyName(address, vwTempAddress);

                    vwBusiness.Addresses.Add(vwTempAddress);
                }

                foreach (var phoneNumber in phoneNumbers)
                {
                    VwPhoneNumber vwTempPhoneNumber = new VwPhoneNumber();

                    SharedLib.Common.EntityMapper<PhoneNumber, VwPhoneNumber>.CopyByPropertyName(phoneNumber, vwTempPhoneNumber);

                    vwBusiness.PhoneNumbers.Add(vwTempPhoneNumber);
                }
            }
            else
            {
                vwBusiness = null;
            }

            return Task.FromResult(vwBusiness);
        }
        
        public async Task<DataSet> GetAuctionedNumber(string chassisNumber)
        {
            Dictionary<string, object> paramDict = new Dictionary<string, object>();
            paramDict.Add("@ChassisNumber", chassisNumber);

            var ds = await this.dbHelper.GetDataSetByStoredProcedure("[SRNRPL].[GetAuctionedNumber]", paramDict);

            return ds;
        }


        public async Task<DataSet> GetRemarks(long applicationId)
        {
            SqlParameter[] sql = new SqlParameter[2];
            sql[0] = new SqlParameter("@ApplicationId", applicationId);
            sql[1] = new SqlParameter("@UserId", this.VwUser.UserId);

            var ds = await this.adoNet.ExecuteUsingDataAdapter("[Core].[GetRemarks]", sql);

            return ds;
        }
        
        public async Task<DataSet> GetAssessment(long applicationId)
        {
            SqlParameter[] sql = new SqlParameter[1];
            sql[0] = new SqlParameter("@ApplicationId", applicationId);

            var ds = await this.adoNet.ExecuteUsingDataAdapter("[Core].[GetAssessment]", sql);

            return ds;
        }
        
        public async Task<DataSet> GetChallanDetail(long applicationId)
        {
            SqlParameter[] sql = new SqlParameter[1];
            sql[0] = new SqlParameter("@ApplicationId", applicationId);

            var ds = await this.adoNet.ExecuteUsingDataAdapter("[Core].[GetChallanDetail]", sql);

            return ds;
        }

        #endregion

        #region Public-Save-Methods

        public async Task<DataSet> NRSaveOwner(VwTempOwner_v1 vwOwner)
        {
            Dictionary<string, object> paramDict = new Dictionary<string, object>();
            //BusinessProcessId = 21 is for NR as 21 id is used in oracle.
            paramDict.Add("@Application", new List<Application>()
            {
                new Application()
                {
                    ApplicationId = vwOwner.ApplicationId.Value,
                    BusinessProcessId = 21,
                    DistrictId = this.VwUser.UserDistrictId.Value,
                    CreatedBy = this.VwUser.UserId,
                    ReceivedAt = DateTime.Now

                }
            }.ToDataTable());

            paramDict.Add("@Owner", new List<Owner>()
            {
                new Owner()
                {
                    OwnerId = vwOwner.OwnerId.Value,
                    OwnerTypeId = vwOwner.OwnerTypeId.Value,
                    OwnerTaxGroupId = vwOwner.OwnerTaxGroupId.Value,
                    CreatedBy = this.VwUser.UserId

                }
            }.ToDataTable());

            if (vwOwner.OwnerTypeId == 1)
            {
                var person = vwOwner.Persons.FirstOrDefault();
                var personDataModel = new Person();
                EntityMapper<VwPerson, Person>.CopyByPropertyNameAndType(person, personDataModel);

                var personList = new List<Person>();
                personList.Add(personDataModel);

                var addresses = new List<Address>();

                person.Addresses.ForEach(x =>
                {
                    var address = new Address();
                    EntityMapper<VwAddress, Address>.CopyByPropertyNameAndType(x, address);
                    addresses.Add(address);
                });

                var phoneNumbers = new List<PhoneNumber>();

                person.PhoneNumbers.ForEach(x =>
                {
                    var phoneNumber = new PhoneNumber();
                    EntityMapper<VwPhoneNumber, PhoneNumber>.CopyByPropertyNameAndType(x, phoneNumber);
                    phoneNumbers.Add(phoneNumber);
                });

                paramDict.Add("@Person", personList.ToDataTable());
                paramDict.Add("@Business", null);
                paramDict.Add("@Address", addresses.ToDataTable());
                paramDict.Add("@PhoneNumber", phoneNumbers.ToDataTable());
            }
            else if (vwOwner.OwnerTypeId == 2)
            {
                var business = new Business();
                EntityMapper<VwBusiness, Business>.CopyByPropertyNameAndType(vwOwner.Business, business);
                var businessList = new List<Business>();
                businessList.Add(business);

                var addresses = new List<Address>();

                vwOwner.Business.Addresses.ForEach(x =>
                {
                    var address = new Address();
                    EntityMapper<VwAddress, Address>.CopyByPropertyNameAndType(x, address);
                    addresses.Add(address);
                });

                var phoneNumbers = new List<PhoneNumber>();

                vwOwner.Business.PhoneNumbers.ForEach(x =>
                {
                    var phoneNumber = new PhoneNumber();
                    EntityMapper<VwPhoneNumber, PhoneNumber>.CopyByPropertyNameAndType(x, phoneNumber);
                    phoneNumbers.Add(phoneNumber);
                });

                paramDict.Add("@Person", null);
                paramDict.Add("@Business", businessList.ToDataTable());
                paramDict.Add("@Address", addresses.ToDataTable());
                paramDict.Add("@PhoneNumber", phoneNumbers.ToDataTable());
            }

            paramDict.Add("@UserId", this.VwUser.UserId);

            var ds = await this.dbHelper.GetDataSetByStoredProcedure("[Core].[NR_SaveOwner]", paramDict);

            return ds;
        }

        public async Task<DataSet> NRSaveVehicle(VwVehicle vwVehicle)
        {

            Dictionary<string, object> paramDict = new Dictionary<string, object>();

            paramDict.Add("@Vehicle", new List<Vehicle>()
            {
                new Vehicle()
                {
                    VehicleId = vwVehicle.VehicleId.Value,
                    VehicleBodyConventionId = vwVehicle.VehicleBodyConventionId,
                    VehicleBodyTypeId = vwVehicle.VehicleBodyTypeId,
                    VehicleCategoryId = vwVehicle.VehicleCategoryId,
                    VehicleClassId = vwVehicle.VehicleClassId,
                    VehicleClassificationId = vwVehicle.VehicleClassificationId,
                    VehicleColorId = vwVehicle.VehicleColorId,
                    VehicleEngineTypeId = vwVehicle.VehicleEngineTypeId,
                    VehicleFuelTypeId = vwVehicle.VehicleFuelTypeId,
                    VehicleMakeId = vwVehicle.VehicleMakeId,
                    VehicleMakerId = vwVehicle.VehicleMakerId,
                    VehiclePurchaseTypeId = vwVehicle.VehiclePurchaseTypeId,
                    VehicleTypeId =  vwVehicle.VehicleTypeId.Value,
                    VehicleStatusId = vwVehicle.VehicleStatusId,
                    VehicleUsageId = vwVehicle.VehicleUsageId,
                    CreatedBy =this.VwUser.UserId,
                    RegistrationDistrictId = vwVehicle.DistrictId.Value,
                    ChasisNo = vwVehicle.ChasisNo,
                    NoOfCylinder = vwVehicle.NoOfCylinder.Value,
                    EngineNo = vwVehicle.EngineNo,
                    EngineSize = vwVehicle.EngineSize.Value,
                    HorsePower = vwVehicle.HorsePower.Value,
                    LadenWeight = vwVehicle.LadenWeight.Value,
                    UnLadenWeight = vwVehicle.UnLadenWeight.Value,
                    Price = vwVehicle.Price.Value,
                    PurchaseDate = vwVehicle.PurchaseDate.Value,
                    RegistrationDate=System.DateTime.Now,
                    ManufacturingYear = vwVehicle.ManufacturingYear.Value,
                    SeatingCapacity = vwVehicle.SeatingCapacity.Value,
                    WheelBase = vwVehicle.WheelBase
                }
            }.ToDataTable());
            paramDict.Add("@VehicleAdditionalInfo", new List<VehicleAdditionalInfo>()
            {
                new VehicleAdditionalInfo()
                {
                    VehicleId = vwVehicle.VehicleId.Value,
                    VehicleRCStatusId = vwVehicle.VehicleRCStatusId,
                    IsIncomeTaxExempted = vwVehicle.IsIncomeTaxExempted,
                    IsHPA = false,
                    IsTaxExempted = vwVehicle.IsTaxExempted,
                    TaxFrequency = vwVehicle.TaxFrequency,
                    FitnessCertValidFrom = vwVehicle.FitnessCertValidFrom,
                    FitnessCertValidTo = vwVehicle.FitnessCertValidTo,
                    CreatedBy = this.VwUser.UserId
                }
            }.ToDataTable());

            paramDict.Add("@ApplicationId", vwVehicle.ApplicationId);
            paramDict.Add("@UserId", this.VwUser.UserId);

            var ds = await this.dbHelper.GetDataSetByStoredProcedure("[Core].[NR_SaveVehicle]", paramDict);

            return ds;
        }

        public async Task<DataSet> NRSaveKeeper(VwKeeper vwTempKeeper)
        {
            Dictionary<string, object> paramDict = new Dictionary<string, object>();

            paramDict.Add("@ApplicationId", vwTempKeeper.ApplicationId);

            paramDict.Add("@Keeper", new List<Keeper>()
            {
                new Keeper()
                {
                    KeeperId = vwTempKeeper.KeeperId.Value,
                    OwnerId = vwTempKeeper.OwnerId,
                    VehicleId = 0,
                    KeeperPersonId = vwTempKeeper.KeeperId.Value
                }
            }.ToDataTable());


            //  var person = vwTempKeeper.Person;

            //EntityMapper<VwPerson, Person>.CopyByPropertyNameAndType(vwTempKeeper.Person, person);

            paramDict.Add("@Person", new List<Person>()
            {
                new Person()
                {

                    PersonId = vwTempKeeper.Person.PersonId,
                    CountryId = vwTempKeeper.Person.CountryId,
                    PersonName = vwTempKeeper.Person.PersonName,
                    FatherHusbandName = vwTempKeeper.Person.FatherHusbandName,
                    CNIC = vwTempKeeper.Person.CNIC,
                    Email = vwTempKeeper.Person.Email,
                    OldCNIC = vwTempKeeper.Person.OldCNIC,
                    NTN = vwTempKeeper.Person.NTN,
                    FTN = vwTempKeeper.Person.FTN,
                    CreatedBy = this.VwUser.UserId
                }

            }.ToDataTable());

            var addresses = new List<Address>();

            vwTempKeeper.Person.Addresses.ForEach(x =>
            {
                var address = new Address();
                EntityMapper<VwAddress, Address>.CopyByPropertyNameAndType(x, address);
                addresses.Add(address);
            });

            var phoneNumbers = new List<PhoneNumber>();

            vwTempKeeper.Person.PhoneNumbers.ForEach(x =>
            {
                var phoneNumber = new PhoneNumber();
                EntityMapper<VwPhoneNumber, PhoneNumber>.CopyByPropertyNameAndType(x, phoneNumber);
                phoneNumbers.Add(phoneNumber);
            });

            paramDict.Add("@Address", addresses.ToDataTable());
            paramDict.Add("@PhoneNumber", phoneNumbers.ToDataTable());
            paramDict.Add("@UserId", this.VwUser.UserId);

            var ds = await this.dbHelper.GetDataSetByStoredProcedure("[Core].[NR_SaveKeeper]", paramDict);

            return ds;
        }



        public async Task<DataSet> NRSaveVehiclePurchaseInfo(VwVehiclePurchaseInfo vehiclePurchaseInfo)
        {

            Dictionary<string, object> paramDict = new Dictionary<string, object>();
            if (vehiclePurchaseInfo.AuctionInfo == null)
            {
                paramDict.Add("@AuctionInfo", new List<VehicleAuctionInfo>() { }.ToDataTable());
            }
            else
            {
                paramDict.Add("@AuctionInfo", new List<VehicleAuctionInfo>()
                {
                    new VehicleAuctionInfo()
                    {
                       VehicleAuctionInfoId = vehiclePurchaseInfo.AuctionInfo.AuctionInfoId,
                       ApplicationId = vehiclePurchaseInfo.ApplicationId.Value,
                       VehicleId = vehiclePurchaseInfo.VehicleId.Value,
                       LotNo = vehiclePurchaseInfo.AuctionInfo.LotNo,
                       BatchNo = vehiclePurchaseInfo.AuctionInfo.BatchNo,
                       CategoryNo = vehiclePurchaseInfo.AuctionInfo.CategoryNo,
                       VoucherNo = vehiclePurchaseInfo.AuctionInfo.VoucherNo,
                       VoucherDated = vehiclePurchaseInfo.AuctionInfo.VoucherDated.Value,
                       CreatedBy = this.VwUser.UserId
                    }
                }.ToDataTable());
            }

            if (vehiclePurchaseInfo.ImportInfo == null)
            {
                paramDict.Add("@ImportInfo", new List<VehicleImportInfo>() { }.ToDataTable());

            }
            else
            {
                paramDict.Add("@ImportInfo", new List<VehicleImportInfo>()
                {
                    new VehicleImportInfo()
                    {
                       VehicleImportInfoId = vehiclePurchaseInfo.ImportInfo.ImportInfoId,
                       ApplicationId = vehiclePurchaseInfo.ApplicationId.Value,
                       VehicleId = vehiclePurchaseInfo.VehicleId.Value,
                       ImporterName = vehiclePurchaseInfo.ImportInfo.ImporterName,
                       ImporterAddress = vehiclePurchaseInfo.ImportInfo.ImporterAddress,
                       IGMNo = vehiclePurchaseInfo.ImportInfo.IGMNo,
                       IGMDate = vehiclePurchaseInfo.ImportInfo.IGMDate,
                       IndexNo = vehiclePurchaseInfo.ImportInfo.IndexNo.Value,
                       PlaceOfIssue = vehiclePurchaseInfo.ImportInfo.PlaceOfIssue,
                       ImportPermitNo = vehiclePurchaseInfo.ImportInfo.ImportPermitNo,
                       PermitIssueDate    = vehiclePurchaseInfo.ImportInfo.PermitIssueDate.Value,
                       PortId = vehiclePurchaseInfo.ImportInfo.PortId.Value,
                       ClearingAgentId = vehiclePurchaseInfo.ImportInfo.ClearingAgentId.Value,
                       CustomCollectorateId = vehiclePurchaseInfo.ImportInfo.CustomCollectorateId.Value,
                       CountryId = vehiclePurchaseInfo.ImportInfo.CountryId.Value,
                       VehicleSchemeId = vehiclePurchaseInfo.ImportInfo.VehicleSchemeId.Value,
                       ImportValue = vehiclePurchaseInfo.ImportInfo.ImportValue.Value,
                       CustomDuty = vehiclePurchaseInfo.ImportInfo.CustomDuty.Value,
                       SalesTax = vehiclePurchaseInfo.ImportInfo.SalesTax.Value,
                       ImportLicenseFee = vehiclePurchaseInfo.ImportInfo.ImportLicenseFee.Value,
                       Insurrance = vehiclePurchaseInfo.ImportInfo.Insurrance.Value,
                       AnyOtherCost = vehiclePurchaseInfo.ImportInfo.AnyOtherCost.Value,
                       LandedCost = vehiclePurchaseInfo.ImportInfo.LandedCost.Value,
                       PaymentDate = vehiclePurchaseInfo.ImportInfo.PaymentDate.Value,
                       BankId = vehiclePurchaseInfo.ImportInfo.BankId.Value,
                       CreatedBy = this.VwUser.UserId
                    }
                }.ToDataTable());
            }

            if (vehiclePurchaseInfo.LocalPurchaseInfo == null)
            {
                paramDict.Add("@PurchaseInfo", new List<VehiclePurchaseInfo>() { }.ToDataTable());
            }
            else
            {
                paramDict.Add("@PurchaseInfo", new List<VehiclePurchaseInfo>()
                    {
                        new VehiclePurchaseInfo()
                        {

                           VehiclePurchaseInfoId = vehiclePurchaseInfo.LocalPurchaseInfo.LocalPurchaseInfoId,
                            ApplicationId  = vehiclePurchaseInfo.ApplicationId.Value,
                            VehicleId = vehiclePurchaseInfo.VehicleId.Value,
                            VehicleSchemeId = vehiclePurchaseInfo.LocalPurchaseInfo.VehicleSchemeId.Value,
                            DealerId = vehiclePurchaseInfo.LocalPurchaseInfo.DealerId.Value,
                            CertificateNo = vehiclePurchaseInfo.LocalPurchaseInfo.CertificateNo,
                            CertificateDated = vehiclePurchaseInfo.LocalPurchaseInfo.CertificateDated.Value,
                            InvoiceNo = vehiclePurchaseInfo.LocalPurchaseInfo.InvoiceNo,
                            InvoiceDated = vehiclePurchaseInfo.LocalPurchaseInfo.InvoiceDated.Value,
                            CreatedBy = this.VwUser.UserId
                        }
                     }.ToDataTable());
            }



            paramDict.Add("@ApplicationId", vehiclePurchaseInfo.ApplicationId.Value);
            paramDict.Add("@VehiclePurchaseTypeId", vehiclePurchaseInfo.VehiclePurchaseTypeId);
            paramDict.Add("@UserId", this.VwUser.UserId);

            var ds = await this.dbHelper.GetDataSetByStoredProcedure("[Core].[NR_SavePurchaseInfo]", paramDict);
            return ds;
        }

        //Either Vehicle Reg_No or Application ID, Delivery Article Service
        public async Task<DataSet> GetVehicleOwnerInfo(string SearchCriteria)
        {
            SqlParameter[] sql = new SqlParameter[1];
            sql[0] = new SqlParameter("@SearchCriteria", SearchCriteria);

            var ds = await this.adoNet.ExecuteUsingDataAdapter("[Core].[GetVehicleOwnerInfo]", sql);

            return ds;
        }

        //Either Vehicle Reg_No or Application ID, Scan Documents Service
        public async Task<DataSet> GetVehicleOwnerInfoForScanning(string SearchCriteria)
        {
            SqlParameter[] sql = new SqlParameter[1];
            sql[0] = new SqlParameter("@SearchCriteria", SearchCriteria);

            var ds = await this.adoNet.ExecuteUsingDataAdapter("[Core].[GetVehicleOwnerInfoForDocumentScanning]", sql);

            return ds;
        }
        #endregion

        public async Task<DataSet> NRSaveVehicleDocument(VwVehicleDocument vwVehicleDocument)
        {
            var vehicleDocuments = new List<VehicleDocument>();

            vwVehicleDocument.VehicleDocumentDetail.ForEach(x =>
            {
                var vehicleDocument = new VehicleDocument();
                EntityMapper<VwVehicleDocumentDetail, VehicleDocument>.CopyByPropertyNameAndType(x, vehicleDocument);
                vehicleDocuments.Add(vehicleDocument);
            });

            Dictionary<string, object> paramDict = new Dictionary<string, object>();

            paramDict.Add("@ApplicationId", vwVehicleDocument.ApplicationId);
            paramDict.Add("@VehicleId", vwVehicleDocument.VehicleId);
            paramDict.Add("@VehicleDocument", vehicleDocuments.ToDataTable());
            paramDict.Add("@UserId", this.VwUser.UserId);

            var ds = await this.dbHelper.GetDataSetByStoredProcedure("[Core].[NR_SaveDocuments]", paramDict);

            return ds;
        }

        public async Task<DataSet> NRSaveHPA(VwHPA vwTempHPA)
        {
            Dictionary<string, object> paramDict = new Dictionary<string, object>();

            //paramDict.Add("@HPA", hPAs.ToDataTable());
            paramDict.Add("@HPA", new List<HPA>()
                    {
                        new HPA()
                        {

                          HPAId = vwTempHPA.HPAId.Value,
                          ApplicationId = vwTempHPA.ApplicationId.Value,
                          OwnerId = vwTempHPA.OwnerId.Value,
                          VehicleId = 0,
                          SponserBusinessId = vwTempHPA.SponserBusinessId.Value,
                          HPADate = vwTempHPA.HPADate,
                          HPAStartDate = vwTempHPA.StartDate.Value,
                          HPAEndDate = vwTempHPA.EndDate,
                          LetterNo = vwTempHPA.HPALetterNo,
                          LetterDate = vwTempHPA.LetterDate.Value,
                          Terms = vwTempHPA.HPATerms,
                          HPACurrentStatusId = vwTempHPA.HPAStatusId.Value,
                          HPAStatusDated = vwTempHPA.HPAStatusDated.Value,
                          CreatedBy = this.VwUser.UserId
                        }
                     }.ToDataTable());

            paramDict.Add("@UserId", this.VwUser.UserId);
            var ds = await this.dbHelper.GetDataSetByStoredProcedure("[Core].[NR_SaveHPA]", paramDict);

            return ds;
        }

        public async Task<VwApplication> SaveOwner(VwTempOwner_v1 vwOwner)
        {
            Application application;

            if (vwOwner.ApplicationId == 0)
            {
                var ds = await this.GetBusinessProcessInitialState(vwOwner.BusinessProcessId.Value);

                var applicationProcessFlow = ds.Tables[0].ToList<ApplicationProcessFlow>().FirstOrDefault();

                application = new Application()
                {
                    BusinessPhaseId = applicationProcessFlow.CurrentBusinessPhaseId,
                    BusinessPhaseStatusId = applicationProcessFlow.CurrentBusinessPhaseStatusId,
                    BusinessProcessId = applicationProcessFlow.BusinessProcessId,
                    ApplicationStatusId = applicationProcessFlow.NextApplicationStatusId,
                    DistrictId = this.VwUser.UserDistrictId,
                    CreatedBy = this.VwUser.UserId,
                    ReceivedAt = DateTime.Now
                };

                this.appDbContext.Application.Add(application);
            }
            else
            {
                application = this.appDbContext.Application.SingleOrDefault(x => x.ApplicationId == vwOwner.ApplicationId);
            }

            if (application.BusinessProcessId == 1)
            {
                Owner owner;
                OwnerGroup ownerGroup;

                if (application.OwnerId is null)
                {
                    owner = new Owner()
                    {
                        OwnerTypeId = vwOwner.OwnerTypeId.Value,
                        OwnerTaxGroupId = vwOwner.OwnerTaxGroupId.Value,
                        CreatedBy = this.VwUser.UserId
                    };

                    this.appDbContext.Owner.Add(owner);
                    application.Owner = owner;
                }
                else
                {
                    owner = this.appDbContext.Owner.SingleOrDefault(x => x.OwnerId == application.OwnerId); // do not use vwOwner.OwnerId

                    owner.OwnerTaxGroupId = vwOwner.OwnerTaxGroupId.Value;

                    this.appDbContext.Entry<Owner>(owner).State = EntityState.Modified;
                }

                if (owner.OwnerTypeId == 1) // do not use vwOwner.OwnerTypeId
                {
                    var vwPerson = vwOwner.Persons.FirstOrDefault();

                    var person = await this.SavePerson(vwPerson);

                    var isOwnerInGroup = this.appDbContext.OwnerGroup.Any(x => x.OwnerId == owner.OwnerId && x.PersonId == person.PersonId);

                    if (isOwnerInGroup == false)
                    {
                        ownerGroup = new OwnerGroup()
                        {
                            IsFirstGroupMember = vwOwner.ApplicationId == 0,
                            Owner = owner,
                            Person = person,
                            CreatedBy = this.VwUser.UserId
                        };

                        this.appDbContext.OwnerGroup.Add(ownerGroup);
                    }

                    await this.CommitTransaction();

                    vwPerson.PersonId = person.PersonId;
                }
                else if (vwOwner.OwnerTypeId == 2)
                {
                    var business = await this.SaveBusiness(vwOwner.Business);

                    var isOwnerInGroup = this.appDbContext.OwnerGroup.Any(x => x.OwnerId == owner.OwnerId && x.BusinessId == business.BusinessId);

                    if (isOwnerInGroup == false)
                    {
                        ownerGroup = new OwnerGroup()
                        {
                            IsFirstGroupMember = vwOwner.ApplicationId == 0,
                            Owner = owner,
                            Business = business,
                            CreatedBy = this.VwUser.UserId
                        };

                        this.appDbContext.OwnerGroup.Add(ownerGroup);
                    }

                    await this.CommitTransaction();

                    vwOwner.Business.BusinessId = business.BusinessId;
                }

                vwOwner.OwnerId = owner.OwnerId;
            }

            return new VwApplication()
            {
                ApplicationId = application.ApplicationId,
                ReceivedAt = application.CreatedAt,
                Owner = new VwTempOwner_v1()
                {
                    OwnerId = vwOwner.OwnerId,
                    Persons = vwOwner.Persons,
                    Business = vwOwner.Business
                }
            };
        }

        public async Task<Person> SavePerson(VwPerson vwPerson)
        {
            Person person;

            if (vwPerson.PersonId == 0)
            {
                person = new Person()
                {
                    CountryId = vwPerson.CountryId,
                    CNIC = vwPerson.CNIC,
                    Email = vwPerson.Email,
                    FatherHusbandName = vwPerson.FatherHusbandName,
                    PersonName = vwPerson.PersonName,
                    NTN = vwPerson.NTN,
                    OldCNIC = vwPerson.OldCNIC,
                    CreatedBy = this.VwUser.UserId
                };

                foreach (var address in vwPerson.Addresses)
                {
                    this.appDbContext.Address.Add(new Address()
                    {
                        AddressDescription = address.AddressDescription,
                        AddressTypeId = address.AddressTypeId.Value,
                        CreatedBy = this.VwUser.UserId,
                        City = address.City,
                        DistrictId = address.DistrictId.Value,
                        PostalCode = address.PostalCode,
                        Person = person
                    });
                }

                foreach (var phoneNumber in vwPerson.PhoneNumbers)
                {
                    this.appDbContext.PhoneNumber.Add(new PhoneNumber()
                    {
                        CountryId = phoneNumber.CountryId,
                        CreatedBy = this.VwUser.UserId,
                        Person = person,
                        PhoneNumberTypeId = phoneNumber.PhoneNumberTypeId.Value,
                        PhoneNumberValue = phoneNumber.PhoneNumberValue
                    });
                }

                this.appDbContext.Person.Add(person);
            }
            else
            {
                person = this.appDbContext.Person.SingleOrDefault(x => x.PersonId == vwPerson.PersonId);

                person.CountryId = vwPerson.CountryId;
                //person.CNIC = vwPerson.CNIC;
                person.Email = vwPerson.Email;
                //person.FatherHusbandName = vwPerson.FatherHusbandName;
                //person.PersonName = vwPerson.PersonName;
                person.NTN = vwPerson.NTN;
                person.OldCNIC = vwPerson.OldCNIC;

                this.appDbContext.Entry<Person>(person).State = EntityState.Modified;

                foreach (var address in vwPerson.Addresses)
                {
                    var addressObj = this.appDbContext.Address.SingleOrDefault(x => x.PersonId == vwPerson.PersonId && x.AddressTypeId == address.AddressTypeId);

                    if (addressObj is null)
                    {
                        this.appDbContext.Address.Add(new Address()
                        {
                            AddressDescription = address.AddressDescription,
                            AddressTypeId = address.AddressTypeId.Value,
                            CreatedBy = this.VwUser.UserId,
                            City = address.City,
                            DistrictId = address.DistrictId.Value,
                            PostalCode = address.PostalCode,
                            Person = person
                        });
                    }
                    else
                    {
                        addressObj.AddressDescription = address.AddressDescription;
                        addressObj.AddressTypeId = address.AddressTypeId.Value;
                        addressObj.City = address.City;
                        addressObj.DistrictId = address.DistrictId.Value;
                        addressObj.TehsilId = address.TehsilId;
                        addressObj.PostalCode = address.PostalCode;

                        this.appDbContext.Entry<Address>(addressObj).State = EntityState.Modified;
                    }
                }

                foreach (var phoneNumber in vwPerson.PhoneNumbers)
                {
                    var phoneNumberObj = this.appDbContext.PhoneNumber.SingleOrDefault(x => x.PersonId == vwPerson.PersonId && x.PhoneNumberTypeId == phoneNumber.PhoneNumberTypeId);

                    if (phoneNumberObj is null)
                    {
                        this.appDbContext.PhoneNumber.Add(new PhoneNumber()
                        {
                            CountryId = phoneNumber.CountryId,
                            CreatedBy = this.VwUser.UserId,
                            Person = person,
                            PhoneNumberTypeId = phoneNumber.PhoneNumberTypeId.Value,
                            PhoneNumberValue = phoneNumber.PhoneNumberValue
                        });
                    }
                    else
                    {
                        phoneNumberObj.CountryId = phoneNumber.CountryId;
                        phoneNumberObj.PhoneNumberTypeId = phoneNumber.PhoneNumberTypeId.Value;
                        phoneNumberObj.PhoneNumberValue = phoneNumber.PhoneNumberValue;

                        this.appDbContext.Entry<PhoneNumber>(phoneNumberObj).State = EntityState.Modified;
                    }
                }
            }

            await this.CommitTransaction();

            vwPerson.PersonId = person.PersonId;

            return person;
        }

        public async Task<Business> SaveBusiness(VwBusiness vwBusiness)
        {
            Business business;

            if (vwBusiness.BusinessId == 0)
            {
                business = new Business()
                {
                    BusinessName = vwBusiness.BusinessName,
                    BusinessRegNo = vwBusiness.BusinessRegNo,
                    BusinessSectorId = vwBusiness.BusinessSectorId.Value,
                    BusinessStatusId = vwBusiness.BusinessStatusId.Value,
                    BusinessTypeId = vwBusiness.BusinessTypeId.Value,
                    Email = vwBusiness.Email,
                    FTN = vwBusiness.FTN,
                    NTN = vwBusiness.NTN,
                    STN = vwBusiness.STN,
                    CreatedBy = this.VwUser.UserId
                };

                foreach (var address in vwBusiness.Addresses)
                {
                    this.appDbContext.Address.Add(new Address()
                    {
                        AddressDescription = address.AddressDescription,
                        AddressTypeId = address.AddressTypeId.Value,
                        CreatedBy = this.VwUser.UserId,
                        City = address.City,
                        DistrictId = address.DistrictId.Value,
                        PostalCode = address.PostalCode,
                        Business = business
                    });
                }

                foreach (var phoneNumber in vwBusiness.PhoneNumbers)
                {
                    this.appDbContext.PhoneNumber.Add(new PhoneNumber()
                    {
                        CountryId = phoneNumber.CountryId,
                        CreatedBy = this.VwUser.UserId,
                        Business = business,
                        PhoneNumberTypeId = phoneNumber.PhoneNumberTypeId.Value,
                        PhoneNumberValue = phoneNumber.PhoneNumberValue
                    });
                }

                this.appDbContext.Business.Add(business);
            }
            else
            {
                business = this.appDbContext.Business.SingleOrDefault(x => x.BusinessId == vwBusiness.BusinessId);

                //business.BusinessName = vwBusiness.BusinessName;
                business.BusinessRegNo = vwBusiness.BusinessRegNo;
                business.BusinessSectorId = vwBusiness.BusinessSectorId.Value;
                business.BusinessStatusId = vwBusiness.BusinessStatusId.Value;
                business.BusinessTypeId = vwBusiness.BusinessTypeId.Value;
                business.Email = vwBusiness.Email;
                business.FTN = vwBusiness.FTN;
                //business.NTN = vwBusiness.NTN;
                business.STN = vwBusiness.STN;

                this.appDbContext.Entry<Business>(business).State = EntityState.Modified;

                foreach (var address in vwBusiness.Addresses)
                {
                    var addressObj = this.appDbContext.Address.SingleOrDefault(x => x.BusinessId == vwBusiness.BusinessId && x.AddressTypeId == address.AddressTypeId);

                    if (addressObj is null)
                    {
                        this.appDbContext.Address.Add(new Address()
                        {
                            AddressDescription = address.AddressDescription,
                            AddressTypeId = address.AddressTypeId.Value,
                            CreatedBy = this.VwUser.UserId,
                            City = address.City,
                            DistrictId = address.DistrictId.Value,
                            PostalCode = address.PostalCode,
                            Business = business
                        });
                    }
                    else
                    {
                        addressObj.AddressDescription = address.AddressDescription;
                        addressObj.AddressTypeId = address.AddressTypeId.Value;
                        addressObj.City = address.City;
                        addressObj.DistrictId = address.DistrictId.Value;
                        addressObj.TehsilId = address.TehsilId;
                        addressObj.PostalCode = address.PostalCode;

                        this.appDbContext.Entry<Address>(addressObj).State = EntityState.Modified;
                    }
                }

                foreach (var phoneNumber in vwBusiness.PhoneNumbers)
                {
                    var phoneNumberObj = this.appDbContext.PhoneNumber.SingleOrDefault(x => x.BusinessId == vwBusiness.BusinessId && x.PhoneNumberTypeId == phoneNumber.PhoneNumberTypeId);

                    if (phoneNumberObj is null)
                    {
                        this.appDbContext.PhoneNumber.Add(new PhoneNumber()
                        {
                            CountryId = phoneNumber.CountryId,
                            CreatedBy = this.VwUser.UserId,
                            Business = business,
                            PhoneNumberTypeId = phoneNumber.PhoneNumberTypeId.Value,
                            PhoneNumberValue = phoneNumber.PhoneNumberValue
                        });
                    }
                    else
                    {
                        phoneNumberObj.CountryId = phoneNumber.CountryId;
                        phoneNumberObj.PhoneNumberTypeId = phoneNumber.PhoneNumberTypeId.Value;
                        phoneNumberObj.PhoneNumberValue = phoneNumber.PhoneNumberValue;

                        this.appDbContext.Entry<PhoneNumber>(phoneNumberObj).State = EntityState.Modified;
                    }
                }
            }

            await this.CommitTransaction();

            vwBusiness.BusinessId = business.BusinessId;

            return business;
        }

        public async Task<VwApplication> SaveAndAssociateVehicle(VwVehicle vwVehicle)
        {
            Application application = this.appDbContext.Application.SingleOrDefault(x => x.ApplicationId == vwVehicle.ApplicationId);

            if (application.BusinessPhaseId == 0)
            {
                if (application.VehicleId is null)
                {
                    application.Vehicle = await this.SaveVehicle(vwVehicle);
                }
                else
                {
                    vwVehicle.VehicleId = application.VehicleId;
                    await this.SaveVehicle(vwVehicle);
                }
            }

            return null;
        }

        public async Task<Vehicle> SaveVehicle(VwVehicle vwVehicle)
        {
            Vehicle vehicle;

            if (vwVehicle.VehicleId == 0)
            {
                var application = this.appDbContext.Application.SingleOrDefault(x => x.ApplicationId == vwVehicle.ApplicationId);
                var owner = this.appDbContext.Owner.SingleOrDefault(x => x.OwnerId == application.OwnerId); // do not use vwOwner.OwnerId
                var hPA = this.appDbContext.HPA.SingleOrDefault(x => x.ApplicationId == application.ApplicationId && x.OwnerId == application.OwnerId);

                vehicle = new Vehicle()
                {
                    //RegistrationNo = vwVehicle.RegistrationNo,
                    VehicleBodyConventionId = vwVehicle.VehicleBodyConventionId,
                    VehicleBodyTypeId = vwVehicle.VehicleBodyTypeId,
                    VehicleCategoryId = vwVehicle.VehicleCategoryId,
                    VehicleClassId = vwVehicle.VehicleClassId,
                    VehicleClassificationId = vwVehicle.VehicleClassificationId,
                    VehicleColorId = vwVehicle.VehicleColorId,
                    VehicleEngineTypeId = vwVehicle.VehicleEngineTypeId,
                    VehicleFuelTypeId = vwVehicle.VehicleFuelTypeId,
                    VehicleMakeId = vwVehicle.VehicleMakeId,
                    VehicleMakerId = vwVehicle.VehicleMakerId,
                    VehiclePurchaseTypeId = vwVehicle.VehiclePurchaseTypeId,
                    VehicleTypeId = vwVehicle.VehicleTypeId.Value,
                    //VehicleSchemeId = vehicle.VehicleSchemeId,
                    VehicleStatusId = vwVehicle.VehicleStatusId,
                    VehicleUsageId = vwVehicle.VehicleUsageId,
                    CreatedBy = this.VwUser.UserId,
                    RegistrationDistrictId = vwVehicle.DistrictId.Value,
                    ChasisNo = vwVehicle.ChasisNo,
                    NoOfCylinder = vwVehicle.NoOfCylinder.Value,
                    EngineNo = vwVehicle.EngineNo,
                    EngineSize = vwVehicle.EngineSize.Value,
                    HorsePower = vwVehicle.HorsePower.Value,
                    LadenWeight = vwVehicle.LadenWeight.Value,
                    UnLadenWeight = vwVehicle.UnLadenWeight.Value,
                    Price = vwVehicle.Price.Value,
                    PurchaseDate = vwVehicle.PurchaseDate.Value,
                    ManufacturingYear = vwVehicle.ManufacturingYear.Value,
                    SeatingCapacity = vwVehicle.SeatingCapacity.Value,
                    WheelBase = vwVehicle.WheelBase,
                    Owner = owner
                };

                var vehicleAdditionalInfo = new VehicleAdditionalInfo()
                {
                    Vehicle = vehicle,
                    VehicleRCStatusId = vwVehicle.VehicleRCStatusId,
                    IsIncomeTaxExempted = vwVehicle.IsIncomeTaxExempted,
                    IsHPA = hPA is not null,
                    IsTaxExempted = vwVehicle.IsTaxExempted,
                    TaxFrequency = vwVehicle.TaxFrequency,
                    FitnessCertValidFrom = vwVehicle.FitnessCertValidFrom,
                    FitnessCertValidTo = vwVehicle.FitnessCertValidTo,
                    CreatedBy = this.VwUser.UserId
                };

                this.appDbContext.Vehicle.Add(vehicle);
                this.appDbContext.VehicleAdditionalInfo.Add(vehicleAdditionalInfo);
                
                if (hPA is not null) 
                {
                    hPA.Vehicle = vehicle;
                    this.appDbContext.Entry<HPA>(hPA).State = EntityState.Modified;
                }

                application.Vehicle = vehicle;
                this.appDbContext.Entry<Application>(application).State = EntityState.Modified;
            }
            else
            {
                var application = this.appDbContext.Application.SingleOrDefault(x => x.ApplicationId == vwVehicle.ApplicationId);
                vehicle = this.appDbContext.Vehicle.SingleOrDefault(x => x.VehicleId == vwVehicle.VehicleId);
                var vehicleAdditionalInfo = this.appDbContext.VehicleAdditionalInfo.SingleOrDefault(x => x.VehicleId == vwVehicle.VehicleId);
                var hPA = this.appDbContext.HPA.SingleOrDefault(x => x.ApplicationId == vwVehicle.ApplicationId);

                if (vehicle.VehicleId == application.VehicleId)
                {
                    if (vehicle.VehiclePurchaseTypeId != vwVehicle.VehiclePurchaseTypeId.Value)
                    {
                        var vehiclePurchaseInfo = this.appDbContext.VehiclePurchaseInfo.Where(x => x.VehicleId == vehicle.VehicleId);
                        var vehicleImportInfo = this.appDbContext.VehicleImportInfo.Where(x => x.VehicleId == vehicle.VehicleId);
                        var vehicleAuctionInfo = this.appDbContext.VehicleAuctionInfo.Where(x => x.VehicleId == vehicle.VehicleId);

                        foreach (var localPurchase in vehiclePurchaseInfo)
                        {
                            this.appDbContext.Entry<VehiclePurchaseInfo>(localPurchase).State = EntityState.Deleted;
                        } 

                        foreach (var importInfo in vehicleImportInfo)
                        {
                            this.appDbContext.Entry<VehicleImportInfo>(importInfo).State = EntityState.Deleted;
                        }

                        foreach (var auctionInfo in vehicleAuctionInfo)
                        {
                            this.appDbContext.Entry<VehicleAuctionInfo>(auctionInfo).State = EntityState.Deleted;
                        }
                    }

                    //vehicle.RegistrationNo = vwVehicle.RegistrationNo;
                    vehicle.VehicleBodyConventionId = vwVehicle.VehicleBodyConventionId;
                    vehicle.VehicleBodyTypeId = vwVehicle.VehicleBodyTypeId;
                    vehicle.VehicleCategoryId = vwVehicle.VehicleCategoryId;
                    vehicle.VehicleClassId = vwVehicle.VehicleClassId;
                    vehicle.VehicleClassificationId = vwVehicle.VehicleClassificationId;
                    vehicle.VehicleColorId = vwVehicle.VehicleColorId;
                    vehicle.VehicleEngineTypeId = vwVehicle.VehicleEngineTypeId;
                    vehicle.VehicleFuelTypeId = vwVehicle.VehicleFuelTypeId;
                    vehicle.VehicleMakeId = vwVehicle.VehicleMakeId;
                    vehicle.VehicleMakerId = vwVehicle.VehicleMakerId;
                    vehicle.VehiclePurchaseTypeId = vwVehicle.VehiclePurchaseTypeId;
                    //tempVehicle.VehicleSchemeId = vehicle.VehicleSchemeId;
                    vehicle.VehicleStatusId = vwVehicle.VehicleStatusId;
                    vehicle.VehicleUsageId = vwVehicle.VehicleUsageId;
                    vehicle.RegistrationDistrictId = vwVehicle.DistrictId.Value;

                    vehicle.ChasisNo = vwVehicle.ChasisNo;
                    vehicle.NoOfCylinder = vwVehicle.NoOfCylinder.Value;
                    vehicle.EngineNo = vwVehicle.EngineNo;
                    vehicle.EngineSize = vwVehicle.EngineSize.Value;
                    vehicle.HorsePower = vwVehicle.HorsePower.Value;
                    vehicle.LadenWeight = vwVehicle.LadenWeight.Value;
                    vehicle.UnLadenWeight = vwVehicle.UnLadenWeight.Value;
                    vehicle.Price = vwVehicle.Price.Value;
                    vehicle.PurchaseDate = vwVehicle.PurchaseDate.Value;
                    vehicle.ManufacturingYear = vwVehicle.ManufacturingYear.Value;
                    vehicle.SeatingCapacity = vwVehicle.SeatingCapacity.Value;
                    vehicle.WheelBase = vwVehicle.WheelBase;

                    if (vehicleAdditionalInfo is null)
                    {
                        vehicleAdditionalInfo = new VehicleAdditionalInfo()
                        {
                            Vehicle = vehicle,
                            VehicleRCStatusId = vwVehicle.VehicleRCStatusId,
                            IsIncomeTaxExempted = vwVehicle.IsIncomeTaxExempted,
                            IsHPA = hPA is not null,
                            IsTaxExempted = vwVehicle.IsTaxExempted,
                            TaxFrequency = vwVehicle.TaxFrequency,
                            FitnessCertValidFrom = vwVehicle.FitnessCertValidFrom,
                            FitnessCertValidTo = vwVehicle.FitnessCertValidTo
                        };

                        this.appDbContext.VehicleAdditionalInfo.Add(vehicleAdditionalInfo);
                    }
                    else
                    {
                        vehicleAdditionalInfo.VehicleRCStatusId = vwVehicle.VehicleRCStatusId;
                        vehicleAdditionalInfo.IsIncomeTaxExempted = vwVehicle.IsIncomeTaxExempted;
                        vehicleAdditionalInfo.IsTaxExempted = vwVehicle.IsTaxExempted;
                        vehicleAdditionalInfo.TaxFrequency = vwVehicle.TaxFrequency;
                        vehicleAdditionalInfo.FitnessCertValidFrom = vwVehicle.FitnessCertValidFrom;
                        vehicleAdditionalInfo.FitnessCertValidTo = vwVehicle.FitnessCertValidTo;

                        this.appDbContext.Entry<VehicleAdditionalInfo>(vehicleAdditionalInfo).State = EntityState.Modified;
                    }

                    this.appDbContext.Entry<Vehicle>(vehicle).State = EntityState.Modified;
                }
            }

            await this.CommitTransaction();

            vwVehicle.VehicleId = vehicle.VehicleId;

            return vehicle;
        }

        public async Task<VwVehiclePurchaseInfo> SaveVehiclePurchaseInfo(VwVehiclePurchaseInfo vehiclePurchaseInfo)
        {
            var application = this.appDbContext.Application.SingleOrDefault(x => x.ApplicationId == vehiclePurchaseInfo.ApplicationId.Value);
            var vehicle = this.appDbContext.Vehicle.SingleOrDefault(x => x.VehicleId == application.VehicleId); // do not user vehiclePurchaseInfo.VehicleId

            VehiclePurchaseInfo VehiclePurchaseInfo;
            VehicleImportInfo VehicleImportInfo;
            VehicleAuctionInfo VehicleAuctionInfo;

            switch (vehiclePurchaseInfo.VehiclePurchaseTypeId)
            {
                case 1:
                    VehicleImportInfo = await this.SaveVehicleImportInfo(application, vehicle, vehiclePurchaseInfo.ImportInfo);
                    await this.CommitTransaction();
                    vehiclePurchaseInfo.ImportInfo.ImportInfoId = VehicleImportInfo.VehicleImportInfoId;
                    break;
                case 2:
                    VehiclePurchaseInfo = await this.SaveLocalPurchaseInfo(application, vehicle, vehiclePurchaseInfo.LocalPurchaseInfo);
                    await this.CommitTransaction();
                    vehiclePurchaseInfo.LocalPurchaseInfo.LocalPurchaseInfoId = VehiclePurchaseInfo.VehiclePurchaseInfoId;
                    break;
                case 3:
                    VehicleImportInfo = await this.SaveVehicleImportInfo(application, vehicle, vehiclePurchaseInfo.ImportInfo);
                    VehicleAuctionInfo = await this.SaveVehicleAuctionInfo(application, vehicle, vehiclePurchaseInfo.AuctionInfo);
                    await this.CommitTransaction();
                    vehiclePurchaseInfo.ImportInfo.ImportInfoId = VehicleImportInfo.VehicleImportInfoId;
                    vehiclePurchaseInfo.AuctionInfo.AuctionInfoId = VehicleAuctionInfo.VehicleAuctionInfoId;
                    break;
                case 4:
                    VehiclePurchaseInfo = await this.SaveLocalPurchaseInfo(application, vehicle, vehiclePurchaseInfo.LocalPurchaseInfo);
                    VehicleAuctionInfo = await this.SaveVehicleAuctionInfo(application, vehicle, vehiclePurchaseInfo.AuctionInfo);
                    await this.CommitTransaction();
                    vehiclePurchaseInfo.LocalPurchaseInfo.LocalPurchaseInfoId = VehiclePurchaseInfo.VehiclePurchaseInfoId;
                    vehiclePurchaseInfo.AuctionInfo.AuctionInfoId = VehicleAuctionInfo.VehicleAuctionInfoId;
                    break;
                case 5:
                    break;
                case 6:
                    VehiclePurchaseInfo = await this.SaveLocalPurchaseInfo(application, vehicle, vehiclePurchaseInfo.LocalPurchaseInfo);
                    VehicleImportInfo = await this.SaveVehicleImportInfo(application, vehicle, vehiclePurchaseInfo.ImportInfo);
                    await this.CommitTransaction();
                    vehiclePurchaseInfo.LocalPurchaseInfo.LocalPurchaseInfoId = VehiclePurchaseInfo.VehiclePurchaseInfoId;
                    vehiclePurchaseInfo.ImportInfo.ImportInfoId = VehicleImportInfo.VehicleImportInfoId;
                    break;
                case 7:
                    VehiclePurchaseInfo = await this.SaveLocalPurchaseInfo(application, vehicle, vehiclePurchaseInfo.LocalPurchaseInfo);
                    await this.CommitTransaction();
                    vehiclePurchaseInfo.LocalPurchaseInfo.LocalPurchaseInfoId = VehiclePurchaseInfo.VehiclePurchaseInfoId;
                    break;
                case 8:
                    VehiclePurchaseInfo = await this.SaveLocalPurchaseInfo(application, vehicle, vehiclePurchaseInfo.LocalPurchaseInfo);
                    VehicleAuctionInfo = await this.SaveVehicleAuctionInfo(application, vehicle, vehiclePurchaseInfo.AuctionInfo);
                    await this.CommitTransaction();
                    vehiclePurchaseInfo.LocalPurchaseInfo.LocalPurchaseInfoId = VehiclePurchaseInfo.VehiclePurchaseInfoId;
                    vehiclePurchaseInfo.AuctionInfo.AuctionInfoId = VehicleAuctionInfo.VehicleAuctionInfoId;
                    break;
            }

            return vehiclePurchaseInfo;
        }

        public Task<VehiclePurchaseInfo> SaveLocalPurchaseInfo(Application application, Vehicle vehicle, VwVehicleLocalPurchaseInfo localPurchaseInfo)
        {
            VehiclePurchaseInfo vehiclePurchaseInfo;

            if (localPurchaseInfo.LocalPurchaseInfoId == 0)
            {
                vehiclePurchaseInfo = new VehiclePurchaseInfo()
                {
                    Application = application,
                    InvoiceNo = localPurchaseInfo.InvoiceNo,
                    InvoiceDated = localPurchaseInfo.InvoiceDated.Value,
                    DealerId = localPurchaseInfo.DealerId.Value,
                    VehicleSchemeId = localPurchaseInfo.VehicleSchemeId.Value,
                    CertificateDated = localPurchaseInfo.CertificateDated.Value,
                    CertificateNo = localPurchaseInfo.CertificateNo,
                    CreatedBy = this.VwUser.UserId,
                    Vehicle = vehicle
                };

                this.appDbContext.VehiclePurchaseInfo.Add(vehiclePurchaseInfo);
            }
            else
            {
                vehiclePurchaseInfo = this.appDbContext.VehiclePurchaseInfo.SingleOrDefault(x => x.VehiclePurchaseInfoId == localPurchaseInfo.LocalPurchaseInfoId);

                vehiclePurchaseInfo.InvoiceNo = localPurchaseInfo.InvoiceNo;
                vehiclePurchaseInfo.InvoiceDated = localPurchaseInfo.InvoiceDated.Value;
                vehiclePurchaseInfo.DealerId = localPurchaseInfo.DealerId.Value;
                vehiclePurchaseInfo.VehicleSchemeId = localPurchaseInfo.VehicleSchemeId.Value;
                vehiclePurchaseInfo.CertificateDated = localPurchaseInfo.CertificateDated.Value;
                vehiclePurchaseInfo.CertificateNo = localPurchaseInfo.CertificateNo;

                this.appDbContext.Entry<VehiclePurchaseInfo>(vehiclePurchaseInfo).State = EntityState.Modified;
            }

            localPurchaseInfo.LocalPurchaseInfoId = vehiclePurchaseInfo.VehiclePurchaseInfoId;

            return Task.FromResult(vehiclePurchaseInfo);
        }

        public Task<VehicleImportInfo> SaveVehicleImportInfo(Application application, Vehicle vehicle, VwVehicleImportInfo vwVehicleImportInfo)
        {
            VehicleImportInfo vehicleImportInfo;

            if (vwVehicleImportInfo.ImportInfoId == 0)
            {
                vehicleImportInfo = new VehicleImportInfo()
                {
                    Application = application,
                    AnyOtherCost = vwVehicleImportInfo.AnyOtherCost.Value,
                    BankId = vwVehicleImportInfo.BankId.Value,
                    ClearingAgentId = vwVehicleImportInfo.ClearingAgentId.Value,
                    CountryId = vwVehicleImportInfo.CountryId.Value,
                    CustomCollectorateId = vwVehicleImportInfo.CustomCollectorateId.Value,
                    CustomDuty = vwVehicleImportInfo.CustomDuty.Value,
                    LandedCost = vwVehicleImportInfo.LandedCost.Value,
                    IGMDate = vwVehicleImportInfo.IGMDate,
                    IGMNo = vwVehicleImportInfo.IGMNo,
                    ImporterAddress = vwVehicleImportInfo.ImporterAddress,
                    ImporterName = vwVehicleImportInfo.ImporterName,
                    ImportLicenseFee = vwVehicleImportInfo.ImportLicenseFee.Value,
                    ImportPermitNo = vwVehicleImportInfo.ImportPermitNo,
                    ImportValue = vwVehicleImportInfo.ImportValue.Value,
                    IndexNo = vwVehicleImportInfo.IndexNo.Value,
                    Insurrance = vwVehicleImportInfo.Insurrance.Value,
                    PermitIssueDate = vwVehicleImportInfo.PermitIssueDate.Value,
                    PaymentDate = vwVehicleImportInfo.PaymentDate.Value,
                    PlaceOfIssue = vwVehicleImportInfo.PlaceOfIssue,
                    PortId = vwVehicleImportInfo.PortId.Value,
                    SalesTax = vwVehicleImportInfo.SalesTax.Value, 
                    VehicleSchemeId = vwVehicleImportInfo.VehicleSchemeId.Value,
                    CreatedBy = this.VwUser.UserId,
                    Vehicle = vehicle
                };

                this.appDbContext.VehicleImportInfo.Add(vehicleImportInfo);
            }
            else
            {
                vehicleImportInfo = this.appDbContext.VehicleImportInfo.SingleOrDefault(x => x.VehicleImportInfoId == vwVehicleImportInfo.ImportInfoId);

                vehicleImportInfo.AnyOtherCost = vwVehicleImportInfo.AnyOtherCost.Value;
                vehicleImportInfo.BankId = vwVehicleImportInfo.BankId.Value;
                vehicleImportInfo.ClearingAgentId = vwVehicleImportInfo.ClearingAgentId.Value;
                vehicleImportInfo.CountryId = vwVehicleImportInfo.CountryId.Value;
                vehicleImportInfo.CustomCollectorateId = vwVehicleImportInfo.CustomCollectorateId.Value;
                vehicleImportInfo.CustomDuty = vwVehicleImportInfo.CustomDuty.Value;
                vehicleImportInfo.LandedCost = vwVehicleImportInfo.LandedCost.Value;
                vehicleImportInfo.IGMDate = vwVehicleImportInfo.IGMDate;
                vehicleImportInfo.IGMNo = vwVehicleImportInfo.IGMNo;
                vehicleImportInfo.ImporterAddress = vwVehicleImportInfo.ImporterAddress;
                vehicleImportInfo.ImporterName = vwVehicleImportInfo.ImporterName;
                vehicleImportInfo.ImportLicenseFee = vwVehicleImportInfo.ImportLicenseFee.Value;
                vehicleImportInfo.ImportPermitNo = vwVehicleImportInfo.ImportPermitNo;
                vehicleImportInfo.ImportValue = vwVehicleImportInfo.ImportValue.Value;
                vehicleImportInfo.IndexNo = vwVehicleImportInfo.IndexNo.Value;
                vehicleImportInfo.Insurrance = vwVehicleImportInfo.Insurrance.Value;
                vehicleImportInfo.PermitIssueDate = vwVehicleImportInfo.PermitIssueDate.Value;
                vehicleImportInfo.PaymentDate = vwVehicleImportInfo.PaymentDate.Value;
                vehicleImportInfo.PlaceOfIssue = vwVehicleImportInfo.PlaceOfIssue;
                vehicleImportInfo.PortId = vwVehicleImportInfo.PortId.Value;
                vehicleImportInfo.SalesTax = vwVehicleImportInfo.SalesTax.Value;
                vehicleImportInfo.VehicleSchemeId = vwVehicleImportInfo.VehicleSchemeId.Value;

                this.appDbContext.Entry<VehicleImportInfo>(vehicleImportInfo).State = EntityState.Modified;
            }

            vwVehicleImportInfo.ImportInfoId = vehicleImportInfo.VehicleImportInfoId;

            return Task.FromResult(vehicleImportInfo);
        }

        public Task<VehicleAuctionInfo> SaveVehicleAuctionInfo(Application application, Vehicle vehicle, VwVehicleAuctionInfo vwVehicleAuctionInfo)
        {
            VehicleAuctionInfo vehicleAuctionInfo;

            if (vwVehicleAuctionInfo.AuctionInfoId == 0)
            {
                vehicleAuctionInfo = new VehicleAuctionInfo()
                {
                    Application = application,
                    BatchNo = vwVehicleAuctionInfo.BatchNo,
                    CategoryNo = vwVehicleAuctionInfo.CategoryNo,
                    LotNo = vwVehicleAuctionInfo.LotNo,
                    VoucherDated = vwVehicleAuctionInfo.VoucherDated.Value,
                    VoucherNo = vwVehicleAuctionInfo.VoucherNo,
                    CreatedBy = this.VwUser.UserId,
                    Vehicle = vehicle
                };

                this.appDbContext.VehicleAuctionInfo.Add(vehicleAuctionInfo);
            }
            else
            {
                vehicleAuctionInfo = this.appDbContext.VehicleAuctionInfo.SingleOrDefault(x => x.VehicleAuctionInfoId == vwVehicleAuctionInfo.AuctionInfoId);

                vehicleAuctionInfo.BatchNo = vwVehicleAuctionInfo.BatchNo;
                vehicleAuctionInfo.CategoryNo = vwVehicleAuctionInfo.CategoryNo;
                vehicleAuctionInfo.LotNo = vwVehicleAuctionInfo.LotNo;
                vehicleAuctionInfo.VoucherDated = vwVehicleAuctionInfo.VoucherDated.Value;
                vehicleAuctionInfo.VoucherNo = vwVehicleAuctionInfo.VoucherNo;

                this.appDbContext.Entry<VehicleAuctionInfo>(vehicleAuctionInfo).State = EntityState.Modified;
            }

            vwVehicleAuctionInfo.AuctionInfoId = vehicleAuctionInfo.VehicleAuctionInfoId;

            return Task.FromResult(vehicleAuctionInfo);
        }

        public async Task<Remarks> SaveRemarks(VwRemarks vwRemarks)
        {
            var application = this.appDbContext.Application.SingleOrDefault(x => x.ApplicationId == vwRemarks.ApplicationId);

            Remarks remarks = new Remarks()
            {
                ApplicationId = vwRemarks.ApplicationId.Value,
                CreatedBy = this.VwUser.UserId,
                OwnerId = application.OwnerId,
                VehicleId = application.VehicleId.Value,
                RemarksStatement = vwRemarks.RemarksStatement,
                RoleId = this.VwUser.UserRoles.FirstOrDefault().RoleId
            };

            this.appDbContext.Remarks.Add(remarks);

            await this.CommitTransaction();

            remarks.RemarksId = remarks.RemarksId;

            return remarks;
        }

        public async Task<bool> SaveVehicleDocument(VwVehicleDocument vwVehicleDocument)
        {
            var application = this.appDbContext.Application.SingleOrDefault(x => x.ApplicationId == vwVehicleDocument.ApplicationId.Value);

            if (application.VehicleId == vwVehicleDocument.VehicleId)
            {
                var existingVehicleDocument = this.appDbContext.VehicleDocument.Where(x => x.ApplicationId == vwVehicleDocument.ApplicationId.Value).ToList();

                this.appDbContext.VehicleDocument.RemoveRange(existingVehicleDocument);

                foreach (var doc in vwVehicleDocument.VehicleDocumentDetail)
                {
                    var vehicleDocument = new VehicleDocument()
                    {
                        Application = application,
                        CreatedBy = this.VwUser.UserId,
                        VehicleDocumentTypeId = doc.VehicleDocumentTypeId.Value,
                        VehicleId = vwVehicleDocument.VehicleId.Value,
                        TotalPages = doc.TotalPages.Value
                    };

                    this.appDbContext.VehicleDocument.Add(vehicleDocument);
                }

                await this.CommitTransaction();

                return true;
            }
            
            return false;
        }

        public async Task<HPA> SaveHPA(VwHPA vwTempHPA)
        {
            HPA hPA = new HPA();

            var application = this.appDbContext.Application.SingleOrDefault(x => x.ApplicationId == vwTempHPA.ApplicationId.Value);

            if (vwTempHPA.HPAId == 0)
            {
                var owner = this.appDbContext.Owner.SingleOrDefault(x => x.OwnerId == application.OwnerId);
                var business = this.appDbContext.Business.SingleOrDefault(x => x.BusinessId == vwTempHPA.SponserBusinessId);

                if (application is not null && owner is not null && business is not null)
                {
                    hPA = new HPA()
                    {
                        Application = application,
                        Owner = owner,
                        Business = business,
                        CreatedBy = this.VwUser.UserId,
                        HPACurrentStatusId = vwTempHPA.HPAStatusId.Value,
                        HPAStatusDated = vwTempHPA.HPAStatusDated.Value,
                        HPADate = vwTempHPA.LetterDate.Value,
                        LetterNo = vwTempHPA.HPALetterNo,
                        Terms = vwTempHPA.HPATerms,
                        HPAStartDate = vwTempHPA.LetterDate.Value,
                        OwnerId = vwTempHPA.OwnerId.Value
                    };

                    var hpaStatusHistory = new HPAStatusHistory()
                    {
                        Application = application,
                        HPA = hPA,
                        HPAStatusId = vwTempHPA.HPAStatusId.Value,
                        HPAStatusDate = vwTempHPA.HPAStatusDated.Value,
                        CreatedBy = this.VwUser.UserId
                    };

                    this.appDbContext.HPAStatusHistory.Add(hpaStatusHistory);
                    this.appDbContext.HPA.Add(hPA);
                }
            }
            else
            {
                hPA = this.appDbContext.HPA.SingleOrDefault(x => x.HPAId == vwTempHPA.HPAId &&
                    x.ApplicationId == vwTempHPA.ApplicationId &&
                    x.OwnerId == vwTempHPA.OwnerId &&
                    x.SponserBusinessId == vwTempHPA.SponserBusinessId);

                var hPAStatusHistory = this.appDbContext.HPAStatusHistory.SingleOrDefault(x => x.HPAId == hPA.HPAId);

                if (hPA is not null)
                {
                    hPA.SponserBusinessId = vwTempHPA.SponserBusinessId.Value;
                    hPA.HPACurrentStatusId = vwTempHPA.HPAStatusId.Value;
                    hPA.HPAStatusDated = vwTempHPA.HPAStatusDated.Value;
                    hPA.HPADate = vwTempHPA.LetterDate.Value;
                    hPA.LetterNo = vwTempHPA.HPALetterNo;
                    hPA.Terms = vwTempHPA.HPATerms;
                    hPA.HPAStartDate = vwTempHPA.LetterDate.Value;
                    hPA.OwnerId = vwTempHPA.OwnerId.Value;

                    this.appDbContext.Entry<HPA>(hPA).State = EntityState.Modified;
                }

                //if (hPAStatusHistory is not null)
                //{
                //    hPAStatusHistory.HPAStatusId = vwTempHPA.HPAStatusId.Value;
                //    hPAStatusHistory.HPAStatusDate = vwTempHPA.HPAStatusDate.Value;

                //    this.appDbContext.Entry<HPAStatusHistory>(hPAStatusHistory).State = EntityState.Modified;
                //}

                var hpaStatusHistory = new HPAStatusHistory()
                {
                    Application = application,
                    HPA = hPA,
                    HPAStatusId = vwTempHPA.HPAStatusId.Value,
                    HPAStatusDate = vwTempHPA.HPAStatusDated.Value,
                    CreatedBy = this.VwUser.UserId
                };

                this.appDbContext.HPAStatusHistory.Add(hpaStatusHistory);
            }

            await this.CommitTransaction();

            return hPA;
        }

        public async Task<Keeper> SaveKeeper(VwKeeper vwTempKeeper)
        {
            var application = this.appDbContext.Application.SingleOrDefault(x => x.ApplicationId == vwTempKeeper.ApplicationId.Value);
            var vehicle = this.appDbContext.Vehicle.SingleOrDefault(x => x.VehicleId == application.VehicleId);
            var owner = this.appDbContext.Owner.SingleOrDefault(x => x.OwnerId == application.OwnerId);

            Keeper keeper = new Keeper();

            if (vwTempKeeper.KeeperId == 0 && vwTempKeeper.Person.PersonId == 0)
            {
                var person = await this.SavePerson(vwTempKeeper.Person);

                keeper = new Keeper()
                {
                    Person = person,
                    Vehicle = vehicle,
                    Owner = owner,
                    CreatedBy = this.VwUser.UserId,
                };

                this.appDbContext.Keeper.Add(keeper);
            }
            else if (vwTempKeeper.KeeperId == 0 && vwTempKeeper.Person.PersonId > 0)
            {
                var person = await this.SavePerson(vwTempKeeper.Person);

                keeper = new Keeper()
                {
                    Person = person,
                    Vehicle = vehicle,
                    Owner = owner,
                    CreatedBy = this.VwUser.UserId,
                };

                this.appDbContext.Keeper.Add(keeper);
            }
            else if (vwTempKeeper.KeeperId > 0 && vwTempKeeper.Person.PersonId > 0)
            {
                var person = await this.SavePerson(vwTempKeeper.Person);

                keeper = this.appDbContext.Keeper.SingleOrDefault(x => x.KeeperId == vwTempKeeper.KeeperId);

                keeper.Person = person;

                this.appDbContext.Entry<Keeper>(keeper).State = EntityState.Modified;
            }

            await this.CommitTransaction();

            return keeper;
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

        public async Task<long> SaveVehicleArticleDelivery(VwVehicleArticleDelivery vwVehicleArticleDelivery)
        {

            SqlParameter[] sql = new SqlParameter[10];
            sql[0] = new SqlParameter("@VehicleArticleDeliveryId", vwVehicleArticleDelivery.VehicleArticleDeliveryId);
            sql[1] = new SqlParameter("@ApplicationId", vwVehicleArticleDelivery.ApplicationId);
            sql[2] = new SqlParameter("@VehicleArticleId", vwVehicleArticleDelivery.VehicleArticleId);

            sql[3] = new SqlParameter("@Name", vwVehicleArticleDelivery.Name);
            sql[4] = new SqlParameter("@FatherName", vwVehicleArticleDelivery.FatherName);
            sql[5] = new SqlParameter("@CNIC", vwVehicleArticleDelivery.CNIC);
            sql[6] = new SqlParameter("@Mobile", vwVehicleArticleDelivery.Mobile);
            sql[7] = new SqlParameter("@Address", vwVehicleArticleDelivery.Address);
            sql[8] = new SqlParameter("@Remarks", vwVehicleArticleDelivery.Remarks);
            sql[9] = new SqlParameter("@Createdby", this.VwUser.UserId);

            var rowsAffected = await this.adoNet.ExecuteNonQuery("[Core].[SaveVehicelArticleDelivery]", sql);
            return rowsAffected;
        }

        public async Task<long> SaveVehicleDocumentUploadInfo(VehicleDocument vehicleDocument)
        {

            SqlParameter[] sql = new SqlParameter[9];
            sql[0] = new SqlParameter("@VehicleDocumentId", vehicleDocument.VehicleDocumentId);
            sql[1] = new SqlParameter("@ApplicationId", vehicleDocument.ApplicationId);
            sql[2] = new SqlParameter("@DocumentTypeId", vehicleDocument.VehicleDocumentTypeId);

            sql[3] = new SqlParameter("@TotalPages", vehicleDocument.TotalPages);
            sql[4] = new SqlParameter("@FileName", vehicleDocument.FileName);
            sql[5] = new SqlParameter("@MIMEType", vehicleDocument.MIMEType);
            sql[6] = new SqlParameter("@FileExtension", vehicleDocument.FileExtension);
            sql[7] = new SqlParameter("@FilePath", vehicleDocument.FilePath);
            sql[8] = new SqlParameter("@Updatedby", 1);// this.VwUser.UserId);

            var rowsAffected = await this.adoNet.ExecuteNonQuery("[Core].[UpdateVehicleDocumentsInfo]", sql);
            return rowsAffected;
        }


        public async Task<DataSet> SaveApplicationProcessFlow(VwApplicationProcessFlow vwApplicationProcessFlow)
        {
            Dictionary<string, object> paramDict = new Dictionary<string, object>();

            paramDict.Add("@ApplicationId", vwApplicationProcessFlow.BusinessProcessId);
            paramDict.Add("@BusinessEventId", vwApplicationProcessFlow.BusinessEventId);
            paramDict.Add("@CurrentBusinessPhaseId", vwApplicationProcessFlow.CurrentBusinessPhaseId);
            paramDict.Add("@CurrentBusinessPhaseStatusId", vwApplicationProcessFlow.CurrentBusinessPhaseStatusId);
            paramDict.Add("@CurrentApplicationStatusId", vwApplicationProcessFlow.CurrentApplicationStatusId);
            paramDict.Add("@NextBusinessPhaseId", vwApplicationProcessFlow.NextBusinessPhaseId);
            paramDict.Add("@NextBusinessPhaseStatusId", vwApplicationProcessFlow.NextBusinessPhaseStatusId);
            paramDict.Add("@NextApplicationStatusId", vwApplicationProcessFlow.NextApplicationStatusId);
            paramDict.Add("@RoleId", vwApplicationProcessFlow.RoleId);
            paramDict.Add("@UserId", this.VwUser.UserId);

            var ds = await this.dbHelper.GetDataSetByStoredProcedure("[Core].[SaveApplicationProcessFlow]", paramDict);

            return ds;
        }

    }
}

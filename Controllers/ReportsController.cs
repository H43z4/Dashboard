using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models.ViewModels.Identity;
using Models.ViewModels.Payment;
using Models.ViewModels.VehicleRegistration.Core;
using SharedLib.Common;
using SharedLib.APIs;
using System.Data;
using SharedLib.Security;
using Reports.Services;
using Wkhtmltopdf.NetCore;
using Wkhtmltopdf.NetCore.Interfaces;

namespace Reports.Controllers.Reports
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = AuthenticationSchemes.JWT_BEARER_TOKEN_STATELESS)]

    public class ReportsController : ControllerBase
    {
        private readonly IReportsService reportsService;
        private readonly IPaymentService paymentService;

        private readonly IGeneratePdf _generatePdf;
        private readonly IRazorViewToStringRenderer _engine;
        protected IConvertOptions _convertOptions;

        public VwUser User
        {
            get
            {
                return (VwUser)this.Request.HttpContext.Items["User"];
            }
        }

        public ReportsController(IReportsService reportsService,
            IPaymentService paymentService,
            IGeneratePdf generatePdf,
            IRazorViewToStringRenderer engine)
        {
            this.reportsService = reportsService;
            this.paymentService = paymentService;

            _generatePdf = generatePdf;
            //_webHostEnvironment = webHostEnvironment;
            _engine = engine;
            _convertOptions = new ConvertOptions();
        }

        private async Task<FileResult> GenerateBlankPDF(string fileName)
        {
            return await Task.Factory.StartNew(() =>
            {
                //byte[] pDF = Encoding.UTF8.GetBytes("Not Found!");
                byte[] pDF = WkhtmlDriver.Convert(WkhtmltopdfConfiguration.RotativaPath, _convertOptions.GetConvertOptions(), "Not Found!");
                MemoryStream memoryStream = new MemoryStream();
                memoryStream.Write(pDF, 0, pDF.Length);
                memoryStream.Position = 0L;
                return File(memoryStream, "application/pdf", $"{fileName}");
            });
        }

        private async Task<FileResult> GeneratePDF(string viewPath, object data, string fileName)
        {
            string html = await _engine.RenderViewToStringAsync<VwApplication>(viewPath, data as VwApplication);
            //string html = await _engine.RenderViewToStringAsync(viewPath);
            //string html = string.Format("<body>Hello world: </body>");
            byte[] pDF = WkhtmlDriver.Convert(WkhtmltopdfConfiguration.RotativaPath, _convertOptions.GetConvertOptions(), html);
            MemoryStream memoryStream = new MemoryStream();
            memoryStream.Write(pDF, 0, pDF.Length);
            memoryStream.Position = 0L;
            return File(memoryStream, "application/pdf", $"{fileName}");
        }

        private async Task<VwApplication> GetApplicationDetails(long applicationId)
        {
            this.reportsService.VwUser = this.User;

            var ds = await this.reportsService.GetApplicationDetails(applicationId);

            VwApplication currentApp = SharedLib.Common.Extentions.ToList<VwApplication>(ds.Tables[0]).FirstOrDefault();

            if (currentApp is not null)
            {
                currentApp.Owner = SharedLib.Common.Extentions.ToList<VwTempOwner_v1>(ds.Tables[1]).FirstOrDefault();
                var addresses = SharedLib.Common.Extentions.ToList<VwAddress>(ds.Tables[3]).ToList();
                var phoneNumbers = SharedLib.Common.Extentions.ToList<VwPhoneNumber>(ds.Tables[4]).ToList();

                if (currentApp.Owner.OwnerTypeId == 1)
                {
                    currentApp.Owner.Persons = SharedLib.Common.Extentions.ToList<VwPerson>(ds.Tables[2]).ToList();
                    currentApp.Owner.Persons.ForEach(x =>
                    {
                        x.Addresses = new List<VwAddress>();
                        x.PhoneNumbers = new List<VwPhoneNumber>();

                        x.Addresses.AddRange(addresses.Where(add => add.PersonId == x.PersonId));
                        x.PhoneNumbers.AddRange(phoneNumbers.Where(add => add.PersonId == x.PersonId));
                    });
                }
                else if (currentApp.Owner.OwnerTypeId == 2)
                {
                    currentApp.Owner.Business = SharedLib.Common.Extentions.ToList<VwBusiness>(ds.Tables[2]).FirstOrDefault();
                    currentApp.Owner.Business.Addresses = new List<VwAddress>();
                    currentApp.Owner.Business.PhoneNumbers = new List<VwPhoneNumber>();

                    currentApp.Owner.Business.Addresses.AddRange(addresses.Where(add => add.BusinessId == currentApp.Owner.Business.BusinessId));
                    currentApp.Owner.Business.PhoneNumbers.AddRange(phoneNumbers.Where(add => add.BusinessId == currentApp.Owner.Business.BusinessId));
                }

                currentApp.Vehicle = SharedLib.Common.Extentions.ToList<VwVehicle>(ds.Tables[5]).FirstOrDefault();

                currentApp.PurchaseInfo = new VwVehiclePurchaseInfo();

                currentApp.PurchaseInfo.LocalPurchaseInfo = SharedLib.Common.Extentions.ToList<VwVehicleLocalPurchaseInfo>(ds.Tables[6]).FirstOrDefault();
                currentApp.PurchaseInfo.ImportInfo = SharedLib.Common.Extentions.ToList<VwVehicleImportInfo>(ds.Tables[7]).FirstOrDefault();
                currentApp.PurchaseInfo.AuctionInfo = SharedLib.Common.Extentions.ToList<VwVehicleAuctionInfo>(ds.Tables[8]).FirstOrDefault();
                currentApp.HPAInfo = SharedLib.Common.Extentions.ToList<VwHPA>(ds.Tables[9]).FirstOrDefault();

                if (currentApp.HPAInfo is not null)
                {
                    currentApp.HPAInfo.SponserBusiness = SharedLib.Common.Extentions.ToList<VwBusiness>(ds.Tables[10]).FirstOrDefault();
                    currentApp.HPAInfo.SponserBusiness.Addresses = SharedLib.Common.Extentions.ToList<VwAddress>(ds.Tables[11]).ToList();
                    currentApp.HPAInfo.SponserBusiness.PhoneNumbers = SharedLib.Common.Extentions.ToList<VwPhoneNumber>(ds.Tables[12]).ToList();
                }

                currentApp.Keeper = SharedLib.Common.Extentions.ToList<VwKeeper>(ds.Tables[13]).FirstOrDefault();

                if (currentApp.Keeper is not null)
                {
                    currentApp.Keeper.Person = SharedLib.Common.Extentions.ToList<VwPerson>(ds.Tables[14]).FirstOrDefault();
                    currentApp.Keeper.Person.Addresses = SharedLib.Common.Extentions.ToList<VwAddress>(ds.Tables[15]).ToList();
                    currentApp.Keeper.Person.PhoneNumbers = SharedLib.Common.Extentions.ToList<VwPhoneNumber>(ds.Tables[16]).ToList();
                }

                currentApp.Documents = SharedLib.Common.Extentions.ToList<VwVehicleDocumentDetail>(ds.Tables[17]).ToList();
                currentApp.TaxAssessment = ds.Tables[18].ToList<VwAssessment>();
            }

            return currentApp;
        }

        private VwApplication GetApplicationDetails(DataSet ds, int startingTableIndex)
        {
            //ds.Tables.RemoveAt(0);
            //ds.Tables.RemoveAt(1);

            VwApplication currentApp = SharedLib.Common.Extentions.ToList<VwApplication>(ds.Tables[startingTableIndex + 0]).FirstOrDefault();

            if (currentApp is not null)
            {
                currentApp.Owner = SharedLib.Common.Extentions.ToList<VwTempOwner_v1>(ds.Tables[startingTableIndex + 1]).FirstOrDefault();
                var addresses = SharedLib.Common.Extentions.ToList<VwAddress>(ds.Tables[startingTableIndex + 3]).ToList();
                var phoneNumbers = SharedLib.Common.Extentions.ToList<VwPhoneNumber>(ds.Tables[startingTableIndex + 4]).ToList();

                if (currentApp.Owner.OwnerTypeId == 1)
                {
                    currentApp.Owner.Persons = SharedLib.Common.Extentions.ToList<VwPerson>(ds.Tables[startingTableIndex + 2]).ToList();
                    currentApp.Owner.Persons.ForEach(x =>
                    {
                        x.Addresses = new List<VwAddress>();
                        x.PhoneNumbers = new List<VwPhoneNumber>();

                        x.Addresses.AddRange(addresses.Where(add => add.PersonId == x.PersonId));
                        x.PhoneNumbers.AddRange(phoneNumbers.Where(add => add.PersonId == x.PersonId));
                    });
                }
                else if (currentApp.Owner.OwnerTypeId == 2)
                {
                    currentApp.Owner.Business = SharedLib.Common.Extentions.ToList<VwBusiness>(ds.Tables[startingTableIndex + 2]).FirstOrDefault();
                    currentApp.Owner.Business.Addresses = new List<VwAddress>();
                    currentApp.Owner.Business.PhoneNumbers = new List<VwPhoneNumber>();

                    currentApp.Owner.Business.Addresses.AddRange(addresses.Where(add => add.BusinessId == currentApp.Owner.Business.BusinessId));
                    currentApp.Owner.Business.PhoneNumbers.AddRange(phoneNumbers.Where(add => add.BusinessId == currentApp.Owner.Business.BusinessId));
                }

                currentApp.Vehicle = SharedLib.Common.Extentions.ToList<VwVehicle>(ds.Tables[startingTableIndex + 5]).FirstOrDefault();

                currentApp.PurchaseInfo = new VwVehiclePurchaseInfo();

                currentApp.PurchaseInfo.LocalPurchaseInfo = SharedLib.Common.Extentions.ToList<VwVehicleLocalPurchaseInfo>(ds.Tables[startingTableIndex + 6]).FirstOrDefault();
                currentApp.PurchaseInfo.ImportInfo = SharedLib.Common.Extentions.ToList<VwVehicleImportInfo>(ds.Tables[startingTableIndex + 7]).FirstOrDefault();
                currentApp.PurchaseInfo.AuctionInfo = SharedLib.Common.Extentions.ToList<VwVehicleAuctionInfo>(ds.Tables[startingTableIndex + 8]).FirstOrDefault();
                currentApp.HPAInfo = SharedLib.Common.Extentions.ToList<VwHPA>(ds.Tables[startingTableIndex + 9]).FirstOrDefault();

                if (currentApp.HPAInfo is not null)
                {
                    currentApp.HPAInfo.SponserBusiness = SharedLib.Common.Extentions.ToList<VwBusiness>(ds.Tables[startingTableIndex + 10]).FirstOrDefault();
                    currentApp.HPAInfo.SponserBusiness.Addresses = SharedLib.Common.Extentions.ToList<VwAddress>(ds.Tables[startingTableIndex + 11]).ToList();
                    currentApp.HPAInfo.SponserBusiness.PhoneNumbers = SharedLib.Common.Extentions.ToList<VwPhoneNumber>(ds.Tables[startingTableIndex + 12]).ToList();
                }

                currentApp.Keeper = SharedLib.Common.Extentions.ToList<VwKeeper>(ds.Tables[startingTableIndex + 13]).FirstOrDefault();

                if (currentApp.Keeper is not null)
                {
                    currentApp.Keeper.Person = SharedLib.Common.Extentions.ToList<VwPerson>(ds.Tables[startingTableIndex + 14]).FirstOrDefault();
                    currentApp.Keeper.Person.Addresses = SharedLib.Common.Extentions.ToList<VwAddress>(ds.Tables[startingTableIndex + 15]).ToList();
                    currentApp.Keeper.Person.PhoneNumbers = SharedLib.Common.Extentions.ToList<VwPhoneNumber>(ds.Tables[startingTableIndex + 16]).ToList();
                }

                currentApp.Documents = SharedLib.Common.Extentions.ToList<VwVehicleDocumentDetail>(ds.Tables[startingTableIndex + 17]).ToList();
                currentApp.TaxAssessment = ds.Tables[startingTableIndex + 18].ToList<VwAssessment>();
            }

            return currentApp;
        }

        private VwApplication GetChallanDetail(DataSet ds)
        {
            VwApplication currentApp = ds.Tables[0].ToList<VwApplication>().FirstOrDefault();

            if (currentApp is not null)
            {
                var sellerList = ds.Tables[1].ToList<VwTempOwner_v1>();

                if (sellerList.Any())
                {
                    currentApp.Seller = sellerList.FirstOrDefault();
                    var addresses = ds.Tables[3].ToList<VwAddress>();
                    var phoneNumbers = ds.Tables[4].ToList<VwPhoneNumber>();

                    if (currentApp.Seller.OwnerTypeId == 1)
                    {
                        currentApp.Seller.Persons = ds.Tables[2].ToList<VwPerson>();
                        currentApp.Seller.Persons.ForEach(x =>
                        {
                            x.Addresses = new List<VwAddress>();
                            x.PhoneNumbers = new List<VwPhoneNumber>();

                            x.Addresses.AddRange(addresses.Where(add => add.PersonId == x.PersonId));
                            x.PhoneNumbers.AddRange(phoneNumbers.Where(add => add.PersonId == x.PersonId));
                        });
                    }
                    else if (currentApp.Seller.OwnerTypeId == 2)
                    {
                        currentApp.Seller.Business = ds.Tables[2].ToList<VwBusiness>().FirstOrDefault();
                        currentApp.Seller.Business.Addresses = new List<VwAddress>();
                        currentApp.Seller.Business.PhoneNumbers = new List<VwPhoneNumber>();

                        currentApp.Seller.Business.Addresses.AddRange(addresses.Where(add => add.BusinessId == currentApp.Seller.Business.BusinessId));
                        currentApp.Seller.Business.PhoneNumbers.AddRange(phoneNumbers.Where(add => add.BusinessId == currentApp.Seller.Business.BusinessId));
                    }
                }

                var ownersList = ds.Tables[5].ToList<VwTempOwner_v1>();

                if (ownersList.Any())
                {
                    currentApp.Owner = ownersList.FirstOrDefault();
                    var addresses = ds.Tables[7].ToList<VwAddress>();
                    var phoneNumbers = ds.Tables[8].ToList<VwPhoneNumber>();

                    if (currentApp.Owner.OwnerTypeId == 1)
                    {
                        currentApp.Owner.Persons = ds.Tables[6].ToList<VwPerson>();
                        currentApp.Owner.Persons.ForEach(x =>
                        {
                            x.Addresses = new List<VwAddress>();
                            x.PhoneNumbers = new List<VwPhoneNumber>();

                            x.Addresses.AddRange(addresses.Where(add => add.PersonId == x.PersonId));
                            x.PhoneNumbers.AddRange(phoneNumbers.Where(add => add.PersonId == x.PersonId));
                        });
                    }
                    else if (currentApp.Owner.OwnerTypeId == 2)
                    {
                        currentApp.Owner.Business = ds.Tables[6].ToList<VwBusiness>().FirstOrDefault();
                        currentApp.Owner.Business.Addresses = new List<VwAddress>();
                        currentApp.Owner.Business.PhoneNumbers = new List<VwPhoneNumber>();

                        currentApp.Owner.Business.Addresses.AddRange(addresses.Where(add => add.BusinessId == currentApp.Owner.Business.BusinessId));
                        currentApp.Owner.Business.PhoneNumbers.AddRange(phoneNumbers.Where(add => add.BusinessId == currentApp.Owner.Business.BusinessId));
                    }
                }

                currentApp.Vehicle = ds.Tables[9].ToList<VwVehicle>().FirstOrDefault();

                currentApp.ChallanPayments = ds.Tables[10].ToList<VwChallanPaymentInfo>().ToList();
                var taxAssessment = ds.Tables[11].ToList<VwAssessment>().ToList();

                currentApp.ChallanPayments.ForEach(x =>
                {
                    x.TaxAssessment = taxAssessment.Where(a => a.AssessmentBaseId == x.AssessmentBaseId).ToList();
                });

                var data = new
                {
                    currentApp.ApplicationId,
                    currentApp.ApplicationStatus,
                    currentApp.ApplicationStatusId,
                    currentApp.BusinessProcess,
                    currentApp.BusinessProcessId,
                    currentApp.BusinessPhase,
                    currentApp.BusinessPhaseId,
                    currentApp.BusinessPhaseStatus,
                    currentApp.BusinessPhaseStatusId,
                    currentApp.ReceivedAt,
                    currentApp.Seller,
                    currentApp.Owner,
                    currentApp.Vehicle,
                    currentApp.ChallanPayments,
                };

                return currentApp;
            }

            return null;
        }

        [HttpGet(Name = "GetChallan")]
        public async Task<FileResult> GetChallan(long applicationId)
        {
            var application = await this.GetApplicationDetails(applicationId);

            if (application is null)
            {
                return await this.GenerateBlankPDF(applicationId.ToString());
            }

            var viewPath = "Reports/GetChallan";
            var file = await GeneratePDF(viewPath, application, $"{application.ApplicationId}-challan.pdf");

            if (file is not null)
            {
                this.reportsService.VwUser = this.User;

                var ds = await this.reportsService.SaveApplicationPhase(new VwApplicationPhaseChange()
                {
                    ApplicationId = application.ApplicationId,
                    BusinessEventId = 1,
                });
            }

            return file;
        }

        //[HttpGet(Name = "GenerateChallan")]
        //public async Task<FileResult> GenerateChallan(long applicationId)
        //{
        //    try
        //    {
        //        this.registrationService.VwUser = this.User;
        //        this.paymentService.VwUser = this.User;

        //        //var application = new VwApplication();

        //        var dsChallan = await this.paymentService.GenerateChallan(applicationId);

        //        if (dsChallan.Tables.Count == 0)
        //        {
        //            throw new DataException();
        //        }

        //        int STATUS_TABLE_INDEX = dsChallan.Tables.Count - 1;

        //        if (Convert.ToInt16(dsChallan.Tables[STATUS_TABLE_INDEX].Rows[0][0]) == 0)  //Challan generated.
        //        {
        //            var dsPayeeInfo = await this.paymentService.GetPayeeInfo(applicationId);

        //            var payeeInfo = dsPayeeInfo.Tables[0].ToList<VwPayeeInfo>().FirstOrDefault();

        //            //GetPSId

        //            payeeInfo = await this.paymentService.GetPSId(payeeInfo);

        //            //SavePSId

        //            if (payeeInfo is null)
        //            {
        //                throw new DataException();
        //            }

        //            await this.paymentService.SavePSId(payeeInfo);
        //        }

        //        //var dsApplication = await this.registrationService.GetApplicationDetails(applicationId);
        //        //var application = this.GetApplicationDetails(dsApplication, 0);

        //        var dsApplication = await this.registrationService.GetChallanDetail(applicationId);
        //        var application = this.GetChallanDetail(dsApplication);


        //        var viewPath = "Reports/GetChallan";
        //        var file = await GeneratePDF(viewPath, application, $"{applicationId}-challan.pdf");

        //        return file;
        //    }
        //    catch(Exception ex)
        //    {
        //        Console.WriteLine("\n--------------GenerateChallan--------------\n");
        //        Console.Write(ex);

        //        return await this.GenerateBlankPDF(applicationId.ToString());
        //    }
        //}
        
        [HttpGet(Name = "GenerateChallan")]
        public async Task<FileResult> GenerateChallan(long businessProcessId, long applicationId, long assessmentBaseId)
        {
            try
            {
                this.reportsService.VwUser = this.User;
                this.paymentService.VwUser = this.User;

                var dsChallan = await this.paymentService.GenerateChallan(businessProcessId, applicationId, assessmentBaseId);

                if (dsChallan.Tables.Count == 0)
                {
                    throw new DataException();
                }

                int STATUS_TABLE_INDEX = dsChallan.Tables.Count - 1;

                if (Convert.ToInt16(dsChallan.Tables[STATUS_TABLE_INDEX].Rows[0][0]) == 0)  //Challan generated.
                {
                    var challanId = Convert.ToInt64(dsChallan.Tables[STATUS_TABLE_INDEX].Rows[0][2]);

                    var dsEPayTask = await this.paymentService.SaveEPayTask(businessProcessId, applicationId, challanId);
                    
                    if (Convert.ToInt16(dsEPayTask.Tables[0].Rows[0][0]) == 0)  //epay task generated
                    {
                        var epayTaskId = Convert.ToInt64(dsEPayTask.Tables[0].Rows[0][2]);
                        await EPayHttpClient.GeneratePSIDRequest(epayTaskId);
                    }
                }

                var dsApplication = await this.reportsService.GetChallanDetail(applicationId);
                var application = this.GetChallanDetail(dsApplication);


                var viewPath = "Reports/GetChallan";
                var file = await GeneratePDF(viewPath, application, $"{applicationId}-challan.pdf");

                return file;
            }
            catch(Exception ex)
            {
                Console.WriteLine("\n--------------GenerateChallan--------------\n");
                Console.Write(ex);

                return await this.GenerateBlankPDF(applicationId.ToString());
            }
        }

        [HttpPost(Name = "UpdatePSIDStatus")]
        public async Task<ApiResponse> UpdatePSIDStatus(long epayTaskId, long status)
        {
            this.paymentService.VwUser = this.User;
            var ds = await this.paymentService.UpdatePsidStatus(epayTaskId, status);

            if (ds.Tables.Count == 0 || ds.Tables[0].Rows[0][0].ToString() == "1")
            {
                return ApiResponse.GetApiResponse(ApiResponseType.FAILED, null, Constants.DATA_NOT_SAVED_MESSAGE);
            }

            if (status == 2)
            {
                if (!await EPayHttpClient.GeneratePSIDRequest(epayTaskId))
                {
                    return ApiResponse.GetApiResponse(ApiResponseType.FAILED, null, Constants.DATA_NOT_SAVED_MESSAGE);
                }
            }

            return ApiResponse.GetApiResponse(ApiResponseType.SUCCESS, null, Constants.DATA_SAVED_MESSAGE);
        }

        [HttpGet(Name = "GetChallanApplications")]
        public async Task<ApiResponse> GetChallanApplications(long id, DateTime? _appDate)
        {
            this.paymentService.VwUser = this.User;

            var ds = await this.paymentService.GetChallanApplications(id, _appDate);

            if (ds.Tables.Count == 0 || ds.Tables[0].Rows.Count == 0)
            {
                return ApiResponse.GetApiResponse(ApiResponseType.FAILED, null, Constants.NOT_FOUND_MESSAGE);
            }

            var data = new
            {
                psidstatus = ds.Tables[0]
            };
            return ApiResponse.GetApiResponse(ApiResponseType.SUCCESS, data, Constants.RECORD_FOUND_MESSAGE);
        }

        [HttpGet(Name = "GetPSIDStatusDropDown")]
        public async Task<ApiResponse> GetPSIDStatusDropDown()
        {
            this.paymentService.VwUser = this.User;

            var ds = await this.paymentService.GetPSIDStatusDropDown();

            if (ds.Tables.Count == 0 || ds.Tables[0].Rows.Count == 0)
            {
                return ApiResponse.GetApiResponse(ApiResponseType.FAILED, null, Constants.NOT_FOUND_MESSAGE);
            }

            var data = new
            {
                psidstatus = ds.Tables[0]
            };
            return ApiResponse.GetApiResponse(ApiResponseType.SUCCESS, data, Constants.RECORD_FOUND_MESSAGE);
        }
    }
}

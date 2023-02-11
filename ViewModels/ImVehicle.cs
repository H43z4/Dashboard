using Models.ViewModels.VehicleRegistration.Core;
using System.ComponentModel.DataAnnotations;

namespace Inquiry.ViewModels
{
    public class ImVehicle
    {
        //public long VehicleId { get; set; }
        //public string VehicleClass { get; set; }
        //public string VehicleBodyConvention { get; set; }
        //public string VehicleBodyType { get; set; }
        //public string VehicleMake { get; set; }
        //public string VehicleMaker { get; set; }
        //public long ManufacturingYear { get; set; }
        //public long NoOfCylinder { get; set; }
        //public long EngineSize { get; set; }
        //public string ChasisNo { get; set; }
        //public string EngineNo { get; set; }
        //public long SeatingCapacity { get; set; }
        //public string VehicleFuelType { get; set; }
        //public string VehicleColor { get; set; }
        //public long Price { get; set; }
        //public string VehiclePurchaseType { get; set; }
        //public DateTime PurchaseDate { get; set; }
        //public DateTime TaxPaidUpto { get; set; }
        public long? VehicleId { get; set; }
        public string RegistrationNo { get; set; }
        public DateTime? RegistrationDate { get; set; }
        public long? DistrictId { get; set; }
        public long? VehicleBodyConventionId { get; set; }
        public string VehicleBodyConvention { get; set; }
        public long? VehicleBodyTypeId { get; set; }
        public string VehicleBodyType { get; set; }
        public long? VehicleCategoryId { get; set; }
        public string VehicleCategory { get; set; }
        public long? VehicleClassId { get; set; }
        public string VehicleClass { get; set; }
        public long? VehicleClassificationId { get; set; }
        public string VehicleClassification { get; set; }
        public long? VehicleColorId { get; set; }
        public string VehicleColor { get; set; }
        public long? VehicleEngineTypeId { get; set; }
        public string VehicleEngineType { get; set; }
        public long? VehicleFuelTypeId { get; set; }
        public string VehicleFuelType { get; set; }
        public long? VehicleMakeId { get; set; }
        public string VehicleMake { get; set; }
        public long? VehicleMakerId { get; set; }
        public string VehicleMaker { get; set; }
        public long? VehiclePurchaseTypeId { get; set; }
        public string VehiclePurchaseType { get; set; }
        public long? VehicleStatusId { get; set; }
        public string VehicleStatus { get; set; }
        public long? VehicleUsageId { get; set; }
        public string VehicleUsage { get; set; }
        public long? VehicleTypeId { get; set; }
        public string VehicleType { get; set; }
        public int? ManufacturingYear { get; set; }
        public int? NoOfCylinder { get; set; }
        public int? EngineSize { get; set; }
        public int? HorsePower { get; set; }
        public string ChasisNo { get; set; }
        public string EngineNo { get; set; }
        public int? SeatingCapacity { get; set; }
        public long? Price { get; set; }
        public DateTime? PurchaseDate { get; set; }
        public long? LadenWeight { get; set; }
        public long? UnLadenWeight { get; set; }
        public string WheelBase { get; set; }
        public long? VehicleRCStatusId { get; set; }
        public string VehicleRCStatus { get; set; }
        public int TaxFrequency { get; set; }
        public int TaxPeriod { get; set; }
        public bool IsHPA { get; set; }
        public bool? IsTaxExempted { get; set; }
        public bool? IsIncomeTaxExempted { get; set; }
        public DateTime? DateOfFirstRegistration { get; set; }
        public DateTime? FitnessCertValidFrom { get; set; }
        public DateTime? FitnessCertValidTo { get; set; }
        public DateTime? TaxPaidUpto { get; set; }
        public long RegistrationNoPrice { get; set; }
    }
}

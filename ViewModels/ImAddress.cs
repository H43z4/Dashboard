using Models.ViewModels.VehicleRegistration.Core;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Inquiry.ViewModels
{
    public class ImAddress:CommonFeature
    {
        public long AddressId { get; set; }
        public string AddressDescription { get; set; }
        public string PostalCode { get; set; }
        public string City { get; set; } 
        public long DistrictId { get; set; } 
        public string District { get; set; }
        public string AddressType { get; set; }
        public string AreaName { get; set; }
        public string PropertyNo { get; set; }
        public string Street { get; set; }
        public string AddressArea { get; set; }
        public long AddressTypeId { get; set; }
    }
}

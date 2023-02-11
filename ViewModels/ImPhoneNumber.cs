using Models.ViewModels.VehicleRegistration.Core;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Inquiry.ViewModels
{
    public class ImPhoneNumber:CommonFeature
    {
        public long PhoneNumberId { get; set; } 
        public string PhoneNumberValue { get; set; } 
        public long CountryId { get; set; }
        public long PersonId { get; set; }
        public long PhoneNumberTypeId { get; set; }
        public string PhoneNumberType { get; set; }
    }
}

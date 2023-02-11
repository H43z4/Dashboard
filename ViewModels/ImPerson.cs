using Models.ViewModels.VehicleRegistration.Core;
using System.ComponentModel.DataAnnotations;

namespace Inquiry.ViewModels
{
    public class ImPerson:CommonFeature
    {
        public long PersonId { get; set; } 
        public long CountryId { get; set; }
        public string PersonName { get; set; } 
        public string FatherHusbandName { get; set; }
        public string Cnic { get; set; }
        public string Email { get; set; }
        public string OldCNIC { get; set; }
        public string Ntn { get; set; }
        public List<ImAddress> Addresses { get; set; }
        public List<ImPhoneNumber> PhoneNumbers { get; set; }


    }
}

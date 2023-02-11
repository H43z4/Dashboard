namespace Inquiry.ViewModels
{
    public class ImBusiness
    {
        public long BusinessId { get; set; }
        public long BusinessTypeId { get; set; }
        public string BusinessType { get; set; }
        public long BusinessSectorId { get; set; }
        public string BusinessSector { get; set; }
        public long BusinessStatusId { get; set; }
        public string BusinessStatus { get; set; }
        public string BusinessRegNo { get; set; }
        public string BusinessName { get; set; }
        public string Email { get; set; }
        public string Ntn { get; set; }
        public string Ftn { get; set; }
        public string Stn { get; set; }
        public List<ImAddress> Addresses { get; set; }
        public List<ImPhoneNumber> PhoneNumbers { get; set; }
    }
}

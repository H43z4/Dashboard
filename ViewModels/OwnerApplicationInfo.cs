namespace Inquiry.ViewModels
{
    public class OwnerApplicationInfo
    {
        public long ApplicationId { get; set; } = 0;
        public long BusinessProcessId { get; set; } = 0;
        public long OwnerId { get; set; } = 0;
        public long OwnerTypeId { get; set; } = 0;
        public long OwnerTaxGroupId { get; set; } = 0;

        public string OwnerType { get; set; }
        public string OwnerTaxGroup { get; set; }

        public bool IsHPA { get; set; } = false;

        public List<ImPerson> Persons { get; set; }
        public ImBusiness Business { get; set; }
        public ImVehicle Vehicle { get; set; }
        public List<ImApplicationHist> Application { get; set; }
        public long? VehicleId { get; set; }
        public long? SellerId { get; set; }
        public long AccessLevel { get; set; } = 0;
    }
}

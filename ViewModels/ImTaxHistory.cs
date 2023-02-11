namespace Inquiry.ViewModels
{
    public class ImTaxHistory
    {
        public long ApplicationId { get; set; } 
        public long AmountPaid { get; set; }
        public string BusinessProcess { get; set; }
        public string BusinessPhase { get; set; }
        public string BusinessPhaseStatus { get; set; }
        public DateTime TaxInitalDate { get; set; }
        public DateTime TaxTillDate { get; set; }
        public DateTime ApplicationDate { get; set; }
    }
}

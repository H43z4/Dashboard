namespace Inquiry.ViewModels
{
    public class ImApplicationsList
    {
        public long VehicleId { get; set; }
        public long ApplicationId { get; set; }
        public DateTime ReceivedAt { get; set; }
        public DateTime CreatedAt { get; set; }
        public string CreatedBy { get; set; }
        public string BusinessPhaseStatus { get; set; }
        public string BusinessPhase { get; set; }
        public string BusinessProcess { get; set; }
        public string ApplicationStatus { get; set; }
        public DateTime CardChallanDate { get; set; }
        public DateTime NPChallanDate { get; set; }
        public string CardPrintingStatus { get; set; }
        public string NumPlatePrintingStatus { get; set; }
    }
}

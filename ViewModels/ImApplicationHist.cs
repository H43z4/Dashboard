namespace Inquiry.ViewModels
{
    public class ImApplicationHist
    {
        public long? VehicleId { get; set; }
        public long? ApplicationId { get; set; }
        public long? AssessmentBaseId { get; set; }
        public long? BusinessProcessId { get; set; }
        public string BusinessProcess { get; set; }
        public long? BusinessPhaseId { get; set; }
        public string BusinessPhase { get; set; }
        public long? BusinessPhaseStatusId { get; set; }
        public string BusinessPhaseStatus { get; set; }
        public long? ApplicationStatusId { get; set; }
        public string ApplicationStatus { get; set; }
        public long? ChallanId { get; set; }
        public long? ChallanAmount { get; set; }
        public string ChallanStatus { get; set; }
        public string Remarks { get; set; } = "";
    }
}

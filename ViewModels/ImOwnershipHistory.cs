namespace Inquiry.ViewModels
{
    public class ImOwnershipHistory
    {
        public long ApplicationId { get; set; } 
        public long SellerId { get; set; }
        public string SellerName { get; set; }
        public long BuyerId { get; set; } 
        public string BuyerName { get; set; } 

        public long BusinessProcessId { get; set; }
        public DateTime TransferDate { get; set; }
        public DateTime ApplicationDate { get; set; }
    }
}

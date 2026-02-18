namespace GreenBowl.Models
{
    public class ProductInventory
    {
        public int Lot { get; set; }
        public ProductName ProductName { get; set; }

        public double BatchWeight { get; set; }
        public int Pouch { get; set; }

        public DateTime ProductionDate { get; set; }
        public DateTime ExpiryDate { get; set; }
        public DateTime CartonDate { get; set; }

        public bool QACheck { get; set; }

        public int WeightRejected { get; set; }
        public int XRayRejected { get; set; }
        public int SealingRejected { get; set; }
        public int NoPrint { get; set; }
        public int Sample { get; set; }
        public int QCRetention { get; set; }
        public int TotalRejected { get; set; }

        public int ProductForSale { get; set; }
        public int TotalCase { get; set; }


        public int ActualInventory { get; set; }
        public int Productivity { get; set; }
    }
}

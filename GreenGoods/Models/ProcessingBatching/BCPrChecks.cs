namespace GreenBowl.Models
{
    public class BCPrChecks
    {
        public BatchCode Batch { get; set; }
        public int Lot { get; set; }
        public DateTime TimeOfCheck { get; set; }
        public bool XRayRejectedPouches { get; set; }

        public BatchBCPr? Parent { get; set; }
    }
}

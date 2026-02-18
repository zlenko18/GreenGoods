namespace GreenBowl.Models
{
    public class BCPaChecks
    {
        public BatchCode Batch { get; set; }
        public int Lot { get; set; }
        public DateTime TimeOfCheck { get; set; }
        public bool SealingCondition { get; set; }

        public BatchBCPa? Parent { get; set; }
    }
}

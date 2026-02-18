namespace GreenBowl.Models
{
    public class BatchBCPa
    {
        public BatchCode Batch { get; set; }
        public int Lot { get; set; }

        public DateTime StartTimeFilling { get; set; }
        public DateTime FinishTimeFilling { get; set; }

        public bool PouchHygienicCondition { get; set; }
        public int NumberOfPouchMade { get; set; }
        public int WeightRejected { get; set; }
        public int SealingRejected { get; set; }
        public int NoPrintRejected { get; set; }


        public string? Comments { get; set; }

        public BatchingControlPackaging? Parent { get; set; }
        public ICollection<BCPaChecks> Child { get; set; } = new List<BCPaChecks>();
    }
}

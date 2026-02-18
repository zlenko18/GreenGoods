namespace GreenBowl.Models
{
    public class BatchBCPr
    {
        public BatchCode Batch { get; set; }
        public int Lot { get; set; }

        public DateTime StartTimeProcessing { get; set; }
        public DateTime FinishTimeProcessing { get; set; }

        public bool Alarm { get; set; }
        public string? Reason { get; set; }
        public string? CorrectionAction { get; set; }

        public int NumberOfRejectedPouch { get; set; }
        public string? Comments { get; set; }

        public BatchingControlProcessing? Parent { get; set; }
        public ICollection<BCPrChecks> Child { get; set; } = new List<BCPrChecks>();
    }
}

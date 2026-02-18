namespace GreenBowl.Models
{
    public class BatchingControlProcessing
    {
        public DateTime EffectiveDate { get; set; }
        public int RevisionNumber { get; set; }
        public DateTime ProductionDate { get; set; }
        public string ProductionSupervisor { get; set; } = string.Empty;

        public BatchCode Batch { get; set; }
        public int Lot { get; set; }

        public BatchBCPr? Child { get; set; }
    }
}

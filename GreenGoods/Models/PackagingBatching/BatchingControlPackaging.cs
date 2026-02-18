namespace GreenBowl.Models
{
    public class BatchingControlPackaging
    {
        public DateTime EffectiveDate { get; set; }
        public int RevisionNumber { get; set; }
        public DateTime ProductionDate { get; set; }
        public string FillerSupervisor { get; set; } = string.Empty;

        public string? TypeOfPackaging { get; set; }

        public BatchCode Batch { get; set; }
        public int Lot { get; set; }

        public BatchBCPa? Child { get; set; }
    }
}

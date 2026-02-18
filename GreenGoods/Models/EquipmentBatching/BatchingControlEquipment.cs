namespace GreenBowl.Models
{
    public class BatchingControlEquipment
    {
        public DateTime EffectiveDate { get; set; }
        public int RevisionNumber { get; set; }
        public DateTime ProductionDate { get; set; }
        public string FillerSupervisor { get; set; } = string.Empty;

        public BatchCode Batch { get; set; }
        public int Lot { get; set; }

        public BatchBCE? Child { get; set; }
    }
}

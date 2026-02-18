namespace GreenBowl.Models
{
    public class BatchingControlBatchingStep
    {
        public DateTime EffectiveDate { get; set; }
        public ProductName ProductName { get; set; }
        public DateTime ProductionDate { get; set; }
        public string ProductionSupervisor { get; set; } = string.Empty;

        public BatchCode Batch { get; set; }
        public int Lot { get; set; }

        public ICollection<BatchBCBS> Child { get; set; } = new List<BatchBCBS>();
    }
}

namespace GreenBowl.Models
{
    public class XRayMonitoringRecord
    {
        public BatchCode Batch { get; set; }
        public int Lot { get; set; }

        public ProductName ProductName { get; set; }

        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public DateTime Date { get; set; }

        public bool Indicator1 { get; set; }
        public bool Indicator2 { get; set; }
        public bool Indicator3 { get; set; }

        public string? Deviation_CorrectiveAction { get; set; }
    }
}

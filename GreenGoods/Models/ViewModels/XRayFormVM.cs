using GreenBowl.Models;

namespace GreenBowl.Models.ViewModels
{
    public class XRayFormVM
    {
        public int Lot { get; set; }

        // Optional "header" data you might want to show (we’ll populate from ProductInventory if available)
        public ProductName ProductName { get; set; }
        public DateTime Date { get; set; } = DateTime.Today;

        public XRayBatchVM A { get; set; } = new() { Batch = BatchCode.A };
        public XRayBatchVM B { get; set; } = new() { Batch = BatchCode.B };
        public XRayBatchVM C { get; set; } = new() { Batch = BatchCode.C };
        public XRayBatchVM D { get; set; } = new() { Batch = BatchCode.D };
    }

    public class XRayBatchVM
    {
        public BatchCode Batch { get; set; }
        public int Lot { get; set; }

        public ProductName ProductName { get; set; }
        public DateTime Date { get; set; } = DateTime.Today;

        public DateTime StartTime { get; set; } = DateTime.Today;
        public DateTime EndTime { get; set; } = DateTime.Today;

        public bool Indicator1 { get; set; }
        public bool Indicator2 { get; set; }
        public bool Indicator3 { get; set; }

        public string? Deviation_CorrectiveAction { get; set; }
    }
}

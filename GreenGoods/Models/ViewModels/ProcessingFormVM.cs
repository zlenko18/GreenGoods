using GreenBowl.Models;

namespace GreenBowl.Models.ViewModels
{
    public class ProcessingFormVM
    {
        public int Lot { get; set; }

        public DateTime ProductionDate { get; set; }
        public string ProductionSupervisor { get; set; } = string.Empty;

        public ProcessingBatchVM A { get; set; } = new() { Batch = BatchCode.A };
        public ProcessingBatchVM B { get; set; } = new() { Batch = BatchCode.B };
        public ProcessingBatchVM C { get; set; } = new() { Batch = BatchCode.C };
        public ProcessingBatchVM D { get; set; } = new() { Batch = BatchCode.D };
    }

    public class ProcessingBatchVM
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

        public List<ProcessingCheckVM> Checks { get; set; } = new();
    }

    public class ProcessingCheckVM
    {
        public BatchCode Batch { get; set; }
        public int Lot { get; set; }
        public DateTime TimeOfCheck { get; set; }

        public bool XRayRejectedPouches { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;

namespace GreenBowl.Models.ViewModels
{
    public class PackagingFormVM
    {
        [Required]
        public int Lot { get; set; }

        [DataType(DataType.Date)]
        public DateTime ProductionDate { get; set; }

        public string FillerSupervisor { get; set; } = string.Empty;

        public string TypeOfPackaging { get; set; } = string.Empty;

        public PackagingBatchVM A { get; set; } = new() { Batch = BatchCode.A };
        public PackagingBatchVM B { get; set; } = new() { Batch = BatchCode.B };
        public PackagingBatchVM C { get; set; } = new() { Batch = BatchCode.C };
        public PackagingBatchVM D { get; set; } = new() { Batch = BatchCode.D };
    }

    public class PackagingBatchVM
    {
        public BatchCode Batch { get; set; }
        public int Lot { get; set; }

        [DataType(DataType.Time)]
        public DateTime StartTimeFilling { get; set; }

        [DataType(DataType.Time)]
        public DateTime FinishTimeFilling { get; set; }


        public bool PouchHygienicCondition { get; set; }

        public int NumberOfPouchMade { get; set; }
        public int WeightRejected { get; set; }
        public int SealingRejected { get; set; }
        public int NoPrintRejected { get; set; }

        public string? Comments { get; set; }

        public List<PackagingCheckVM> Checks { get; set; } = new();
    }

    public class PackagingCheckVM
    {
        public BatchCode Batch { get; set; }
        public int Lot { get; set; }

        public DateTime TimeOfCheck { get; set; }

        public bool SealingCondition { get; set; }
    }
}


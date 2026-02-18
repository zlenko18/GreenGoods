using System.ComponentModel.DataAnnotations;

namespace GreenBowl.Models.ViewModels
{
    public class EquipmentFormVM
    {
        // Header (same for whole LOT)
        [DataType(DataType.Date)]
        public DateTime EffectiveDate { get; set; }

        public int RevisionNumber { get; set; }

        [DataType(DataType.Date)]
        public DateTime ProductionDate { get; set; }

        public string FillerSupervisor { get; set; } = string.Empty;

        public int Lot { get; set; }

        // Four batch parts
        public EquipmentBatchVM A { get; set; } = new() { Batch = BatchCode.A };
        public EquipmentBatchVM B { get; set; } = new() { Batch = BatchCode.B };
        public EquipmentBatchVM C { get; set; } = new() { Batch = BatchCode.C };
        public EquipmentBatchVM D { get; set; } = new() { Batch = BatchCode.D };
    }

    public class EquipmentBatchVM
    {
        public int Lot { get; set; }
        public BatchCode Batch { get; set; }

        // Store as flags (checkbox list)
        public List<EquipmentType> Equipments { get; set; } = new();

        public bool CIP_COPBeforeStarting { get; set; }
        public bool FunctionCondition { get; set; }
        public bool HygienicCondition { get; set; }

        public string? Comments { get; set; }
        public string? InitialOfController { get; set; }
    }
}

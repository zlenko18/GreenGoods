namespace GreenBowl.Models
{
    public class BatchBCE
    {
        public BatchCode Batch { get; set; }
        public int Lot { get; set; }

        // Stores the checked items (not “all possible” ones)
        public List<EquipmentType> Equipments { get; set; } = new();

        public bool CIP_COPBeforeStarting { get; set; }
        public bool FunctionCondition { get; set; }
        public bool HygienicCondition { get; set; }

        public string? Comments { get; set; }
        public string? InitialOfController { get; set; }
        public BatchingControlEquipment? Parent { get; set; }
    }
}

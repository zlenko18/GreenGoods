namespace GreenBowl.Models
{
    public class BulgingTestOnPouch
    {
        public ProductName ProductName { get; set; }
        public int Lot { get; set; }

        public int NumberOfPouch { get; set; }
        public DateTime ProductionDate { get; set; }
        public DateTime ExpiryDate { get; set; }

        public int TimeOfIncubation { get; set; }
        public int TemperatureOfIncubation { get; set; }

        public bool Results { get; set; }
        public string? Comments { get; set; }
        public bool Taste { get; set; }

        public DateTime DateOfRelease { get; set; }
        public string? QAInitial { get; set; }

        public int TPC { get; set; }
        public int YM { get; set; }
    }
}

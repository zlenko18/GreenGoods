using GreenBowl.Models;

namespace GreenBowl.Models.ViewModels
{
    public class BulgingFormVM
    {
        public int Lot { get; set; }
        public ProductName ProductName { get; set; }

        public int NumberOfPouch { get; set; }
        public DateTime ProductionDate { get; set; } = DateTime.Today;
        public DateTime ExpiryDate { get; set; } = DateTime.Today;

        public int TimeOfIncubation { get; set; }
        public int TemperatureOfIncubation { get; set; }

        public bool Results { get; set; }
        public bool Taste { get; set; }

        public string? Comments { get; set; }

        public DateTime DateOfRelease { get; set; } = DateTime.Today;
        public string? QAInitial { get; set; }

        public int TPC { get; set; }
        public int YM { get; set; }
    }
}

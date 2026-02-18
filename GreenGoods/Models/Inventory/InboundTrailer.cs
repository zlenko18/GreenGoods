using System.ComponentModel.DataAnnotations;

namespace GreenBowl.Models
{
    public class InboundTrailer
    {
        [Key]
        public string IngredientLot { get; set; } = string.Empty;
        public string SupplierName { get; set; } = string.Empty;
        public string TruckPlate { get; set; } = string.Empty;
        public bool QualityCheck { get; set; }
    }
}

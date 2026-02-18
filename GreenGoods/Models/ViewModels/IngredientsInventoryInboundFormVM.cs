using System.ComponentModel.DataAnnotations;

namespace GreenBowl.Models.ViewModels
{
    public class IngredientsInventoryInboundFormVM
    {
        [Key]
        public string IngredientLot { get; set; } = string.Empty;

        public int Quantity { get; set; }

        public IngredientName IngredientName { get; set; }
        public string SupplierName { get; set; } = string.Empty;
        public string TruckPlate { get; set; } = string.Empty;
        public bool QualityCheck { get; set; }
    }
}


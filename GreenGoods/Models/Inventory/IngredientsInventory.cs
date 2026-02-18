using System.ComponentModel.DataAnnotations;

namespace GreenBowl.Models
{
    public class IngredientsInventory
    {
        [Key]
        public string IngredientLot { get; set; } = string.Empty;

        public int Quantity { get; set; }

        public IngredientName IngredientName { get; set; }
    }
}

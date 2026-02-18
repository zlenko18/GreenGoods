namespace GreenBowl.Models
{
    public class BatchBCBS
    {
        public BatchCode Batch { get; set; }
        public int Lot { get; set; }

        public IngredientName IngredientName { get; set; }
        public string IngredientLot { get; set; } = string.Empty;

        public bool Allergens { get; set; }
        public bool HygenicCondition { get; set; }

        public double AmountPerBatch { get; set; }
        public DateTime ExpiryDate { get; set; }

        public string? Comments { get; set; }

        public BatchingControlBatchingStep? Parent { get; set; }
        public IngredientsInventory? Ingredient { get; set; }
    }
}

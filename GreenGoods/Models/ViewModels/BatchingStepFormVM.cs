using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using GreenBowl.Models;

namespace GreenBowl.Models.ViewModels
{
    public class BatchingStepFormVM
    {
        public int Lot { get; set; }

        [DataType(DataType.Date)]
        public DateTime EffectiveDate { get; set; } = DateTime.Today;

        public ProductName ProductName { get; set; }

        [DataType(DataType.Date)]
        public DateTime ProductionDate { get; set; } = DateTime.Today;

        [Required]
        public string ProductionSupervisor { get; set; } = string.Empty;

        // A/B/C/D batch columns
        public BatchingStepBatchVM A { get; set; } = new() { Batch = BatchCode.A };
        public BatchingStepBatchVM B { get; set; } = new() { Batch = BatchCode.B };
        public BatchingStepBatchVM C { get; set; } = new() { Batch = BatchCode.C };
        public BatchingStepBatchVM D { get; set; } = new() { Batch = BatchCode.D };

        // dropdown options (rebuilt server-side each request)
        public List<SelectListItem> IngredientLotOptions { get; set; } = new();
    }

    public class BatchingStepBatchVM
    {
        public BatchCode Batch { get; set; }
        public int Lot { get; set; }

        public List<BatchingStepLineVM> Lines { get; set; } = new();
    }

    public class BatchingStepLineVM
    {
        public BatchCode Batch { get; set; }
        public int Lot { get; set; }

        public IngredientName IngredientName { get; set; } // we display this (derived)
        public string IngredientLot { get; set; } = string.Empty; // user selects this (FK)

        public bool Allergens { get; set; }
        public bool HygenicCondition { get; set; }

        public double AmountPerBatch { get; set; }

        [DataType(DataType.Date)]
        public DateTime ExpiryDate { get; set; } = DateTime.Today;

        public string? Comments { get; set; }
    }
}

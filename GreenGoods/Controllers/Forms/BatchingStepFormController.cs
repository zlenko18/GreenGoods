using GreenBowl.Data;
using GreenBowl.Models;
using GreenBowl.Models.ViewModels;
using GreenBowl.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace GreenBowl.Controllers
{
    public class BatchingStepFormController : FormControllerBase
    {
        private readonly InventoryCalculationService _inventoryCalc;

        public BatchingStepFormController(AppDbContext context, InventoryCalculationService inventoryCalc) : base(context)
        {
            _inventoryCalc = inventoryCalc;
        }

        // GET: /BatchingStepForm/Edit?lot=145
        public async Task<IActionResult> Edit(int lot)
        {
            if (!await LotExistsAsync(lot))
            {
                TempData["Error"] = $"Lot {lot} does not exist. Create it in Product Inventory first.";
                return RedirectToAction("Index", "ProductInventory");
            }

            var header = await _context.BatchingControlBatchingSteps
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Lot == lot && x.Batch == BatchCode.A);

            var rows = await _context.BatchBCBS
                .AsNoTracking()
                .Where(x => x.Lot == lot)
                .OrderBy(x => x.Batch)
                .ThenBy(x => x.IngredientName)
                .ToListAsync();

            var vm = new BatchingStepFormVM
            {
                Lot = lot,
                EffectiveDate = header?.EffectiveDate ?? DateTime.Today,
                ProductName = header?.ProductName ?? default,
                ProductionDate = header?.ProductionDate ?? DateTime.Today,
                ProductionSupervisor = header?.ProductionSupervisor ?? string.Empty
            };

            await PopulateIngredientOptions(vm);

            LoadBatch(vm.A, rows.Where(x => x.Batch == BatchCode.A));
            LoadBatch(vm.B, rows.Where(x => x.Batch == BatchCode.B));
            LoadBatch(vm.C, rows.Where(x => x.Batch == BatchCode.C));
            LoadBatch(vm.D, rows.Where(x => x.Batch == BatchCode.D));

            // Ensure at least one empty row per batch to start with
            EnsureOneBlankRow(vm.A);
            EnsureOneBlankRow(vm.B);
            EnsureOneBlankRow(vm.C);
            EnsureOneBlankRow(vm.D);

            return View(vm);
        }

        // POST: /BatchingStepForm/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(BatchingStepFormVM vm)
        {
            await PopulateIngredientOptions(vm);

            return await SaveOrShowErrorAsync(
                vm,
                successMessage: "Batching steps saved.",
                failureMessage: "Failed to save batching steps.",
                successRedirect: () => RedirectToAction(nameof(Edit), new { lot = vm.Lot }),
                failureReturn: () => View(vm),
                mutate: async () =>
                {
                    // header replicated per (Batch,Lot)
                    await UpsertHeaderAsync(vm, BatchCode.A);
                    await UpsertHeaderAsync(vm, BatchCode.B);
                    await UpsertHeaderAsync(vm, BatchCode.C);
                    await UpsertHeaderAsync(vm, BatchCode.D);

                    // wipe + reinsert rows per batch
                    await ReplaceLinesAsync(vm.A, vm.Lot);
                    await ReplaceLinesAsync(vm.B, vm.Lot);
                    await ReplaceLinesAsync(vm.C, vm.Lot);
                    await ReplaceLinesAsync(vm.D, vm.Lot);

                    await _inventoryCalc.RecalculateAsync(vm.Lot);
                }
            );
        }

        // Add one new empty row to a batch column
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddRow(BatchingStepFormVM vm, BatchCode batch)
        {
            await PopulateIngredientOptions(vm);

            var target = GetBatch(vm, batch);
            target.Lines.Add(new BatchingStepLineVM { Batch = batch, Lot = vm.Lot });

            return View("Edit", vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveRow(BatchingStepFormVM vm, BatchCode batch, int index)
        {
            await PopulateIngredientOptions(vm);

            var target = GetBatch(vm, batch);
            if (index >= 0 && index < target.Lines.Count)
                target.Lines.RemoveAt(index);

            EnsureOneBlankRow(target);
            return View("Edit", vm);
        }

        // Helpers 

        private static BatchingStepBatchVM GetBatch(BatchingStepFormVM vm, BatchCode batch) =>
            batch switch
            {
                BatchCode.A => vm.A,
                BatchCode.B => vm.B,
                BatchCode.C => vm.C,
                BatchCode.D => vm.D,
                _ => vm.A
            };

        private static void LoadBatch(BatchingStepBatchVM target, IEnumerable<BatchBCBS> rows)
        {
            target.Lines = rows.Select(x => new BatchingStepLineVM
            {
                Batch = x.Batch,
                Lot = x.Lot,
                IngredientLot = x.IngredientLot,
                IngredientName = x.IngredientName,
                Allergens = x.Allergens,
                HygenicCondition = x.HygenicCondition,
                AmountPerBatch = x.AmountPerBatch,
                ExpiryDate = x.ExpiryDate,
                Comments = x.Comments
            }).ToList();
        }

        private static void EnsureOneBlankRow(BatchingStepBatchVM batchVm)
        {
            if (batchVm.Lines.Count == 0)
                batchVm.Lines.Add(new BatchingStepLineVM { Batch = batchVm.Batch, Lot = batchVm.Lot });
        }

        private async Task PopulateIngredientOptions(BatchingStepFormVM vm)
        {
            var inv = await _context.IngredientsInventories
                .AsNoTracking()
                .OrderBy(x => x.IngredientLot)
                .ToListAsync();

            vm.IngredientLotOptions = inv.Select(x =>
                new SelectListItem(
                    text: $"{x.IngredientLot} ({x.IngredientName})",
                    value: x.IngredientLot
                )).ToList();
        }

        private async Task UpsertHeaderAsync(BatchingStepFormVM vm, BatchCode batch)
        {
            var entity = await _context.BatchingControlBatchingSteps
                .FirstOrDefaultAsync(x => x.Batch == batch && x.Lot == vm.Lot);

            if (entity == null)
            {
                entity = new BatchingControlBatchingStep { Batch = batch, Lot = vm.Lot };
                _context.BatchingControlBatchingSteps.Add(entity);
            }

            entity.EffectiveDate = vm.EffectiveDate;
            entity.ProductName = vm.ProductName;
            entity.ProductionDate = vm.ProductionDate;
            entity.ProductionSupervisor = vm.ProductionSupervisor ?? string.Empty;
        }

        private async Task ReplaceLinesAsync(BatchingStepBatchVM batchVm, int lot)
        {
            batchVm.Lot = lot;

            // Wipe existing rows for that batch+lot
            var wipe = await _context.BatchBCBS
                .Where(x => x.Batch == batchVm.Batch && x.Lot == lot)
                .ToListAsync();

            _context.BatchBCBS.RemoveRange(wipe);

            // Insert from VM, skip blank lines
            foreach (var line in batchVm.Lines)
            {
                if (string.IsNullOrWhiteSpace(line.IngredientLot))
                    continue;

                // Derive IngredientName from inventory (prevents mismatch)
                var inv = await _context.IngredientsInventories
                    .AsNoTracking()
                    .FirstOrDefaultAsync(x => x.IngredientLot == line.IngredientLot);

                if (inv == null)
                {
                    ModelState.AddModelError("", $"IngredientLot '{line.IngredientLot}' does not exist in Ingredients Inventory.");
                    continue;
                }

                _context.BatchBCBS.Add(new BatchBCBS
                {
                    Batch = batchVm.Batch,
                    Lot = lot,
                    IngredientLot = line.IngredientLot,
                    IngredientName = inv.IngredientName,

                    Allergens = line.Allergens,
                    HygenicCondition = line.HygenicCondition,
                    AmountPerBatch = line.AmountPerBatch,
                    ExpiryDate = line.ExpiryDate,
                    Comments = line.Comments
                });
            }
        }
    }
}

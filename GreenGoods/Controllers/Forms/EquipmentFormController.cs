using GreenBowl.Data;
using GreenBowl.Models;
using GreenBowl.Models.ViewModels;
using GreenBowl.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GreenBowl.Controllers
{
    public class EquipmentFormController : FormControllerBase
    {
        private readonly InventoryCalculationService _inventoryCalc;

        public EquipmentFormController(AppDbContext context, InventoryCalculationService inventoryCalc) : base(context)
        {
            _inventoryCalc = inventoryCalc;
        }

        // GET: /EquipmentForm/Edit?lot=145
        public async Task<IActionResult> Edit(int lot)
        {
            if (!await LotExistsAsync(lot))
            {
                TempData["Error"] = $"Lot {lot} does not exist. Create it in Product Inventory first.";
                return RedirectToAction("Index", "ProductInventory");
            }

            // Header (store per A, show as header values)
            var header = await _context.BatchingControlEquipments
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Lot == lot && x.Batch == BatchCode.A);

            var details = await _context.BatchBCEs
                .AsNoTracking()
                .Where(x => x.Lot == lot)
                .ToListAsync();

            var vm = new EquipmentFormVM
            {
                Lot = lot,
                EffectiveDate = header?.EffectiveDate ?? DateTime.Today,
                RevisionNumber = header?.RevisionNumber ?? 0,
                ProductionDate = header?.ProductionDate ?? DateTime.Today,
                FillerSupervisor = header?.FillerSupervisor ?? string.Empty
            };

            LoadBatch(vm.A, details.FirstOrDefault(x => x.Batch == BatchCode.A), lot);
            LoadBatch(vm.B, details.FirstOrDefault(x => x.Batch == BatchCode.B), lot);
            LoadBatch(vm.C, details.FirstOrDefault(x => x.Batch == BatchCode.C), lot);
            LoadBatch(vm.D, details.FirstOrDefault(x => x.Batch == BatchCode.D), lot);

            return View(vm);
        }

        // POST: /EquipmentForm/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public Task<IActionResult> Edit(EquipmentFormVM vm)
        {
            return SaveOrShowErrorAsync(
                vm,
                successMessage: "Equipment saved.",
                failureMessage: "Failed to save equipment.",
                successRedirect: () => RedirectToAction(nameof(Edit), new { lot = vm.Lot }),
                failureReturn: () => View(vm),
                mutate: async () =>
                {
                    // Header replicated to A/B/C/D
                    await UpsertHeaderAsync(vm, BatchCode.A);
                    await UpsertHeaderAsync(vm, BatchCode.B);
                    await UpsertHeaderAsync(vm, BatchCode.C);
                    await UpsertHeaderAsync(vm, BatchCode.D);

                    // Details per batch
                    await UpsertDetailAsync(vm.A, vm.Lot);
                    await UpsertDetailAsync(vm.B, vm.Lot);
                    await UpsertDetailAsync(vm.C, vm.Lot);
                    await UpsertDetailAsync(vm.D, vm.Lot);

                    await _inventoryCalc.RecalculateAsync(vm.Lot);
                }
            );
        }

        private static void LoadBatch(EquipmentBatchVM target, BatchBCE? src, int lot)
        {
            target.Lot = lot;
            if (src == null) return;

            target.Equipments = src.Equipments ?? new List<EquipmentType>();
            target.CIP_COPBeforeStarting = src.CIP_COPBeforeStarting;
            target.FunctionCondition = src.FunctionCondition;
            target.HygienicCondition = src.HygienicCondition;
            target.Comments = src.Comments;
            target.InitialOfController = src.InitialOfController;
        }

        private async Task UpsertHeaderAsync(EquipmentFormVM vm, BatchCode batch)
        {
            var entity = await _context.BatchingControlEquipments
                .FirstOrDefaultAsync(x => x.Batch == batch && x.Lot == vm.Lot);

            if (entity == null)
            {
                entity = new BatchingControlEquipment { Batch = batch, Lot = vm.Lot };
                _context.BatchingControlEquipments.Add(entity);
            }

            entity.EffectiveDate = vm.EffectiveDate;
            entity.RevisionNumber = vm.RevisionNumber;
            entity.ProductionDate = vm.ProductionDate;
            entity.FillerSupervisor = vm.FillerSupervisor ?? string.Empty;
        }

        private async Task UpsertDetailAsync(EquipmentBatchVM src, int lot)
        {
            src.Lot = lot;

            var entity = await _context.BatchBCEs
                .FirstOrDefaultAsync(x => x.Batch == src.Batch && x.Lot == src.Lot);

            if (entity == null)
            {
                entity = new BatchBCE { Batch = src.Batch, Lot = src.Lot };
                _context.BatchBCEs.Add(entity);
            }

            entity.Equipments = src.Equipments ?? new List<EquipmentType>();
            entity.CIP_COPBeforeStarting = src.CIP_COPBeforeStarting;
            entity.FunctionCondition = src.FunctionCondition;
            entity.HygienicCondition = src.HygienicCondition;
            entity.Comments = src.Comments ?? string.Empty;
            entity.InitialOfController = src.InitialOfController ?? string.Empty;
        }
    }
}

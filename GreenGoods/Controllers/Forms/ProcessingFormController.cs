using GreenBowl.Data;
using GreenBowl.Models;
using GreenBowl.Models.ViewModels;
using GreenBowl.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GreenBowl.Controllers
{
    public class ProcessingFormController : FormControllerBase
    {
        private readonly InventoryCalculationService _inventoryCalc;

        public ProcessingFormController(AppDbContext context, InventoryCalculationService inventoryCalc) : base(context)
        {
            _inventoryCalc = inventoryCalc;
        }

        // GET: /ProcessingForm/Edit?lot=145
        public async Task<IActionResult> Edit(int lot)
        {
            if (!await LotExistsAsync(lot))
            {
                TempData["Error"] = $"Lot {lot} does not exist. Create it in Product Inventory first.";
                return RedirectToAction("Index", "ProductInventory");
            }

            var header = await _context.BatchingControlProcessings
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Lot == lot && x.Batch == BatchCode.A);

            var details = await _context.BatchBCPrs
                .AsNoTracking()
                .Where(x => x.Lot == lot)
                .ToListAsync();

            var checks = await _context.BCPrChecks
                .AsNoTracking()
                .Where(x => x.Lot == lot)
                .OrderBy(x => x.Batch).ThenBy(x => x.TimeOfCheck)
                .ToListAsync();

            var vm = new ProcessingFormVM
            {
                Lot = lot,
                ProductionDate = header?.ProductionDate ?? DateTime.Today,
                ProductionSupervisor = header?.ProductionSupervisor ?? string.Empty,
            };

            LoadBatch(vm.A, details.FirstOrDefault(x => x.Batch == BatchCode.A), lot);
            LoadBatch(vm.B, details.FirstOrDefault(x => x.Batch == BatchCode.B), lot);
            LoadBatch(vm.C, details.FirstOrDefault(x => x.Batch == BatchCode.C), lot);
            LoadBatch(vm.D, details.FirstOrDefault(x => x.Batch == BatchCode.D), lot);

            vm.A.Checks = checks.Where(x => x.Batch == BatchCode.A).Select(ToCheckVM).ToList();
            vm.B.Checks = checks.Where(x => x.Batch == BatchCode.B).Select(ToCheckVM).ToList();
            vm.C.Checks = checks.Where(x => x.Batch == BatchCode.C).Select(ToCheckVM).ToList();
            vm.D.Checks = checks.Where(x => x.Batch == BatchCode.D).Select(ToCheckVM).ToList();

            return View(vm);
        }

        // POST: /ProcessingForm/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public Task<IActionResult> Edit(ProcessingFormVM vm)
        {
            return SaveOrShowErrorAsync(
                vm,
                successMessage: "Processing saved.",
                failureMessage: "Failed to save processing.",
                successRedirect: () => RedirectToAction(nameof(Edit), new { lot = vm.Lot }),
                failureReturn: () => View(vm),
                mutate: async () =>
                {
                    // Header stored per (Batch,Lot)
                    await UpsertHeaderAsync(vm, BatchCode.A);
                    await UpsertHeaderAsync(vm, BatchCode.B);
                    await UpsertHeaderAsync(vm, BatchCode.C);
                    await UpsertHeaderAsync(vm, BatchCode.D);

                    await UpsertDetailAsync(vm.A, vm.Lot);
                    await UpsertDetailAsync(vm.B, vm.Lot);
                    await UpsertDetailAsync(vm.C, vm.Lot);
                    await UpsertDetailAsync(vm.D, vm.Lot);

                    // Generate checks based on time window
                    await EnsureProcessingChecksAsync(vm.A, vm.Lot);
                    await EnsureProcessingChecksAsync(vm.B, vm.Lot);
                    await EnsureProcessingChecksAsync(vm.C, vm.Lot);
                    await EnsureProcessingChecksAsync(vm.D, vm.Lot);

                    // Apply checkbox edits 
                    await ApplyChecksAsync(vm.A, vm.Lot);
                    await ApplyChecksAsync(vm.B, vm.Lot);
                    await ApplyChecksAsync(vm.C, vm.Lot);
                    await ApplyChecksAsync(vm.D, vm.Lot);

                    await _inventoryCalc.RecalculateAsync(vm.Lot);
                }
            );
        }

        // POST: /ProcessingForm/GenerateChecks
        [HttpPost]
        [ValidateAntiForgeryToken]
        public Task<IActionResult> GenerateChecks(ProcessingFormVM vm)
        {
            return SaveOrShowErrorAsync(
                vm,
                successMessage: "Checks generated.",
                failureMessage: "Failed to generate checks.",
                successRedirect: () => RedirectToAction(nameof(Edit), new { lot = vm.Lot }),
                failureReturn: () => View("Edit", vm),
                mutate: async () =>
                {
                    // Make sure header + detail rows exist
                    await UpsertHeaderAsync(vm, BatchCode.A);
                    await UpsertHeaderAsync(vm, BatchCode.B);
                    await UpsertHeaderAsync(vm, BatchCode.C);
                    await UpsertHeaderAsync(vm, BatchCode.D);

                    await UpsertDetailAsync(vm.A, vm.Lot);
                    await UpsertDetailAsync(vm.B, vm.Lot);
                    await UpsertDetailAsync(vm.C, vm.Lot);
                    await UpsertDetailAsync(vm.D, vm.Lot);

                    // Generate/delete checks based on time window (NO history)
                    await EnsureProcessingChecksAsync(vm.A, vm.Lot);
                    await EnsureProcessingChecksAsync(vm.B, vm.Lot);
                    await EnsureProcessingChecksAsync(vm.C, vm.Lot);
                    await EnsureProcessingChecksAsync(vm.D, vm.Lot);
                }
            );
        }

        // Helpers 

        private static void LoadBatch(ProcessingBatchVM target, BatchBCPr? src, int lot)
        {
            target.Lot = lot;

            if (src == null)
                return;

            target.StartTimeProcessing = src.StartTimeProcessing;
            target.FinishTimeProcessing = src.FinishTimeProcessing;
            target.Alarm = src.Alarm;
            target.Reason = src.Reason;
            target.CorrectionAction = src.CorrectionAction;
            target.NumberOfRejectedPouch = src.NumberOfRejectedPouch;
            target.Comments = src.Comments;
        }

        private static ProcessingCheckVM ToCheckVM(BCPrChecks x) => new()
        {
            Batch = x.Batch,
            Lot = x.Lot,
            TimeOfCheck = x.TimeOfCheck,
            XRayRejectedPouches = x.XRayRejectedPouches
        };

        private async Task UpsertHeaderAsync(ProcessingFormVM vm, BatchCode batch)
        {
            var entity = await _context.BatchingControlProcessings
                .FirstOrDefaultAsync(x => x.Batch == batch && x.Lot == vm.Lot);

            if (entity == null)
            {
                entity = new BatchingControlProcessing
                {
                    Batch = batch,
                    Lot = vm.Lot
                };
                _context.BatchingControlProcessings.Add(entity);
            }

            entity.ProductionDate = vm.ProductionDate;
            entity.ProductionSupervisor = vm.ProductionSupervisor ?? string.Empty;
        }

        private async Task UpsertDetailAsync(ProcessingBatchVM src, int lot)
        {
            src.Lot = lot;

            var entity = await _context.BatchBCPrs
                .FirstOrDefaultAsync(x => x.Batch == src.Batch && x.Lot == src.Lot);

            if (entity == null)
            {
                entity = new BatchBCPr
                {
                    Batch = src.Batch,
                    Lot = src.Lot
                };
                _context.BatchBCPrs.Add(entity);
            }

            entity.StartTimeProcessing = src.StartTimeProcessing;
            entity.FinishTimeProcessing = src.FinishTimeProcessing;
            entity.Alarm = src.Alarm;
            entity.Reason = src.Reason ?? string.Empty;
            entity.CorrectionAction = src.CorrectionAction ?? string.Empty;
            entity.NumberOfRejectedPouch = src.NumberOfRejectedPouch;
            entity.Comments = src.Comments ?? string.Empty;
        }

        // Wipe + regenerate 15-min checks
        private async Task EnsureProcessingChecksAsync(ProcessingBatchVM batchVm, int lot)
        {
            var batch = batchVm.Batch;

            var start = batchVm.StartTimeProcessing.TimeOfDay;
            var finish = batchVm.FinishTimeProcessing.TimeOfDay;

            // Wipe existing checks for this batch+lot first
            var wipe = await _context.BCPrChecks
                .Where(x => x.Batch == batch && x.Lot == lot)
                .ToListAsync();

            _context.BCPrChecks.RemoveRange(wipe);

            // If invalid window, stop after wiping
            if (finish <= start)
                return;

            var baseDate = DateTime.Today;

            for (var t = start; t <= finish; t = t.Add(TimeSpan.FromMinutes(15)))
            {
                _context.BCPrChecks.Add(new BCPrChecks
                {
                    Batch = batch,
                    Lot = lot,
                    TimeOfCheck = baseDate + t,
                    XRayRejectedPouches = false
                });
            }
        }

        private async Task ApplyChecksAsync(ProcessingBatchVM batchVm, int lot)
        {
            if (batchVm.Checks == null || batchVm.Checks.Count == 0)
                return;

            foreach (var c in batchVm.Checks)
            {
                var row = await _context.BCPrChecks
                    .FirstOrDefaultAsync(x =>
                        x.Batch == batchVm.Batch &&
                        x.Lot == lot &&
                        x.TimeOfCheck == c.TimeOfCheck);

                if (row != null)
                    row.XRayRejectedPouches = c.XRayRejectedPouches;
            }
        }
    }
}

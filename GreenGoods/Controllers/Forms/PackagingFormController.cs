using GreenBowl.Data;
using GreenBowl.Models;
using GreenBowl.Models.ViewModels;
using GreenBowl.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GreenBowl.Controllers
{
    public class PackagingFormController : FormControllerBase
    {
        private readonly InventoryCalculationService _inventoryCalc;

        public PackagingFormController(AppDbContext context, InventoryCalculationService inventoryCalc) : base(context)
        {
            _inventoryCalc = inventoryCalc;
        }

        public async Task<IActionResult> Edit(int lot)
        {
            if (!await LotExistsAsync(lot))
            {
                TempData["Error"] = $"Lot {lot} does not exist. Create it in Product Inventory first.";
                return RedirectToAction("Index", "ProductInventory");
            }

            var header = await _context.BatchingControlPackagings
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Lot == lot && x.Batch == BatchCode.A);

            var details = await _context.BatchBCPas
                .AsNoTracking()
                .Where(x => x.Lot == lot)
                .ToListAsync();

            var checks = await _context.BCPaChecks
                .AsNoTracking()
                .Where(x => x.Lot == lot)
                .OrderBy(x => x.Batch).ThenBy(x => x.TimeOfCheck)
                .ToListAsync();

            var vm = new PackagingFormVM
            {
                Lot = lot,
                ProductionDate = header?.ProductionDate ?? DateTime.Today,
                FillerSupervisor = header?.FillerSupervisor ?? string.Empty,
                TypeOfPackaging = header?.TypeOfPackaging ?? string.Empty
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public Task<IActionResult> Edit(PackagingFormVM vm)
        {
            return SaveOrShowErrorAsync(
                vm,
                successMessage: "Packaging saved.",
                failureMessage: "Failed to save packaging.",
                successRedirect: () => RedirectToAction(nameof(Edit), new { lot = vm.Lot }),
                failureReturn: () => View(vm),
                mutate: async () =>
                {
                    await UpsertHeaderAsync(vm, BatchCode.A);
                    await UpsertHeaderAsync(vm, BatchCode.B);
                    await UpsertHeaderAsync(vm, BatchCode.C);
                    await UpsertHeaderAsync(vm, BatchCode.D);

                    await UpsertDetailAsync(vm.A, vm.Lot);
                    await UpsertDetailAsync(vm.B, vm.Lot);
                    await UpsertDetailAsync(vm.C, vm.Lot);
                    await UpsertDetailAsync(vm.D, vm.Lot);

                    await ApplyChecksAsync(vm.A, vm.Lot);
                    await ApplyChecksAsync(vm.B, vm.Lot);
                    await ApplyChecksAsync(vm.C, vm.Lot);
                    await ApplyChecksAsync(vm.D, vm.Lot);

                    await _inventoryCalc.RecalculateAsync(vm.Lot);
                }
            );
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public Task<IActionResult> GenerateChecks(PackagingFormVM vm)
        {
            return SaveOrShowErrorAsync(
                vm,
                successMessage: "Checks generated.",
                failureMessage: "Failed to generate checks.",
                successRedirect: () => RedirectToAction(nameof(Edit), new { lot = vm.Lot }),
                failureReturn: () => View("Edit", vm),
                mutate: async () =>
                {
                    await UpsertHeaderAsync(vm, BatchCode.A);
                    await UpsertHeaderAsync(vm, BatchCode.B);
                    await UpsertHeaderAsync(vm, BatchCode.C);
                    await UpsertHeaderAsync(vm, BatchCode.D);

                    await UpsertDetailAsync(vm.A, vm.Lot);
                    await UpsertDetailAsync(vm.B, vm.Lot);
                    await UpsertDetailAsync(vm.C, vm.Lot);
                    await UpsertDetailAsync(vm.D, vm.Lot);

                    await EnsureChecksAsync(vm.A, vm.Lot);
                    await EnsureChecksAsync(vm.B, vm.Lot);
                    await EnsureChecksAsync(vm.C, vm.Lot);
                    await EnsureChecksAsync(vm.D, vm.Lot);

                    await _inventoryCalc.RecalculateAsync(vm.Lot);
                }
            );
        }

        private static void LoadBatch(PackagingBatchVM target, BatchBCPa? src, int lot)
        {
            target.Lot = lot;
            if (src == null) return;

            target.StartTimeFilling = src.StartTimeFilling;
            target.FinishTimeFilling = src.FinishTimeFilling;
            target.Lot = src.Lot;
            target.PouchHygienicCondition = src.PouchHygienicCondition;
            target.NumberOfPouchMade = src.NumberOfPouchMade;
            target.WeightRejected = src.WeightRejected;
            target.SealingRejected = src.SealingRejected;
            target.NoPrintRejected = src.NoPrintRejected;
            target.Comments = src.Comments;
        }

        private static PackagingCheckVM ToCheckVM(BCPaChecks x) => new()
        {
            Batch = x.Batch,
            Lot = x.Lot,
            TimeOfCheck = x.TimeOfCheck,
            SealingCondition = x.SealingCondition
        };

        private async Task UpsertHeaderAsync(PackagingFormVM vm, BatchCode batch)
        {
            var entity = await _context.BatchingControlPackagings
                .FirstOrDefaultAsync(x => x.Batch == batch && x.Lot == vm.Lot);

            if (entity == null)
            {
                entity = new BatchingControlPackaging { Batch = batch, Lot = vm.Lot };
                _context.BatchingControlPackagings.Add(entity);
            }

            entity.ProductionDate = vm.ProductionDate;
            entity.FillerSupervisor = vm.FillerSupervisor ?? string.Empty;
            entity.TypeOfPackaging = vm.TypeOfPackaging ?? string.Empty;
        }

        private async Task UpsertDetailAsync(PackagingBatchVM src, int lot)
        {
            src.Lot = lot;

            var entity = await _context.BatchBCPas
                .FirstOrDefaultAsync(x => x.Batch == src.Batch && x.Lot == src.Lot);

            if (entity == null)
            {
                entity = new BatchBCPa { Batch = src.Batch, Lot = src.Lot };
                _context.BatchBCPas.Add(entity);
            }

            entity.StartTimeFilling = src.StartTimeFilling;
            entity.FinishTimeFilling = src.FinishTimeFilling;
            entity.Lot = src.Lot;
            entity.PouchHygienicCondition = src.PouchHygienicCondition;
            entity.NumberOfPouchMade = src.NumberOfPouchMade;
            entity.WeightRejected = src.WeightRejected;
            entity.SealingRejected = src.SealingRejected;
            entity.NoPrintRejected = src.NoPrintRejected;
            entity.Comments = src.Comments ?? string.Empty;
        }

        private async Task EnsureChecksAsync(PackagingBatchVM batchVm, int lot)
        {
            var batch = batchVm.Batch;
            var start = batchVm.StartTimeFilling.TimeOfDay;
            var finish = batchVm.FinishTimeFilling.TimeOfDay;

            var wipe = await _context.BCPaChecks.Where(x => x.Batch == batch && x.Lot == lot).ToListAsync();
            _context.BCPaChecks.RemoveRange(wipe);

            if (finish <= start) return;

            var baseDate = DateTime.Today;
            for (var t = start; t <= finish; t = t.Add(TimeSpan.FromMinutes(15)))
            {
                _context.BCPaChecks.Add(new BCPaChecks
                {
                    Batch = batch,
                    Lot = lot,
                    TimeOfCheck = baseDate + t,
                    SealingCondition = false
                });
            }
        }

        private async Task ApplyChecksAsync(PackagingBatchVM batchVm, int lot)
        {
            if (batchVm.Checks == null || batchVm.Checks.Count == 0)
                return;

            foreach (var c in batchVm.Checks)
            {
                var row = await _context.BCPaChecks
                    .FirstOrDefaultAsync(x => x.Batch == batchVm.Batch && x.Lot == lot && x.TimeOfCheck == c.TimeOfCheck);

                if (row != null)
                    row.SealingCondition = c.SealingCondition;
            }
        }
    }
}

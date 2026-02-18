using GreenBowl.Data;
using GreenBowl.Models;
using GreenBowl.Models.ViewModels;
using GreenBowl.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GreenBowl.Controllers
{
    public class XRayFormController : FormControllerBase
    {
        private readonly InventoryCalculationService _inventoryCalc;

        public XRayFormController(AppDbContext context, InventoryCalculationService inventoryCalc) : base(context)
        {
            _inventoryCalc = inventoryCalc;
        }

        // GET: /XRayForm/Edit?lot=145
        public async Task<IActionResult> Edit(int lot)
        {
            var inv = await _context.ProductInventories.AsNoTracking().FirstOrDefaultAsync(x => x.Lot == lot);
            if (inv == null)
            {
                TempData["Error"] = $"Lot {lot} does not exist. Create it first.";
                return RedirectToAction("Index", "ProductInventory");
            }

            var rows = await _context.XRayMonitoringRecords
                .AsNoTracking()
                .Where(x => x.Lot == lot)
                .ToListAsync();

            var vm = new XRayFormVM
            {
                Lot = lot,
                ProductName = inv.ProductName,
                Date = DateTime.Today
            };

            Load(vm.A, rows.FirstOrDefault(x => x.Batch == BatchCode.A), inv, lot);
            Load(vm.B, rows.FirstOrDefault(x => x.Batch == BatchCode.B), inv, lot);
            Load(vm.C, rows.FirstOrDefault(x => x.Batch == BatchCode.C), inv, lot);
            Load(vm.D, rows.FirstOrDefault(x => x.Batch == BatchCode.D), inv, lot);

            return View(vm);
        }

        // POST: /XRayForm/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(XRayFormVM vm)
        {
            // Redirect behaviour if lot doesn't exist.
            var inv = await _context.ProductInventories.FirstOrDefaultAsync(x => x.Lot == vm.Lot);
            if (inv == null)
            {
                TempData["Error"] = $"Lot {vm.Lot} does not exist.";
                return RedirectToAction("Index", "ProductInventory");
            }

            return await SaveOrShowErrorAsync(
                vm,
                successMessage: "X-Ray saved.",
                failureMessage: "Failed to save X-Ray.",
                successRedirect: () => RedirectToAction(nameof(Edit), new { lot = vm.Lot }),
                failureReturn: () => View(vm),
                mutate: async () =>
                {
                    await Upsert(vm.A, inv);
                    await Upsert(vm.B, inv);
                    await Upsert(vm.C, inv);
                    await Upsert(vm.D, inv);

                    await _inventoryCalc.RecalculateAsync(vm.Lot);
                }
            );
        }

        // Helpers

        private static void Load(XRayBatchVM target, XRayMonitoringRecord? src, ProductInventory inv, int lot)
        {
            target.Lot = lot;
            target.ProductName = inv.ProductName;
            target.Date = DateTime.Today;

            if (src == null) return;

            target.ProductName = src.ProductName;
            target.Date = src.Date;
            target.StartTime = src.StartTime;
            target.EndTime = src.EndTime;
            target.Indicator1 = src.Indicator1;
            target.Indicator2 = src.Indicator2;
            target.Indicator3 = src.Indicator3;
            target.Deviation_CorrectiveAction = src.Deviation_CorrectiveAction;
        }

        private async Task Upsert(XRayBatchVM src, ProductInventory inv)
        {
            src.Lot = inv.Lot;
            src.ProductName = inv.ProductName;

            var e = await _context.XRayMonitoringRecords
                .FirstOrDefaultAsync(x => x.Batch == src.Batch && x.Lot == src.Lot);

            if (e == null)
            {
                e = new XRayMonitoringRecord { Batch = src.Batch, Lot = src.Lot };
                _context.XRayMonitoringRecords.Add(e);
            }

            e.ProductName = inv.ProductName;
            e.Date = src.Date;
            e.StartTime = src.StartTime;
            e.EndTime = src.EndTime;
            e.Indicator1 = src.Indicator1;
            e.Indicator2 = src.Indicator2;
            e.Indicator3 = src.Indicator3;
            e.Deviation_CorrectiveAction = src.Deviation_CorrectiveAction ?? string.Empty;
        }
    }
}

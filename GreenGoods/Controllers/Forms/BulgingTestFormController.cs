using GreenBowl.Data;
using GreenBowl.Models;
using GreenBowl.Models.ViewModels;
using GreenBowl.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GreenBowl.Controllers
{
    public class BulgingTestFormController : FormControllerBase
    {
        private readonly InventoryCalculationService _inventoryCalc;

        public BulgingTestFormController(AppDbContext context, InventoryCalculationService inventoryCalc) : base(context)
        {
            _inventoryCalc = inventoryCalc;
        }

        // GET: /BulgingTestForm/Edit?lot=145
        public async Task<IActionResult> Edit(int lot)
        {
            var inv = await _context.ProductInventories.AsNoTracking().FirstOrDefaultAsync(x => x.Lot == lot);
            if (inv == null)
            {
                TempData["Error"] = $"Lot {lot} does not exist. Create it first.";
                return RedirectToAction("Index", "ProductInventory");
            }

            var entity = await _context.BulgingTestOnPouches.AsNoTracking().FirstOrDefaultAsync(x => x.Lot == lot);

            var vm = new BulgingFormVM
            {
                Lot = lot,
                ProductName = inv.ProductName,

                NumberOfPouch = entity?.NumberOfPouch ?? 0,
                ProductionDate = entity?.ProductionDate ?? inv.ProductionDate,
                ExpiryDate = entity?.ExpiryDate ?? inv.ExpiryDate,
                TimeOfIncubation = entity?.TimeOfIncubation ?? 0,
                TemperatureOfIncubation = entity?.TemperatureOfIncubation ?? 0,
                Results = entity?.Results ?? false,
                Comments = entity?.Comments ?? string.Empty,
                Taste = entity?.Taste ?? false,
                DateOfRelease = entity?.DateOfRelease ?? DateTime.Today,
                QAInitial = entity?.QAInitial ?? string.Empty,
                TPC = entity?.TPC ?? 0,
                YM = entity?.YM ?? 0
            };

            return View(vm);
        }

        // POST: /BulgingTestForm/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(BulgingFormVM vm)
        {
            var inv = await _context.ProductInventories.FirstOrDefaultAsync(x => x.Lot == vm.Lot);
            if (inv == null)
            {
                TempData["Error"] = $"Lot {vm.Lot} does not exist.";
                return RedirectToAction("Index", "ProductInventory");
            }

            return await SaveOrShowErrorAsync(
                vm,
                successMessage: "Bulging test saved.",
                failureMessage: "Failed to save bulging test.",
                successRedirect: () => RedirectToAction(nameof(Edit), new { lot = vm.Lot }),
                failureReturn: () => View(vm),
                mutate: async () =>
                {
                    var entity = await _context.BulgingTestOnPouches.FirstOrDefaultAsync(x => x.Lot == vm.Lot);
                    if (entity == null)
                    {
                        entity = new BulgingTestOnPouch { Lot = vm.Lot };
                        _context.BulgingTestOnPouches.Add(entity);
                    }

                    entity.ProductName = inv.ProductName;

                    entity.NumberOfPouch = vm.NumberOfPouch;
                    entity.ProductionDate = vm.ProductionDate;
                    entity.ExpiryDate = vm.ExpiryDate;
                    entity.TimeOfIncubation = vm.TimeOfIncubation;
                    entity.TemperatureOfIncubation = vm.TemperatureOfIncubation;
                    entity.Results = vm.Results;
                    entity.Comments = vm.Comments ?? string.Empty;
                    entity.Taste = vm.Taste;
                    entity.DateOfRelease = vm.DateOfRelease;
                    entity.QAInitial = vm.QAInitial ?? string.Empty;
                    entity.TPC = vm.TPC;
                    entity.YM = vm.YM;

                    await _inventoryCalc.RecalculateAsync(vm.Lot);
                }
            );
        }
    }
}

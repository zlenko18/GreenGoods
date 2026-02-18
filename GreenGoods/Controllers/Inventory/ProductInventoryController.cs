using GreenBowl.Data;
using GreenBowl.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GreenBowl.Services;
using Microsoft.AspNetCore.Authorization;

namespace GreenBowl.Controllers
{
    public class ProductInventoryController : Controller
    {
        private readonly AppDbContext _context;
        private readonly InventoryCalculationService _inventoryCalc;
        public ProductInventoryController(AppDbContext context, InventoryCalculationService inventoryCalc)
        {
            _context = context;
            _inventoryCalc = inventoryCalc;
        }

        // GET: /ProductInventory
        public async Task<IActionResult> Index()
        {
            try
            {
                var items = await _context.ProductInventories
                    .AsNoTracking()
                    .OrderByDescending(x => x.Lot)
                    .ToListAsync();

                return View(items);
            }
            catch (Exception ex)
            {
                TempData["Err"] = $"Failed to load Product Inventory. {ex.Message}";
                return View(new List<ProductInventory>());
            }
        }

        // GET: /ProductInventory/Create
        public IActionResult Create()
        {
            return View(new ProductInventory
            {
                ProductionDate = DateTime.Today,
                ExpiryDate = DateTime.Today.AddMonths(6),
                CartonDate = DateTime.Today
            });
        }

        // POST: /ProductInventory/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductInventory model)
        {
            if (!ModelState.IsValid)
            {
                TempData["Err"] = "Please fix validation errors and try again.";
                return View(model);
            }

            try
            {
                _context.ProductInventories.Add(model);
                await _context.SaveChangesAsync();

                TempData["Ok"] = $"Lot {model.Lot} created.";
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException dbEx)
            {
                TempData["Err"] = $"Database rejected the save. {dbEx.InnerException?.Message ?? dbEx.Message}";
                return View(model);
            }
            catch (Exception ex)
            {
                TempData["Err"] = $"Unexpected error while creating lot. {ex.Message}";
                return View(model);
            }
        }

        // GET: /ProductInventory/Edit?lot=123
        public async Task<IActionResult> Edit(int lot)
        {
            try
            {
                var item = await _context.ProductInventories
                    .AsNoTracking()
                    .FirstOrDefaultAsync(x => x.Lot == lot);

                if (item == null)
                {
                    TempData["Err"] = $"Lot {lot} not found.";
                    return RedirectToAction(nameof(Index));
                }

                return View(item);
            }
            catch (Exception ex)
            {
                TempData["Err"] = $"Failed to load lot {lot}. {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: /ProductInventory/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ProductInventory model)
        {
            if (!ModelState.IsValid)
            {
                TempData["Err"] = "Please fix validation errors and try again.";
                return View(model);
            }

            try
            {
                var entity = await _context.ProductInventories
                    .FirstOrDefaultAsync(x => x.Lot == model.Lot);

                if (entity == null)
                {
                    TempData["Err"] = $"Lot {model.Lot} not found.";
                    return RedirectToAction(nameof(Index));
                }

                entity.ProductName = model.ProductName;
                entity.BatchWeight = model.BatchWeight;
                entity.Pouch = model.Pouch;

                entity.ProductionDate = model.ProductionDate;
                entity.ExpiryDate = model.ExpiryDate;
                entity.CartonDate = model.CartonDate;

                entity.QACheck = model.QACheck;

                entity.WeightRejected = model.WeightRejected;
                entity.XRayRejected = model.XRayRejected;
                entity.SealingRejected = model.SealingRejected;
                entity.NoPrint = model.NoPrint;
                entity.Sample = model.Sample;
                entity.QCRetention = model.QCRetention;
                entity.TotalRejected = model.TotalRejected;

                entity.ProductForSale = model.ProductForSale;
                entity.TotalCase = model.TotalCase;

                entity.ActualInventory = model.ActualInventory;
                entity.Productivity = model.Productivity;

                await _context.SaveChangesAsync();
                await _inventoryCalc.RecalculateAsync(model.Lot);

                TempData["Ok"] = $"Lot {model.Lot} updated.";
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException dbEx)
            {
                TempData["Err"] = $"Database rejected the save. {dbEx.InnerException?.Message ?? dbEx.Message}";
                return View(model);
            }
            catch (Exception ex)
            {
                TempData["Err"] = $"Unexpected error while saving. {ex.Message}";
                return View(model);
            }
        }

        // GET: /ProductInventory/Delete?lot=123
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int lot)
        {
            try
            {
                var item = await _context.ProductInventories
                    .AsNoTracking()
                    .FirstOrDefaultAsync(x => x.Lot == lot);

                if (item == null)
                {
                    TempData["Err"] = $"Lot {lot} not found.";
                    return RedirectToAction(nameof(Index));
                }

                return View(item);
            }
            catch (Exception ex)
            {
                TempData["Err"] = $"Failed to load delete page. {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: /ProductInventory/Delete
        [Authorize(Roles = "Admin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int lot)
        {
            try
            {
                var entity = await _context.ProductInventories
                    .FirstOrDefaultAsync(x => x.Lot == lot);

                if (entity == null)
                {
                    TempData["Err"] = $"Lot {lot} not found.";
                    return RedirectToAction(nameof(Index));
                }

                _context.ProductInventories.Remove(entity);
                await _context.SaveChangesAsync();

                TempData["Ok"] = $"Lot {lot} deleted.";
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException dbEx)
            {
                TempData["Err"] = $"Cannot delete lot {lot} because it is referenced by other forms. {dbEx.InnerException?.Message ?? dbEx.Message}";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Err"] = $"Unexpected error while deleting. {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }
    }
}

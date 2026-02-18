using GreenBowl.Data;
using GreenBowl.Models;
using GreenBowl.Models.ViewModels;
using GreenBowl.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace GreenBowl.Controllers
{
    public class SellInventoryController : Controller
    {
        private readonly AppDbContext _context;
        private readonly InventoryCalculationService _inventoryCalc;

        public SellInventoryController(AppDbContext context, InventoryCalculationService inventoryCalc)
        {
            _context = context;
            _inventoryCalc = inventoryCalc;
        }

        // GET: /SellInventory
        public async Task<IActionResult> Index()
        {
            try
            {
                var receipts = await _context.Receipts
                    .AsNoTracking()
                    .OrderByDescending(x => x.ID)
                    .ToListAsync();

                return View(receipts);
            }
            catch (Exception ex)
            {
                TempData["Err"] = $"Failed to load receipts. {ex.Message}";
                return View(new List<Receipt>());
            }
        }

        // GET: /SellInventory/Create
        public async Task<IActionResult> Create()
        {
            try
            {
                var vm = new ReceiptVM
                {
                    SellingDate = DateTime.Today,
                    DeliveryDate = DateTime.Today
                };

                await PopulateLotOptions(vm);
                return View(vm);
            }
            catch (Exception ex)
            {
                TempData["Err"] = $"Failed to load create form. {ex.Message}";
                return View(new ReceiptVM());
            }
        }

        // POST: /SellInventory/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ReceiptVM vm)
        {
            // ID is generated server-side
            ModelState.Remove(nameof(vm.ID));

            if (!ModelState.IsValid)
            {
                await PopulateLotOptions(vm);
                TempData["Err"] = "Please fix validation errors and try again.";
                return View(vm);
            }

            try
            {
                vm.ID = await GenerateSellingIDAsync(vm.Lot);

                var receipt = new Receipt
                {
                    ID = vm.ID,
                    Lot = vm.Lot,
                    ProductName = vm.ProductName,
                    Client = vm.Client,
                    Address = vm.Address,
                    SellingDate = vm.SellingDate,
                    DeliveryDate = vm.DeliveryDate,
                    Quantity = vm.Quantity
                };

                _context.Receipts.Add(receipt);
                await _context.SaveChangesAsync();
                await _inventoryCalc.RecalculateAsync(vm.Lot);

                TempData["Ok"] = $"Receipt {vm.ID} created.";
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException dbEx)
            {
                await PopulateLotOptions(vm);
                TempData["Err"] = $"Database rejected the save. {dbEx.InnerException?.Message ?? dbEx.Message}";
                return View(vm);
            }
            catch (Exception ex)
            {
                await PopulateLotOptions(vm);
                TempData["Err"] = $"Unexpected error while creating receipt. {ex.Message}";
                return View(vm);
            }
        }

        // GET: /SellInventory/Edit?id=XX000001
        public async Task<IActionResult> Edit(string id)
        {
            try
            {
                var receipt = await _context.Receipts
                    .AsNoTracking()
                    .FirstOrDefaultAsync(x => x.ID == id);

                if (receipt == null)
                {
                    TempData["Err"] = $"Receipt {id} not found.";
                    return RedirectToAction(nameof(Index));
                }

                var vm = new ReceiptVM
                {
                    ID = receipt.ID,
                    Lot = receipt.Lot,
                    ProductName = receipt.ProductName,
                    Client = receipt.Client,
                    Address = receipt.Address,
                    SellingDate = receipt.SellingDate,
                    DeliveryDate = receipt.DeliveryDate,
                    Quantity = receipt.Quantity
                };

                await PopulateLotOptions(vm);
                return View(vm);
            }
            catch (Exception ex)
            {
                TempData["Err"] = $"Failed to load receipt {id}. {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: /SellInventory/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ReceiptVM vm)
        {
            // ID is the PK; don't validate it from the form
            ModelState.Remove(nameof(vm.ID));

            if (!ModelState.IsValid)
            {
                await PopulateLotOptions(vm);
                TempData["Err"] = "Please fix validation errors and try again.";
                return View(vm);
            }

            try
            {
                var receipt = await _context.Receipts
                    .FirstOrDefaultAsync(x => x.ID == vm.ID);

                if (receipt == null)
                {
                    TempData["Err"] = $"Receipt {vm.ID} not found.";
                    return RedirectToAction(nameof(Index));
                }

                receipt.Lot = vm.Lot;
                receipt.ProductName = vm.ProductName;
                receipt.Client = vm.Client;
                receipt.Address = vm.Address;
                receipt.SellingDate = vm.SellingDate;
                receipt.DeliveryDate = vm.DeliveryDate;
                receipt.Quantity = vm.Quantity;

                await _context.SaveChangesAsync();
                await _inventoryCalc.RecalculateAsync(vm.Lot);

                TempData["Ok"] = $"Receipt {vm.ID} updated.";
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException dbEx)
            {
                await PopulateLotOptions(vm);
                TempData["Err"] = $"Database rejected the save. {dbEx.InnerException?.Message ?? dbEx.Message}";
                return View(vm);
            }
            catch (Exception ex)
            {
                await PopulateLotOptions(vm);
                TempData["Err"] = $"Unexpected error while saving. {ex.Message}";
                return View(vm);
            }
        }

        private async Task<string> GenerateSellingIDAsync(int Lot)
        {
            var product = await _context.ProductInventories
                .Where(x => x.Lot == Lot)
                .Select(x => x.ProductName)
                .FirstOrDefaultAsync();

            if (product == null)
                TempData["Err"] = $"Lot {Lot} was not found in Product Inventory.";

            var prefix = product.ToString().ToUpperInvariant();
            prefix = prefix.Length >= 2 ? prefix.Substring(0, 2) : (prefix + "X");

            var maxExisting = await _context.Receipts
                .Where(x => x.ID.StartsWith(prefix))
                .Select(x => x.ID)
                .ToListAsync();

            var maxNum = 0;
            foreach (var lot in maxExisting)
            {
                var tail = lot.Length > 2 ? lot.Substring(2) : "";
                if (int.TryParse(tail, out var n) && n > maxNum)
                    maxNum = n;
            }

            var next = maxNum + 1;
            return $"{prefix}{next:D6}";
        }

        private async Task PopulateLotOptions(ReceiptVM vm)
        {
            var inv = await _context.ProductInventories
                .AsNoTracking()
                .OrderBy(x => x.Lot)
                .ToListAsync();

            vm.LotOptions = inv.Select(x =>
                new SelectListItem(
                    text: $"{x.Lot} ({x.ProductName})",
                    value: x.Lot.ToString()
                )).ToList();
        }
    }
}
